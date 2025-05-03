/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ReportBalanceSumSdoView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    this.init = function () {

        ReportLayout.init();

        var columns = [
            { data: 'item', visible: false },
            { data: 'idCuentaContable', width: "10%", orderable: false, searchable: false},
            { data: 'codigoIntegracion', width: "10%", orderable: false, searchable: false },
            { data: 'descripcion', width: "50%", render: renderCuenta, orderable: false, searchable: false },
            { data: 'debe', width: "15%", orderable: false, searchable: false, render: renderImporte, class: "table-td-right" },
            { data: 'haber', width: "15%", orderable: false, searchable: false, render: renderImporte, class: "table-td-right" },
            { data: 'deudor', width: "15%", orderable: false, searchable: false, render: renderImporte, class: "table-td-right" },
            { data: 'acredor', width: "15%", orderable: false, searchable: false, render: renderImporte, class: "table-td-right" }
        ];

        var order = [[0, 'asc']];

        dtView = ReportLayout.initializeDatatable('item', columns, order, 1000);
    }

    function renderImporte(data, type, row, meta) {
        if (data == null)
            return "";
        else {
            if (row.descripcion == "Total")
                return `<strong>${accounting.formatMoney(data)}</strong>`;
            return accounting.formatMoney(data);
        }
    }

    function renderCuenta(data, type, row, meta) {
        if(data == "Total")
            return "<strong>" + data + "</strong>";
        return data;
    }
});

$(function () {
    ReportBalanceSumSdoView.init();
});
