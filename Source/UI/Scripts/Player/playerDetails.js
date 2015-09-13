//Usings
Namespace("Views.Player");

//Initialization
Views.Player.Details = function () {
    this.$playerId = null;
};

//Implementation
Views.Player.Details.prototype = {
    init: function(playerId) {
        this.$playerId = playerId;
    },
    renderGameDefinitionsPieChart: function () {
        var url = '/api/v1/GamingGroups/1/PlayerStats/' + this.$playerId.playerId;
        $.get(url, function (data) {
            drawPieChart(data.gameDefinitionTotals.summariesOfGameDefinitionTotals);
        });
    }
};