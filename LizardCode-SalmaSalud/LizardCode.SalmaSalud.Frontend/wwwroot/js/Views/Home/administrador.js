var DashboardView = new (function () {

    var defaultDom =
        "<'dt--top-section'<l><f>>" +
        "<'table-responsive'tr>" +
        "<'dt--bottom-section d-flex flex-row-reverse'<'dt--pages-count  mb-sm-0 mb-3'i><'dt--pagination'p>>";

    //#region Init

    var dtTurnosHoy = null;
    var dtMensajesHoy = null;

    var domTurnos =
        "<'dt--top-section'<l>>" +
        "<'table-responsive'tr>";
        //"<'dt--bottom-section d-sm-flex justify-content-sm-between text-center'<'dt--pagination'p>>";

    var domMensajes =
        "<'dt--top-section'<l>>" +
        "<'table-responsive'tr>";

    this.init = function () {

        buildControls();
        bindControlsEvents();

        initTotales();
        initDataTables();
    };

    function initTotales() {
        Ajax.GetJson(RootPath + 'TurnosSolicitud/ObtenerTotalesDashboard')
            .done(function (data) {

                $('.vAfiliados').html(data.totalAfiliados);                
                $('.vSolicitados').html(data.solicitados);
                $('.vAsignados').html(data.asignados);
                $('.vCancelados').html(data.cancelados);

                //initCharts();
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

    function initDataTables() {
        dtTurnosHoy = $('.dtTurnosHoy')
            .DataTableEx({

                ajax: {
                    url: RootPath + '/Turnos/ObtenerProximosTurnos',
                    type: 'POST',
                    error: function (xhr, ajaxOptions, thrownError) {
                        Ajax.ShowError(xhr, xhr.statusText, thrownError);
                    },
                    callback: function (xhr) {
                        Ajax.ShowError(xhr, xhr.statusText, '');
                    }
                },
                processing: true,
                serverSide: true,
                pageLength: 3,
                lengthChange: false,
                dom: domTurnos,

                /*select: { style: selectionStyle, info: (selectionStyle === 'os') },*/
                columns: [
                    //{ data: null, width: '5%', class: 'text-center', orderable: false, render: renderTipo },
                    { data: null, width: '5%', orderable: false, render: renderHora },
                    { data: null, orderable: false, width: '20%', render: renderPaciente },
                    { data: null, width: '10%', orderable: false, render: renderEstadoTurno },
                    { data: null, orderable: false, width: '15%', render: renderProfesional },
                    { data: null, orderable: false, width: '15%', render: renderCobertura },
                    { data: 'fechaInicio', visible: false },
                    { data: 'fechaRecepcion', visible: false }
                ],
                order: [[6, 'ASC']],

                //onSelected: datatableSelectedRow,
                onDraw: datatableDraw,
                //onInit: datatableInit
            });

        dtMensajesHoy = $('.dtMensajesHoy')
            .DataTableEx({

                ajax: {
                    url: RootPath + '/AuditoriasChatApi/ObtenerUltimosMensajes',
                    type: 'POST',
                    error: function (xhr, ajaxOptions, thrownError) {
                        Ajax.ShowError(xhr, xhr.statusText, thrownError);
                    },
                    callback: function (xhr) {
                        Ajax.ShowError(xhr, xhr.statusText, '');
                    }
                },
                processing: true,
                serverSide: true,
                pageLength: 10,
                paging: false,
                lengthChange: false,
                dom: domMensajes,

                /*select: { style: selectionStyle, info: (selectionStyle === 'os') },*/
                columns: [
                    { data: 'fecha', render: DataTableEx.renders.dateTimeSecs, orderable: false },
                    { data: 'paciente', orderable: false },
                    { data: 'telefono', orderable: false },
                    { data: null, orderable: false, searchable: false, class: 'text-center', render: renderEstadoMensaje },
                    { data: 'idAuditoria', visible: false }
                ],
                order: [[4, 'DESC']],

                //onSelected: datatableSelectedRow,
                onDraw: datatableDraw,
                //onInit: datatableInit
            });
    };

    //#endregion

    //#region Funciones

    function buildControls() {

    }

    function bindControlsEvents() {

    }

    function datatableDraw() {

        feather.replace();
    }

    function renderPaciente(data, type, row) {

        return '<p class="td-field">' + data.paciente + '</p><p class="td-sub-field">' + data.tipoTurnoDescripcion + '</p>';
    }

    function renderProfesional(data, type, row) {

        return '<p class="td-field">' + data.profesional + '</p><p class="td-sub-field">' + data.especialidad + '</p>';
    }

    function renderCobertura(data, type, row) {

        if (data.financiador) {
            return '<p class="td-field">' + data.financiador + '</p><p class="td-sub-field">' + data.financiadorPlan + '</p>';
        } else {
            return '<p class="td-field">SIN COBERTURA</p>';
        }
    }

    function renderHora(data, type, row) {

        return '<div class="avatar avatar-sm"><span class="avatar-title rounded-circle azul-avatar">' + data.hora.substring(0, 5) + '</span></div>';
    }

    function renderEstadoTurno(data, type, row) {

        return '<span class="badge badge-pills badge-' + data.estadoClase + ' font10">' + data.estado + '</span>';
    }

    function renderEstadoMensaje(data, type, row) {

        return '<span class="badge badge-pills badge-' + data.estadoClase + ' font10">' + data.estado + '</span>';
    }

    //#endregion

    function initDataTables() {
        dtTiposDeCambio = $('.dtTurnosSolicitud')
            .DataTableEx({

                ajax: {
                    url: RootPath + '/TurnosSolicitud/TurnosSolicitudDashboard',
                    type: 'POST',
                    error: function (xhr, ajaxOptions, thrownError) {
                        Ajax.ShowError(xhr, xhr.statusText, thrownError);
                    },
                    callback: function (xhr) {
                        Ajax.ShowError(xhr, xhr.statusText, '');
                    }
                },
                processing: true,
                serverSide: true,
                pageLength: 15,
                lengthChange: false,
                dom: defaultDom,
                columns: [
                    { data: 'profesional', width: '70%' },
                    { data: 'asignadosHoy', width: '10%', class: 'text-center' },
                    { data: 'asignadosMes', width: '10%', class: 'text-center' },
                    { data: 'canceladosMes', width: '10%', class: 'text-center' }
                ],
                order: [[0, 'ASC']],
                onDraw: datatableDraw
            });
    };
});

$(function () {

    DashboardView.init();
});