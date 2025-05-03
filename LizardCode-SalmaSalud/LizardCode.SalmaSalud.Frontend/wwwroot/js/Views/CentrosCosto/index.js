/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMCentrosCostoView = new (function () {

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
            { data: 'idCentroCosto' },
            { data: 'descripcion' }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idCentroCosto', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;

    }


    function editDialogOpening($form, entity) {

        $form.find('#IdCentroCosto_Edit').val(entity.idCentroCosto);
        $form.find('#Descripcion_Edit').val(entity.descripcion);

    }
    
});

$(function () {
    ABMCentrosCostoView.init();
});
