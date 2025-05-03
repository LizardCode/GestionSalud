/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var PrestacionesProfesionalView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    this.init = function () {

        ReportLayout.init();

        var columns = [
            { data: 'idEvolucionPrestacion', visible: false, orderable: false, searchable: false },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'prestacion', width: "20%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'codigo', width: "5%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'profesional', width: "20%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'especialidad', width: "15%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'valor', class: 'text-right', width: "15%", orderable: false, searchable: false, render: renderImporte }
        ];

        var order = [[0, 'asc']];
        var columnsFooterTotal = [6];

        dtView = ReportLayout.initializeDatatable('idEvolucionPrestacion', columns, order, 30, false, columnsFooterTotal);
    }

    function renderImporte (data, type, row, meta) {
        if (data == null)
            return "";
        else
            return accounting.formatMoney(data);
    }
});

$(function () {
    PrestacionesProfesionalView.init();
});
