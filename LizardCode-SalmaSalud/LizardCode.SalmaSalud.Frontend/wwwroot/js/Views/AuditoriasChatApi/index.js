/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var AuditoriasChatApiView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var btConsHTML = '<i class="fas fa-eye"></i> <span>Consultar</span>';

    var modalEdit;
    var btEdit;
    var modalEditSave;

    this.init = function () {

        modalEdit = $('.modal.modalEdit', mainClass);
        btEdit = $('.toolbar-actions button.btEdit', mainClass);
        modalEditSave = $('.modal.modalEdit button.btSave', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }

    function buildControls() {

        modalEdit.find('> .modal-dialog').addClass('modal-75');
        btEdit.html(btConsHTML);

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'idAuditoria' },
            { data: 'fecha', render: DataTableEx.renders.dateTimeSecs },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderEvento },    
            { data: 'paciente' },
            { data: 'telefono' },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderEstado },    
        ];

        var order = [[0, 'desc']];

        dtView = MaestroLayout.initializeDatatable('idAuditoria', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.mainTableDraw = mainTableDraw;
        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.editDialogShown = editDialogShown;

    }

    function editDialogShown(dialog, $form) {

        $form.find('input, select, textarea').attr('readonly', 'readonly').attr('disabled', 'disabled');
        modalEditSave.hide();
    }

    function mainTableRowSelected(dataArray, api) {

        var data = dtView.selected();

        btEdit.prop('disabled', true);

        if (data.length == 1) {
            btEdit.prop('disabled', false);
        }

    }

    function mainTableDraw() {

        var data = dtView.selected();

        btEdit.prop('disabled', true);

        if (data.length == 1) {
            btEdit.prop('disabled', false);
        }

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdAuditoria_Edit').val(entity.idAuditoria);
        $form.find('#Fecha_Edit').val(moment(entity.fecha).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#Paciente_Edit').val(entity.paciente);
        $form.find('#Telefono_Edit').val(entity.telefono);
        $form.find('#Respuesta_Edit').val(entity.respuesta);

    }

    function renderEvento(data, type, row) {

        return '<span class="badge badge-' + data.eventoClase + '">' + data.evento + '</span>';
    }

    function renderEstado(data, type, row) {

        return '<span class="badge badge-' + data.estadoClase + '">' + data.estado + '</span>';
    }

});

$(function () {
    AuditoriasChatApiView.init();
});
