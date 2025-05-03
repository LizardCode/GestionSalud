var AjaxTimeout = AjaxTimeout || 60000;

var Utils = new (function () {

	var self = this;
	this.errorTooltips = true;

	this.descargarDocumento = function (textoLoader, generationUrl, id, downloadUrl, urlParams, callback) {
		//if (textoLoader)
		//	Utils.modalWait(textoLoader);
		//else
		//	Utils.modalWait('Obteniendo...');
		Utils.modalLoader();

		var params;
		if (!urlParams)
			params = { id: id }
		else
			params = urlParams;

		$.ajax({
			type: 'POST',
			url: generationUrl,
			data: params,
			dataType: 'json',
			success: function (retobj) {
				Utils.modalClose();
				if (retobj.success) {
					window.location = downloadUrl + '?fileGUID=' + retobj.message + '&fileName=' + retobj.fileName;

					if (callback != undefined)
						callback();
				}
				else
					Utils.modalError('Error', retobj.message);
			},
			error: function (xhr, ajaxOptions, thrownError) {
				Ajax.ShowError(xhr, ajaxOptions, thrownError);
			}
		});
	};

	this.descargarDocumentoUsingGet = function (textoLoader, generationUrl, downloadUrl, callback) {
		if (textoLoader)
			Utils.modalWait(textoLoader);
		else
			Utils.modalWait('Obteniendo...');

		$.ajax({
			type: 'GET',
			url: generationUrl,
			success: function (retobj) {
				Utils.modalClose();
				if (retobj.success) {
					window.location = downloadUrl + '?fileGUID=' + retobj.message + '&fileName=' + retobj.fileName;

					if (callback != undefined)
						callback();
				}
				else
					Utils.modalError('Error', retobj.message);
			},
			error: function (xhr, ajaxOptions, thrownError) {
				Ajax.ShowError(xhr, ajaxOptions, thrownError);
			}
		});
	};

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
	this.modalWarning = function (title, text, timerOut, buttons, callback) {

		swal({
			title: title,
			text: text,
			timer: timerOut === undefined ? 5000 : timerOut,
			icon: "warning",
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

	this.modalLoader_OLD = function (text) {

		var div = document.createElement('div');
		div.innerHTML = "<div class='uil-ring-css' style='transform:scale(0.77);'><div style='margin-left: 170px;'></div></div>";

		swal({
			text: text,
			buttons: false,
			content: div
		});
	}

	this.modalLoader = function (text) {

		var div = document.createElement('div');
		div.innerHTML = '<img src="/img/slack.gif" alt="Estamos trabajando en su solicitud" class="modalWaitPeritacion" style="width:100px;">';

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
			if (AutoNumeric.isManagedByAutoNumeric(element))
				return true;
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

		$form.find("textarea")
			.removeClass("field-validation-error")
			.addClass("field-validation-valid")
			.empty();

		$form.find('.select2-field')
			.select2('data', null);

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


		var validator = $(form).data('validator');
		validator.settings.showErrors = function (errorMap, errorList) {
			console.log('validator showErrors', errorList);

			this.defaultShowErrors();
			//markTabToggle($(form));

			if (!self.errorTooltips)
				return;

			for (var i in errorList) {
				var item = errorList[i];
				var tooltipValidation = $(item.element).data('tooltip-validation');

				if (tooltipValidation !== true)
					continue;

				var tooltipContainer = item.element;
				var title = $(item.element).attr('title');

				if ($(item.element).hasClass('repeater-select2', 'validate', 'repeater-control'))
					tooltipContainer = $(item.element).prev('div.repeater-control.select2-container');

				if (title !== undefined && title != '' && title != item.message)
					$(tooltipContainer).tooltip('dispose');

				var method = 'show';

				if ($('div.tooltip.show').length > 0)
					method = undefined;

				$(tooltipContainer)
					.attr('title', item.message)
					.not('.show')
					.tooltip({
						placement: 'top',
						trigger: 'manual'
					})
					.tooltip(method);

				break;
			}
		};

		var success = validator.settings.success;
		validator.settings.success = function (label, element) {
			console.log('validator success');

			success.apply($(form).get(0), [$(label)]);

			if (self.errorTooltips)
				$(element).tooltip('dispose');
		}
	}

	this.download = function (url, data, method) {
		if (url && data) {
			data = typeof data == 'string' ? data : jQuery.param(data);
			var inputs = '';

			$.each(data.split('&'), function () {
				var pair = this.split('=');
				inputs += '<input type="hidden" name="' + pair[0] + '" value="' + pair[1] + '" />';
			});

			$('<form action="' + url + '" method="' + (method || 'post') + '">' + inputs + '</form>')
				.attr('target', '_blank')
				.appendTo('body')
				.submit()
				.remove();
		};
	};

	this.GenerateGUID = function () {
		return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
			(c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
		);
	}

	this.EsCredito = function (idComprobante) {

		switch (parseInt(idComprobante)) {
			case enums.Comprobante.NCREDITO_A:
			case enums.Comprobante.NCREDITO_B:
			case enums.Comprobante.NCREDITO_C:
			case enums.Comprobante.NCREDITO_E:
			case enums.Comprobante.NCREDITO_MIPYME_A:
			case enums.Comprobante.NCREDITO_MIPYME_B:
			case enums.Comprobante.NCREDITO_MIPYME_C:
			case enums.Comprobante.NCREDITO_M:
				return true;
				break;

			default:
				return false;
				break;
		}

	};

	this.EsMiPyme = function (idComprobante) {

		switch (parseInt(idComprobante)) {
			case enums.Comprobante.FACTURA_MIPYME_A:
			case enums.Comprobante.FACTURA_MIPYME_B:
			case enums.Comprobante.FACTURA_MIPYME_C:
			case enums.Comprobante.NDEBITO_MIPYME_A:
			case enums.Comprobante.NDEBITO_MIPYME_B:
			case enums.Comprobante.NDEBITO_MIPYME_C:
			case enums.Comprobante.NCREDITO_MIPYME_A:
			case enums.Comprobante.NCREDITO_MIPYME_B:
			case enums.Comprobante.NCREDITO_MIPYME_C:
				return true;
				break;

			default:
				return false;
				break;
		}

	};


});
