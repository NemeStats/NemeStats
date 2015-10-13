//Usings
Namespace("Views.Player");

//Initialization
Views.Player.Details = function () {
    this.$apiUri = null;
};

//Implementation
Views.Player.Details.prototype = {
    init: function (settings) {
        this.$apiUri = settings.apiUri;
    },
    renderGameDefinitionsPieChart: function () {
        $.ajax({
            type: "GET",
            url: this.$apiUri,
            success: function (data) {
                nv.addGraph(function () {
                    var chart = nv.models.pieChart()
                        .x(function (d) { return d.gameDefinitionName; })
                        .y(function (d) { return d.gamesLost + d.gamesWon; })
                        .showLabels(false);

                    d3.select("#GamesPieChart svg")
                        .datum(data.gameDefinitionTotals.summariesOfGameDefinitionTotals)
                        .transition().duration(350)
                        .call(chart);

                    nv.utils.windowResize(function () { chart.update(); });
                });
            }
        });
    }
};