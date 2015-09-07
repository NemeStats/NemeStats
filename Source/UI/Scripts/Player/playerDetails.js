//Usings
Namespace("Views.Player");

//Initialization
Views.Player.Details = function () {
    this.$playerId = null;
};

//Implementation
Views.Player.Details.prototype = {
    init: function(playerId) {
        this.$playerId = playerId;
    },
    renderGameDefinitionsPieChart : function () {
        alert("Michael call your pie chart stuff here and use the $playerId as the playerId");
    }
};