Namespace("Views.Shared.GoogleAnalytics");

Views.Shared.GoogleAnalytics = function () {
    this._lastAction = null;
    this._lastCategory = null;
    this._lastLabel = null;
    this._lastValue = null;
}

Views.Shared.GoogleAnalytics.prototype = {
    trackGAEvent: function (category, action, label, value) {
        if (category == null) {
            console.log("ERROR: The category is required.");
            return;
        }

        if (action == null) {
            console.log("ERROR: The action is required");
            return;
        }

        ga('send', 'event', category, action, label, value);

        this._lastCategory = category;
        this._lastAction = action;
        this._lastLabel = label;
        this._lastValue = value;
    }
}