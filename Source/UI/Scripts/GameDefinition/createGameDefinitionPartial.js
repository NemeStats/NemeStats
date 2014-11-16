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
	this._results = null;
    this._titles = null;
	this._serviceUrl = "/GameDefinition/SearchBoardGameGeekHttpGet";
};

//Implementation
Views.GameDefinition.CreateGameDefinitionPartial.prototype = {
	init: function () {
	    var owner = this;
	    this._titles = {};
		this.formAction = "/gamedefinition/save";
		this.$container = $(".createGameDefinitionPartial");
		this.$form = this.$container.find("form");
		this.$form.attr('action', this.formAction);
		this.$gameNameInput = this.$form.find("input[type='text']");
	    this.$boardGameId = this.$form.find("input[type='hidden']");
		this.$createBtn = this.$form.find("button");
		this.$createBtn.click(function (e) {
			e.preventDefault();
			owner.createGameDefinition();
		});

		this.$gameNameInput.autocomplete({
		    minLength: 3,
		    source: $.proxy(owner.getGameName, owner),
		    select: $.proxy(owner.onItemSelected, owner),
            change: $.proxy(owner.onInputChange, owner)
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
	onItemSelected: function (event, ui) {
	    event.preventDefault();

	    var item = ui.item;
	    this.$boardGameId.val(item.value);
	    this.$gameNameInput.val(item.label);
	},
	onInputChange: function (event, ui) {
	    var textEntered = this.$gameNameInput.val();
	    if (textEntered.length == 0) {
	        this._titles = {};
	    }

	    if (!this._titles[textEntered]) {
	        this.$boardGameId.val("");
	    } else {
	        this.$boardGameId.val(this._titles[textEntered]);
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
			    owner._results = [];

			    for (var item in data) {
			        owner._results.push({
			            label: data[item].BoardGameName + " (" + data[item].YearPublished + ")",
			            value: data[item].BoardGameId
			        });
			        owner._titles[data[item].BoardGameName + " (" + data[item].YearPublished + ")"] = data[item].BoardGameId;
			    }
			    response(owner._results);
			},
			error: function (err) {
				alert("Error " + err.status + ":\r\n" + err.statusText);
			},
			dataType: "json"
		});
	}
}