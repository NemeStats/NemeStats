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
	this._serviceUrl = "/GameDefinition/SearchBoardGameGeekHttpGet";
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

		this.$gameNameInput.autocomplete({
		    minLength: 3,
		    source: $.proxy(owner.getGameName, owner)
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
	getGameName: function (request, response) {
	    var owner = this;
		$.ajax({
			url: owner._serviceUrl,
			type: "GET",
			async: true,
			data: { searchText: request.term },
			success: function (data) {
			    var result = [];
			    for (var item in data) {
			        result.push(data[item].BoardGameName + " (" + data[item].YearPublished + ")");
			    }
				response(result);
			},
			error: function (err) {
				alert("Error " + err.status + ":\r\n" + err.statusText);
			},
			dataType: "json"
		});
	}
}