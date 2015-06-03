//Usings
Namespace("Views.PlayedGame");

//Initialization
Views.PlayedGame.Search = function () {
	this.$datePicker = null;
};

//Implementation
Views.PlayedGame.Search.prototype = {
	//Method definitions
	init: function() {
        //Fields
        var parent = this;
        this.$datePicker = $(".date-picker").datepicker({
            showOn: "button",
            buttonText: "<i class='fa fa-calendar'></i>",
            showButtonPanel: true,
            minDate: new Date(2014, 1, 1)
        });
    }
}; //end prototypes