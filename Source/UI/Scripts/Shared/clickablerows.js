//Usings
Namespace("Views.Shared");

//Initialization
Views.Shared.ClickableRows = function () {
    this.$tbodyId = null;
};

//Implementation
Views.Shared.ClickableRows.prototype = {
    init: function (settings) {
        this.$tbodyId = settings.tbodyId;

        $("#" + this.$tbodyId + " > tr").click(function() {
            var element = this;
            window.location = element.getAttribute("data-details-url");
        });
    }
}