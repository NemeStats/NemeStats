function drawPieChart(summariesOfGames) {
    var diameter = 360;
    var radius = diameter / 2;

    var color = d3.scale.category20();

    var arc = d3.svg.arc()
        .outerRadius(radius)
        .innerRadius(0);

    var pie = d3.layout.pie()
        .value(function (summaryOfGame) {
            return summaryOfGame.gamesLost + summaryOfGame.gamesWon;
        })
        .sort(null);

    var svg = d3.select('#GamesPieChart')
        .append('svg')
            .attr('width', diameter)
            .attr('height', diameter)
        .append('g')
            .attr('transform', 'translate(' + radius + ',' + radius + ')');

    var g = svg.selectAll('.arc')
        .data(pie(summariesOfGames))
        .enter().append('g')
            .attr('class', 'arc');

    g.append('path')
        .attr('d', arc)
        .style('fill', function (gamePieSlice) {
            return color(gamePieSlice.data.gameDefinitionName);
        });

    g.append('text')
        .attr('transform', function (gamePieSlice) {
            return 'translate(' + arc.centroid(gamePieSlice) + ')';
        })
        .attr('dy', '.35em')
        .style('text-anchor', 'middle')
        .text(function (gamePieSlice) {
            return gamePieSlice.data.gameDefinitionName;
        });
}