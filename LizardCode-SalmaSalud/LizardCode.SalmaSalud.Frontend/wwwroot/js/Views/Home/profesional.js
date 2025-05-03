var DashboardView = new (function () {

    //#region Init

    this.init = function () {

        buildControls();
        bindControlsEvents();

        initTotales();
        initDataTables();
    };

    function initTotales() {
        Ajax.GetJson(RootPath + 'Turnos/ObtenerTotalesDashboard')
            .done(function (data) {

                $('.chart-agendados').attr("data-percentage", 100 - data.porcentajeAgendados);
                $('.bTurnosAgendados').html(data.agendadosHoy + '/' + data.totalHoy);

                $('.chart-confirmados').attr("data-percentage", 100 - data.porcentajeConfirmados);
                $('.bTurnosConfirmados').html(data.confirmadosManiana + '/' + data.agendadosManiana);

                initCharts();
            })
            .fail(Ajax.ShowError);

        Ajax.GetJson(RootPath + 'AuditoriasChatApi/ObtenerTotalesByEstado')
            .done(function (data) {

                $('.bMensajesError').html(data.auditoriaChatApiErrorHoy);
                $('.bMensajesEnviados').html(data.auditoriaChatApiEnviadosHoy);
            })
            .fail(Ajax.ShowError);
    }

    function initCharts() {
        var donutCharts = $(".donut-chart-js");

        if (donutCharts.length > 0) {
            $.each(donutCharts, function (index, item) {
                var donutChartPercentage = $(item).attr("data-percentage");
                var donutChartRadio = $(item).find(".donut-chart").attr("r");
                var donutChartValue =
                    ((100 - Number(donutChartPercentage)) *
                        (6.28 * Number(donutChartRadio))) /
                    100;

                $(item)
                    .find(".donut-chart-value")
                    .html(donutChartPercentage + "%");
                $(item)
                    .find("circle.donut-chart")
                    .css("stroke-dashoffset", donutChartValue);
            });
        }
    }

    //#endregion

    //#region Funciones

    function buildControls() {

    }

    function bindControlsEvents() {

    }

    //#endregion
});

$(function () {

    DashboardView.init();
});