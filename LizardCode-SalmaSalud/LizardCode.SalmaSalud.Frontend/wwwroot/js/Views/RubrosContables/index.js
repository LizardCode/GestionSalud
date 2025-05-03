/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMRubrosContablesView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var btEdit;
    var btRemove;


    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        btEdit = $('.toolbar-actions button.btEdit', mainClass);
        btRemove = $('.toolbar-actions button.btRemove', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }


    function buildControls() {

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'idRubroContable', render: renderStrong },
            { data: 'codigoRubro', render: renderStrong },
            { data: 'descripcion', render: renderStrong },
            { data: 'rubroPadre' }
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idRubroContable', columns, order, 50);

    }

    function renderStrong(data, type, row) {
        if (row.rubroPadre == null)
            return `<strong>${data}</strong>`;
        else
            return data;
    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.mainTableRowSelected = mainTableRowSelected;

        $('input[name^="IdRubroPadre"]')
            .Select2Ex({ url: '/RubrosContables/GetRubrosContables', allowClear: true, placeholder: 'Buscar...', minimumInputLength: 1 });
    }

    function mainTableRowSelected(dataArray, api) {

        btEdit.prop('disabled', true);
        btRemove.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        if (dataArray.length == 1) {
            btEdit.prop('disabled', dataArray[0].rubroPadre == null);
            btRemove.prop('disabled', dataArray[0].rubroPadre == null);
        }

    }

    function newDialogOpening(dialog, $form) {

        $form.find('#IdRubroPadre').select2('data', null);

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdRubroContable_Edit').val(entity.idRubroContable);
        $form.find('#CodigoRubro_Edit').val(entity.codigoRubro);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#IdRubroPadre_Edit').select2('data', { id: entity.idRubroPadre, text: entity.rubroPadre });
       
    }

});

$(function () {
    ABMRubrosContablesView.init();
});
