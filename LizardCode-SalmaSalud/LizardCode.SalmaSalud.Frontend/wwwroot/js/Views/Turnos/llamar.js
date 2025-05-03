/* Script LlamarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */
var TurnosLlamarView = new (function () {
    var self = this;

    this.init = function () {

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        Utils.rebuildValidator($('.frmTurnosLlamar'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');
    }

    function bindControlEvents() {

        $('.consultorioLlamar').select2();

        $('.btSave')
            .on('click', function () {
                if (!$('.frmTurnosLlamar').valid())
                    return;

                Utils.modalLoader();
                $('.frmTurnosLlamar').submit();
            });
    }

    this.ajaxLlamarBegin = function (context, arguments) {

        //Utils.modalLoader();
    }

    this.ajaxLlamarSuccess = function (context) {
        Utils.modalClose();

        $('.actionsDialog').modal('hide');

        if (typeof context === "object")
            Utils.modalWarning('Ya se ha realizado un llamado', context.message, 5000);
        else
            Utils.modalInfo('Llamado realizado', 'Se ha ENVIADO UN MENSAJE AL PACIENTE de manera correcta', 5000);

    }

    this.ajaxLlamarFailure = function (context) {
        Utils.modalClose();
        //Utils.ajaxFormFailure(context);

        Ajax.ShowError(context);
    }
});

$(function () {
    TurnosLlamarView.init();
});

//# sourceURL=llamar.js