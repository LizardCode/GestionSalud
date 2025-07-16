/* Script AsignarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */
var BUSQUEDA_DOCUMENTO_PACIENTE = 0;

var TurnosNuevoView = new (function () {
    var self = this;

    this.init = function () {

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        Utils.rebuildValidator($('.frmNuevo'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');

        $('.paciente').select2();
        $('.especialidad').select2();
        $('.profesional').select2();
    }

    function bindControlEvents() {

        $('.btSave')
            .on('click', function () {
                nuevo($('.frmNuevo'));
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

    function nuevo($form) {

        if (!$form.valid())
            return;

        Utils.modalLoader();
        $form.submit();

    };

    this.ajaxNuevoBegin = function (context, arguments) {

        //Utils.modalLoader();
    }

    this.ajaxNuevoSuccess = function (context) {
        Utils.modalClose();

        //$('.turnosDialog').modal('hide');
        //$('.asignarDialog').modal('hide');

        Utils.modalInfo('Turno Asignado', 'Se ha asignado el turno de manera correcta', 5000);
        setTimeout(function () { window.location = '/TurnosSolicitud' }, 1000);

    }

    this.ajaxNuevoFailure = function (context) {
        Utils.modalClose();

        Ajax.ShowError(context);
    }
});

$(function () {
    TurnosNuevoView.init();
});

//# sourceURL=nuevo.js