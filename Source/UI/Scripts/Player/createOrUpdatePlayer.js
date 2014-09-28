//Usings
window.Views = window.Views || {};
window.Views.Player = Views.Player || {};

//Initialization
window.Views.Player.CreateOrUpdate = function () {
    this.$container = null;
    this.$cbActiveGroup = null;
    this.$btnSubmit = null;
    this.$form = null;
    this.$playerNameInput = null;
    this.formAction = null;
    this.onPlayerSaved = null;
};

//Implementation
window.Views.Player.CreateOrUpdate.prototype = {
    init: function (onPlayerSaved) {
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
        this.onPlayerSaved = onPlayerSaved;
        
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
                    owner.onPlayerSaved(player);
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