    //Usings
    Namespace("Views.PlayedGame");

    //Initialization
    Views.PlayedGame.CreatePlayedGame = function () {
        this._playerRank = 1;
        this._playerIndex = 0;
        this.$rankedPlayers = null;
        this.$rankedPlayersListItem = null;
        this.$playerId = null;
        this.$players = null;
        this.$playerFormData = null;
        this.$btnAddPlayer = null;
        this.$addPlayer = null;
        this.$datePicker = null;
    };

    //Implementation
    Views.PlayedGame.CreatePlayedGame.prototype = {

        //Method definitions
        init: function () {
            //Fields
            var parent = this;
            this.$rankedPlayers = $("#rankedPlayers");
            this.$players = $("#Players");
            this.$playerFormData = $("#playerFormData");
            this.$playerDiv = $("#playerDiv");
            this.$addPlayer = $("#addPlayer");
            this.$btnAddPlayer = $("#btnAddPlayer");
            this.$anchorAddPlayer = $("#addPlayerAnchor");
            this.$datePicker = $(".date-picker").datepicker({
                showOn: "button",
                buttonText: "<i class='fa fa-calendar'></i>",
                showButtonPanel: true,
                maxDate: new Date(),
                minDate: new Date(2014, 1, 1)
            }).datepicker("setDate", new Date());

            //Event handlers
            this.$players.change(function () { parent.addPlayer(); });
            this.$rankedPlayers.sortable({
                stop: function() {
                    parent.onReorder();
                }
            });

            this.$btnAddPlayer.on("click", function() {
                if (parent.$addPlayer.hasClass("hidden")) {
                    parent.$addPlayer.removeClass("hidden");
                } else {
                    parent.$addPlayer.addClass("hidden");
                }
                document.location = parent.$anchorAddPlayer.attr("href");
            });
        },
        onReorder: function () {
            var parent = this;

            this.$rankedPlayersListItem = $("#rankedPlayers li");
            this.$rankedPlayersListItem.each(function (index, value) {
                var listItem = $(value);
                var playerId = listItem.data("playerId");
                var rank = index + 1;
                $("#" + playerId).val(rank);

                var playerName = listItem.data("playerName");
                listItem.html(parent.generatePlayerRankListItemString(index, playerId, playerName, rank));
            });
        },
        generatePlayerRankListItemString: function (playerIndex, playerId, playerName, playerRank) {

            return "<span style='cursor:pointer'>" +
                   "<div class='alert alert-info' role='alert' style='max-width:300px;'>" + playerName + " - Rank: " +
						"<button style='padding: 3px 4px; margin-left:5px;' type='button' class='btn btn-default btnRemovePlayer fl-right' data-playerid='" + playerId + "' data-playername='" + playerName + "' title='Remove player'>" +
							"<i class='fa fa-minus'></i>" +
						"</button>" +
						"<input class='fl-right' type='text' id='" + playerId + "' name='PlayerRanks[" + playerIndex + "].GameRank' value='" + playerRank + "' style='text-align:center;'/>" +
						"<input type='hidden' name='PlayerRanks[" + playerIndex + "].PlayerId' value='" + playerId + "'/>" +
                   "</div></span>";
        },
        addPlayer: function () {
            var parent = this;
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

            var removePlayerButtons = $(".btnRemovePlayer");
            removePlayerButtons.off('click').on("click", function () {
                parent.removePlayer(this);
            });

            return null;
        },
        removePlayer: function (data) {
        	var playerId = $(data).data("playerid");
        	var playerName = $(data).data("playername");
        	var player = {Id: playerId, Name: playerName};
        	this.onPlayerCreated(player);

	      	$("#li" + playerId).remove();
        },
        onPlayerCreated: function (player) {
            var newPlayer = $('<option value="' + player.Id + '">' + player.Name + '</option>');
            this.$players.append(newPlayer);
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

