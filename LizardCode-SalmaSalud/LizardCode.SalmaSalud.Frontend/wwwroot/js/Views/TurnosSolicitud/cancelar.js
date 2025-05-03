/* Script CancelarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */
var TurnosCancelarView = new (function () {
    var self = this;

    this.init = function () {

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        Utils.rebuildValidator($('.frmCancelar'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');
    }

    function bindControlEvents() {

        $('.btSave')
            .on('click', function () {
                if (!$('.frmCancelar').valid())
                    return;

                Utils.modalLoader();
                $('.frmCancelar').submit();
            });
    }

    this.ajaxCancelarBegin = function (context, arguments) {

        //Utils.modalLoader();
    }

    this.ajaxCancelarSuccess = function (context) {
        Utils.modalClose();

        $('.actionsDialog').modal('hide');

        Utils.modalInfo('Turno CANCELADO', 'Se ha CANCELADO el turno de manera correcta', 5000);
        setTimeout(function () { window.location = '/TurnosSolicitud' }, 1000);

    }

    this.ajaxCancelarFailure = function (context) {
        Utils.modalClose();

        Ajax.ShowError(context);
    }
});

$(function () {
    TurnosCancelarView.init();
});

//# sourceURL=cancelar.js