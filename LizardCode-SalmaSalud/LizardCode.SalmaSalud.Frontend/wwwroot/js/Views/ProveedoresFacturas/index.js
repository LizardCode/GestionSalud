/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ReportProveedoresFacturasView = new (function () {

    var self = this;

    this.init = function () {

        ReportLayout.init();

        var columns = [
            { data: 'idComprobanteCompra', visible: false },
            { data: 'comprobante', width: '10%' },
            { data: 'sucursal', visible: false },
            { data: 'numero', visible: false },
            {
                data: null,
                width: '10%',
                render: function (data, type, row, meta) {
                    return `${String("00000" + row.sucursal).slice(-5)}-${String("00000000" + row.numero).slice(-8)}`
                }
            },
            { data: 'fecha', width: '5%', render: DataTableEx.renders.date },
            { data: 'subtotal', visible: false },
            {
                data: 'total',
                width: '10%',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { data: 'pagada', orderable: false, searchable: false, class: 'text-center', width: '5%', render: renderPagada },
            { data: 'cae', orderable: false, searchable: false, width: '10%', class: 'text-center' },
            { data: 'vencimientoCAE', orderable: false, searchable: false, width: '5%', class: 'text-center', render: DataTableEx.renders.date },
        ];

        var order = [[0, 'desc']];
        //var columnsFooterTotal = [8, 9, 11, 12];

        dtView = ReportLayout.initializeDatatable('idComprobanteCompra', columns, order, 50, false); //, columnsFooterTotal);

    }

    function renderPagada(data, type, row) {

        if (data === true)
            return `<span class="badge badge-success">PAGADA</span>`;
        else
            return `<span class="badge badge-warning">INGRESADA</span>`;
    }
});

$(function () {
    ReportProveedoresFacturasView.init();
});
