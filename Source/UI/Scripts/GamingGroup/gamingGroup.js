//Usings
Namespace("Views.GamingGroup");

//Initialization
Views.GamingGroup.GamingGroupView = function () {
    this.$container = null;
    this.$title = null;
    this.$fromDatePicker = null;
    this.$toDatePicker = null;

    this._settings = {
        fromDate: null,
        toDate: new Date(),
        gamingGroupId : null,
        playersTabId : null,
        playersDivId: null,
        gamesTabId: null,
        gamesDivId: null,
        playedGamesTabId: null,
        playedGamesDivId: null
    };
//TODO NOT SURE THIS IS WORKING AT THE MOMENT
    this._updateGamingGroupNameServiceAddress = "/GamingGroup/UpdateGamingGroupName";
    this._getGamingGroupPlayersServiceAddress = "/GamingGroup/GetGamingGroupPlayers";
    this._googleAnalytics = null;
    this._playersTabLoaded = false;
    this._gamesTabLoaded = false;
    this._playedGamesTabLoaded = false;
};

//Implementation
Views.GamingGroup.GamingGroupView.prototype = {
    init: function (gaObject, options) {
        var parent = this;
        this.$title = $("#gamingGroupTitle");
        this.$title.toEditBox({ onFocusOut: $.proxy(parent.renameGamingGroup, this), cssClass: "gaming-group-name" });
        this._googleAnalytics = gaObject;

        if (options.gamingGroupId == null) {
            throw "gamingGroupId is required for panels to be able to load.";
        }
        this._settings.gamingGroupId = options.gamingGroupId;

        if (options.playersTabId == null) {
            throw "playersTabId is required.";
        }
        this._settings.playersTabId = options.playersTabId;

        if (options.playersDivId == null) {
            throw "playersDivId is required.";
        }
        this._settings.playersDivId = options.playersDivId;

        if (options.gamesTabId == null) {
            throw "gamesTabId is required.";
        }
        this._settings.gamesTabId = options.gamesTabId;

        if (options.gamesDivId == null) {
            throw "gamesDivId is required.";
        }
        this._settings.gamesDivId = options.gamesDivId;

        if (options.playedGamesTabId == null) {
            throw "playedGamesTabId is required.";
        }
        this._settings.playedGamesTabId = options.playedGamesTabId;

        if (options.playedGamesDivId == null) {
            throw "playedGamesDivId is required.";
        }
        this._settings.playedGamesDivId = options.playedGamesDivId;

        this._settings.gamingGroupId = options.gamingGroupId;

        if (options.fromDate != null) {
            this._settings.fromDate = options.fromDate;
        }

        if (options.toDate != null) {
            this._settings.toDate = options.toDate;
        }

        this.$fromDatePicker = $("#from-date-picker");
        this.$toDatePicker = $("#to-date-picker");
        var minDate = new Date(2000, 0, 1);
        var currentMoment = moment();

        if (Modernizr.inputtypes.date) {
            //if supports HTML5 then use native date picker
            var toDateIso8601 = new moment(this._settings.toDate).format("YYYY-MM-DD");
            this.$toDatePicker.attr("value", toDateIso8601);
            var minDateIso8601 = minDate.toISOString().split("T")[0];
            this.$fromDatePicker.attr("max", currentMoment.add(1, "days").format("YYYY-MM-DD"));
            this.$fromDatePicker.attr("min", minDateIso8601);
            this.$toDatePicker.attr("max", currentMoment.add(1, "days").format("YYYY-MM-DD"));
            this.$toDatePicker.attr("min", minDateIso8601);
        } else {
            // If not native HTML5 support, fallback to jQuery datePicker
            this.$fromDatePicker.datepicker({
                showOn: "button",
                buttonText: "<i class='fa fa-calendar'></i>",
                showButtonPanel: true,
                maxDate: new Date(),
                minDate: minDate,
                onClose: function (selectedDate) {
                    $("#to-date-picker").datepicker("option", "minDate", selectedDate);
                }
            }).datepicker("setDate", this._settings.fromDate)
                .datepicker("option", "dateFormat", "yy-mm-dd");

            this.$toDatePicker.datepicker({
                showOn: "button",
                buttonText: "<i class='fa fa-calendar'></i>",
                showButtonPanel: true,
                maxDate: new Date(),
                minDate: minDate,
                onClose: function (selectedDate) {
                    $("#from-date-picker").datepicker("option", "minDate", selectedDate);
                }
            }).datepicker("setDate", this._settings.toDate)
              .datepicker("option", "dateFormat", "yy-mm-dd");
        }

        var fromDateYYYYMMDD = this.$fromDatePicker.val();
        var toDateYYYYMMDD = this.$toDatePicker.val();
        this.renderNemeStatsPointsLineGraph("/api/v2/PlayedGames/?gamingGroupId=" + this._settings.gamingGroupId + "&datePlayedFrom=" + fromDateYYYYMMDD + "&datePlayedTo=" + toDateYYYYMMDD);

        var owner = this;

        $("#" + this._settings.playersTabId).click((function () {
            return owner.getPlayers(owner._settings.gamingGroupId, owner.$fromDatePicker, parent.$toDatePicker, owner._settings.playersDivId);
        }));

        $("#" + this._settings.gamesTabId).click((function () {
            return owner.getGameDefinitions(owner._settings.gamingGroupId, owner.$fromDatePicker, owner.$toDatePicker, owner._settings.gamesDivId);
        }));

        $("#" + this._settings.playedGamesTabId).click((function () {
            return owner.getPlayedGames(owner._settings.gamingGroupId, owner.$fromDatePicker, owner.$toDatePicker, owner._settings.playedGamesDivId);
        }));

        var defaultTab = window.location.hash;

        switch (defaultTab) {
            case "#" + this._settings.gamesTabId:
                this.getGameDefinitions(this._settings.gamingGroupId,
                    this.$fromDatePicker,
                    this.$toDatePicker,
                    this._settings.gamesDivId);
            case "#" + this._settings.playersTabId:
                this.getPlayers(this._settings.gamingGroupId,
                    this.$fromDatePicker,
                    this.$toDatePicker,
                    this._settings.playersDivId);
                break;
            case "#" +this._settings.playedGamesTabId:
                this.getPlayedgames(this._settings.gamingGroupId,
                    this.$fromDatePicker,
                    this.$toDatePicker,
                    this._settings.playedGamesDivId);
                break;
            default:
                this.getPlayers(this._settings.gamingGroupId,
                    this.$fromDatePicker,
                    this.$toDatePicker,
                    this._settings.playersDivId);
                break;
        }
    },
    getPlayers: function (gamingGroupId, fromDatePicker, toDatePicker, divIdForRenderingResults) {
        var fromDate = fromDatePicker.val();
        var toDate = toDatePicker.val();
        this.updateUrl(this._settings.playersTabId, fromDate, toDate);

        if (!this._playersTabLoaded) {
            var owner = this;
            $.ajax({
                url: "/GamingGroup/GetGamingGroupPlayers/",
                data: {
                    "id": gamingGroupId,
                    "datePlayedFrom": fromDatePicker.val(),
                    "datePlayedTo": toDatePicker.val()
                },
                cache: false,
                type: "GET",
                success: function(html) {
                    $("#" + divIdForRenderingResults).html(html);
                    var players = new window.Views.Player.Players();
                    players.init();

                    var createOrUpdatePlayer = new window.Views.Player.CreateOrUpdate();
                    createOrUpdatePlayer.init($.proxy(players.onPlayerSaved, players));

                    var playersValues = [{ name: 'player-name-col', attr: 'data-name' }, { name: 'total-nemepoints-col', attr: 'data-nemepoints' }, 'played-games-col', 'avg-nemepoints-col', 'overall-win-col', 'championed-games-col', { name: 'nemesis-col', attr: 'data-nemesis' }, { name: 'achievements-col', attr: 'data-achievements' }];
                    var playersTableId = "playersList";

                    if (ResponsiveBootstrapToolkit.is('>=md')) {
                        new List(playersTableId, { valueNames: playersValues, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
                    } else {
                        new List(playersTableId, { valueNames: playersValues });
                    }

                    owner._playersTabLoaded = true;
                }
            });
        }
    },
    getGameDefinitions: function (gamingGroupId, fromDatePicker, toDatePicker, divIdForRenderingResults) {
        if (!this._gamesTabLoaded) {
            $.ajax({
                url: "/GamingGroup/GetGamingGroupGameDefinitions/",
                data: {
                    "id": gamingGroupId,
                    "datePlayedFrom": fromDatePicker.val(),
                    "datePlayedTo": toDatePicker.val()
                },
                cache: false,
                type: "GET",
                success: function (html) {
                    $("#" + divIdForRenderingResults).html(html);

                    var gameDefinition = new window.Views.GameDefinition.CreateGameDefinitionPartial();
                    gameDefinition.init();
                    gameDefinition.configureViewModel();

                    var gameDefinitions = new window.Views.GameDefinition.GameDefinitions();
                    gameDefinitions.init();
                    gameDefinition.onDefinitionCreated = $.proxy(gameDefinitions.onGameCreated, gameDefinitions);

                    var gamedefinitionsValues = ['name', 'plays-col', { name: 'champion-col', attr: 'data-champion' }];
                    var gameDefinitionTableId = "gameDefinitionsList";

                    if (ResponsiveBootstrapToolkit.is('>=md')) {
                        new List(gameDefinitionTableId, { valueNames: gamedefinitionsValues, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
                    } else {
                        new List(gameDefinitionTableId, { valueNames: gamedefinitionsValues });
                    }

                    this._gamesTabLoaded = true;
                }
            });
        }
    },
    getPlayedGames: function (gamingGroupId, fromDatePicker, toDatePicker, divIdForRenderingResults) {
        if (!this._playedGamesTabLoaded) {
            $.ajax({
                url: "/GamingGroup/GetGamingGroupPlayedGames/",
                data: {
                    "id": gamingGroupId,
                    "datePlayedFrom": fromDatePicker.val(),
                    "datePlayedTo": toDatePicker.val()
                },
                cache: false,
                type: "GET",
                success: function (html) {
                    $("#" + divIdForRenderingResults).html(html);

                    var gameDefinition = new window.Views.GameDefinition.CreateGameDefinitionPartial();
                    gameDefinition.init();
                    gameDefinition.configureViewModel();

                    var gameDefinitions = new window.Views.GameDefinition.GameDefinitions();
                    gameDefinitions.init();
                    gameDefinition.onDefinitionCreated = $.proxy(gameDefinitions.onGameCreated, gameDefinitions);

                    this._playedGamesTabLoaded = true;
                }
            });
        }
    },
    renameGamingGroup: function (element) {
        var parent = this;
        $.ajax({
            type: "POST",
            url: parent._updateGamingGroupNameServiceAddress,
            data: { "gamingGroupName": element.value },
            success: function (data) {

            },
            error: function (err) {
                alert("Error " + err.status + ":\r\n" + err.statusText);
            },
            dataType: "json"
        });

        this.trackGAEvent("GamingGroups", "GamingGroupRenamed", "GamingGroupRenamed");
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
                        var nextValue = playerDataMap[gameInfo.playerId].values[lastIndex].y + gameInfo.totalNemeStatsPointsAwarded;
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
                        .axisLabel("Date")
                        .tickFormat(function (d) {
                            return d3.time.format("%x")(new Date(d))
                        });

                    chart.yAxis
                        .axisLabel("NemePoints")
                        .tickFormat(d3.format("d"));

                    d3.select("#NemeStatsPointsLineGraph svg")
                        .datum(playerData)
                        .call(chart);

                    nv.utils.windowResize(function () { chart.update() });
                });
            }
        });
    },
    updateUrl: function(newHash, iso8601FromDate, iso8601ToDate) {
        if (history.pushState) {
            var newUrl = window.location.protocol + "//" + window.location.host + window.location.pathname;
            var params = new Object();

            if (iso8601FromDate) {
                params.Iso8601FromDate = iso8601FromDate;
            }

            if (iso8601ToDate) {
                params.Iso8601ToDate = iso8601ToDate;
            }

            var queryString = jQuery.param(params);
            if (queryString) {
                newUrl += "?" + queryString;
            }

            if (newHash) {
                newUrl += "#" + newHash;
            }

            window.history.pushState({ path: newUrl }, "", newUrl);
        }
    }
}

var clickElement = function (elementId) {
    $("#" + elementId).click();
}