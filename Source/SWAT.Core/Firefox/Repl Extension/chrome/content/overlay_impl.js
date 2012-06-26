// GLOBAL DEFINITIONS
// ----------------------------------------------------------------------

const Ci = Components.interfaces;
const Cc = Components.classes;
const pref = Cc['@mozilla.org/preferences-service;1']
    .getService(Ci.nsIPrefService)
    .getBranch('extensions.mozrepl.');


// GLOBAL STATE
// ----------------------------------------------------------------------

var server;


// INITIALIZATION
// ----------------------------------------------------------------------

function initOverlay() {
    server = Cc['@hyperstruct.net/mozlab/mozrepl;1'].getService(Ci.nsIMozRepl);

    updateStartStopButton();

    // upgradeCheck(
    //     'mozrepl@hyperstruct.net',
    //     'extensions.mozrepl.version', {
    //         onFirstInstall: function() {
    //             openURL('http://hyperstruct.net/projects/mozlab/news');
    //         },

    //         onUpgrade: function() {
    //             openURL('http://hyperstruct.net/projects/mozlab/news');
    //         }
    //     });
}

function togglePref(prefName) {
    pref.setBoolPref(prefName, !pref.getBoolPref(prefName));
}

function toggleServer(sourceCommand) {
    var port = pref.getIntPref('port');

    if(server.isActive())
        server.stop();
    else
        server.start(port);

    updateStartStopButton();
}

function updateStartStopButton() {
    // NOTE: update mozrepl-startstop-button if exists
    //       (inside desktop preference-window or fennec options)
    var btn = document.getElementById("mozrepl-startstop-button");

    if(btn) {
	if(server.isActive())
	    btn.label = "Stop";
	else
	    btn.label = "Start";
    }
}

function updateMenu(xulPopup) {    
    document.getElementById('mozrepl-command-toggle')
        .setAttribute('label', server.isActive() ? 'Stop' : 'Start');
    document.getElementById('mozrepl-command-listen-external')
        .setAttribute('checked', !pref.getBoolPref('loopbackOnly'));
    document.getElementById('mozrepl-command-autostart')
        .setAttribute('checked', pref.getBoolPref('autoStart'));
}

function changePort() {
    var value = window.prompt('Choose listening port', pref.getIntPref('port'));
    if(value)
        pref.setIntPref('port', value);
}

function openHelp() {
    openURL('http://github.com/bard/mozrepl/wikis/home');
}

function openURL(url) {
    if(typeof(getBrowser().addTab) == 'function')
        setTimeout(function() {
            getBrowser().selectedTab = getBrowser().addTab(url)
        }, 500);
    else
        Cc['@mozilla.org/uriloader/external-protocol-service;1']
            .getService(Ci.nsIExternalProtocolService)
            .loadUrl(Cc['@mozilla.org/network/io-service;1']
                     .getService(Ci.nsIIOService)
                     .newURI(url, null, null));
}

function upgradeCheck(id, versionPref, actions) {
    const pref = Cc['@mozilla.org/preferences-service;1']
    .getService(Ci.nsIPrefService);

    function getExtensionVersion(id) {
        return Cc['@mozilla.org/extensions/manager;1']
        .getService(Ci.nsIExtensionManager)
        .getItemForID(id).version;
    }

    function compareVersions(a, b) {
        return Cc['@mozilla.org/xpcom/version-comparator;1']
        .getService(Ci.nsIVersionComparator)
        .compare(curVersion, prevVersion);
    }

    var curVersion = getExtensionVersion(id);
    if(curVersion) {
        var prevVersion = pref.getCharPref(versionPref);
        if(prevVersion == '') {
            if(typeof(actions.onFirstInstall) == 'function')
                actions.onFirstInstall();
        } else {
            if(compareVersions(curVersion, prevVersion) > 0)
                if(typeof(actions.onUpgrade) == 'function')
                    actions.onUpgrade();
        }

        pref.setCharPref(versionPref, curVersion);
    }
}
