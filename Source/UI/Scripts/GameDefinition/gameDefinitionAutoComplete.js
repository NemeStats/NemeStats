//Usings
Namespace("Views.GameDefinition");

//Initialization
Views.GameDefinition.GameDefinitionAutoComplete = function() {
    this.$textBox = null;
    this.$boardGameId = null;
    this._titles = null;
    this._results = null;
    this._serviceUrl = "/GameDefinition/SearchBoardGameGeekHttpGet";
};

Views.GameDefinition.GameDefinitionAutoComplete.prototype = {
    init: function (textBoxObject, boardGameIdHiddenInput) {
        var owner = this;
        this._titles = {};
        this.$textBox = textBoxObject;
        this.$boardGameId = boardGameIdHiddenInput;
        this.$textBox.autocomplete({
            minLength: 3,
            source: $.proxy(owner.getGameName, owner),
            select: $.proxy(owner.onItemSelected, owner),
            change: $.proxy(owner.onInputChange, owner)
        });
    },
    onItemSelected: function (event, ui) {
        event.preventDefault();

        var item = ui.item;
        this.$boardGameId.val(item.value);
        this.$textBox.val(item.label);
    },
    onInputChange: function (event, ui) {
        var textEntered = this.$textBox.val();
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
        this.$textBox.addClass("autocomplete-loading");
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
            complete: function () {
                owner.$textBox.removeClass("autocomplete-loading");
            },
            dataType: "json"
        });
    }
}