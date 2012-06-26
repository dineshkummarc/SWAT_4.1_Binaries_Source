window.addEventListener(
    'load', function(event) { mozrepl.initOverlay(); }, false);

var mozrepl = {};

Components
.classes['@mozilla.org/moz/jssubscript-loader;1']
.getService(Components.interfaces.mozIJSSubScriptLoader)
    .loadSubScript('chrome://mozrepl/content/overlay_impl.js', mozrepl);
