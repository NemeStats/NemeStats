//Namespace
Namespace("Views.GameDefinition");

//Initialization
Views.GameDefinition.Create = function () {
	var $form = null;
	var $gameNameInput = null;
	var $boardGameId = null;
};
//Implementation
Views.GameDefinition.Create.prototype = {
	init: function () {
		this.$form = $("form");
		this.$gameNameInput = $("#gameNameInput");
		this.$boardGameId = $("#boardGameId");
	
		var gameDefinitionAutoComplete = new Views.GameDefinition.GameDefinitionAutoComplete();
		gameDefinitionAutoComplete.init(this.$gameNameInput, this.$boardGameId);	
	}
}

$(document).ready(function () {
	var gameDefinitionCreator = new Views.GameDefinition.Create();

	gameDefinitionCreator.init();
});