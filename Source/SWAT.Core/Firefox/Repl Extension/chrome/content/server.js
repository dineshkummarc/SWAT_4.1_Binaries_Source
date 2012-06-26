// GLOBAL DEFINITIONS
// ----------------------------------------------------------------------

const Cc = Components.classes;
const Ci = Components.interfaces;
const loader = Cc['@mozilla.org/moz/jssubscript-loader;1']
    .getService(Ci.mozIJSSubScriptLoader);
const srvPref = Cc['@mozilla.org/preferences-service;1']
    .getService(Ci.nsIPrefService);
const srvObserver = Cc['@mozilla.org/observer-service;1']
    .getService(Ci.nsIObserverService);
const pref = srvPref.getBranch('extensions.mozrepl.');


function REPL() {};
loader.loadSubScript('chrome://mozrepl/content/repl.js', REPL.prototype);


// STATE
// ----------------------------------------------------------------------

var serv;


// CODE
// ----------------------------------------------------------------------

var sessions = {
    _list: [],

    add: function(session) {
        this._list.push(session);
    },

    remove: function(session) {
        var index = this._list.indexOf(session);
        if(index != -1)
            this._list.splice(index, 1);
    },

    get: function(index) {
        return this._list[index];
    },

    quit: function() {
        this._list.forEach(
            function(session) { session.quit; });
        this._list.splice(0, this._list.length);
    }
};


function start(port) {
    try {
        serv = Cc['@mozilla.org/network/server-socket;1']
            .createInstance(Ci.nsIServerSocket);
        serv.init(port, pref.getBoolPref('loopbackOnly'), -1);
        serv.asyncListen(this);
        log('MozRepl: Listening...');
        pref.setBoolPref('started', true);
    } catch(e) {
        log('MozRepl: Exception: ' + e);
    }
}

function onSocketAccepted(serv, transport) {
    try {
        var outstream = transport.openOutputStream(Ci.nsITransport.OPEN_BLOCKING , 0, 0);
        var stream = transport.openInputStream(0, 0, 0);
        var instream = Cc['@mozilla.org/intl/converter-input-stream;1']
            .createInstance(Ci.nsIConverterInputStream);
        instream.init(stream, 'UTF-8', 8192,
                      Ci.nsIConverterInputStream.DEFAULT_REPLACEMENT_CHARACTER);
    } catch(e) {
        log('MozRepl: Error: ' + e);
    }
    log('MozRepl: Accepted connection.');

    var window = Cc['@mozilla.org/appshell/window-mediator;1']
        .getService(Ci.nsIWindowMediator)
        .getMostRecentWindow('');

    var session = new REPL();
    session.onOutput = function(string) {
        outstream.write(string, string.length);
    };
    session.onQuit = function() {
        instream.close();
        outstream.close();
        sessions.remove(session);
    };
    session.init(window);

    var pump = Cc['@mozilla.org/network/input-stream-pump;1']
        .createInstance(Ci.nsIInputStreamPump);
    pump.init(stream, -1, -1, 0, 0, false);
    pump.asyncRead({
        onStartRequest: function(request, context) {},
        onStopRequest: function(request, context, status) {
                session.quit();
            },
        onDataAvailable: function(request, context, inputStream, offset, count) {
            var str = {}
            instream.readString(count, str)
            session.receive(str.value);
            }
        }, null);

    sessions.add(session);
}

function onStopListening(serv, status) {
}


function stop() {
    log('MozRepl: Closing...');
    serv.close();
    sessions.quit();
    pref.setBoolPref('started', false);
    serv = undefined;
}

function isActive() {
    if(serv)
        return true;
}

function observe(subject, topic, data) {
    switch(topic) {
    case 'app-startup': // Gecko 1.9.2 only
	srvObserver.addObserver(this, 'profile-after-change', false);
        break;
    case 'profile-after-change': // Gecko 1.9.2 and Gecko 2.0 
        srvObserver.addObserver(this, 'network:offline-status-changed', false);
        if(srvPref.getBranch('network.').getBoolPref('online') &&
           pref.getBoolPref('autoStart'))
            this.start(pref.getIntPref('port'));

        break;
    case 'network:offline-status-changed':
        switch(data) {
        case 'online':
            if(pref.getBoolPref('autoStart'))
                this.start(pref.getIntPref('port'));
            break;
        case 'offline':
            if(isActive())
                this.stop();
            break;
        }
        break;
    case 'quit-application-granted':
	this.stop();
    }
}

// UTILITIES
// ----------------------------------------------------------------------

function log(msg) {
    dump(msg + '\n');
}

