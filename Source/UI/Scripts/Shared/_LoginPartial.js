//Usings
Namespace("Views.Shared");

//Initialization
Views.Shared.LoginPartial = function () {
    this.$gamingGroupSwitcher = null;
    this.$currentGamingGroup = null;
    this._cookieName = "gamingGroupsCookie";
    this._getGamingGroupsService = "/GamingGroup/GetUsersGamingGroups";
};

//Implementation
Views.Shared.LoginPartial.prototype = {
    init: function () {
        this.$gamingGroupSwitcher = $("#gamingGroupSwitcher");
        this.$currentGamingGroup = $("#currentGamingGroup");

        if (!$.cookie(this._cookieName)) {
            this.getGamingGroups();
        } else {
            this.setGamingGroups(JSON.parse($.cookie(this._cookieName)));
        }
        
    },
    getGamingGroups: function () {
        var parent = this;
        $.ajax({
            type: "POST",
            url: parent._getGamingGroupsService,
            async: false,
            success: function (data) {
                $.cookie(parent._cookieName, JSON.stringify(data), { expires: 1, path: '/' });
            },
            error: function (err) {
                console.log("Error " + err.status + ":\r\n" + err.statusText);
            },
            dataType: "json"
        });
    },
    setGamingGroups: function (gamingGroups) {
        var parent = this;
        this.$gamingGroupSwitcher.empty();

        for (var i = 0; i < gamingGroups.length; i++) {
            var gamingGroup = gamingGroups[i];
            if (gamingGroup.IsCurrentGamingGroup == true) {
                this.$currentGamingGroup.html(gamingGroup.Name);
            }
            var gamingGroupItem = "<li><a href=# class='gamingGroupItem' data-groupid='"+ gamingGroup.Id +"'>"+ gamingGroup.Name +"</a></li>";
            this.$gamingGroupSwitcher.append(gamingGroupItem);
            $(".gamingGroupItem").off('click').on('click', function () {
                parent.switchGamingGroup(this);
            });
        }
    },
    switchGamingGroup: function (gamingGroupItem) {
        $.cookie(this._cookieName, null, { path: '/', expires: -5 });
        $('<form action="/GamingGroup/SwitchGamingGroups" method="POST"/>')
        .append($('<input type="hidden" name="gamingGroupId" value="' + $(gamingGroupItem).data("groupid") + '">'))
        .appendTo($(document.body))
        .submit();
    }
};

$(document).ready(function () {
    var loginPartial = new Views.Shared.LoginPartial();
    loginPartial.init();
});