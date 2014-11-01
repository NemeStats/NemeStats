//Usings
Namespace("Views.Shared");

//Initialization
Views.Shared.Layout = function () {
    this.$input = null;
};

//Implementation
Views.Shared.Layout.prototype = {
    init: function () {
        this.$input = $('input:visible:enabled:first');
        this.$input.focus();
    }
};

$(document).ready(function () {
    var layout = new Views.Shared.Layout();
    layout.init();
});