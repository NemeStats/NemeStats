//Namespace
Namespace("Views.GameDefinition");

//Initialization
Views.GameDefinition.Create = function () {
	var parent = null;
	var formAction = null;
	var queryString = null;
	var $btnSubmit = null;
	var $container = null;
	var $form = null;
	var $token = null;
};

//Implementation
Views.GameDefinition.Create.prototype = {
	init: function () {
		parent = this;
		parent.formAction = "/GameDefinition/Create";
		$btnSubmit = $("#btnCreateGame");
		$form = $('#__AjaxAntiForgeryForm');
		$token = $('input[name="__RequestVerificationToken"]', $form).val();
	
		$btnSubmit.on("click", function () {
			parent.determineRedirect();
		});
	},
	getQueryString: function(queryParamName) {
		var result =  window.location.search.match(
			new RegExp("(\\?|&)" + queryParamName + "(\\[\\])?=([^&]*)")
		);

		return result ? result[3] : false;
	},
	determineRedirect: function () {
		this.queryString = this.getQueryString("returnUrl");
		$form = $("form").serialize();
		$.ajax({
			type: "POST",
			url: parent.formAction,
			data: {
				__RequestVerificationToken: $token,
				gameDefinition: this.$form,
				returnUrl: this.queryString
			},
			dataType: "text",
			success: function (response) {
				alert(response["url"]);
			},
			error: function (err) {
				alert("Error " + err.status + ":\r\n" + err.statusText);
			}
		});
	}
};

$(document).ready(function () {
	var initialize = new Views.GameDefinition.Create();

	initialize.init();
});