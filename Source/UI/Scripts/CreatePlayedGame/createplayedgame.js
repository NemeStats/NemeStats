//Usings
Namespace("Views.PlayedGame");

//Initialization
Views.PlayedGame.CreatePlayedGame = function () {
	this._playerRank = 1;
	this._playerIndex = 0;
	this.$recordPlayedGameForm = null;
	this.$rankedPlayers = null;
	this.$rankedPlayersListItem = null;
	this.$playerId = null;
	this.$players = null;
	this.$playersErrorContainer = null;
	this.$playersErrorRankTooHigh = null;
	this.$playerFormData = null;
	this.$btnAddPlayer = null;
	this.$addPlayer = null;
	this.$datePicker = null;
	this.$playerItemTemplate = null;
	this._googleAnalytics = null;
	this._rankButtons = null;
	this.$gameDefinitionDropDown = null;
	this.$gameId = null;
	this.$btnEveryoneWonButton = null;
	this.$btnEveryoneLostButton = null;
};

//Implementation
Views.PlayedGame.CreatePlayedGame.prototype = {
	//Method definitions
	init: function (gaObject) {
		//Fields
		var parent = this;
		this._rankButtons = [];
		this.$recordPlayedGameForm = $("#recordPlayedGame");
		this.$playersErrorContainer = $("#players-error");
		this.$playersErrorRankTooHigh = $("#players-error-rank-too-high");
		this.$rankedPlayers = $("#rankedPlayers");
		this.$players = $("#Players");
		this.$playerFormData = $("#playerFormData");
		this.$playerDiv = $("#playerDiv");
		this.$addPlayer = $("#addPlayer");
		this.$btnAddPlayer = $("#btnAddPlayer");
		this.$anchorAddPlayer = $("#addPlayerAnchor");
		this.$gameDefinitionDropDown = $("[name='GameDefinitionId']");
		this.$datePicker = $(".date-picker").datepicker({
			showOn: "button",
			buttonText: "<i class='fa fa-calendar'></i>",
			showButtonPanel: true,
			maxDate: new Date(),
			minDate: new Date(2014, 1, 1)
		}).datepicker("setDate", new Date());
		this.$btnEveryoneWonButton = $("#btnEveryoneWonButton");
		this.$btnEveryoneLostButton = $("#btnEveryoneLostButton");

		this._googleAnalytics = gaObject;

		this.$recordPlayedGameForm.on("submit", $.proxy(parent.validatePlayers, parent));

		var currentRank = this.$rankedPlayers.attr("data-numRankedPlayers");
		
		if (currentRank > 0)
			this._playerRank = currentRank;

		//Event handlers
		this.$players.change(function () { parent.addPlayer(); });
		this.$rankedPlayers.sortable({
			stop: function () {
				parent.onReorder();
			}
		});

		this.$btnAddPlayer.on("click", function () {
			if (parent.$addPlayer.hasClass("hidden")) {
				parent.$addPlayer.removeClass("hidden");
			} else {
				parent.$addPlayer.addClass("hidden");
			}
			document.location = parent.$anchorAddPlayer.attr("href");
			parent._googleAnalytics.trackGAEvent("PlayedGames", "AddNewPlayerClicked", "AddNewPlayerClicked");
		});

		this.$btnEveryoneWonButton.on("click", function () {
			if (confirm("Are you sure you want to give every player a win?")) {
				$(".playerRank,input[name$='.GameRank'").val(1);
				parent._googleAnalytics.trackGAEvent("PlayedGames", "TeamWinRecorded", "TeamWinRecorded");
				return true;
			} else {
				return false;
			}
		});

		this.$btnEveryoneLostButton.on("click", function () {
			if (confirm("Are you sure you want to give every player a loss?")) {
				$(".playerRank,input[name$='.GameRank'").val(2);
				parent._googleAnalytics.trackGAEvent("PlayedGames", "TeamLossRecorded", "TeamLossRecorded");
				return true;
			} else {
				return false;
			}
		});

		this.$playerItemTemplate = $("#player-item-template");

		var shared = new Views.Shared.Layout();
		this.$gameId = shared.getQueryString("gameId");

		parent.validateGameDefinition();
	},
	onReorder: function () {
		var parent = this;
		this.$rankedPlayersListItem = $("#rankedPlayers li");

		var removePlayerButtons = $(".btnRemovePlayer");
		removePlayerButtons.off('click').on("click", function () {
			parent.removePlayer(this);
		});

		this.recalculateRanks();
		this._googleAnalytics.trackGAEvent("PlayedGames", "PlayersReordered", "PlayersReordered");
	},
	generatePlayerRankListItemString: function (playerIndex, playerId, playerName, playerRank) {
		var template = Handlebars.compile(this.$playerItemTemplate.html());
		var context = { playerIndex: playerIndex, playerId: playerId, playerName: playerName, playerRank: playerRank };

		return template(context);
	},
	addPlayer: function () {
		var parent = this;
		var selectedOption = this.$players.find(":selected");
		this.$playersErrorContainer.addClass("hidden");
		this.$playersErrorRankTooHigh.addClass("hidden");

		if (selectedOption.text() == "Add A Player") {
			return alert("You must pick a player.");
		}

		var playerId = selectedOption.val();
		var playerName = selectedOption.text();

		var playerItem = null;
		$.ajax({
			url: "/PlayedGame/AddPlayer/",
			type: "GET",
			data: { PlayerId: playerId, PlayerName: playerName, GameRank: this._playerRank },
			success: function (response) {
				this.$rankedPlayers = $("#rankedPlayers");
				playerItem = response;

				this.$rankedPlayers.append(playerItem);
			},
			error: function (err) {
				alert("Error " + err.status + ":\r\n" + err.statusText);
			},
		});
		
		this._playerIndex++;
		this._playerRank++;
		selectedOption.remove();

		var buttonUp = $(".rankButton-up");
		var buttonDown = $(".rankButton-down");
		buttonUp.off("click").on("click", function () {
			parent.movePlayerUp(this);
		});

		buttonDown.off("click").on("click", function () {
			parent.movePlayerDown(this);
		});

		var removePlayerButtons = $(".btnRemovePlayer");
		removePlayerButtons.off('click').on("click", function () {
			parent.removePlayer(this);
		});

		this.recalculateRanks();

		return null;
	},
	removePlayer: function (button) {
		var playerId = $(button).data("playerid");
		var playerName = $(button).data("playername");
		$(button).parents('li').remove();
		this._playerRank--;
		var newPlayer = $('<option value="' + playerId + '">' + playerName + '</option>');
		this.$players.append(newPlayer);
		this.recalculateRanks();

		this._googleAnalytics.trackGAEvent("PlayedGames", "PlayerRemoved", "PlayerRemoved");
	},
	movePlayerUp: function (button) {
		var item = $(button).closest("li");
		var previous = item.prev();

		if (previous.length > 0) {
			item.insertBefore(previous);
			this.recalculateRanks();
			this._googleAnalytics.trackGAEvent("PlayedGames", "PlayersReorderedViaArrow", "PlayersReorderedViaArrow");
		}
	},
	movePlayerDown: function (button) {
		var item = $(button).closest("li");
		var next = item.next();

		if (next.length > 0) {
			item.insertAfter(next);
			this.recalculateRanks();
			this._googleAnalytics.trackGAEvent("PlayedGames", "PlayersReorderedViaArrow", "PlayersReorderedViaArrow");
		}
	},
	recalculateRanks: function () {
		var playerItems = this.$rankedPlayers.children();
		var idx = 1;
		for (var i = 0; i < playerItems.length; i++) {
			var playerItem = $(playerItems[i]);
			var inputTextField = playerItem.find("input:text");
			var inputHiddenField = playerItem.find("input:hidden");
			inputTextField.val(idx);
			inputTextField.attr("name", "PlayerRanks[" + i + "].GameRank");
			inputHiddenField.attr("name", "PlayerRanks[" + i + "].PlayerId");
			idx++;
		}
	},
	onPlayerCreated: function (player) {
		var newPlayer = $('<option value="' + player.Id + '">' + player.Name + '</option>');
		this.$players.append(newPlayer);

		this._googleAnalytics.trackGAEvent("PlayedGames", "NewPlayerAdded", "NewPlayerAdded");
	},

	validatePlayers: function (event) {
		if (this.$rankedPlayers.children().length < 2) {
			this.$playersErrorContainer.removeClass("hidden");
			return false;
		}
		this.$playersErrorContainer.addClass("hidden");

		var allRankValues = this.$rankedPlayers.children().map(function () {
			return $(this).find("input[type=text]").val();
		});
		if (Math.max.apply(Math, allRankValues) > this.$rankedPlayers.children().length) {
			this.$playersErrorRankTooHigh.removeClass("hidden");
			return false;
		}
		this.$playersErrorRankTooHigh.addClass("hidden");

		return true;
	},

	validateGameDefinition: function () {
		if ($("#gameDefinitionDropDown").data("numofdefinitions") == 0) {
			$("#gameDefinitionDropDown").popover({ html: true });
			$("#gameDefinitionDropDown").popover("show");
			$(":input").prop("disabled", true);
		} else if (this.$gameId != "")
			this.$gameDefinitionDropDown.val(this.$gameId);
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