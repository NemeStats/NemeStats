//Usings
Namespace("Views.GameDefinition");

//Initialization
Views.GameDefinition.CreateGameDefinitionPartial = function () {
	this.$container = null;
	this.$form = null;
	this.$createBtn = null;
	this.$gameNameInput = null;
	this.$gamesTable = null;
	this.onDefinitionCreated = null;
	this.formAction = null;
	var dictionary = null;
};

//Implementation
Views.GameDefinition.CreateGameDefinitionPartial.prototype = {
	init: function () {
		var owner = this;
		this.formAction = "/gamedefinition/save";
		this.$container = $(".createGameDefinitionPartial");
		this.$form = this.$container.find("form");
		this.$form.attr('action', this.formAction);
		this.$gameNameInput = this.$form.find("input[type='text']");
		this.$createBtn = this.$form.find("button");
		this.$createBtn.click(function (e) {
			e.preventDefault();
			owner.createGameDefinition();
		});

		this.$gameNameInput.on("input", function () {
			dictionary = owner.getGameName();
			owner.setAutoComplete(dictionary);
		});
	},
	createGameDefinition: function () {
		var owner = this;
		if (this.$form.valid()) {
			$.ajax({
				type: "POST",
				url: this.formAction,
				data: this.$form.serialize(),
				success: function (game) {
					owner.onDefinitionCreated(game);
					owner.$gameNameInput.val('');
				},
				error: function (err) {
					alert("Error " + err.status + ":\r\n" + err.statusText);
				},
				dataType: "json"
			});
		}
	},
	getGameName: function () {
		var inputText = $('#gameNameInput').val()	
		var results = [];
		var autocompleteDictionary = [];

		if (inputText.length >= 3) {
			$.ajax({
				url: "/GameDefinition/SearchBoardGameGeekHttpGet",
				type: "GET",
				async: false,
				data: { searchText: inputText },
				success: function (data) {
					results = data;
			
					for (var i = 0; i < results.length; i++)
						autocompleteDictionary.push({ 'value': results[i].BoardGameName, 'label': results[i].BoardGameName });	
				},
				error: function (err) {
					alert("Error " + err.status + ":\r\n" + err.statusText);
				},
				dataType: "json"
			});
		}
		return autocompleteDictionary;
	},
	setAutoComplete: function (dictionary) {
		$('#gameNameInput').autocomplete({
			minLength: 3,
			source: dictionary
		});
	}
}