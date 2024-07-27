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

    this.previousUserRankingsAndScores = null;
};

//Implementation
Views.PlayedGame.CreatePlayedGame.prototype = {
    init: function () {
        this.setupDatePicker();
        this.configureViewModel();
        this.setupAutocomplete();
        this.setupPlayersDragAndDrop();

        this.gaObject = new window.Views.Shared.GoogleAnalytics();
    },
    setupPlayersDragAndDrop: function () {
        var parent = this;
        var rankedGameContainer = document.getElementById("ranked-game");
        var list = dragula([rankedGameContainer]);

        list.on("drop", function (el) {
            $.each($(el).parent().find("li"), function (i, player) {
                var $player = $(player);
                var key = $player.data("index");
                parent.component.setRank(key, i + 1);
                parent.gaObject.trackGAEvent("PlayedGames", "SetRank", "DragAndDrop", key);
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
                    'There are no matching games in your Gaming Group.',
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

            Vue.filter('winnertype',
                function (value) {
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

            Vue.filter('scoredposition',
                function(rank) {
                    var s = ["th", "st", "nd", "rd"],
                        v = rank % 100;
                    return rank + (s[(v - 20) % 10] || s[v] || s[0]);

                });

            Vue.filter('convertToLocalDate',
                function(isoDate) {
                    return moment(isoDate).format("LL");
                });

            var editMode = $(componentSelector).data("edit-mode");
            var model = $(componentSelector).data("model");

            if (editMode) {
                model.RecentPlayers.forEach(function(recentPlayer) {
                    recentPlayer.Selected = true;
                });
            }
            
            this._viewModel.RecentPlayers = model.RecentPlayers;
            this._viewModel.OtherPlayers = model.OtherPlayers;

            if (model.UserPlayer) {
                model.UserPlayer.Selected = true;
                model.UserPlayer.PlayerName += " (me)";
                this._viewModel.RecentPlayers.push(model.UserPlayer);
            }

            this._viewModel.Game = {};
            this._viewModel.CompletedSteps = {};

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
                parent.previousUserRankingsAndScores = model.PlayerRanks;// We back up our existing rankings and scores so we can merge with new players later.

                this._viewModel.GameNotes = model.Notes;
                this._viewModel.WinnerType = model.WinnerType;
                this._viewModel.PlayedGameId = model.PlayedGameId;
                this._viewModel.GameType = model.GameType;

                this._viewModel.CompletedSteps[this._steps.SelectDate] = true;
                this._viewModel.CompletedSteps[this._steps.SelectGame] = true;
                this._viewModel.CompletedSteps[this._steps.SelectPlayers] = true;
                this._viewModel.CompletedSteps[this._steps.SetResult] = true;
                this._viewModel.CompletedSteps[this._steps.Summary] = true;
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
                    editMode: editMode,
                    gameHasBeenRecorded: false
                },
                computed: {
                    newPlayedGameUrl: function () {
                        return "/PlayedGame/Details/" + this.recentlyPlayedGameId;
                    },
                    numberOfPlayersLabel: function () {
                        return this.viewModel.Players.length + " players";
                    },
                    orderedPlayers: function () {
                        // Uncomment this to replace dynamic sorting of players
                        if (editMode) {
                            return this.viewModel.Players.sort(function (player1, player2) {
                                //if (player1.RankScored < player2.RankScored) return -1;
                                //if (player1.RankScored > player2.RankScored) return 1;
                                return 0;
                            });
                        } else {
                            return this.viewModel.Players.sort(function (player1, player2) {
                                //if (player1.Rank < player2.Rank) return -1;
                                //if (player1.Rank > player2.Rank) return 1;
                                return 0;
                            });
                        }
                    }
                },
                methods: {                    
                    stepAlreadyCompleted: function(step) {
                        return this.viewModel.CompletedSteps[step];
                    },
                    changeStep: function (step) {
                        this.currentStep = step;
                        this.viewModel.CompletedSteps[step - 1] = true;
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
                        if ((!this.gameHasBeenRecorded) && this.viewModel.Date) {
                            parent.gaObject.trackGAEvent("PlayedGames", "Back", "BackToSelectDate", this.currentStep);
                            this.changeStep(parent._steps.SelectDate);
                        }
                    },
                    backToSelectGame: function () {
                        if ((!this.gameHasBeenRecorded) && this.viewModel.Game.Id) {
                            parent.gaObject.trackGAEvent("PlayedGames", "Back", "BackToSelectGame", this.currentStep);
                            this.changeStep(parent._steps.SelectGame);
                        }
                    },
                    backToSelectPlayers: function () {
                        if ((!this.gameHasBeenRecorded) && this.viewModel.Players.length > 0 && this.viewModel.Game !== null) {
                            parent.gaObject.trackGAEvent("PlayedGames", "Back", "BackToSelectPlayers", this.currentStep);
                            parent.previousUserRankingsAndScores = this.viewModel.Players;
                            this.changeStep(parent._steps.SelectPlayers);
                        }
                    },
                    backToSetResult: function () {
                        if ((!this.gameHasBeenRecorded) && this.viewModel.Players.length > 1 && this.viewModel.Game !== null) {
                            parent.gaObject.trackGAEvent("PlayedGames", "Back", "BackToSetResult", this.currentStep);
                            this.changeStep(parent._steps.SetResult);
                        }
                    },
                    createNewPlayer: function () {
                        if (this.newPlayerName) {
                            var owner = this;
                            $.ajax({
                                type: "POST",
                                url: "/player/save",
                                data: { 'Name': this.newPlayerName },
                                success: function (newPlayer) {
                                    var player = {
                                        PlayerName: owner.newPlayerName,
                                        PlayerId: newPlayer.Id,
                                        Selected: true
                                    };
                                    owner.viewModel.RecentPlayers.push(player);
                                    var recentPlayersElement = $(".recent-players");
                                    recentPlayersElement.addClass("animated pulse");
                                    recentPlayersElement.one("webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend", function () {
                                        $(this).removeClass("animated pulse");
                                    });
                                    owner.newPlayerName = "";

                                    parent.gaObject.trackGAEvent("PlayedGames", "SetPlayers", "CreatedNewPlayer");
                                },
                                error: function (err) {
                                    if (err.status === 409) {
                                        alert("There is already a player with this name in your Gaming Group!");
                                    } else {
                                        alert("There was an unexpected error saving this Player. Please try again or report the issue if it persists.");
                                    }
                                },
                                dataType: "json"
                            });
                        }
                    },
                    recalculateSelectedPlayers: function() {
                        var _this = this;

                        this.viewModel.Players = [];
                        this.alertText = "";
                        this.alertVisible = false;

                        var numberOfSelectedPlayers = 1;

                        this.viewModel.RecentPlayers.forEach(function (player) {
                            if (player.Selected) {
                                _this.viewModel.Players.push({
                                    Id: player.PlayerId,
                                    Name: player.PlayerName,
                                    Rank: numberOfSelectedPlayers,
                                    PointsScored: 0,
                                    RankScored: 1
                                });
                                numberOfSelectedPlayers++;
                            }
                        });

                        this.viewModel.OtherPlayers.forEach(function (player) {
                            if (player.Selected) {
                                _this.viewModel.Players.push({
                                    Id: player.PlayerId,
                                    Name: player.PlayerName,
                                    Rank: numberOfSelectedPlayers,
                                    PointsScored: 0,
                                    RankScored: 1
                                });
                                numberOfSelectedPlayers++;
                            }
                        });

                        this.mergePlayerResults();

                        return numberOfSelectedPlayers;
                    },
                    mergePlayerResults: function () {
                        // Merge existing scores back in if edit mode.
                        
                        var existingScores = parent.previousUserRankingsAndScores;
                        if (existingScores !== null){
                            this.viewModel.Players.forEach(function (_player) {
                                playerName = _player.Name;

                                existingScores.forEach(function (_oldPlayer) {
                                    if (_oldPlayer.Name === undefined) {
                                        oldName = _oldPlayer.PlayerName;
                                    }
                                    else {
                                        oldName = _oldPlayer.Name
                                    }                                    
                                    if (playerName === oldName) {
                                        _player.PointsScored = _oldPlayer.PointsScored;
                                    }
                                });
                            });

                            this.recalculateRankScored();
                        }
                    },
                    gotoSetGameResult: function () {
                        var numberOfSelectedPlayers = this.recalculateSelectedPlayers();

                        if (numberOfSelectedPlayers > 1) {
                            this.changeStep(parent._steps.SetResult);
                        } else {
                            this.alertText = "You must select at least 2 players to continue.";
                            this.alertVisible = true;
                        }
                    },
                    setGameType: function(gameType) {
                        this.viewModel.GameType = gameType;
                    },
                    setRank: function (key, rank) {
                        var owner = this;
                        this.viewModel.Players.forEach(function (player, index) {
                            if (player.Id === key) {
                                player.Rank = rank;
                                Vue.set(owner.viewModel.Players, index, player);
                                return;
                            }
                        });                        
                    },
                    changeRank: function (player, increase) {
                        var newRank;

                        //var elementMoved = $("[data-index=" + player.Id + "]");
                        //elementMoved.addClass("animated pulse");
                        //elementMoved.one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function() {
                        //    $(this).removeClass("animated pulse");
                        //});

                        if (increase) {
                            if (player.Rank === 1) {
                                return;
                            }
                            newRank = player.Rank - 1;
                        } else {
                            if (player.Rank + 1 > this.viewModel.Players.length) {
                                return;
                            }
                            newRank = player.Rank + 1;
                        }

                        //this.viewModel.Players.forEach(function (p) {
                        //    if (p.Rank === newRank) {
                        //        p.Rank = player.Rank;
                        //        return;
                        //    }
                        // });

                        player.Rank = newRank;
                    },
                    isLastRank: function (player) {
                        return (player.Rank >= this.viewModel.Players.length)
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
                                    component.viewModel.CompletedSteps[parent._steps.SetResult] = true;
                                    component.gameHasBeenRecorded = true;
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
            });//--end creating Vue component
        }
    }
};