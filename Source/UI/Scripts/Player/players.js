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
            "<tr> \
                <td> \
                    <b><a href='/Player/Details/" + player.Id + "'>" + player.Name + "</a></b> \
                </td> \
                <td><input class=\"check-box\" disabled=\"disabled\" type=\"checkbox\"> </td>\
                <td> </td>\
                <td> </td>\
                <td> \
                    <a href='/Player/UpdateGamingGroupName/" + player.Id + "' title='UpdateGamingGroupName'> \
                        <i class='fa fa-pencil fa-2x'></i> \
                    </a> \
                </td> \
            </tr>");
    }
}