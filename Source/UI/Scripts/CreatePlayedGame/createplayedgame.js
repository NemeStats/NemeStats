//Usings
Namespace("Views.PlayedGame");

//Initialization
Views.PlayedGame.CreatePlayedGame = function () {
    this._steps = {
        SelectDate: 1,
        SelectGame: 2,
        SelectPlayers: 3,
        SetResult: 4,
        Summary: 5
    };

    this._winnerTypes = {
        PlayerWin: 1,
        TeamWin: 2,
        TeamLoss: 3
    }

    this._gameTypes = {
        Ranked: 1,
        Scored: 2,
        Cooperative: 3
    }

    this._viewModel = {
        Date: null,
        Game: null,
        Players: [],
        GameNotes: null,
        WinnerType: null,
        GameType: this._gameTypes.Ranked
    };

    this.component = null;
    this.gaObject = null;
};

//Implementation
Views.PlayedGame.CreatePlayedGame.prototype = {
    init: function () {
        this.setupDatePicker();
        this.setupAutocomplete();;
        this.setupPlayersDragAndDrop();
        this.configureViewModel();

        this.gaObject = new window.Views.Shared.GoogleAnalytics();
    },
    setupPlayersDragAndDrop: function () {
        var parent = this;
        var rankedGameContainer = document.getElementById("ranked-game");
        var list = dragula([rankedGameContainer]);
        list.on("dragend", function (el) {
            $.each($(el).parent().find("li"), function (i, player) {
                var $player = $(player);
                var index = $player.data("index");
                parent.component.$data.viewModel.Players[index].Rank = i + 1;

                parent.gaObject.trackGAEvent("PlayedGames", "SetRank", "DragAndDrop", index);
            });
        });

    },
    setupDatePicker: function () {
        this.$datePicker = $(".date-picker");

        var minDate = new Date(2000, 0, 1);
        var currentMoment = moment();

        if (Modernizr.inputtypes.date) {
            //if supports HTML5 then use native date picker
            var minDateIso8601 = minDate.toISOString().split("T")[0];
            this.$datePicker.attr("max", currentMoment.add(1,"days").format("YYYY-MM-DD"));
            this.$datePicker.attr("min", minDateIso8601);
        } else {
            // If not native HTML5 support, fallback to jQuery datePicker
            this.$datePicker.datepicker({
                showOn: "button",
                buttonText: "<i class='fa fa-calendar'></i>",
                showButtonPanel: true,
                minDate: new Date(2000, 1, 1),
                maxDate: new Date()
            }).datepicker("option", "dateFormat", "yy-mm-dd");
        }
    },
    setupAutocomplete: function () {
        var parent = this;

        var gameDefinitions = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: {
                url: '/gamedefinition/SearchGameDefinition?q=%QUERY',
                wildcard: '%QUERY',
                filter: function (parsedResponse) {
                    parent.component.$data.searchingGameDefinition = false;
                    return parsedResponse;
                }
            }
        });

        $('#search-game-definition.typeahead').typeahead(null, {
            name: 'gaming-group-game-definitions',
            display: 'Name',
            hint: true,
            highlight: true,
            source: gameDefinitions,
            limit: Infinity,
            templates: {
                empty: [
                  '<div class="empty-message">',
                    'there are no game matching this text on your gaming group.',
                  '</div>'
                ].join('\n'),
            }
        })
            .bind('typeahead:select', function (ev, suggestion) {
                parent.component.selectGame(suggestion.Id, suggestion.Name);
                parent.gaObject.trackGAEvent("PlayedGames", "SetGame", "SearchedOnYourGamingGroup");
            }).bind('typeahead:asyncrequest', function (ev, suggestion) {
                parent.component.$data.searchingGameDefinition = true;
            });


        var bgg = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: {
                url: '/gamedefinition/SearchBoardGameGeekHttpGet?searchText=%QUERY',
                wildcard: '%QUERY',
                filter: function (parsedResponse) {
                    parent.component.$data.searchingBGG = false;
                    return parsedResponse;
                }
            }
        });

        $('#search-bgg.typeahead').typeahead(null, {
            name: 'bgg-games',
            hint: true,
            highlight: true,
            source: bgg,
            display: 'BoardGameName',
            limit: Infinity,
            templates: {
                empty: [
                  '<div class="empty-message" id="bggsearchnotfound">',
                    'This game not exists on BGG. Click here to create it!',
                  '</div>'
                ].join('\n'),
                suggestion: Handlebars.compile('<div><strong>{{BoardGameName}}</strong> – {{YearPublished}}</div>')
            },
        })
            .bind('typeahead:select', function (ev, suggestion) {
                parent.component.selectGame(null, suggestion.BoardGameName, suggestion.BoardGameId);
                parent.gaObject.trackGAEvent("PlayedGames", "SetGame", "RecentlyCreated");
            }).bind('typeahead:asyncrequest', function (ev, suggestion) {
                parent.component.$data.searchingBGG = true;
            });

        $(document).on('click', "#bggsearchnotfound", function (e) {
            parent.component.selectGame(null, $("#search-bgg")[0].value);
        });
    },
    configureViewModel: function () {
        var parent = this;

        var componentSelector = "#record-played-game";
        var container = $(componentSelector);
        if (container) {

            Vue.filter('winnertype', function (value) {
                if (value === parent._winnerTypes.PlayerWin) {
                    return "Ranked game";
                }
                if (value === parent._winnerTypes.TeamWin) {
                    return "Everybody won";
                }
                if (value === parent._winnerTypes.TeamLoss) {
                    return "Everybody lost";
                }
                return "";
            });

            Vue.filter('scoredposition', function (rank) {
                var s = ["th", "st", "nd", "rd"],
                v = rank % 100;
                return rank + (s[(v - 20) % 10] || s[v] || s[0]);

            });

            var editMode = $(componentSelector).data("edit-mode");
            var model = $(componentSelector).data("model");

            this._viewModel.RecentPlayers = model.RecentPlayers;
            this._viewModel.OtherPlayers = model.OtherPlayers;

            if (model.UserPlayer) {
                model.UserPlayer.Selected = true;
                model.UserPlayer.PlayerName += " (me)";
                this._viewModel.RecentPlayers.push(model.UserPlayer);
            }

            if (editMode) {
                this._viewModel.Date = moment(model.DatePlayed).format("YYYY-MM-DD");
                this._viewModel.Game = {
                    Id: model.GameDefinitionId,
                    BoardGameGeekGameDefinitionId: model.BoardGameGeekGameDefinitionId,
                    Name: model.GameDefinitionName
                };
                model.PlayerRanks.forEach(function (playerRank) {
                    parent._viewModel.Players.push({
                        Id: playerRank.PlayerId,
                        Name: playerRank.PlayerName,
                        Rank: playerRank.GameRank,
                        PointsScored: playerRank.PointsScored,
                        RankScored: playerRank.GameRank
                    });
                });
                this._viewModel.GameNotes = model.Notes;
                this._viewModel.WinnerType = model.WinnerType;
                this._viewModel.PlayedGameId = model.PlayedGameId;
            }

            this.component = new Vue({
                el: componentSelector,
                data: {
                    viewModel: this._viewModel,
                    currentStep: editMode ? this._steps.SetResult : this._steps.SelectDate,
                    searchingGameDefinition: false,
                    searchingBGG: false,
                    alertVisible: false,
                    alertText: '',
                    newPlayerName: '',
                    serverRequestInProgress: false,
                    recentlyPlayedGameId: null,
                    editMode: editMode
                },
                computed: {
                    newPlayedGameUrl: function () {
                        return "/PlayedGame/Details/" + this.recentlyPlayedGameId;
                    }
                },
                methods: {
                    changeStep : function(step) {
                        this.currentStep = step;
                        window.scrollTo(0,0);
                    },
                    hideAlert: function () {
                        this.alertVisible = false;
                    },
                    setDateYesterday: function () {
                        this.viewModel.Date = moment().add(-1,"days").startOf("day").format("YYYY-MM-DD");
                        this.gotoSelectGame();
                    },
                    setDateToday: function () {
                        this.viewModel.Date = moment().startOf("day").format("YYYY-MM-DD");
                        this.gotoSelectGame();
                    },
                    gotoSelectGame: function () {
                        if (this.viewModel.Date) {
                            this.alertVisible = false;
                            this.viewModel.Date = moment(this.viewModel.Date).startOf("day").format("YYYY-MM-DD");
                            this.changeStep(parent._steps.SelectGame);
                        } else {
                            this.alertText = "You must set the played game or use the yesterday/today buttons.";
                            this.alertVisible = true;
                        }
                    },
                    selectGame: function (id, name, bggid) {
                        this.viewModel.Game = {
                            Id: id,
                            BoardGameGeekGameDefinitionId: bggid,
                            Name: name
                        };
                        this.changeStep(parent._steps.SelectPlayers);
                    },
                    backToSelectDate: function () {
                        if (this.viewModel.Date) {
                            parent.gaObject.trackGAEvent("PlayedGames", "Back", "BackToSelectDate", this.currentStep);
                            this.changeStep(parent._steps.SelectDate);
                        }
                    },
                    backToSelectGame: function () {
                        if (this.viewModel.Game) {
                            this.viewModel.Game = null;
                            parent.gaObject.trackGAEvent("PlayedGames", "Back", "BackToSelectGame", this.currentStep);
                            this.changeStep(parent._steps.SelectGame);
                        }
                    },
                    backToSelectPlayers: function () {
                        if (this.viewModel.Players.length > 1 && this.viewModel.Game != null) {
                            parent.gaObject.trackGAEvent("PlayedGames", "Back", "BackToSelectPlayers", this.currentStep);
                            this.changeStep(parent._steps.SelectPlayers);
                        }
                    },
                    createNewPlayer: function () {
                        if (this.newPlayerName) {
                            var player = {
                                PlayerName: this.newPlayerName,
                                Selected: true
                            };
                            this.viewModel.RecentPlayers.push(player);
                            $(".recent-players").addClass("animated pulse");
                            $(".recent-players").one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
                                $(this).removeClass("animated pulse");
                            });
                            this.newPlayerName = "";


                            parent.gaObject.trackGAEvent("PlayedGames", "SetPlayers", "CreatedNewPlayer");
                        }
                    },
                    gotoSetGameResult: function () {
                        var _this = this;

                        this.viewModel.Players = [];
                        this.alertText = "";
                        this.alertVisible = false;

                        var i = 1;

                        this.viewModel.RecentPlayers.forEach(function(player) {
                            if (player.Selected) {
                                _this.viewModel.Players.push({
                                    Id: player.PlayerId,
                                    Name: player.PlayerName,
                                    Rank: i,
                                    PointsScored: 0,
                                    RankScored: 1
                                });
                                i++;
                            }
                        });

                        this.viewModel.OtherPlayers.forEach(function (player) {
                            if (player.Selected) {
                                _this.viewModel.Players.push({
                                    Id: player.PlayerId,
                                    Name: player.PlayerName,
                                    Rank: i,
                                    PointsScored: 0,
                                    RankScored: 1
                                });
                                i++;
                            }
                        });

                        if (i > 1) {
                            this.changeStep(parent._steps.SetResult);
                        } else {
                            this.alertText = "You must select at least 2 players to continue.";
                            this.alertVisible = true;
                        }
                    },
                    setGameType: function(gameType) {
                        this.viewModel.GameType = gameType;
                    },
                    changeRank: function ($index, player, increase) {
                        var newRank;

                        var elementMoved = $("[data-index=" + $index + "]");
                        elementMoved.addClass("animated pulse");
                        elementMoved.one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function() {
                            $(this).removeClass("animated pulse");
                        });

                        if (increase) {
                            if (player.Rank === 1) {
                                return;
                            }
                            newRank = player.Rank - 1;
                        } else {
                            newRank = player.Rank + 1;
                        }

                        var replacingPlayer = null;

                        this.viewModel.Players.forEach(function (p) {
                            if (p.Rank === newRank) {
                                replacingPlayer = p;
                                return;
                            }
                        });

                        if (replacingPlayer) {
                            replacingPlayer.Rank = player.Rank;
                        }
                        player.Rank = newRank;
                    },
                    isLastRank: function (player) {
                        var hasMoreRankThanOtherPlayer = false;
                        var hasLessRankThanOtherPlayer = false;
                        this.viewModel.Players.forEach(function (p) {
                            if (player.Name !== p.Name) {
                                if (player.Rank >= p.Rank) {
                                    hasMoreRankThanOtherPlayer = true;
                                } else {
                                    hasLessRankThanOtherPlayer = true;
                                }
                            }
                        });
                        return hasMoreRankThanOtherPlayer && !hasLessRankThanOtherPlayer;
                    },
                    focus: function(e) {
                        e.target.select();
                    }, 
                    recalculateRankScored: function(e) {
                        var playersByScore = this.viewModel.Players
                            .sort(function (a, b) { return b.PointsScored - a.PointsScored });

                        var currentRank = 1;
                        for (var i = 1; i <= playersByScore.length; i++) {
                            var currentPlayer = playersByScore[i - 1];
                            var nextPlayer = playersByScore[i];

                            currentPlayer.RankScored = currentRank;

                            if (nextPlayer && nextPlayer.PointsScored < currentPlayer.PointsScored) {
                                currentRank++;
                            }
                        }
                    },
                    setGameResult: function (winnerType) {
                        var component = this;
                        this.serverRequestInProgress = true;

                        this.alertVisible = false;

                        var form = $('#__AjaxAntiForgeryForm');
                        var token = $('input[name="__RequestVerificationToken"]', form).val();

                        this.viewModel.WinnerType = winnerType;

                        var data = {
                            __RequestVerificationToken: token,
                            GameDefinitionId: this.viewModel.Game.Id,
                            GameDefinitionName: this.viewModel.Game.Name,
                            BoardGameGeekGameDefinitionId: this.viewModel.Game.BoardGameGeekGameDefinitionId,
                            Notes: this.viewModel.GameNotes,
                            DatePlayed: this.viewModel.Date,
                            WinnerType: this.viewModel.WinnerType,
                            PlayerRanks: [],
                            PlayedGameId: this.viewModel.PlayedGameId,
                            EditMode: this.editMode ? true : false
                        };

                        
                        this.viewModel.Players.forEach(function (player) {
                            var rank = player.Rank;
                            if (component.viewModel.WinnerType === parent._winnerTypes.TeamWin) {
                                rank = 1;
                            } else if (component.viewModel.WinnerType === parent._winnerTypes.TeamLoss) {
                                rank = 2;
                            } else if (component.viewModel.GameType === parent._gameTypes.Scored) {
                                rank = player.RankScored;
                            }
                            data.PlayerRanks.push({
                                PlayerId: player.Id,
                                GameRank: rank,
                                PlayerName: player.Name,
                                PointsScored: player.PointsScored
                            });
                        });

                        $.ajax({
                            type: "POST",
                            url: "/playedgame/save",
                            data: data,
                            success: function (response) {
                                component.serverRequestInProgress = false;
                                if (response.success) {
                                    component.recentlyPlayedGameId = response.playedGameId;
                                    component.currentStep = parent._steps.Summary;
                                } else {

                                    if (response.errors) {
                                        response.errors.forEach(function (e) {
                                            component.alertText += " - ";
                                            component.alertText += e.Value[0];
                                        });
                                    } else {
                                        component.alertText = "Error creating played game. Please, try again later :_(";
                                    }

                                    component.alertVisible = true;
                                }
                            },
                            error: function (XMLHTttpRequest, status, error) {
                                component.serverRequestInProgress = false;
                                component.alertText = "Error creating played game. Please, try again later :_(";
                                component.alertVisible = true;
                            }
                        });
                    },
                    gotoRecentlyPlayedGame: function () {
                        window.location = this.newPlayedGameUrl;
                        parent.gaObject.trackGAEvent("PlayedGames", "Summary", "GoToRecentlyCreatedPlayedGame", this.newPlayedGameUrl);
                    },
                    postTweet: function () {
                        var url = "https://nemestats.com" + this.newPlayedGameUrl + "&utm_source=twitter&utm_medium=tweet&utm_campaign=recentlycreatedplayedgame";
                        var twitterurl = "https://twitter.com/intent/tweet?hashtags=boardgames&original_referer=" + "encodeURIComponent(url)" + "&ref_src=twsrc%5Etfw&related=nemestats&text=Check%20out%20this%20game%20I%20played%20on%20%40nemestats&tw_p=tweetbutton&url=" + url;
                        parent.gaObject.trackGAEvent("PlayedGames", "Summary", "Tweet", url);
                        window.open(twitterurl);
                    },
                    reload: function () {
                        location.reload();
                    }
                }
            });



        }

    }
};