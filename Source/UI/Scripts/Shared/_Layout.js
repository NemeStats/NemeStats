$(document).ready(function () {
	Views.Shared.Layout.prototype.init();
});

//Usings
Namespace("Views.Shared");

//Initialization
Views.Shared.Layout = function () {
	this.$container = null;
	this.$input = null;
};

//Implementation
Views.Shared.Layout.prototype = {
	init: function () {
		this.$container = $(document);
		this.$input = $('input:visible:enabled:first');

		this.$input.focus();
	},
}