Namespace("UIUtils");


UIUtils = function() {
    this.toggleVisibility = function (selector) {
        $(selector).fadeToggle();
    }
};

$(document).ready(function () {
    var uiUtils = new UIUtils();
    window.UIUtils = uiUtils;
});