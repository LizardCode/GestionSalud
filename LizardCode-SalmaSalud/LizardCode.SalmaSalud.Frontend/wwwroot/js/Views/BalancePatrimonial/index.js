/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ReportBalancePatrimonialView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    this.init = function () {

        ReportLayout.init();

        var columns = [
            { data: 'item', visible: false, orderable: false, searchable: false },
            { data: 'rubro', width: "10%", orderable: false, searchable: false, render: renderRubro },
            { data: 'codigoIntegracion', width: "10%", orderable: false, searchable: false },
            { data: 'numeroCuenta', visible: false, orderable: false, searchable: false },
            { data: 'descripcion', width: "50%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'saldo', width: "15%", orderable: false, searchable: false, render: renderImporte },
            { data: 'total', width: "15%", orderable: false, searchable: false, render: renderImporteTotal }
        ];

        var order = [[0, 'asc']];

        dtView = ReportLayout.initializeDatatable('rubro', columns, order, 1000);
    }

    function renderImporte (data, type, row, meta) {
        if (data == null)
            return "";
        else
            return accounting.formatMoney(data);
    }

    function renderImporteTotal(data, type, row, meta) {
        if (data == null)
            return "";
        else
            return `<strong>${accounting.formatMoney(data)}</strong>`;
    }

    function renderRubro(data, type, row, meta) {
        return "<strong>" + data + "</strong>";
    }
});

$(function () {
    ReportBalancePatrimonialView.init();
});
