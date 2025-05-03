/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMBancosView = new (function () {

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
            { data: 'idBanco' },
            { data: 'descripcion' },
            { data: 'cuit' },
            { data: 'nroCuenta' },
            { data: 'saldoDescubierto', render: DataTableEx.renders.currency }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idBanco', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;
    }


    function editDialogOpening($form, entity) {

        $form.find('#IdBanco_Edit').val(entity.idBanco);
        $form.find('#IdCuentaContable_Edit').select2('val', entity.idCuentaContable);
        $form.find('#IdMoneda_Edit').select2('val', entity.idMoneda);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#CUIT_Edit').val(entity.cuit);
        $form.find('#NroCuenta_Edit').val(entity.nroCuenta);
        $form.find('#CBU_Edit').val(entity.cbu);
        AutoNumeric.set($form.find('#SaldoDescubierto_Edit')[0], entity.saldoDescubierto);
        $form.find('#IdProveedor_Edit').select2('val', entity.idProveedor);
        $form.find('#EsDefault_Edit').prop('checked', entity.esDefault);

    }

});

$(function () {
    ABMBancosView.init();
});
