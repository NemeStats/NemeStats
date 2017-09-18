//Usings
Namespace("Views.Home");

//Initialization
Views.Home.IndexView = function () {
    this._settings = {
        widget1DivId: null,
        widget1ServiceEndpoint : null
    };
    this._updateGamingGroupNameServiceAddress = "/GamingGroup/UpdateGamingGroupName";
    this._getGamingGroupPlayersServiceAddress = "/GamingGroup/GetGamingGroupPlayers/";
    this._getGamingGroupGameDefinitionsServiceAddress = "/GamingGroup/GetGamingGroupGameDefinitions/";
    this._getGamingGroupPlayedGamesServiceAddress = "/GamingGroup/GetGamingGroupPlayedGames/";
};

//Implementation
Views.Home.IndexView.prototype = {
    init: function (options) {
        var owner = this;
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
            }
        });
    }
}