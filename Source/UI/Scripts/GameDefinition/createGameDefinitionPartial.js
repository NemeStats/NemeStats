//Usings
Namespace("Views.GameDefinition");

//Initialization
Views.GameDefinition.CreateGameDefinitionPartial = function () {
    this.$container = null;
    this.$form = null;
    this.$createBtn = null;
    this.$gameNameInput = null;
    this.$gamesTable = null;
    this.$boardGameId = null;
    this.onDefinitionCreated = null;
    this.formAction = null;
};

//Implementation
Views.GameDefinition.CreateGameDefinitionPartial.prototype = {
    init: function () {
        var owner = this;
        this.formAction = "/gamedefinition/ajaxcreate";
        this.$container = $(".createGameDefinitionPartial");
        this.$form = this.$container.find("#js-createGameForm");
        this.$form.attr('action', this.formAction);
        this.$gameNameInput = this.$form.find("#gameNameInput");
        this.$boardGameId = this.$form.find("input[type='hidden']");
        this.$createBtn = this.$form.find("button");
        this.$createBtn.click(function (e) {
            e.preventDefault();
            owner.createGameDefinition();
        });

        var gameDefinitionAutoComplete = new Views.GameDefinition.GameDefinitionAutoComplete();
        gameDefinitionAutoComplete.init(this.$gameNameInput, this.$boardGameId);
    },
    createGameDefinition: function () {
        var owner = this;
        if (this.$form.valid()) {
            $.ajax({
                type: "POST",
                url: this.formAction,
                data: this.$form.serialize(),
                success: function (gamingGroupId) {
                    owner.onDefinitionCreated(gamingGroupId);
                    owner.$gameNameInput.val('');
                },
                error: function (err) {
                    alert(err.statusText);
                },
                dataType: "json"
            });
        }
    },
    configureViewModel: function () {
        var componentSelector = "#createGameDefinitionPartial";
        var gaminggroupcontainer = $(componentSelector);
        if (gaminggroupcontainer) {

            var component = new Vue({
                el: componentSelector,
                data: {
                    importProgressBarVisible: false,
                    currentGamesImported: 0,
                    totalGamesToImport: 0,
                    stepPercentage: 0,
                    currentGameName: ""
                },
                computed: {
                    progressWidth: function () {
                        if (this.stepPercentage && this.currentGamesImported) {
                            return this.stepPercentage * this.currentGamesImported + "%";
                        }
                        return 0;
                    }
                },
                methods: {
                    importbgggames: function () {
                        this.importProgressBarVisible = true;
                        var url = gaminggroupcontainer.find("[data-posturl]").data("posturl");
                        $.post(url);
                    }
                }
            });


            var signalrConnection = $.hubConnection();
            var longRunningTaskHub = signalrConnection.createHubProxy("longRunningTaskHub");

            longRunningTaskHub.on('BGGImportDetailsProgress', function (currentGamesImported, gamesToImport, currentGameName) {
                if (currentGamesImported === 0) {
                    component.$data.stepPercentage =  100 / gamesToImport;
                    component.$data.totalGamesToImport = gamesToImport;
                }
                component.$data.currentGamesImported = currentGamesImported;
                component.$data.currentGameName = currentGameName;
            });

            signalrConnection.start().done(function () {

                longRunningTaskHub.invoke("join", gaminggroupcontainer.data("gaminggroupid"));
            });
        }

    },
}