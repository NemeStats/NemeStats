//Usings
window.Views = window.Views || {};
window.Views.Player = Views.Player || {};

//Initialization
window.Views.Player.Players = function () {
    this.$container = null;
    this.$playersTable = null;
};

//Implementation
window.Views.Player.Players.prototype = {
    init: function () {
        this.$container = $(".playersList");
        this.$playersTable = this.$container.find("table");
    },

    onPlayerSaved: function (player) {
        this.$playersTable.find("tr:last").after(
            "<tr> \
                <td> \
                    <a href='/Player/Details/" + player.Id + "'>" + player.Name + "</a> \
                </td> \
                <td> \
                    <a href='/Player/Edit/" + player.Id + "' title='Edit'> \
                        <i class='fa fa-pencil fa-3x'></i> \
                    </a> \
                </td> \
            </tr>");
    }
}