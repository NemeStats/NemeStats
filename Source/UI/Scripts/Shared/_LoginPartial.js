//Usings
Namespace("Views.Shared");

//Initialization
Views.Shared.LoginPartial = function () {
    
};

//Implementation
Views.Shared.LoginPartial.prototype = {
    init: function () {
        alert("Tosho");
    }
};

$(document).ready(function () {
    var loginPartial = new Views.Shared.LoginPartial();
    loginPartial.init();
});