/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ReportMayorCuentasView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var tableDetalleMayorCuenta;

    this.init = function () {

        ReportLayout.init();

        var columns = [
            { data: 'idCuentaContable' },
            { data: 'codigoCuenta' },
            { data: 'descripcion' },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'asc']];

        dtView = ReportLayout.initializeDatatable('idCuentaContable', columns, order);

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.mayor', retrieveDetalleMayorCuenta);

    }

    function renderViewDetails(data, type, row) {

        var btnMayorCuenta = "";

        btnMayorCuenta = '<i class="far fa-search-plus mayor" title="Mayor de Cuenta"></i>';

        return `
            <ul class="table-controls">
                <li>
                    ${btnMayorCuenta}
                </li>
            </ul>`;
    }

    function retrieveDetalleMayorCuenta() {

        var tr = $(this).closest('tr');
        var row = dtView.api().row(tr);
        var loader =
            `<div class="text-center">
                <div class="loader-sm spinner-grow text-success"></div>
                <div class="loader-sm spinner-grow text-success"></div>
                <div class="loader-sm spinner-grow text-success"></div>
            </div>`;

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
            $(this).removeClass('expanded');
        }
        else {
            row.child(loader).show();
            tr.addClass('shown');
            $(this).addClass('expanded');

            var params = {
                idCuentaContable: row.data().idCuentaContable,
                fechaDesde: $('#Filter_FechaDesde').val(),
                fechaHasta: $('#Filter_FechaHasta').val(),
                idEjercicio: $('#Filter_IdEjercicio').val(),
            };

            Ajax.Execute('/MayorCuentas/GetMayorCuentaDetalle', params)
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowMayorCuentaDetalle(data));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }        

    }

    function formatRowMayorCuentaDetalle(rowData) {

        var tableId = 'dt_mayorCuentaDetail';
        var $template = $('.templateDetalleMayorCuenta').clone();

        $template.removeAttr('class');
        $template.css('display', 'block');

        $template.find('.btnExcelDetalleMayorCuenta').on('click', function (e) {
            e.preventDefault();
            $(tableDetalleMayorCuenta.api().buttons()[0].node).trigger('click')
        });

        $template.find('.btnPDFDetalleMayorCuenta').on('click', function (e) {
            e.preventDefault();
            $(tableDetalleMayorCuenta.api().buttons()[1].node).trigger('click')
        });

        $template.find('.btnPrintDetalleMayorCuenta').on('click', function (e) {
            e.preventDefault();
            $(tableDetalleMayorCuenta.api().buttons()[2].node).trigger('click')
        });

        tableDetalleMayorCuenta = $template
            .find('table')
            .attr('id', tableId)
            .DataTableEx({
                data: rowData,
                buttons: [
                    {
                        extend: "excel",
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: "pdfHtml5",
                        exportOptions: {
                            columns: ':visible'
                        },
                        pageSize: 'A4'
                    },
                    {
                        extend: "print",
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    'colvis'
                ],
                info: false,
                paging: false,
                searching: false,
                autoWidth: false,
                ordering: false,
                columns: [
                    { data: 'fecha', width: '10%', orderable: false, render: DataTableEx.renders.date },
                    { data: 'descripcion', width: '30%', orderable: false },
                    { data: 'debitos', width: '20%', orderable: false, class: 'text-right', render: renderImporte },
                    { data: 'creditos', width: '20%', orderable: false, class: 'text-right', render: renderImporte },
                    { data: 'saldo', width: '20%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency }
                ]
            });

        var div = '<div class="table-detalle">Aguarde por favor...</div>';
        var $row = $(div);

        return $row.html($template);

    }

    function renderImporte (data, type, row, meta) {
        if (data == 0)
            return "";
        else
            return accounting.formatMoney(data);
    }
});

$(function () {
    ReportMayorCuentasView.init();
});
