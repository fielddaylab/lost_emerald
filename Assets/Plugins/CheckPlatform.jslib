mergeInto(LibraryManager.library, {

	IsSafari: function () {

		// Detect Safari
		var safariAgent = navigator.userAgent.indexOf("Safari") > -1;
		var chromeAgent = navigator.userAgent.indexOf("Chrome") > -1;  
		// Discard Safari since it also matches Chrome
		if ((chromeAgent) && (safariAgent)) safariAgent = false;

		return safariAgent;
	}


});