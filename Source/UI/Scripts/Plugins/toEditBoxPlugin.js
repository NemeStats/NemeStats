(function ($) {
    var settings = {
        onFocusOut: null,
        onFocusIn: null
    };

    var $previous = null;
    var $originalElement = null;

    function convertToEdit(element) {
        $previous = $(element).clone(true, true);
        var oldValue = $(element).text();
        var textBox = "<input type='text' id='" + element.id + "' />";
        $(element).replaceWith(textBox);
        element = $("#" + element.id);
        $(element).focus().val(oldValue);
        $(element).on("focusout", function (e) {
            e.preventDefault();
            convertToOrigional(this);
            $(this).off("focusout");

            if (settings.onFocusOut) {
                settings.onFocusOut(this);
            }
        });
    }

    function convertToOrigional(element) {
        $previous.text($(element).val());
        $(element).replaceWith($previous);
    }

    $.fn.toEditBox = function (options) {
        settings = options;
        $originalElement = $(this);
        $originalElement.on("click", function (e) {
            e.preventDefault();
            convertToEdit(this);
            if (settings.onFocusIn) {
                settings.onFocusIn(this);
            }
        });

        return this;
    };
}(jQuery));