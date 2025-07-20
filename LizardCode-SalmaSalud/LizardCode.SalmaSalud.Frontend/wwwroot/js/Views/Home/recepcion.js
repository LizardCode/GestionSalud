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
        "<'table-responsive'tr>" +
        "<'dt--bottom-section d-sm-flex justify-content-sm-between text-center'<'dt--pagination'p>>";

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
            })
            .fail(Ajax.ShowError);

        Ajax.GetJson(RootPath + 'AuditoriasChatApi/ObtenerTotalesByEstado')
            .done(function (data) {

                $('.bMensajesError').html(data.auditoriaChatApiErrorHoy);
                $('.bMensajesEnviados').html(data.auditoriaChatApiEnviadosHoy);
            })
            .fail(Ajax.ShowError);
    }

    function initDataTables() {
        dtTiposDeCambio = $('.dtTurnosSolicitud')
            .DataTableEx({

                ajax: {
                    url: RootPath + '/TurnosSolciitud/TurnosSolicitudDashboard',
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
});

$(function () {

    DashboardView.init();
});