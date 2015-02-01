//Usings
Namespace("Views.Shared");

//Initialization
Views.Shared.Layout = function () {
    this.$input = null;
    this.$popovers = null;
};

//Implementation
Views.Shared.Layout.prototype = {
    init: function () {
        this.$popovers = $('[data-toggle="popover"]');
        this.$popovers.popover({ trigger: 'hover' });
    },
    getQueryString: function (queryParamName) {
    	var result = window.location.search.match(
			new RegExp("(\\?|&)" + queryParamName + "(\\[\\])?=([^&]*)")
		);

    	return result ? result[3] : false;
    }
};

$(document).ready(function () {
    var layout = new Views.Shared.Layout();
    layout.init();
});