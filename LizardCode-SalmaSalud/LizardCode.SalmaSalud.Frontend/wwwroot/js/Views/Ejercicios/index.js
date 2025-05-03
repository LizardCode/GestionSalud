/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMEjerciciosView = new (function () {

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
            { data: 'idEjercicio', visible: false },
            { data: 'codigo' },
            { data: 'fechaInicio', render: DataTableEx.renders.date },
            { data: 'fechaFin', render: DataTableEx.renders.date },
            { data: 'cerrado', render: renderCerrado }
        ];

        var order = [[2, 'desc']];

        dtView = MaestroLayout.initializeDatatable('idEjercicio', columns, order);

    }

    function renderCerrado(data) {
        if (data == enums.Common.No)
            return '<span class="badge badge-success"> No </span>';
        return '<span class="badge badge-danger"> Si </span>';
    }

    function bindControlEvents() {

    }

});

$(function () {
    ABMEjerciciosView.init();
});
