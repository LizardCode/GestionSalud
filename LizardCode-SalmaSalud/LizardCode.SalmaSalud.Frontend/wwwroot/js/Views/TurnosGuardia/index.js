/* Script AsignarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */
var TurnosView = new (function () {
    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var btHistoriaClinica;

    var calendar;
    calendarInitialized = false;

    this.init = function () {
        btHistoriaClinica = $('.toolbar-actions button.btHistoriaClinicaTb', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        if ($('#hidLoggedUserTipo').val() == enums.TipoUsuario.Recepcion)
            $('.btHistoriaClinicaTb').hide();

        var columns = [
            { data: null, orderable: false, class: 'text-center', width: '5%', render: renderTipo },
            //{ data: null, orderable: false, render: renderProfesional, visible: $('#hidLoggedUserTipo').val() != enums.TipoUsuario.Profesional },
            { data: null, orderable: false, render: renderPaciente },
            { data: null, orderable: false, width: '15%', render: renderCobertura },
            { data: null, orderable: false, searchable: false, width: '10%', render: renderTiempo },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderAcciones },

            { data: 'fechaRecepcion', visible: false },
            { data: 'idTurno', visible: false },
            { data: 'idPaciente', visible: false }
        ];

        var order = [[5, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idTurno', columns, order, 50, false);
        
    }

    function onCreatedRow(row, data, dataindex) {

        $(row).addClass('guardiaTurnoRow');
    }

    function bindControlEvents() {

        btHistoriaClinica.on('click', historiaClinicaDialog);

        MaestroLayout.onCreatedRow = onCreatedRow;
        MaestroLayout.mainTableRowSelected = mainTableRowSelected;

        dtView.table().on('click', 'tbody > tr > td > span.btAction', doAction);
        dtView.table().on('click', 'tbody > tr > td > span.btRedirection', doRedirection);

        setInterval(function () {

            dtView.reload();
            console.log('Turnos.Reload() - ' + moment().format('HH:mm:ss'));

        }, 60000);
    }

    function mainTableRowSelected(dataArray, api) {

        var tipoUsuario = $('#hidLoggedUserTipo').val();

        btHistoriaClinica.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        if (dataArray.length == 1) {
            btHistoriaClinica.prop('disabled', tipoUsuario == enums.TipoUsuario.Recepcion);
        }
        else if (dataArray.length > 1) {
            btHistoriaClinica.prop('disabled', true);
        }
    }

    function renderTipo(data, type, row) {

        return '<p class="td-sub-field"><b>' + data.tipoTurnoDescripcion + '</b></p>';
    }

    function renderPaciente(data, type, row) {

        return '<p class="td-field">' + data.paciente + '</p><p class="td-sub-field">DNI: ' + data.documento + '</p>';
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

    function renderTiempo(data, type, row, meta) {
        var ahora = new Date();
        var recepcion = moment(data.fechaRecepcion).toDate();

        var difference = ahora.getTime() - recepcion.getTime();
        var minutos = Math.round(difference / 60000);

        var color = 'azul-avatar';
        var espera = '';

        if (minutos > 30) {
            if (minutos > 59) {
                var horas = '';
                if (minutos > 599) {
                    horas = toHours(minutos);
                } else { 
                    horas = toHoursAndMinutes(minutos);
                }
                color = 'rojo1';
                espera = horas + 'hs';
                                
            } else {
                color = 'rojo1';
                espera = minutos + 'min';
            }
        } else if (minutos < 15) {
            espera = minutos + 'min';
        } else {
            color = 'amarillo1';
            espera = minutos + 'min';
        }

        var templateTD = '<div class="avatar avatar-sm"><span class="avatar-title rounded-circle [COLOR]">[ESPERA]</span></div>'
        return templateTD.replace('[COLOR]', color).replace('[ESPERA]', espera);
    }

    function renderAcciones(data, type, row, meta) {

        var btnCancelar = '<span type="button" class="btn badge badge-danger btnCancelar btAction" title="CANCELAR" data-id-turno="' + data.idTurno + '" data-width-class="modal-70" data-ajax-action="/Turnos/CancelarView?idTurno=' + data.idTurno + '" data-toggle="tooltip" data-placement="top" title="CANCELAR"><i class="fa fa-times"></i></span>';
        var btnReAgendar = '<span type="button" class="btn badge badge-warning btnReAgendar btAction" title="RE-AGENDAR" data-id-turno="' + data.idTurno + '" data-width-class="modal-70" data-ajax-action="/Turnos/ReAgendarView?idTurno=' + data.idTurno + '" data-toggle="tooltip" data-placement="top" title="RE-AGENDAR"><i class="fa fa-calendar"></i></span>';
        

        var btnLlamar = '';
        var btnAtender = '';
        if ($('#hidLoggedUserTipo').val() == enums.TipoUsuario.Profesional || ($('#hidLoggedUserTipo').val() == enums.TipoUsuario.Administrador && $('#hidLoggedUserIdProfesional').val() > 0)) {
            btnLlamar = '<span type="button" class="btn badge badge-primary btLlamar btAction" title="LLAMAR" data-id-turno="' + data.idTurno + '" data-width-class="modal-70" data-ajax-action="/Turnos/LlamarView?idTurno=' + data.idTurno + '" data-toggle="tooltip" data-placement="top" title="LLAMAR"><i class="fa fa-megaphone"></i></span>';
            btnAtender = '<span type="button" class="btn badge badge-info btRedirection" title="ATENDER" data-id-turno="' + data.idTurno + '" data-width-class="modal-95" data-ajax-action="/Evoluciones/NuevaView?idTurno=' + data.idTurno + '" data-toggle="tooltip" data-placement="top" title="EVOLUCIONAR"><i class="fa fa-check-circle"></i></span>';
        }

        var btnObservaciones = '<span type="button" class="btn badge badge-warning btAction" title="VER OBSERVACIONES" data-id-turno="' + data.idTurno + '" data-width-class="modal-60" data-ajax-action="/Turnos/ObservacionesView?idTurno=' + data.idTurno + '" data-toggle="tooltip" data-placement="top" title="VER OBSERVACIONES"><i class="fa fa-comment-plus"></i></span>';

        //return btnCancelar + '&nbsp;' + btnLlamar + '&nbsp;' + btnAtender + (data.observaciones ? '&nbsp;' + btnObservaciones : '');
        return btnLlamar + '&nbsp;' + btnAtender + (data.observaciones ? '&nbsp;' + btnObservaciones : '');
    }

    function doAction() {

        var idTurno = $(this).closest('span').data('id-turno');
        var action = $(this).closest('span').data('ajax-action');
        var widthClass = $(this).closest('span').data('width-class');

        Modals.loadAnyModal('actionsDialog', widthClass, action, function () { }, function () { dtView.reload(); });
    }

    function doRedirection() {

        var idTurno = $(this).closest('span').data('id-turno');
        window.location = '/Evoluciones/NuevaPage?idTurno=' + idTurno;
    }

    function toHoursAndMinutes(totalMinutes) {
        const hours = Math.floor(totalMinutes / 60);
        const minutes = totalMinutes % 60;

        return `${hours}:${padToTwoDigits(minutes)}`;
    }

    function toHours(totalMinutes) {
        const hours = Math.floor(totalMinutes / 60);
        const minutes = totalMinutes % 60;

        return `${hours}`;
    }

    function padToTwoDigits(num) {
        return num.toString().padStart(2, '0');
    }

    function historiaClinicaDialog() {

        var selectedRows = dtView.selected();
        var idPaciente = selectedRows[0].idPaciente;
        var action = '/Pacientes/HistoriaClinicaView?id=' + idPaciente + '&showResumenPaciente=true';

        Modals.loadAnyModal('historiaClinicaDialog', 'modal-95', action, function () { }, function () { });
    }
});

$(function () {
    TurnosView.init();
});

//# sourceURL=index.js