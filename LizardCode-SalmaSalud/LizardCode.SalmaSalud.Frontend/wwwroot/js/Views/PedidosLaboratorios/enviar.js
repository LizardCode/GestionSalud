/* Script AsignarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */
var PedidoLaboratorioMarcarView = new (function () {
    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    this.init = function () {

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        $('#Fecha')
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

        $('#FechaEstimada')
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

        Utils.rebuildValidator($('.frmEnviar'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');
    }

    function bindControlEvents() {

        $('.btSave')
            .on('click', function () {
                $('.frmEnviar').submit();
            });
    }

    this.ajaxBegin = function (context, arguments) {

        //Utils.modalLoader();
    }

    this.ajaxSuccess = function (context) {
        Utils.modalClose();

        var idPresupuesto = $('.hdnMarcarIdPresupuesto').val();
        var lblAccion = $('.hdnMarcarAccion').val();
        Utils.modalInfo(lblAccion, 'Operación exitosa!.', 5000);
        setTimeout(function () { window.location = '/PedidosLaboratorios/Gestion' }, 1800);
    }

    this.ajaxFailure = function (context) {
        Utils.modalClose();

        Ajax.ShowError(context);
    }
});

$(function () {
    PedidoLaboratorioMarcarView.init();
});

//# sourceURL=enviar.js