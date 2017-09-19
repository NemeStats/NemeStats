//Usings
Namespace("Views.Home");

//Initialization
Views.Home.IndexView = function () {
   
};

//Implementation
Views.Home.IndexView.prototype = {
    init: function (options) {
        var owner = this;

        options.dynamicWidgetsToLoad.forEach(function (element)
        {
            if (element.widgetDivId && element.widgetServiceEndpoint) {
                owner.loadWidgetAsynchronously(element.widgetDivId, element.widgetServiceEndpoint);
            }
        });
        if (options.widget1DivId && options.widget1ServiceEndpoint) {
            owner.loadWidgetAsynchronously(options.widget1DivId, options.widget1ServiceEndpoint);
        }

    },
    loadWidgetAsynchronously: function(widgetDivId, widgetServiceEndpoint) {
        $.ajax({
            url: widgetServiceEndpoint,
            cache: false,
            type: "GET",
            success: function (html) {
                $("#" + widgetDivId).html(html);
            },
            error: function (XMLHTttpRequest, status, error) {
                $("#" + widgetDivId).html("<div>Content failed to load due to a timeout. Reloading the page should solve the problem.");
            }
        });
    }
}