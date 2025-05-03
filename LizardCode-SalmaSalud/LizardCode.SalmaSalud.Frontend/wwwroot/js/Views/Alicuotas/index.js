/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMAlicuotasView = new (function () {

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
            { data: 'idAlicuota' },
            { data: 'descripcion' },
            { data: 'tipoAlicuota' },
            { data: 'valor' }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idAlicuota', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdAlicuota_Edit').val(entity.idAlicuota);
        $form.find('#IdTipoAlicuota_Edit').select2('val', entity.idTipoAlicuota);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#Valor_Edit').val(entity.valor);
        $form.find('#CodigoAFIP_Edit').val(entity.codigoAFIP);

    }

});

$(function () {
    ABMAlicuotasView.init();
});
