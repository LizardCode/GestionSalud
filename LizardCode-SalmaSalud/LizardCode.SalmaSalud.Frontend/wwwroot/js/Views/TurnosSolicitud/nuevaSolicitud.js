var NuevaSolicitudView = new (function () {

    //#region Init

    this.init = function () {

        buildControls();
        bindControlsEvents();

    };

    //#endregion

    function buildControls() {
        Utils.rebuildValidator($('.frmMisDatos'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');

        $('[data-toggle="tooltip"]').tooltip();

        $('#IdEspecialidad').select2();
        $('#Dias').select2();
        $('#RangosHorarios').select2();
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

        Utils.modalInfo('Turno Solicitado', 'Se ha solicitado el turno exitosamente.', 5000);
        setTimeout(function () { window.location = '/portal-pacientes/turnos' }, 1000);

    }

    this.ajaxFailure = function (context) {
        Utils.modalClose();

        Ajax.ShowError(context);
    }
});

$(function () {

    NuevaSolicitudView.init();
});

//# sourceURL=nuevaSolicitud.js