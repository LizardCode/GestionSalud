/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMCondicionVentasComprasView = new (function () {

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
            { data: 'idCondicion' },
            { data: 'descripcion' },
            { data: 'dias' },
            { data: 'tipoCondicion', render: tipoCondicion }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idCondicion', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;

    }

    function tipoCondicion(data, type, row) {

        return '<span class="badge badge-success"> ' + data + ' </span>';

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdCondicion_Edit').val(entity.idCondicion);        
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#Dias_Edit').val(entity.dias);
        $form.find('#IdTipoCondicion_Edit').select2('val', entity.idTipoCondicion);

    }

});

$(function () {
    ABMCondicionVentasComprasView.init();
});
