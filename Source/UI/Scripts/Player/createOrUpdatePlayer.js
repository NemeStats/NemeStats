//Usings
window.Views = window.Views || {};
window.Views.Player = Views.Player || {};

//Initialization
window.Views.Player.CreateOrUpdate = function () {
    this.$container = null;
    this.$cbActiveGroup = null;
    this.$btnSubmit = null;
    this.$form = null;
    this.$playersTable = null;
    this.$playerNameInput = null;
    this.formAction = null;
};

//Implementation
window.Views.Player.CreateOrUpdate.prototype = {
    init: function () {
        var owner = this;
        this.formAction = "/player/save";
        this.$container = $(".createOrUpdatePartial");

        this.$cbActiveGroup = this.$container.find(".activeGroup");
        this.$cbActiveGroup.hide();

        this.$form = this.$container.find("form");
        this.$form.attr('action', this.formAction);
        this.$form.on("submit", function(e) {
            owner.preventDefaultSubmit(e);
        });

        this.$playerNameInput = this.$form.find("#Name");
        this.$playersTable = this.$container.parent().parent().parent().find("table");

        this.$btnSubmit = this.$container.find("#btnSubmit");
        this.$btnSubmit.on("click", function () {
            owner.createPlayer();
        });
    },

    preventDefaultSubmit : function (e) {
        e.preventDefault();
    },

    createPlayer: function () {
        var owner = this;
        if (this.$form.valid())
        {
            $.ajax({
                type: "POST",
                url: this.formAction,
                data: this.$form.serialize(),
                success: function (player) {
                    owner.$playersTable.find("tr:last").after(
                        "<tr> \
                                <td> \
                                    <a href='/Player/Details/" + player.Id + "'>" + player.Name + "</a> \
                                </td> \
                                <td> \
                                    <a href='/Player/Edit/" + player.Id + " title='Edit'> \
                                        <i class='fa fa-pencil fa-3x'></i> \
                                    </a> \
                                </td> \
                            </tr>");
                    owner.$playerNameInput.val('');
                },
                error: function (err) {
                    alert("Error " + err.status + ":\r\n" + err.statusText);
                },
                dataType: "json"
            });
        }
    }
};