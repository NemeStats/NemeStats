(function ($) {
    $.fn.rank = function (options) {
        var settings = $.extend({
            votableFeatureId: null,
            serviceGet: "/api/votablefeatures/",
            servicePost: "/api/votablefeatures/"
        }, options);

        var upvoteValue = 0;
        var downVoteValue = 0;
        var gotError = false;
        function receiveValues() {
            $.ajax({
                type: "GET",
                url: settings.serviceGet + settings.votableFeatureId,
                success: function (data) {
                    upvoteValue = data.NumberOfUpvotes;
                    downVoteValue = data.NumberOfDownvotes;
                },
                error: function (err) {
                    gotError = true;
                },
                async: false,
                dataType: "json"
            });
        }

        function sendValues(direction) {
            $.ajax({
                type: "POST",
                url: settings.servicePost,
                data: { VotableFeatureId: settings.votableFeatureId, VoteUp: direction },
                success: function (data) {
                    
                },
                error: function (err) {
                    alert(err);
                },
                dataType: "json"
            });
        }

        if (gotError) {
            return this;
        }
        receiveValues();

        var $originalElement = $(this);
        var template = "<div class=\"rank-item\" id=" + settings.votableFeatureId.toString() + "> \
            <a href=\"#\" class=\"upvote-link\">\
                <i class=\"fa fa-thumbs-up\" data-container=\"body\" data-toggle=\"popover\" data-placement=\"right\" \
        data-content=\"I like this feature. Give me more like this!\"></i>\
            </a>\
            <span class=\"upvote-count\">"+ upvoteValue.toString() +"</span> \
            <span class=\"downvote-count\">-" + downVoteValue.toString() + "</span> \
            <a href=\"#\" class=\"downvote-link\">\
                <i class=\"fa fa-thumbs-down\" data-container=\"body\" data-toggle=\"popover\" data-placement=\"right\" \
        data-content=\"I don't care for this feature. Focus on improving the site elsewhere.\"></i> \
            </a>\
        </div>";

        $originalElement.html(template);
        $originalElement.find('[data-toggle="popover"]').popover({ trigger: 'hover' });

        var cookieValues = {};
        var cookieName = "rankCookie";
        if (!$.cookie(cookieName)) {
            $.cookie(cookieName, JSON.stringify(cookieValues), { path: '/', expires: 20 });
        } else {
            cookieValues = JSON.parse($.cookie(cookieName));
        }

        var elementId = settings.votableFeatureId != null ? settings.votableFeatureId : $originalElement.attr("id");
        var $upVote = $originalElement.find(".upvote-link");
        var $downVote = $originalElement.find(".downvote-link");
        var upCount = $originalElement.find(".upvote-count");
        var downCount = $originalElement.find(".downvote-count");
        var voteClickHandler = function (e, countElement, decrease) {
            e.preventDefault();
            if (cookieValues[elementId]) {
                return;
            }

            var upCnt = parseInt(countElement.text(), 10);
            var downCnt = parseInt(countElement.text().replace("-", ""), 10);

            if (decrease === false) {
                upCnt++;
                countElement.text(upCnt);
            } else {
                downCnt++;
                countElement.text("-" + downCnt.toString());
            }

            sendValues(!decrease);
            cookieValues[elementId] = true;
            $.cookie(cookieName, JSON.stringify(cookieValues), { path:'/', expires: 20 });
        };

        $upVote.on("click", function (e) {
            voteClickHandler(e, upCount, false);
        });

        $downVote.on("click", function (e) {
            voteClickHandler(e, downCount, true);
        });

        return this;
    };
}(jQuery));