var playerData = [];
var playerDataMap = {};

function drawLineGraph(myData) {
    nv.addGraph(function () {
        var chart = nv.models.lineChart()
            .useInteractiveGuideline(true)
            .showLegend(true)
            .showYAxis(true)
            .showXAxis(true);

        chart.xAxis
            .axisLabel('Date')
            .tickFormat(function (d) {
                return d3.time.format('%x')(new Date(d))
            });

        chart.yAxis
            .axisLabel('NemeStats Points')
            .tickFormat(d3.format('d'));

        d3.select('#NemeStatsPointsLineGraph svg')
            .datum(myData)
            .call(chart);

        nv.utils.windowResize(function () { chart.update() });
        return chart;
    });
}