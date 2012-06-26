var SWATUtilities = {

    isValid: function(object) {
        if (object === undefined || object == null) {
            return false;
        }
        return true;
    },

    getCurrentSeconds: function() {
        return new Date().getSeconds();
    },

    isWithinTimeout: function(startSeconds, timeoutSeconds) {
        var now = this.getCurrentSeconds();
        var timeout = startSeconds + timeoutSeconds;
        if (now <= timeout) {
            return true;
        }
        return false;
    },

    delay: function(milliseconds) {
        var start = new Date();
        var now = null;

        do {
            now = new Date();
        } while (now - start < milliseconds);
    }
};

var SWATReplacement = {
	greaterConst: '@%%~@',
	lessConst: '@!!~@',

	replaceProblemChars: function (identifier) {
	    identifier = dealWithJQueryMetaChars(true, identifier);

		while (identifier.indexOf("<") !== -1 && identifier.indexOf(">") !== -1) {
			identifier = identifier.replace("<", SWATReplacement.lessConst);
			identifier = identifier.replace(">", SWATReplacement.greaterConst);
        } 
        
        return identifier;
	},
	
	restoreProblemChars: function(match) {
	    for (var index = 0; index < match.length; index++) {
	        match[index] = dealWithJQueryMetaChars(false, match[index]);

			while (match[index].indexOf(SWATReplacement.lessConst) !== -1 && match[index].indexOf(SWATReplacement.greaterConst) !== -1) {
				match[index] = match[index].replace(SWATReplacement.lessConst, "<");
				match[index] = match[index].replace(SWATReplacement.greaterConst, ">");
            }
        }
        return match;
    }
};

var jQueryMetaChars = ["#", ";", "&", ",", "\\.", "\\+", "\\*", "~", "'", ":", "\"", "!", "\\^", "\\$", "\\[", "\\]", "\\(", "\\)", "=", "\\|", "/"];

function dealWithJQueryMetaChars(replace, identifier) {
    var a = "";
    var b = "";

    if (replace)
        a = "\\\\";
    else
        b = "\\\\\\\\";

    for (var i = 0; i < jQueryMetaChars.length; i++) {
        var regex = new RegExp(b + jQueryMetaChars[i], "g");
        var characterAfterRemove = jQueryMetaChars[i].replace("\\", "");
        identifier = identifier.replace(regex, a + characterAfterRemove);
        //console.log(identifier);
    }

    return identifier;
}
	
