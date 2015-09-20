function drawPieChart(summariesOfGames) {
    //Regular pie chart example
    nv.addGraph(function () {
        var chart = nv.models.pieChart()
            .x(function (d) { return d.gameDefinitionName })
            .y(function (d) { return d.gamesLost + d.gamesWon })
            .showLabels(true);

        d3.select("#GamesPieChart svg")
            .datum(summariesOfGames)
            .transition().duration(350)
            .call(chart);

        return chart;
    });
}