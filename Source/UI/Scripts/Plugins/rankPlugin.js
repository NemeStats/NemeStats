(function ($) {
    var settings = {
        serviceGet: null,
        servicePost: null
    };

    $.fn.rank = function (options) {
        settings = options;
        var upvoteValue = 0;
        var downVoteValue = 0;
        function receiveValues() {
            //TODO: Ajax request that gets the values for the widget
            upvoteValue = 0; //here we should put the received value
            downVoteValue = 0; //received value again
        }

        function sendValues(upvote, downvote) {
            //TODO: put an AJAX request to Jake's service that sends the values
            //The service addresses are passed from outside - check the settings.serviceGet and settings.servicePost
        }

        receiveValues();

        var $originalElement = $(this);
        var template = "<div class=\"rank-item\"> \
            <a href=\"#\" class=\"upvote-link\"> \
                <i class=\"fa fa-thumbs-up\"></i> \
            </a> \
            <span class=\"upvote-count\">"+ upvoteValue.toString() +"</span> \
            <span class=\"downvote-count\">-" + downVoteValue.toString() + "</span> \
            <a href=\"#\" class=\"downvote-link\"> \
                <i class=\"fa fa-thumbs-down\"></i> \
            </a> \
        </div>";

        $originalElement.html(template);
        var cookieValues = {};
        var cookieName = "rankCookie";
        if (!$.cookie(cookieName)) {
            $.cookie(cookieName, JSON.stringify(cookieValues), { path: '/', expires: 20 });
        } else {
            cookieValues = JSON.parse($.cookie(cookieName));
        }

        var elementId = $originalElement.attr("id");
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

            sendValues(upCnt, downCnt);
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