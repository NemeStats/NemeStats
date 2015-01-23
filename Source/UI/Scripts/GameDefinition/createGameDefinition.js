//Namespace
Namespace("Views.GameDefinition");

//Initialization
Views.GameDefinition.Create = function () {
	var parent = null;
	var $form = null;
	var $gameNameInput = null;
	var $gameDescriptionInput = null;
	var $boardGameId = null;
	var $btnCreateGame = null;
	var $returnUrl = null;

};
//Implementation
Views.GameDefinition.Create.prototype = {
	init: function () {
		parent = this;
		this.$form = $("form");
		this.$gameNameInput = $("#gameNameInput");
		this.$gameDescriptionInput = $("#gameDescriptionInput");
		this.$boardGameId = $("#boardGameId");
		this.$btnCreateGame = $("#btnCreateGame");
		this.$form = $('form');
		this.$token = $('input[name="__RequestVerificationToken"]', this.$form).val();

		var gameDefinitionAutoComplete = new Views.GameDefinition.GameDefinitionAutoComplete();
		gameDefinitionAutoComplete.init(this.$gameNameInput, this.$boardGameId);

		var shared = new Views.Shared.Layout();
		this.$returnUrl = shared.getQueryString("returnUrl");

		this.$btnCreateGame.on("click", function () {
			parent.createGame();
		});
	},
	createGame: function () {
		$.ajax({
			type: "POST",
			url: "/GameDefinition/Create",
			dataType: "json",
			data: {
				__RequestVerificationToken: this.$token,
				name: this.$gameNameInput.val(),
				description: this.$gameDescriptionInput.val(),
				boardGameGeekObjectId: this.$boardGameId.val(),
				returnUrl: this.$returnUrl
			},
			success: function (response) {
					window.location = response["url"] + "?gameId=" + response["gameId"];
			},
			error: function (err) {
				alert("Error " + err.status + ":\r\n" + err.statusText);
			}
		});
	},
}

$(document).ready(function () {
	var gameDefinitionCreator = new Views.GameDefinition.Create();

	gameDefinitionCreator.init();
});