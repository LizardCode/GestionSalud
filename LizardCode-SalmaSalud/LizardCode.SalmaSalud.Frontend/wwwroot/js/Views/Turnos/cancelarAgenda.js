/* Script CancelarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */
var TurnosCancelarAgendaView = new (function () {
    var self = this;

    this.init = function () {

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        Utils.rebuildValidator($('.frmCancelar'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');

        $('#FechaCancelacion')
            .inputmask("99/99/9999")
            .flatpickr({
                locale: "es",
                allowInput: true,
                minDate: "today",
                defaultDate: "today",
                dateFormat: "d/m/Y",
                onClose: function (selectedDates, dateStr, instance) {
                    if (dateStr == "")
                        instance.setDate(moment().format(enums.FormatoFecha.DefaultFormat));
                    else
                        instance.setDate(dateStr);
                }
            });

        $('select.profesionalCancelarAgenda').Select2Ex({ allowClear: true });
    }

    function bindControlEvents() {

        $('.btSave')
            .on('click', function () {
                if (!$('.frmCancelar').valid())
                    return;

                Utils.modalQuestion("Cancelar Agenda", "Se procederá a la cancelación de la Agenda del profesional, no pudiendose asignar turnos para ese día. Los turnos asignados para el día, serán cancelados con el respectivo aviso a los pacientes. LA ACCIÓN NO SE PUEDE DESHACER. ¿Desea continuar?.",
                    function (confirm) {
                        if (confirm) {

                            Utils.modalLoader();
                            $('.frmCancelar').submit();
                        }
                    }, "CONTINUAR", "Cancelar", true);

            });
    }

    this.ajaxCancelarAgendaBegin = function (context, arguments) {

        //Utils.modalLoader();
    }

    this.ajaxCancelarAgendaSuccess = function (context) {
        Utils.modalClose();

        $('.cancelarAgendaDialog').modal('hide');

        Utils.modalInfo('Agenda CANCELADA', 'Se ha CANCELADO la agenda del profesional de forma correcta.', 5000);

    }

    this.ajaxCancelarAgendaFailure = function (context) {
        Utils.modalClose();

        Ajax.ShowError(context);
    }
});

$(function () {
    TurnosCancelarAgendaView.init();
});

//# sourceURL=cancelarAgenda.js 