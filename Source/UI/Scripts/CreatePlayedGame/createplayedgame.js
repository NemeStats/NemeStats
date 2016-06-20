//Usings
Namespace("Views.PlayedGame");

//Initialization
Views.PlayedGame.CreatePlayedGame = function () {
    this._steps = {
        SelectDate: 1,
        SelectGame: 2,
        SelectPlayers: 3,
        SetResult: 4,
        Confirm: 5
    };

    this._viewModel = {
        Date: null,
        Game: null,
        Players: []
    };

    this.component = null;
};

//Implementation
Views.PlayedGame.CreatePlayedGame.prototype = {
    init: function () {
        this.setupDatePicker();
        this.setupAutocomplete();
        this.setupMultiselect();
        this.configureViewModel();
    },
    setupMultiselect: function () {
        var parent = this;

        $("#optgroup").multiselect({
            keepRenderingSort: true,
            afterMoveToRight: function ($left, $right, $options) {
                $.each($options, function (i, $option) {
                    parent.component.$data.viewModel.Players.push({
                        Id: $option.value,
                        Name: $option.text
                    });
                });

            },
            afterMoveToLeft: function ($left, $right, $options) {
                $.each($options, function (i, $option) {
                    $.each(parent.component.$data.viewModel.Players, function (j, player) {
                        if (player.Id == $option.value) {
                            parent.component.$data.viewModel.Players.pop(player);
                            return;
                        }
                    });
                });

            }
        });
    },
    setupDatePicker: function () {
        this.$datePicker = $(".date-picker");

        var minDate = new Date(2000, 0, 1);
        var currentMoment = moment();

        if (Modernizr.inputtypes.date) {
            //if supports HTML5 then use native date picker
            var minDateIso8601 = minDate.toISOString().split("T")[0];
            this.$datePicker.attr("max", currentMoment.add("days", 1).format("YYYY-MM-DD"));
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
    }, setupAutocomplete: function () {
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
                    'there are no game matching this text',
                  '</div>'
                ].join('\n'),
            }
        })
            .bind('typeahead:select', function (ev, suggestion) {
                parent.component.selectGame(suggestion.ID, suggestion.Name);
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
                  '<div class="empty-message">',
                    'there are no game matching this text',
                  '</div>'
                ].join('\n'),
                suggestion: Handlebars.compile('<div><strong>{{BoardGameName}}</strong> – {{YearPublished}}</div>')
            },
        })
            .bind('typeahead:select', function (ev, suggestion) {
                parent.component.selectGame(null, suggestion.BoardGameName);
            }).bind('typeahead:asyncrequest', function (ev, suggestion) {
                parent.component.$data.searchingBGG = true;
            });
    },
    configureViewModel: function () {
        var parent = this;

        var componentSelector = "#record-played-game";
        var container = $(componentSelector);
        if (container) {

            this.component = new Vue({
                el: componentSelector,
                data: {
                    viewModel: this._viewModel,
                    currentStep: this._steps.SelectDate,
                    searchingGameDefinition: false,
                    searchingBGG: false,
                    alertVisible: false,
                    alertText: ''
                },
                methods: {
                    hideAlert: function () {
                        this.alertVisible = false;
                    },
                    setDateYesterday: function () {
                        this.viewModel.Date = moment().add("days", -1).startOf("day");
                        this.gotoSelectGame();
                    },
                    setDateToday: function () {
                        this.viewModel.Date = moment().startOf("day");
                        this.gotoSelectGame();
                    },
                    gotoSelectGame: function () {
                        if (this.viewModel.Date) {
                            this.viewModel.Date = moment(this.viewModel.Date).startOf("day");
                            this.currentStep = parent._steps.SelectGame;
                        }
                    },
                    backToSelectDate: function () {
                        if (this.viewModel.Date) {
                            this.viewModel.Date = this.viewModel.Date.format("YYYY-MM-DD");
                            this.currentStep = parent._steps.SelectDate;
                        }
                    },
                    selectGame: function (id, name) {
                        this.viewModel.Game = {
                            Id: id,
                            Name: name
                        };
                        this.currentStep = parent._steps.SelectPlayers;
                    },
                    backToSelectGame: function () {
                        if (this.viewModel.Game) {
                            this.viewModel.Game = null;
                            this.currentStep = parent._steps.SelectGame;
                        }
                    },
                    gotoSetGameResult: function () {
                        var _this = this;
                        var playersSelected = $("#optgroup_to option");
                        if (playersSelected.length > 1) {

                            this.viewModel.Players = [];

                            $.each(playersSelected, function (i, $option) {
                                _this.viewModel.Players.push({
                                    Id: $option.value,
                                    Name: $option.text
                                });
                            });

                            this.currentStep = parent._steps.SetResult;
                        } else {
                            this.alertText = "You must select at least 2 players to continue.";
                            this.alertVisible = true;
                        }
                    },
                    backToSelectPlayers: function () {
                        this.currentStep = parent._steps.SelectPlayers;
                    }
                }
            });



        }

    }
};