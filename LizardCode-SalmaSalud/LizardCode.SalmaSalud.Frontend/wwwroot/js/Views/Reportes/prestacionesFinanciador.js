/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var PrestacionesFinanciadorView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    this.init = function () {

        ReportLayout.init();

        var columns = [
            { data: 'idEvolucionPrestacion', visible: false, orderable: false, searchable: false },
            { data: 'fecha', width: "5%", render: DataTableEx.renders.date },
            { data: 'idTipoPrestacion', width: "10%", orderable: false, searchable: false, render: renderTipo },
            { data: 'prestacion', width: "25%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'codigo', width: "10%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'financiador', width: "20%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'especialidad', width: "15%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'valor', width: "10%", class: 'text-right', orderable: false, searchable: false, render: renderImporte }
        ];

        var order = [[0, 'asc']];
        var columnsFooterTotal = [7];

        dtView = ReportLayout.initializeDatatable('idEvolucionPrestacion', columns, order, 30, false, columnsFooterTotal);
    }

    function renderTipo (data, type, row, meta) {
        if (data == 1)
            return '<span class="badge badge-pills badge-info font10">PRESTACION</span>';
        else
            return '<span class="badge badge-pills badge-secondary font10">CO-PAGO</span>';
    }

    function renderImporte(data, type, row, meta) {
        if (data == null)
            return "";
        else
            return accounting.formatMoney(data);
    }
});

$(function () {
    PrestacionesFinanciadorView.init();
});
