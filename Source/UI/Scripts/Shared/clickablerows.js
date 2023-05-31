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

        $("#" + this.$tbodyId + " > tr").click(function(e) {
            var element = this;
            if (e.ctrlKey && element.getAttribute("title") === "View Played Game details") {
                element.classList.toggle("active").siblings().removeClass('active');
            } else {
                window.location = element.getAttribute("data-details-url");
            }
        });
    }
}