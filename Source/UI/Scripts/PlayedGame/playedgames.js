//Usings
Namespace("Views.PlayedGame");

//Initialization
Views.PlayedGame.PlayedGames = function () {
};

//Implementation
Views.PlayedGame.PlayedGames.prototype = {
    init: function () {
        $(".clickable-row > tr").click(function() {
            var element = this;
            window.location = element.getAttribute("data-details-url");
        });
    }
}