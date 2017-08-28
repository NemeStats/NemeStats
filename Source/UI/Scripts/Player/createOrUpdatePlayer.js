//Usings
Namespace("Views.Player");

//Initialization
Views.Player.CreateOrUpdate = function () {
    this.$container = null;
    this.$cbActiveGroup = null;
    this.$btnSubmitCreatePlayer = null;
    this.$form = null;
    this.$playerNameInput = null;
    this.$errorContainer = null;
    this.formAction = null;
    this.onPlayerSaved = null;
};

//Implementation
Views.Player.CreateOrUpdate.prototype = {
    init: function (onPlayerSaved) {
        var owner = this;
        this.formAction = "/player/save";
        this.$container = $(".createOrUpdatePartial");
        this.$errorContainer = this.$container.find(".field-validation-valid");
        this.$cbActiveGroup = this.$container.find(".activeGroup");
        this.$cbActiveGroup.hide();

        this.$form = this.$container.find("form");
        this.$form.attr('action', this.formAction);
        this.$form.on("submit", function(e) {
            owner.preventDefaultSubmit(e);
        });

        this.$form.removeData("validator")
            .removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(this.$form);

        this.$playerNameInput = this.$form.find("#Name");
        this.onPlayerSaved = onPlayerSaved;
        
        this.$btnSubmitCreatePlayer = this.$container.find("#btnSubmitCreatePlayer");
        this.$btnSubmitCreatePlayer.on("click", function () {
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
                    owner.$errorContainer.switchClass("field-validation-valid", "field-validation-error");
                    owner.$errorContainer.html(err.statusText);
                    console.log("Error " + err.status + ":\r\n" + err.statusText);
                },
                dataType: "json"
            });
        }
    }
};