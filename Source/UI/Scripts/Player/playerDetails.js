//Usings
Namespace("Views.Player");

//Initialization
Views.Player.Details = function () {
    this.$playerId = null;
};

//Implementation
Views.Player.Details.prototype = {
    init: function (playerId) {
        this.$playerId = playerId;
    },
    renderGameDefinitionsPieChart: function () {
        var url = '/api/v1/GamingGroups/1/PlayerStats/' + this.$playerId.playerId;

        $.get(url, function (data) {
            nv.addGraph(function () {
                var chart = nv.models.pieChart()
                    .x(function (d) { return d.gameDefinitionName })
                    .y(function (d) { return d.gamesLost + d.gamesWon })
                    .showLabels(true);

                d3.select("#GamesPieChart svg")
                    .datum(data.gameDefinitionTotals.summariesOfGameDefinitionTotals)
                    .transition().duration(350)
                    .call(chart);
            });
        });
    }
};