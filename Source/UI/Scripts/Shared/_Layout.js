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
                $item.popover({ trigger: 'click', content: content, placement: 'top', container: "body" });
                $item.addClass("clickable");
            } else {
                $item.popover({ trigger: 'hover',container: "body" });
            }
        });


        this.$readmoreContainers = $("[data-readmore='true']");
        this.$readmoreContainers.readmore({ collapsedHeight: 100, lessLink: '<a href="#">Read less</a>', speed: 200 });
    },
    getQueryString: function (queryParamName) {
        var result = window.location.search.match(
			new RegExp("(\\?|&)" + queryParamName + "(\\[\\])?=([^&]*)")
		);

        return result ? result[3] : false;
    }
};

$(document).ready(function () {
    var layout = new Views.Shared.Layout();
    layout.init();
});