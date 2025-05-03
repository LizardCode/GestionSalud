var DashboardView = new (function () {

    //#region Init

    var defaultDom =
        "<'dt--top-section'<l><f>>" +
        "<'table-responsive'tr>" +
        "<'dt--bottom-section d-sm-flex justify-content-sm-between text-center'<'dt--pages-count  mb-sm-0 mb-3'i><'dt--pagination'p>>";

    this.init = function () {

        buildControls();
        bindControlsEvents();

        initTotales();
    };

    function initTotales() {
        Ajax.GetJson(RootPath + 'Turnos/ObtenerTotalesByEstado')
            .done(function (data) {

                //CANCELADOS
                $('.pCanceladoHoy').html($('.pCanceladoHoy').html().replace('[VALUE]', data.canceladosHoy));
                $('.pCanceladoMensual').html($('.pCanceladoMensual').html().replace('[VALUE]', data.canceladosMensual));

                $('#CanceladoHoyLoader').hide();
                $('#CanceladoMensualLoader').hide();

                $('.pCanceladoHoy').show();
                $('.pCanceladoMensual').show();


                $('.pRecepcionadoHoy').html($('.pRecepcionadoHoy').html().replace('[VALUE]', data.recepcionadosHoy));
                $('.pRecepcionadoMensual').html($('.pRecepcionadoMensual').html().replace('[VALUE]', data.recepcionadosMensual));

                $('#RecepcionadoHoyLoader').hide();
                $('#RecepcionadoMensualLoader').hide();

                $('.pRecepcionadoHoy').show();
                $('.pRecepcionadoMensual').show();


                $('.pAtendidoHoy').html($('.pAtendidoHoy').html().replace('[VALUE]', data.atendidosHoy));
                $('.pAtendidoMensual').html($('.pAtendidoMensual').html().replace('[VALUE]', data.atendidosMensual));

                $('#AtendidoHoyLoader').hide();
                $('#AtendidoMensualLoader').hide();

                $('.pAtendidoHoy').show();
                $('.pAtendidoMensual').show();

                $('.pSobreTurnosHoy').html($('.pSobreTurnosHoy').html().replace('[VALUE]', data.sobreTurnosHoy));
                $('.pSobreTurnosMensual').html($('.pSobreTurnosMensual').html().replace('[VALUE]', data.sobreTurnosMensual));

                $('#SobreTurnosHoyLoader').hide();
                $('#SobreTurnosMensualLoader').hide();

                $('.pSobreTurnosHoy').show();
                $('.pSobreTurnosMensual').show();

                $('[data-toggle="tooltip"]').tooltip();
            })
            .fail(Ajax.ShowError);


        $('[data-toggle="tooltip"]').tooltip();
    }

    //#endregion

    function buildControls() {

    }

    function bindControlsEvents() {

    }
});

$(function () {

    DashboardView.init();
});