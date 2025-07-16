/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMTurnosSolicitudView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var modalNew;
    var modalEdit;
    var btNew;
    var btEdit;
    var btRemove;

    var btNuevoTurno;

    var openingEditDialog = false;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);

        btNew = $('.toolbar-actions button.btNew', mainClass);
        btEdit = $('.toolbar-actions button.btEdit', mainClass);
        btRemove = $('.toolbar-actions button.btRemove', mainClass);
        btNuevoTurno = $('.toolbar-actions button.btNuevoTurno', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

        //var tipoUsuario = $('#hidLoggedUserTipo').val();
        //btNew.prop('disabled', tipoUsuario == enums.TipoUsuario.Profesional);
        //if (tipoUsuario == enums.TipoUsuario.Recepcion) {
        //    btHistoriaClinica.hide();
        //} else if (tipoUsuario == enums.TipoUsuario.Profesional) {
        //    btNew.hide();
        //    btEdit.hide();
        //    btRemove.hide();
        //}
    }

    function buildControls() {

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'fechaSolicitud', render: DataTableEx.renders.date },
            { data: 'paciente' },
            { data: 'documento' },
            { data: 'especialidad' },
            { data: 'dias' },
            { data: 'rangos' },
            { data: null, render: renderEstado },
            { data: 'fechaAsignacion', render: DataTableEx.renders.dateTime },
            { data: 'profesional' },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderAcciones }
        ];

        var order = [[0, 'desc']];

        dtView = MaestroLayout.initializeDatatable('idTurnoSolicitud', columns, order, 50, false, false);

        //$('input[name^=Provincia]').prop('readonly', true);
        //$('input[name^=Localidad]').prop('readonly', true);
        //$('input[name^=CodigoPostal]').prop('readonly', true);
    }

    function bindControlEvents() {

        btNuevoTurno.on('click', nuevoTurnoDialog);

        //$('select[name$="IdFinanciador"]').on('change', function (e) {
        //    reloadPlanes(e);
        //});

        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.tabChanged = tabChanged;

        //$('input[name^=SinCobertura]').change(function () {
        //    var checked = $(this).is(':checked');

        //    $('.financiador').prop('disabled', checked);
        //    //$('select[name^="IdFinanciador"]').prop('disabled', checked);
        //    $('.financiadorPlan').prop('disabled', checked);
        //    //$('select[name^="IdFinanciadorPla"]').prop('disabled', checked);
        //    $('.financiadorNro').prop('disabled', checked);
        //});

        dtView.table().on('click', 'tbody > tr > td > span.btAction', doAction);
        dtView.table().on('click', 'tbody > tr > td > span.btnCancelar', doCancelar);
    }

    function mainTableRowSelected(dataArray, api) {

        //btNew.prop('disabled', tipoUsuario == enums.TipoUsuario.Profesional);
        var tipoUsuario = $('#hidLoggedUserTipo').val();

        //btHistoriaClinica.prop('disabled', true);
        //btEdit.prop('disabled', true);
        //btRemove.prop('disabled', true);

        //if (dataArray === undefined || dataArray === null)
        //    return;

        //if (dataArray.length == 1) {
        //    btHistoriaClinica.prop('disabled', tipoUsuario == enums.TipoUsuario.Recepcion);    
        //    btEdit.prop('disabled', tipoUsuario == enums.TipoUsuario.Profesional);
        //    btRemove.prop('disabled', tipoUsuario != enums.TipoUsuario.Administrador);
        //}
        //else if (dataArray.length > 1) {
        //    btHistoriaClinica.prop('disabled', true);
        //    btEdit.prop('disabled', true);
        //    btRemove.prop('disabled', true);
        //}

    }

    function editDialogOpening($form, entity) {

        openingEditDialog = true;

        //$form.find('#IdPaciente_Edit').val(entity.idPaciente);
        //$form.find('#Nombre_Edit').val(entity.nombre);
        //$form.find('#Documento_Edit').val(entity.documento);
        //$form.find('#FechaNacimiento_Edit').val(moment(entity.fechaNacimiento).format(enums.FormatoFecha.DefaultFormat));
        //$form.find('#Nacionalidad_Edit').val(entity.nacionalidad);

        //$form.find('#IdFinanciador_Edit').select2('val', entity.idFinanciador);
        //$form.find('#FinanciadorNro_Edit').val(entity.financiadorNro);

        //$form.find('#Telefono_Edit').val(entity.telefono);
        //$form.find('#Email_Edit').val(entity.email);

        //reloadPlanes(null, $form.find('#IdFinanciador_Edit'), entity.idFinanciadorPlan);

        //if (!entity.idFinanciador) {
        //    $('#SinCobertura_Edit').prop('checked', true).trigger('change');
        //}
    }
    function tabChanged(dialog, $form, index, name) {

        dialog.addClass('modal-60');
    }

    function renderEstado(data, type, row) {

        return '<span class="badge badge-pills badge-' + data.estadoClase + ' font10">' + data.estado + '</span>';
    }
    function renderAcciones(data, type, row, meta) {
        var tipoUsuario = $('#hidLoggedUserTipo').val();

        var btnCancelar = '<span type="button" class="btn badge badge-danger btnCancelar" title="CANCELAR" data-id-turno-solicitud="' + data.idTurnoSolicitud + '" data-width-class="modal-70" data-ajax-action="/TurnosSolicitud/Cancelar" data-toggle="" data-placement="top" title="CANCELAR"><i class="fa fa-times"></i></span>';
        var btnAsignar = '<span type="button" class="btn badge badge-success btnAsignar btAction" title="ASIGNAR" data-id-turno="' + data.idTurno + '" data-ajax-action="/TurnosSolicitud/AsignarView?idTurnoSolicitud=' + data.idTurnoSolicitud + '" data-toggle="" data-placement="top" title="ASIGNAR"><i class="fa fa-check"></i></span>';

        var sReturn = '';

        
        //if (tipoUsuario != enums.TipoUsuario.Recepcion)
        //    sReturn = btnHistorial + '&nbsp;';

        switch (data.idEstadoTurnoSolicitud) {

            case enums.EstadoTurnoSolicitud.Solicitado:
                sReturn += btnCancelar + '&nbsp;' + btnAsignar;
                break;
            case enums.EstadoTurnoSolicitud.Asignado:
                sReturn += btnCancelar;
                break;                
            case enums.EstadoTurnoSolicitud.Cancelado:
                sReturn += '';
                break;
        }

        return sReturn;
    }

    function nuevoTurnoDialog() {

        var action = '/TurnosSolicitud/NuevoTurnoView';

        Modals.loadAnyModal('nuevoTurnoDialog', 'modal-70', action, function () { }, function () { });
    }

    function doAction() {

        var action = $(this).closest('span').data('ajax-action');
        var widthClass = $(this).closest('span').data('width-class');

        Modals.loadAnyModal('actionsDialog', widthClass, action, function () { }, function () { dtView.reload(); });
    }

    function doCancelar() {

        var action = $(this).closest('span').data('ajax-action');
        var idTurnoSolicitud = $(this).closest('span').data('id-turno-solicitud');

        Utils.modalQuestion("Cancelar turno", "¿Confirma la cancelación de la Solicitud de Turno?.",
            function (confirm) {
                if (confirm) {
                    Utils.modalLoader();

                    var params = {
                        idTurnoSolicitud: idTurnoSolicitud
                    };

                    $.post(action, params, function () {
                        Utils.modalClose();
                    })
                        .done(function (data) {

                            Utils.modalInfo('Solicitud CANCELADA', 'Se ha CANCELADO la solicitud de manera correcta', 5000, undefined, function () { });

                            setTimeout(function () {
                                location.reload(true);
                            }, 2000)
                        })
                        .fail(function () {
                            //alert("error");
                            Utils.modalError('Error CANCELANDO Solicitud', 'Error');
                        })
                        .always(function () {
                            //alert("finished");
                        });
                }
            }, "CONFIRMAR", "Cancelar", true);
    }
});

$(function () {
    ABMTurnosSolicitudView.init();
});
