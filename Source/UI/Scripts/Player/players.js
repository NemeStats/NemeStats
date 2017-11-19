//Usings
Namespace("Views.Player");

//Initialization
Views.Player.Players = function () {
    this.$container = null;
    this.$playersTable = null;
};

//Implementation
Views.Player.Players.prototype = {
    init: function () {
        this.$container = $(".playersList");
        this.$playersTable = this.$container.find("table");
    },

    onPlayerSaved: function (player) {
        this.$playersTable.find("tr:last").after(
            "<tr>\
	            <td class=\"player-name-col\" data-name=\"" + player.Name + "\">\
		            <b><a href=\"/Player/Details/" + player.Id + "\">" + player.Name + "</a></b>\
	            </td>\
	            <td class=\"total-nemepoints-col\" data-nemepoints=\"0\">\
		            <span data-toggle=\"popover\" data-html=\"true\" data-templateselector=\"#34906\" data-title=\"NemePoints breakdown\">\
			            0\
		            </span>\
	            </td>\
	            <td class=\"played-games-col\">0</td>\
	            <td class=\"avg-nemepoints-col\">0.00</td>\
	            <td class=\"overall-win-col\">\
		            0 %\
	            </td>\
	            <td class=\"championed-games-col\">\
		            0\
	            </td>\
	            <td class=\"achievements-col\" data-achievements=\"0\">\
		            <a href=\"/Player/Details/" + player.Id + "\">\
			            <span class=\"achievements\">\
				            <span class=\"achievement bronze\" data-toggle=\"popover\" data-placement=\"top\" data-content=\"Bronze Achievements\">\
					            <span class=\"circle\">\
						            <span class=\"content\">0</span>\
					            </span>\
				            </span>\
				            <span class=\"achievement silver\" data-toggle=\"popover\" data-placement=\"top\" data-content=\"Silver Achievements\">\
					            <span class=\"circle\">\
						            <span class=\"content\">0</span>\
					            </span>\
				            </span>\
				            <span class=\"achievement gold\" data-toggle=\"popover\" data-placement=\"top\" data-content=\"Gold Achievements\">\
					            <span class=\"circle\">\
						            <span class=\"content\">0</span>\
					            </span>\
				            </span>\
			            </span>\
		            </a>\
	            </td>\
	            <td class=\"nemesis-col\" data-nemesis=\" -\">\
	            </td>\
	            <td>\
		            <input class=\"check-box\" disabled=\"disabled\" type=\"checkbox\">\
                    <b>\
                        <a href=\"/Player/InvitePlayer/" + player.Id + "\" title=\"Invite this player to your gaming group\">\
                            <span class=\"fa fa-envelope\" aria-label=\"Invite this player to your gaming group\"></span>\
                        </a>\
                    </b>\
                </td>\
	            <td class=\"edit-col\">\
		            <a href=\"/Player/Edit/" + player.Id + "\" title=\"Edit\">\
			            <i class=\"fa fa-pencil fa-2x\"></i>\
		            </a>\
	            </td>\
           </tr>");
    }
}