/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMMonedasView = new (function () {

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
            { data: 'idMoneda' },
            { data: 'descripcion' },
            { data: 'simbolo' },
            { data: 'codigo' }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idMoneda', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdMoneda_Edit').val(entity.idMoneda);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#Simbolo_Edit').val(entity.simbolo);
        $form.find('#Codigo_Edit').val(entity.codigo);

    }

});

$(function () {
    ABMMonedasView.init();
});
