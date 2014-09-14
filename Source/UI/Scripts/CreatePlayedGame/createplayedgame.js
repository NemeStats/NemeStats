    //Usings
    window.Views = window.Views || {};
    window.Views.PlayedGame = Views.PlayedGame || {};

    //Initialization
    window.Views.PlayedGame.CreatePlayedGame = function () {
        this._playerRank = 1;
        this._playerIndex = 0;
        this.$rankedPlayers = null;
        this.$rankedPlayersListItem = null;
        this.$playerId = null;
        this.$players = null;
        this.$playerFormData = null;
    };

    //Implementation
    window.Views.PlayedGame.CreatePlayedGame.prototype = {

        //Method definitions
        init: function () {
            //Fields
            var parent = this;
            this.$rankedPlayers = $("#rankedPlayers");
            this.$players = $("#Players");
            this.$playerFormData = $("#playerFormData");
            this.$playerDiv = $("#playerDiv");

            //Event handlers
            this.$players.change(function () { parent.addPlayer(); });
            this.$rankedPlayers.sortable({
                stop: function() {
                    parent.onReorder();
                }
            });
        },
        onReorder: function () {
            var parent = this;

            this.$rankedPlayersListItem = $("#rankedPlayers li");
            this.$rankedPlayersListItem.each(function (index, value) {
                var listItem = $(value);
                var playerId = listItem.attr("data-playerId");
                var rank = index + 1;
                $("#" + playerId).val(rank);

                var playerName = listItem.attr("data-playerName");
                listItem.html(parent.generatePlayerRankListItemString(index, playerId, playerName, rank));
            });
        },
        generatePlayerRankListItemString: function (playerIndex, playerId, playerName, playerRank) {

            return "<span style='cursor:pointer'>" +
                   "<div class='alert alert-info' role='alert' style='max-width:280px;'>" + playerName + " - Rank: " +
                    "<input type='text' id='" + playerId + "' name='PlayerRanks[" + playerIndex +"].GameRank' value='" + playerRank + "' style='text-align:center;'/>" +
                    "<input type='hidden' name='PlayerRanks[" + playerIndex + "].PlayerId' value='" + playerId + "'/>" +
                   "</div></span>";
        },
        addPlayer: function () {
            var selectedOption = this.$players.find(":selected");

            if (selectedOption.text() == "Add A Player") {
                return alert("You must pick a player.");
            }

            var playerId = selectedOption.val();
            var playerName = selectedOption.text();

            var playerItem = "<li id='li" + playerId +
                              "' data-playerId='" + playerId +
                              "' data-playerName='" + playerName +
                              "'>" + this.generatePlayerRankListItemString(this._playerIndex, playerId, playerName, this._playerRank) + "</li>";

            this.$rankedPlayers.append(playerItem);
            this._playerIndex++;
            this._playerRank++;
            selectedOption.remove();

            return null;
        },

        //Properties
        set_playerIndex: function (value) {
            this._playerIndex = value;
        },
        get_playerIndex: function () {
            return this._playerIndex;
        },
        set_playerRank: function (value) {
            this._playerRank = value;
        },
        get_playerRank: function () {
            return this._playerRank;
        }
    }; //end prototypes

