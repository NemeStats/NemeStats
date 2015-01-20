//Namespace
Namespace("Views.GameDefinition");

//Initialization
Views.GameDefinition.Create = function () {

};

//Implementation
Views.GameDefinition.Create.prototype = {
	init: function() {
		alert(this.getQueryString("returnUrl"));
	},

	getQueryString: function(queryParamName) {
		var result =  window.location.search.match(
			new RegExp("(\\?|&)" + queryParamName + "(\\[\\])?=([^&]*)")
		);

		return result ? result[3] : false;
	}
};

$(document).ready(function () {
	var initialize = new Views.GameDefinition.Create();

	initialize.init();
});