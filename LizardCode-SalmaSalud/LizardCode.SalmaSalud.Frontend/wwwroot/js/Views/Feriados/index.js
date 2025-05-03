/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMFeriadosView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');


    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }

    function buildControls() {

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'tipoFeriado' },
            { data: 'nombre' }
        ];

        var order = [[0, 'desc']];

        dtView = MaestroLayout.initializeDatatable('idFeriado', columns, order);

    }

    function bindControlEvents() {
        modalNew.find('> .modal-dialog').addClass('modal-50');
        modalEdit.find('> .modal-dialog').addClass('modal-50');

        MaestroLayout.editDialogOpening = editDialogOpening;

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdFeriado_Edit').val(entity.idFeriado);
        $form.find('#Fecha_Edit').val(moment(entity.fecha).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#IdTipoFeriado_Edit').select2('val', entity.idTipoFeriado);
        $form.find('#Nombre_Edit').val(entity.nombre);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#Codigo_Edit').val(entity.codigo);

    }

});

$(function () {
    ABMFeriadosView.init();
});
