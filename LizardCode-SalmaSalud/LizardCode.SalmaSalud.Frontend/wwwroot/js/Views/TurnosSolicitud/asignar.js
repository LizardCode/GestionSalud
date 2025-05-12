/* Script AsignarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */
var BUSQUEDA_DOCUMENTO_PACIENTE = 0;

var TurnosAsignarView = new (function () {
    var self = this;

    this.init = function () {

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        Utils.rebuildValidator($('.frmAsignar'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');
    }

    function bindControlEvents() {

        $('.btSave')
            .on('click', function () {
                asignar($('.frmAsignar'));
            });


        $('#Fecha')
            .inputmask("99/99/9999 99:99")
            .flatpickr({
                locale: "es",
                allowInput: true,
                minDate: "today",
                defaultDate: "today",
                dateFormat: "d/m/Y H:i",
                enableTime: true,
                onClose: function (selectedDates, dateStr, instance) {
                    if (dateStr == "")
                        instance.setDate(moment().format(enums.FormatoFecha.DefaultFormat));
                    else
                        instance.setDate(dateStr);
                }
            });
    }

    function asignar($form) {

        if (!$form.valid())
            return;

        Utils.modalLoader();
        $form.submit();

    };

    this.ajaxAsignarBegin = function (context, arguments) {

        //Utils.modalLoader();
    }

    this.ajaxAsignarSuccess = function (context) {
        Utils.modalClose();

        //$('.turnosDialog').modal('hide');
        //$('.asignarDialog').modal('hide');

        Utils.modalInfo('Turno Asignado', 'Se ha asignado el turno de manera correcta', 5000);
        setTimeout(function () { window.location = '/TurnosSolicitud' }, 1000);

    }

    this.ajaxAsignarFailure = function (context) {
        Utils.modalClose();

        Ajax.ShowError(context);
    }
});

$(function () {
    TurnosAsignarView.init();
});

//# sourceURL=asignar.js