/* Script AsignarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */
var TurnosView = new (function () {
    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var modalDemandaEspontanea;
    var btDemandaEspontanea;
    var modalCancelarAgenda;
    var btCancelarAgenda;

    var tipoUsuario = $('#hidLoggedUserTipo').val();

    this.init = function () {

        modalDemandaEspontanea = $('.modal.modalDemandaEspontanea', mainClass);
        btDemandaEspontanea = $('.toolbar-actions button.btDemandaEspontanea', mainClass);
        modalCancelarAgenda = $('.modal.modalCancelarAgenda', mainClass);
        btCancelarAgenda = $('.toolbar-actions button.btCancelarAgenda', mainClass);
        btNuevoTurno = $('.toolbar-actions button.btNuevoTurno', mainClass);

        if (tipoUsuario != enums.TipoUsuario.Paciente) {
            btDemandaEspontanea.show();
            btCancelarAgenda.show();
            btNuevoTurno.show();
        }

        MaestroLayout.init();

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        modalDemandaEspontanea.find('> .modal-dialog').addClass('modal-95');
        modalCancelarAgenda.find('> .modal-dialog').addClass('modal-50');

        var columns = [
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderFechaTurno },
            { data: null, orderable: false, searchable: false, render: renderHoraTurno },
            { data: null, orderable: false, render: renderPaciente, visible: tipoUsuario != enums.TipoUsuario.Paciente },
            { data: null, orderable: false, render: renderProfesional },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderEstado },
            { data: null, orderable: false, render: renderContacto, visible: tipoUsuario != enums.TipoUsuario.Paciente },
            //{ data: null, render: renderCobertura, visible: tipoUsuario != enums.TipoUsuario.Paciente },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderAcciones },
            { data: 'idTurno', visible: false },
            { data: 'fechaInicio', visible: false }
        ];

        var order = [[8, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idTurno', columns, order, 50, false, 'api');
    }

    function onCreatedRow(row, data, dataindex) {

        if (data.tipoTurno === 'ST')
            $(row).addClass('sobreTurnoRow');

        if (data.tipoTurno === 'DE')
            $(row).addClass('demandaEspontaneaRow');

        if (data.tipoTurno === 'GU')
            $(row).addClass('guardiaRow');

    }

    function bindControlEvents() {

        btDemandaEspontanea.on('click', demandaEspontaneaDialog);
        btCancelarAgenda.on('click', cancelarAgendaDialog);
        btNuevoTurno.on('click', function () { 
            window.location = '/Turnos';
        });

        MaestroLayout.onCreatedRow = onCreatedRow;

        dtView.table().on('click', 'tbody > tr > td > span.btAction', doAction);
        dtView.table().on('click', 'tbody > tr > td > span.btnConfirmar', doConfirmar);
    }

    //function renderTipoTurno(data, type, row, meta) {

    //    if (data.tipoTurno === 'ST')
    //        return '<i class="fa fa-calendar-plus" data-toggle="tooltip" data-placement="right" title="" data-original-title="SOBRE-TURNO"></i>';
    //    else if (data.tipoTurno === 'DE')
    //        return '<i class="fa fa-question" data-toggle="tooltip" data-placement="right" title="" data-original-title="DEMANDA ESPONTANEA"></i>';
    //    else
    //        return '<i class="fa fa-calendar-alt" data-toggle="tooltip" data-placement="right" title="" data-original-title="TURNO"></i>';
    //}  

    function renderFechaTurno(data, type, row) {

        return data.fecha.substring(8, 10) + '/' + data.fecha.substring(5, 7) + '<p class="td-sub-field"><b>' + data.tipoTurnoDescripcion + '</b></p>';
    }

    function renderHoraTurno(data, type, row) {

        return '<div class="avatar avatar-sm"><span class="avatar-title rounded-circle azul-avatar">' + data.hora.substring(0, 5) + '</span></div>';
    }

    function renderPaciente(data, type, row) {

        return '<p class="td-field">' + data.paciente + '</p><p class="td-sub-field">DNI: ' + data.documento + '</p>';
    }

    function renderProfesional(data, type, row) {

        return '<p class="td-field">' + (data.profesional ?? '') + '</p><p class="td-sub-field">' + (data.especialidad ?? '') + '</p>';
    }

    function renderEstado(data, type, row) {

        return '<span class="badge badge-pills badge-' + data.estadoClase + ' font10">' + data.estado + '</span>';
    }

    function renderCobertura(data, type, row) {

        if (data.financiador) {
            return '<p class="td-field">' + data.financiador + '</p><p class="td-sub-field">' + data.financiadorPlan + '</p>';
        } else {
            return '<p class="td-field">SIN COBERTURA</p>';
        }
    }

    function renderContacto(data, type, row) {

        return '<p class="td-field">' + data.telefono + '</p><p class="td-sub-field">' + data.email + '</p>';
    }

    function renderAcciones(data, type, row, meta) {

        var btnCancelar = '<span type="button" class="btn badge badge-danger btnCancelar btAction" title="CANCELAR" data-id-turno="' + data.idTurno + '" data-width-class="modal-70" data-ajax-action="/Turnos/CancelarView?idTurno=' + data.idTurno + '" data-toggle="tooltip" data-placement="top" title="CANCELAR"><i class="fa fa-times"></i></span>';
        var btnConfirmar = '<span type="button" class="btn badge badge-success btnConfirmar" title="CONFIRMAR" data-id-turno="' + data.idTurno + '" data-ajax-action="/Turnos/Confirmar" data-toggle="tooltip" data-placement="top" title="CONFIRMAR"><i class="fa fa-check"></i></span>';
        var btnReAgendar = '<span type="button" class="btn badge badge-warning btnReAgendar btAction" title="RE-AGENDAR" data-id-turno="' + data.idTurno + '" data-width-class="modal-70" data-ajax-action="/Turnos/ReAgendarView?idTurno=' + data.idTurno + '" data-toggle="tooltip" data-placement="top" title="RE-AGENDAR"><i class="fa fa-calendar"></i></span>';
        var btnRecepcionar = '<span type="button" class="btn badge badge-primary btnRecepcionar btAction" title="RECEPCIONAR" data-id-turno="' + data.idTurno + '" data-ajax-action="/Turnos/RecepcionarView?idTurno=' + data.idTurno + '" data-toggle="tooltip" data-placement="top" title="RECEPCIONAR"><i class="fa fa-sign-in"></i></span>';

        var btnHistorial = '<span type="button" class="btn badge badge-light btnRecepcionar btAction" title="HISTORIAL" data-id-turno="' + data.idTurno + '" data-ajax-action="/Turnos/HistorialView?idTurno=' + data.idTurno + '" data-toggle="tooltip" data-placement="top" title="HISTORIAL"><i class="fa fa-list"></i></span>';

        var sReturn = '';

        if (tipoUsuario == enums.TipoUsuario.Paciente) {

            switch (data.idEstadoTurno) {
                case enums.EstadoTurno.Agendado:
                    sReturn += btnCancelar + '&nbsp;' + btnConfirmar;
                    break;
                case enums.EstadoTurno.Confirmado:
                case enums.EstadoTurno.Recepcionado:
                case enums.EstadoTurno.ReAgendado:
                    sReturn += btnCancelar;
                    break;
                case enums.EstadoTurno.Cancelado:
                case enums.EstadoTurno.Atendido:
                case enums.EstadoTurno.AusenteConAviso:
                case enums.EstadoTurno.AusenteSinAviso:
                    sReturn += '';
                    break;
            }
        }
        else {
            if (tipoUsuario != enums.TipoUsuario.Recepcion)
                sReturn = btnHistorial + '&nbsp;';

            switch (data.idEstadoTurno) {

                case enums.EstadoTurno.Agendado:
                    sReturn += btnCancelar + '&nbsp;' + btnReAgendar + '&nbsp;' + btnConfirmar + '&nbsp;' + btnRecepcionar;
                    break;
                case enums.EstadoTurno.Confirmado:
                    sReturn += btnCancelar + '&nbsp;' + btnReAgendar + '&nbsp;' + btnRecepcionar;
                    break;
                case enums.EstadoTurno.Recepcionado:
                    if (data.tipoTurno === 'DE' || data.tipoTurno === 'GU')
                        sReturn += btnCancelar;
                    else
                        sReturn += btnCancelar + '&nbsp;' + btnReAgendar;
                    break;
                case enums.EstadoTurno.ReAgendado:
                    sReturn += btnCancelar + '&nbsp;' + btnReAgendar + '&nbsp;' + btnRecepcionar;
                    break;
                case enums.EstadoTurno.Cancelado:
                case enums.EstadoTurno.Atendido:
                case enums.EstadoTurno.AusenteConAviso:
                case enums.EstadoTurno.AusenteSinAviso:
                    sReturn += '';
                    break;
            }
        }

        return sReturn;
    }

    function doAction() {

        var action = $(this).closest('span').data('ajax-action');
        var widthClass = $(this).closest('span').data('width-class');

        Modals.loadAnyModal('actionsDialog', widthClass, action, function () { }, function () { dtView.reload(); });
    }

    function doConfirmar() {

        var tr = $(this).closest('tr');
        var row = dtView.api().row(tr);
        var idTurno = row.data().idTurno;

        var action = '/Turnos/Confirmar';

        Utils.modalQuestion("Confirmar Turno", "¿Confirma el turno?.",
            function (confirm) {
                if (confirm) {
                    Utils.modalLoader();

                    var params = {
                        idTurno: idTurno
                    };

                    $.post(action, params, function () {
                        Utils.modalClose();
                    })
                        .done(function (data) {
                            dtView.reload();

                            Utils.modalInfo('Turno CONFIRMANDO', 'Se ha CONFIRMADO el turno de manera correcta', 5000, undefined, function () {
                            });
                        })
                        .fail(function () {
                            //alert("error");
                            Utils.modalError('Error CONFIRMANDO TURNO', 'Error');
                        })
                        .always(function () {
                            //alert("finished");
                        });
                }
            }, "CONFIRMAR", "Cancelar", true);

    }

    function demandaEspontaneaDialog() {

        var action = '/Turnos/DemandaEspontaneaView';
        var widthClass = 'modal-90';

        Modals.loadAnyModal('demandaEspontaneaDialog', widthClass, action, function () { }, function () { dtView.reload(); });
    }


    function cancelarAgendaDialog() {

        var action = '/Turnos/CancelarAgendaView';
        var widthClass = 'modal-50';

        Modals.loadAnyModal('cancelarAgendaDialog', widthClass, action, function () { }, function () { dtView.reload(); });
    }

});

$(function () {
    TurnosView.init();
});

//# sourceURL=index.js