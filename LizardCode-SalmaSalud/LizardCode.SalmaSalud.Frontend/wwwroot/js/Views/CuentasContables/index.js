/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMCuentasContablesView = new (function () {

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
            { data: 'idCuentaContable' },
            { data: 'codigoCuenta' },
            { data: 'descripcion' },
            { data: 'codigoRubro' },
            { data: 'rubro' }
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idCuentaContable', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;
    }


    function editDialogOpening($form, entity) {

        $form.find('#IdCuentaContable_Edit').val(entity.idCuentaContable);
        $form.find('#IdRubroContable_Edit').select2('val', entity.idRubroContable);
        $form.find('#CodigoCuenta_Edit').val(entity.codigoCuenta);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#IdCodigoObservacion_Edit').select2('val', entity.idCodigoObservacion);
        $form.find('#EsCtaGastos_Edit').prop('checked', entity.esCtaGastos);
       
    }

});

$(function () {
    ABMCuentasContablesView.init();
});
