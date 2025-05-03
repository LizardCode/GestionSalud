/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMArticulosView = new (function () {

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
            { data: 'idArticulo' },
            { data: 'codigoBarras' },
            { data: 'descripcion' },
            { data: 'alicuota' }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idArticulo', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;
    }


    function editDialogOpening($form, entity) {

        $form.find('#IdArticulo_Edit').val(entity.idArticulo);
        $form.find('#IdRubroArticulo_Edit').select2('val', entity.idRubroArticulo);
        $form.find('#CodigoBarras_Edit').val(entity.codigoBarras);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#Alicuota_Edit').select2('val', entity.alicuota);
        $form.find('#Detalle_Edit').val(entity.detalle);
        $form.find('#IdCuentaContableVentas_Edit').select2('val', entity.idCuentaContableVentas);
        $form.find('#IdCuentaContableCompras_Edit').select2('val', entity.idCuentaContableCompras);

    }

});

$(function () {
    ABMArticulosView.init();
});
