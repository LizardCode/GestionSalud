var MisDatosView = new (function () {

    //#region Init

    this.init = function () {

        buildControls();
        bindControlsEvents();

    };

    //#endregion

    function buildControls() {
        Utils.rebuildValidator($('.frmMisDatos'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');

        $('[data-toggle="tooltip"]').tooltip();

        $('#FechaNacimiento')
            .inputmask("99/99/9999")
            .flatpickr({
                locale: "es",
                allowInput: true,
                maxDate: "today",
                defaultDate: "today",
                dateFormat: "d/m/Y",
                onClose: function (selectedDates, dateStr, instance) {
                    if (dateStr == "")
                        instance.setDate(moment().format(enums.FormatoFecha.DefaultFormat));
                    else
                        instance.setDate(dateStr);
                }
            });

        $('#FechaNacimiento').val(moment($('#FNac').val()).format(enums.FormatoFecha.DefaultFormat));
    }

    function bindControlsEvents() {

        //$('.btnCancelarTurno').on('click', function () {
        //    var action = $(this).data('ajax-action');
        //    var widthClass = $(this).data('width-class');

        //    Modals.loadAnyModal('actionsDialog', widthClass, action, function () { }, function () { dtView.reload(); });
        //});
    }

    this.ajaxBegin = function (context, arguments) {

        Utils.modalLoader();
    }

    this.ajaxSuccess = function (context) {
        Utils.modalClose();

        Utils.modalInfo('Datos Actualizados', 'Se han actualizado los datos exitosamente.', 5000);

    }

    this.ajaxFailure = function (context) {
        Utils.modalClose();

        Ajax.ShowError(context);
    }
});

$(function () {

    MisDatosView.init();
});

//# sourceURL=misDatos.js