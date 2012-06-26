function constructor(server) {
    this._server = server;
    window.addEventListener(
        'load', function(event) {
            document
                .getElementById('mozrepl-command-toggle')
                .setAttribute('label',
                              server.isActive() ? 'Stop Repl' : 'Start Repl');
        }, false);
}

function toggleServer(sourceCommand) {
    var port = Components
        .classes['@mozilla.org/preferences-service;1']
        .getService(Components.interfaces.nsIPrefService)
        .getBranch('extensions.mozrepl.')
        .getIntPref('port');

    if(this._server.isActive()) {
        this._server.stop();
        sourceCommand.setAttribute('label', 'Start Repl');
    }
    else {
        this._server.start(port);
        sourceCommand.setAttribute('label', 'Stop Repl');
    }
}

