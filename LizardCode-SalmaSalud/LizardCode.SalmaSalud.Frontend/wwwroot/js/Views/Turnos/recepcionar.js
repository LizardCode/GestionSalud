/* Script RecepcionarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */

var VALIDACION_CONTINUAR = false;
var VALIDACION_FORZAR_PARTICULAR = false;
var VALIDACION_FORZAR_PADRON = false;
var VALIDACION_FINANCIADOR_NRO = '';

var TurnosRecepcionarView = new (function () {
    var self = this;

    this.init = function () {

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        Utils.rebuildValidator($('.frmRecepcionar'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');
    }

    function bindControlEvents() {

        $('.consultorioRecepcionar').select2();

        $('.btSave')
            .on('click', function () {
                if (!$('.frmRecepcionar').valid())
                    return;

                //var validacion = validarPadron($('.hdnRecepcionarIdPaciente').val());
                $.get("/FinanciadoresPadron/ValidarPadron", { idPaciente: $('.hdnRecepcionarIdPaciente').val() }, function (response) {
                    if (!response.afiliadoValido) {

                        var action = '/FinanciadoresPadron/ValidarPadronView?idPaciente=' + $('.hdnRecepcionarIdPaciente').val();
                        Modals.loadAnyModal('validacionPadronView', 'modal-70', action,
                            function () {

                                
                            }, function () {

                                $('.hdnRecepcionarForzarParticular').val(VALIDACION_FORZAR_PARTICULAR);
                                $('.hdnRecepcionarForzarPadron').val(VALIDACION_FORZAR_PADRON);
                                $('.hdnRecepcionarFinanciadorNro').val(VALIDACION_FINANCIADOR_NRO);

                                if (VALIDACION_CONTINUAR) {
                                    Utils.modalLoader();
                                    $('.frmRecepcionar').submit();
                                }
                            });

                        return;
                    } else {

                        Utils.modalLoader();
                        $('.frmRecepcionar').submit();
                    }
                });                
            });
    }

    this.ajaxRecepcionarBegin = function (context, arguments) {

        //Utils.modalLoader();
    }

    this.ajaxRecepcionarSuccess = function (context) {
        Utils.modalClose();

        $('.actionsDialog').modal('hide');

        Utils.modalInfo('Turno RECEPCIONADO', 'Se ha RECEPCIONADO el turno de manera correcta', 5000);

    }

    this.ajaxRecepcionarFailure = function (context) {
        Utils.modalClose();
        //Utils.ajaxFormFailure(context);

        Ajax.ShowError(context);
    }

    function validarPadron(idPaciente) {

    }
});

$(function () {
    TurnosRecepcionarView.init();
});

//# sourceURL=recepcionar.js