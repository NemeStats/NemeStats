//Namespace
Namespace("Views.GameDefinition");

//Initialization
Views.GameDefinition.Create = function () {
	//var $container = null;
	var $form = null;
	var $gameDefinitionInput = null;
	var $boardGameId = null;
	var $btnCreateGame = null;

};
//Implementation
Views.GameDefinition.Create.prototype = {
	init: function () {
		//this.$container = (".createGameDefinitionForm");
		this.$form = $("form");
		this.$gameDefinitionInput = $("#gameDefinitionInput");
		this.$boardGameId = this.$form.find("input[type='hidden']");
		this.$btnCreateGame = $("#btnCreateGame");
		this.$form = $('form');
		this.$token = $('input[name="__RequestVerificationToken"]', this.$form).val();

		var gameDefinitionAutoComplete = new Views.GameDefinition.GameDefinitionAutoComplete();
		gameDefinitionAutoComplete.init(this.$gameDefinitionInput, this.$boardGameId);

		this.$btnCreateGame.on("click", function () {
			this.createGame();
		});
	},
	createGame: function () {
		$.ajax({
			type: "POST",
			url: "/GameDefinition/Create",
			dataType: "json",
			data: {
				__RequestVerificationToken: this.$token
			},
			success: function () {
				alert("successful");
			},
			error: function (err) {
				alert("Error " + err.status + ":\r\n" + err.statusText);
			}
		});
	}
}

$(document).ready(function () {
	var gameDefinitionCreator = new Views.GameDefinition.Create();

	gameDefinitionCreator.init();
});