var AjaxTimeout = AjaxTimeout || 60000;

var Utils = new (function () {

	var self = this;


	this.modalQuestion = function (title, text, callback, confirmButtonText, cancelButtonText) {

		if (confirmButtonText == null)
			confirmButtonText = 'Si, aceptar';

		if (cancelButtonText == null)
			cancelButtonText = 'No, cancelar';

		swal({
			title: title,
			text: text,
			icon: "warning",
			buttons: [cancelButtonText, confirmButtonText]
		}).then((result) => {

			if (callback != undefined)
				callback(result);
		});
	}

	this.modalError = function (title, text) {
		swal({

			title: title,
			text: text,
			icon: "error",
		});
	}

	this.modalInfo = function (title, text, timerOut, buttons, callback) {

		swal({
			title: title,
			text: text,
			timer: timerOut === undefined ? 5000 : timerOut,
			icon: "success",
			button: buttons === undefined ? "Aceptar" : buttons
		}).then((value) => {
			if (callback != null)
				callback(value);
		});

	}

	this.modalCallback = function (title, text, callback) {

		swal(title, text, "success").then((value) => {
			if (callback != null)
				callback();
		});
	}

	this.modalWait = function (text, cancelCallback) {
		swal({
			title: "<div class='uil-ring-css' style='transform:scale(0.77);'><div style='margin-left: 170px;'></div></div>",
			text: text,
			cancelButtonText: "Cancelar",
			showCancelButton: true,
			showConfirmButton: false,
			closeOnConfirm: false,
			closeOnCancel: false,
			html: true
		}, function (isConfirm) {
			if (cancelCallback != undefined && isConfirm == false)
				cancelCallback();
		});
	}

	this.modalLoader = function (text) {

		var div = document.createElement('div');
		div.innerHTML = "<div class='uil-ring-css' style='transform:scale(0.77);'><div style='margin-left: 170px;'></div></div>";

		swal({
			text: text,
			buttons: false,
			content: div
		});
	}

	this.modalClose = function () {
		swal.close();
	}

	this.ajaxFormSuccess = function (context) {
		console.log('ajaxFormSuccess');
	}

	this.ajaxFormFailure = function (context) {
		console.error('Error en la llamada ajax:', context);

		if (context.status == 400) {

			Ajax.ParseResponse(context.responseJSON,
				undefined,
				function (response) {

					new Noty({
						type: 'error',
						theme: 'bootstrap-v4',
						text: response,
						timeout: 5000,
						layout: 'topCenter',
						animation: {
							open: 'animated bounceInDown',
							close: 'animated fadeOut'
						},
						killer: true
					}).show();

				}
			);

		}
		else if (context.status == 401)
			BaseLayoutView.expiredSession();

	}

	this.showAjaxError = function (jsonResponse, textStatus) {
		var errorText;
		try {
			if (textStatus === 'timeout')
				errorText = "Tiempo de respuesta superado";
			else
				errorText = JSON.parse(jsonResponse).error;
		}
		catch (err) {
			errorText = "No encontrado";
		}
		Utils.modalError("Error", errorText);
	}

	this.setupAjax = function () {

		//Timeout de 30 segundos
		$.ajaxSetup({ timeout: AjaxTimeout });
	}

	this.alertSuccess = function (message, options) {
		this.alert(message, 'success', { timeout: 2000, ...options });
	}

	this.alertError = function (message, options) {
		this.alert(
			message || 'Se produjo un error grave',
			'error',
			{
				closeWith: ['button'],
				timeout: false,
				...options
			}
		);
	}

	this.alertInfo = function (message, options) {
		this.alert(message, 'info', { timeout: 5000, ...options });
	}

	this.alert = function (message, type, options) {
		new Noty({
			type: type,
			theme: 'bootstrap-v4',
			animation: {
				open: 'animated bounceInRight',
				close: 'animated bounceOutRight'
			},
			killer: true,

			text: message,
			...options
		}).show();
	}

	this.setupValidator = function () {

		$.validator.methods.date = function (value, element) {
			return this.optional(element) || moment(value, "DD/MM/YYYY", true).isValid();
		};

		$.validator.methods.range = function (value, element, param) {
			if ($(element).attr('data-val-date')) {
				var min = $(element).attr('data-val-range-min');
				var max = $(element).attr('data-val-range-max');
				var date = moment(value, 'DD/MM/YYYY', true);
				var minDate = moment(min, 'MM/DD/YYYY HH:mm:ss', true);
				var maxDate = moment(max, 'MM/DD/YYYY HH:mm:ss', true);
				return this.optional(element) || date.isBetween(minDate, maxDate, 'day', '[]');
			}
			// use the default method
			return this.optional(element) || (value >= param[0] && value <= param[1]);
		};

		$.validator.methods.number = function (value, element) {
			return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d+)+)?(?:,\d+)?$/.test(value);
		}
	}

	this.resetValidator = function ($form) {

		if ($form.length == 0)
			return;

		$form.get(0).reset();

		$form.find('[aria-describedby*="tooltip"]').tooltip('dispose');

		//reset jQuery Validate's internals
		var validator = $form.data('validator');
		if (validator !== null && validator !== undefined) {
			//limpio la propiedad ignore antes de resetear para que no se ignore
			//ningún elemento del formulario al invocar resetForm().
			//Luego reestablezco la propiedad ignore.
			var ignoreBackup = validator.settings.ignore;
			validator.settings.ignore = '';
			validator.resetForm();
			validator.settings.ignore = ignoreBackup;
        }

		$form.find('input.valid')
			.removeClass('valid');

		$form.find('input.input-validation-error')
			.removeClass('input-validation-error');

		//reset unobtrusive validation summary, if it exists
		$form.find("[data-valmsg-summary=true]")
			.removeClass("validation-summary-errors")
			.addClass("validation-summary-valid")
			.find("ul").empty();

		//reset unobtrusive field level, if it exists
		$form.find("[data-valmsg-replace]")
			.removeClass("field-validation-error")
			.addClass("field-validation-valid")
			.empty();

		$form.find('.select2-field')
			.select2('val', '')
			.trigger('change');

	}

	this.revalidateField = function (form, id) {
		var validator = $(form).data('validator');
		validator.element('#' + id);
	}

	this.rebuildValidator = function (form, ignore) {

		$(form).removeData("validator");
		$(form).removeData("unobtrusiveValidation");
		$.validator.unobtrusive.parse(form);

		if (ignore != undefined && ignore != '' && ignore.length > 0)
			$(form).validate().settings.ignore = ignore;
	}

});
