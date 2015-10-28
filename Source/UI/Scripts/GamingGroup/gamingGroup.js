//Usings
Namespace("Views.GamingGroup");

//Initialization
Views.GamingGroup.GamingGroupView = function () {
    this.$container = null;
    this.$title = null;
    this._serviceAddress = "/GamingGroup/UpdateGamingGroupName";
    this._googleAnalytics = null;
};

//Implementation
Views.GamingGroup.GamingGroupView.prototype = {
    init: function (gaObject) {
        var parent = this;
        this.$title = $("#gamingGroupTitle");
        this.$title.toEditBox({ onFocusOut: $.proxy(parent.renameGamingGroup, this), cssClass: 'gaming-group-name' });
        this._googleAnalytics = gaObject;
    },
    renameGamingGroup: function (element) {
        var parent = this;
        $.ajax({
            type: "POST",
            url: parent._serviceAddress,
            data: { "gamingGroupName": element.value },
            success: function (data) {

            },
            error: function (err) {
                alert("Error " + err.status + ":\r\n" + err.statusText);
            },
            dataType: "json"
        });

        this._googleAnalytics.trackGAEvent("GamingGroups", "GamingGroupRenamed", "GamingGroupRenamed");
    },
    renderNemeStatsPointsLineGraph: function (url) {
        $.ajax({
            type: "GET",
            url: url,
            success: function (data) {
                var playerDataMap = {};
                var playerData = [];

                for (var i = data.playedGames.length - 1; i >= 0; i--) {
                    for (var j = 0; j < data.playedGames[i].playerGameResults.length; j++) {
                        var gameInfo = data.playedGames[i].playerGameResults[j];
                        if (playerDataMap[gameInfo.playerId] == null) {
                            playerDataMap[gameInfo.playerId] = {
                                values: [{ x: new Date(data.playedGames[i].datePlayed), y: 0 }]
                            };
                            playerData.push({ values: playerDataMap[gameInfo.playerId].values, key: gameInfo.playerName, disabled: !gameInfo.playerActive });
                        }
                        var lastIndex = playerDataMap[gameInfo.playerId].values.length - 1;
                        var nextValue = playerDataMap[gameInfo.playerId].values[lastIndex].y + gameInfo.nemeStatsPointsAwarded;
                        playerDataMap[gameInfo.playerId].values.push({ x: new Date(data.playedGames[i].datePlayed), y: nextValue });
                    }
                }

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
                        .axisLabel('NemePoints')
                        .tickFormat(d3.format('d'));

                    d3.select('#NemeStatsPointsLineGraph svg')
                        .datum(playerData)
                        .call(chart);

                    nv.utils.windowResize(function () { chart.update() });
                });
            }
        });
    }
}

var clickElement = function (elementId) {
    $('#' + elementId).click();
}