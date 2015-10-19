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
                var gamesInPieChart = 9;

                var pieChartData = data.gameDefinitionTotals.summariesOfGameDefinitionTotals;

                pieChartData.sort(function (summary1, summary2) {
                    return (summary2.gamesLost + summary2.gamesWon) - (summary1.gamesLost + summary1.gamesWon);
                });

                console.log(pieChartData);

                if (pieChartData.length > gamesInPieChart) {
                    var restOfGamesAccumulation = pieChartData.slice(gamesInPieChart).reduce(
                        function (accumulativeSummary, summary) {
                            return {
                                gameDefinitionName: accumulativeSummary.gameDefinitionName,
                                gamesLost: accumulativeSummary.gamesLost + summary.gamesLost,
                                gamesWon: accumulativeSummary.gamesWon + summary.gamesWon
                            };
                        }, { gameDefinitionName: "Rest Of Games", gamesLost: 0, gamesWon: 0 });
                    pieChartData = pieChartData.slice(0, gamesInPieChart).concat(restOfGamesAccumulation);
                }

                nv.addGraph(function () {
                    var chart = nv.models.pieChart()
                        .x(function (d) { return d.gameDefinitionName; })
                        .y(function (d) { return d.gamesLost + d.gamesWon; })
                        .showLabels(false)
                          .legendPosition("top");
                    ;

                    d3.select("#GamesPieChart svg")
                        .datum(pieChartData)
                        .transition().duration(350)
                        .call(chart);

                    nv.utils.windowResize(function () { chart.update(); });
                });
            }
        });
    }
};