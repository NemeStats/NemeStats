//Usings
Namespace("Views.GamingGroup");

//Initialization
Views.GamingGroup.GamingGroupView = function () {
    this.$container = null;
    this.$title = null;
    this._serviceAddress = "/GamingGroup/Edit";
    this._googleAnalytics = null;
};

//Implementation
Views.GamingGroup.GamingGroupView.prototype = {
    init: function (gaObject) {
        var parent = this;
        this.$title = $("#gamingGroupTitle");
        this.$title.toEditBox({ onFocusOut: $.proxy(parent.renameGamingGroup, this) });
        this._googleAnalytics = gaObject;
    },
    renameGamingGroup: function (element) {
        var parent = this;
        $.ajax({
            type: "POST",
            url: parent._serviceAddress,
            data: { "gamingGroupName": element.value },
            success: function (data) {

            },
            error: function (err) {
                alert("Error " + err.status + ":\r\n" + err.statusText);
            },
            dataType: "json"
        });

        this._googleAnalytics.trackGAEvent("GamingGroup", "GamingGroupRenamed", "GamingGroupRenNamed");
    }
}