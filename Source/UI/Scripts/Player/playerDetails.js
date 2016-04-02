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
        this.initListJs();
    },
    initListJs: function () {

        var gamedefinitionsValues = [{ name: 'name-col', attr: 'data-name' }, { name: 'winloss-col', attr: 'data-winloss' },'win-percentage-col' ];
        var gameDefinitionTableId = "playedgameslist";

        if (ResponsiveBootstrapToolkit.is('>=md')) {
            new List(gameDefinitionTableId, { valueNames: gamedefinitionsValues, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
        } else {
            new List(gameDefinitionTableId, { valueNames: gamedefinitionsValues });
        }

    },
    renderGameDefinitionsPieChart: function () {
        $.ajax({
            type: "GET",
            url: this.$apiUri,
            success: function (data) {
                var gamesInPieChart = 9;

                var pieChartData = data.gameDefinitionTotals.summariesOfGameDefinitionTotals;
                if (pieChartData.length == 0)
                {
                    $("#GamesPieChart").hide();
                    return;
                }
                pieChartData.sort(function (summary1, summary2) {
                    return (summary2.gamesLost + summary2.gamesWon) - (summary1.gamesLost + summary1.gamesWon);
                });

                if (pieChartData.length > gamesInPieChart) {
                    var restOfGamesAccumulation = pieChartData.slice(gamesInPieChart).reduce(
                        function (accumulativeSummary, summary) {
                            return {
                                gameDefinitionName: accumulativeSummary.gameDefinitionName,
                                gamesLost: accumulativeSummary.gamesLost + summary.gamesLost,
                                gamesWon: accumulativeSummary.gamesWon + summary.gamesWon
                            };
                        }, { gameDefinitionName: "All Other Games", gamesLost: 0, gamesWon: 0 });
                    pieChartData = pieChartData.slice(0, gamesInPieChart).concat(restOfGamesAccumulation);
                }

                nv.addGraph(function () {
                    var chart = nv.models.pieChart()
                        .x(function (d) { return d.gameDefinitionName; })
                        .y(function (d) { return d.gamesLost + d.gamesWon; })
                        .showLabels(false)
                          .legendPosition("top");
                    ;

                    chart.tooltip.valueFormatter(function (d) {
                        return (d * 100 / data.totalGames).toFixed() + ' %';
                    });

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