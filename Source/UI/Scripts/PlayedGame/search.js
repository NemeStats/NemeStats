//Usings
Namespace("Views.PlayedGame");

//Initialization
Views.PlayedGame.Search = function () {
    this.$datePicker = null;
};

//Implementation
Views.PlayedGame.Search.prototype = {
    //Method definitions
    init: function () {
        //Fields
        if (!Modernizr.inputtypes.date) {
            // If not native HTML5 support, fallback to jQuery datePicker
            this.$datePicker = $(".date-picker").datepicker({
                showOn: "button",
                buttonText: "<i class='fa fa-calendar'></i>",
                showButtonPanel: true,
                minDate: new Date(2014, 1, 1)
            });
        }
        
    }
}; //end prototypes