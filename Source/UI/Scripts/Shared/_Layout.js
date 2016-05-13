//Usings
Namespace("Views.Shared");

//Initialization
Views.Shared.Layout = function () {
    this.$input = null;
    this.$popovers = null;
    this.$readmoreContainers = null;
};

//Implementation
Views.Shared.Layout.prototype = {
    init: function () {
        this.$popovers = $('[data-toggle="popover"]');
        $.each(this.$popovers, function (i, item) {
            var $item = $(item);
            if ($item.data("templateselector")) {
                var content = $($item.data("templateselector")).html();
                $item.popover({ trigger: 'click', content: content, placement: 'top', container: "body", viewport: $item });
                $item.addClass("clickable");
            } else {
                $item.popover({ trigger: 'hover',container: "body" });
            }
        });


        this.$readmoreContainers = $("[data-readmore='true']");
        this.$readmoreContainers.readmore({ collapsedHeight: 100, lessLink: '<a href="#">Read less</a>', speed: 200 });

        PNotify.prototype.options.styling = "bootstrap3";
        PNotify.prototype.options.styling = "fontawesome";
    },
    getQueryString: function (queryParamName) {
        var result = window.location.search.match(
			new RegExp("(\\?|&)" + queryParamName + "(\\[\\])?=([^&]*)")
		);

        return result ? result[3] : false;
    },
    setupHubConnection : function() {
        var currentUserContainer = $("#currentuserid[data-id]");
        if (currentUserContainer.length === 1) {
            var currentUserId = currentUserContainer.data("id");


            var signalrConnection = $.hubConnection();
            var notificationsHub = signalrConnection.createHubProxy("notificationsHub");

            notificationsHub.on('NewAchievementUnlocked', function(achievementId, achievementLevel) {
                var text = "ID: " + achievementId + " - Level: " + achievementLevel;
                var noty = new PNotify({
                    title: 'New Achievement unlocked',
                    text: text,
                    type: "success"
                });
            });

            signalrConnection.start().done(function() {
                notificationsHub.invoke("join", currentUserId);
            });
        }
    }
};

$(document).ready(function () {
    var layout = new Views.Shared.Layout();
    layout.init();
    //layout.setupHubConnection();
});