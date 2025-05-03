/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ReportResumenCtaCteProView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var tableDetallePagar;
    var tableDetalleCtaCte;

    var btDetalleGeneralCtasPagar;
    var modalDetalleGeneralCtasPagar;

    var dtGeneralCtasPagar;

    var exportFilename = null;

    this.init = function () {

        btDetalleGeneralCtasPagar = $('.toolbar-actions button.btDetalleGeneralCtasPagar', mainClass);
        modalDetalleGeneralCtasPagar = $('.modal.modalDetalleGeneralCtasPagar', mainClass);

        ReportLayout.init();

        var columns = [
            { data: 'idProveedor', visible: false },
            { data: 'razonSocial' },
            { data: 'nombreFantasia' },
            { data: 'cuit' },
            { data: 'saldoPendiente', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'asc']];
        var columnsFooterTotal = [4];

        dtView = ReportLayout.initializeDatatable('idProveedor', columns, order, 30, false, columnsFooterTotal);

        btDetalleGeneralCtasPagar.on('click', retrieveDetalleGeneralCtasPagar);

        //Modal Detalle General
        modalDetalleGeneralCtasPagar.find('> .modal-dialog').addClass('modal-80');
        modalDetalleGeneralCtasPagar
            .on('shown.bs.modal', function () {

            })
            .on('hidden.bs.modal', function () {
                var form = $('form', this);
                Utils.resetValidator(form);
            });

        $('.btExcelDetalleGeneralCtasPagar').on('click', function () {
            $(dtGeneralCtasPagar.api().buttons()[0].node).trigger('click');
        });

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.pagar', retrieveDetalleCtaPagar);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.ctacte', retrieveDetalleCtaCte);

    }

    function retrieveDetalleGeneralCtasPagar() {

        var fechaDesde = $('#Filter_FechaDesde').val();
        var fechaHasta = $('#Filter_FechaHasta').val();

        var params = {
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta
        };

        Ajax.Execute('/ResumenCtaCtePro/DetalleGeneralCtasPagar/', params)
            .done(function (response) {
                dtGeneralCtasPagar = $('.tableDetalleGeneralCtasPagar').DataTableEx({
                    buttons: [
                        {
                            extend: "excel",
                            footer: true,
                            exportOptions: {
                                columns: ':visible'
                            }
                        }
                    ],
                    data: response.detail,
                    info: false,
                    paging: true,
                    lengthChange: false,
                    searching: false,
                    autoWidth: false,
                    destroy: true,
                    ordering: false,
                    columns: [
                        { data: 'proveedor', width: '40%' },
                        { data: 'cuit', width: '10%' },
                        { data: 'fecha', width: '10%', render: DataTableEx.renders.date },
                        { data: 'fechaVto', width: '10%', render: DataTableEx.renders.date },
                        { data: 'comprobante', width: '10%' },
                        { data: 'sucursal', visible: false },
                        { data: 'numero', visible: false },
                        {
                            data: null,
                            width: '10%',
                            orderable: false,
                            searchable: false,
                            class: 'text-center',
                            render: function (data, type, row) {
                                return `${row.sucursal}-${row.numero}`
                            }
                        },
                        { data: 'saldo', width: '10%', class: 'text-right', render: DataTableEx.renders.currency }
                    ]
                });
            })
            .fail(Ajax.ShowError);

        modalDetalleGeneralCtasPagar.modal({ backdrop: 'static' });
    }

    function renderViewDetails(data, type, row) {

        var btnCtasPagar = "", btnCtaCte = "";

        btnCtasPagar = '<i class="far fa-file-invoice-dollar pagar" title="Cuentas por Pagar"></i>';
        btnCtaCte = '<i class="far fa-file-invoice ctacte" title="Resumen Cuenta Corriente"></i>';

        return `
            <ul class="table-controls">
                <li>
                    ${btnCtasPagar}
                </li>
                <li>
                    ${btnCtaCte}
                </li>
            </ul>`;
    }

    function retrieveDetalleCtaCte() {

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
                idProveedor: row.data().idProveedor,
                fechaDesde: $('#Filter_FechaDesde').val(),
                fechaHasta: $('#Filter_FechaHasta').val()
            };

            Ajax.Execute('/ResumenCtaCtePro/GetCtaCteDetalle', params)
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowCtaCteDetalle(data));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }

    }

    function formatRowCtaCteDetalle(rowData) {

        var tableId = 'dt_ctaCteDetail';
        var $template = $('.templateDetalleCtasCte').clone();

        $template.removeAttr('class');
        $template.css('display', 'block');

        $template.find('.btnExcelDetalleCtasCobrar').on('click', function (e) {
            e.preventDefault();
            exportFilename = dtView.data($('.dt-view').find('tr.shown')).proveedor;
            $(tableDetalleCtaCte.api().buttons()[0].node).trigger('click')
        });

        $template.find('.btnPDFDetalleCtasCobrar').on('click', function (e) {
            e.preventDefault();
            exportFilename = dtView.data($('.dt-view').find('tr.shown')).proveedor;
            $(tableDetalleCtaCte.api().buttons()[1].node).trigger('click')
        });

        $template.find('.btnPrintDetalleCtasCobrar').on('click', function (e) {
            e.preventDefault();
            exportFilename = dtView.data($('.dt-view').find('tr.shown')).proveedor;
            $(tableDetalleCtaCte.api().buttons()[2].node).trigger('click')
        });

        tableDetalleCtaCte = $template
            .find('table')
            .attr('id', tableId)
            .on('click', '.pdf', function () {
                var tr = $(this).closest('tr');
                var rowData = tableDetalleCtaCte.data(tr);

                var action = "/OrdenesPago/Imprimir/";
                var params = { GUID: Utils.GenerateGUID() };

                Utils.download(`${action + rowData.idDocumento}`, params, 'GET');

            })
            .DataTableEx({
                data: rowData,
                buttons: [
                    {
                        filename: function () { return exportFilename; },
                        extend: "excel",
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        filename: function () { return exportFilename; },
                        extend: "pdfHtml5",
                        exportOptions: {
                            columns: ':visible'
                        },
                        pageSize: 'A4'
                    },
                    {
                        filename: function () { return exportFilename; },
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
                    { data: 'idDocumento', visible: false },
                    { data: 'fecha', width: '10%', orderable: false, render: DataTableEx.renders.date },
                    { data: 'comprobante', width: '30%', orderable: false },
                    { data: 'sucursal', visible: false },
                    { data: 'numero', visible: false },
                    { data: null, width: '35%', orderable: false, render: numeroComprobante },
                    { data: 'credito', width: '20%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency },
                    { data: 'debito', width: '20%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency },
                    { data: 'saldo', width: '20%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency },
                    { data: null, width: '5%', orderable: false, render: renderIconPDF }
                ]
            });

        var div = '<div class="table-detalle">Aguarde por favor...</div>';
        var $row = $(div);

        return $row.html($template);

    }

    function renderIconPDF(data, type, row) {

        if (row.idDocumento) {
            var btnComprobantePDF = `<i class="far fa-print pdf" title="Comprobante PDF"></i>`;

            return `
                <ul class="table-controls">
                    <li>
                        ${btnComprobantePDF}
                    </li>
                </ul>`;
        }
        else
            return "";

    }

    function retrieveDetalleCtaPagar() {

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
                idProveedor: row.data().idProveedor,
                fechaDesde: $('#Filter_FechaDesde').val(),
                fechaHasta: $('#Filter_FechaHasta').val()
            };

            Ajax.Execute('/ResumenCtaCtePro/GetCtasPagar', params)
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowCtasPagar(data));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }
    }

    function formatRowCtasPagar(rowData) {

        var tableId = 'dt_ctaPagarDetail';
        var $template = $('.templateDetalleCtasPagar').clone();

        $template.removeAttr('class');
        $template.css('display', 'block');

        $template.find('.btnExcelDetalleCtasCobrar').on('click', function (e) {
            e.preventDefault();
            exportFilename = dtView.data($('.dt-view').find('tr.shown')).proveedor;
            $(tableDetallePagar.api().buttons()[0].node).trigger('click')
        });

        $template.find('.btnPDFDetalleCtasCobrar').on('click', function (e) {
            e.preventDefault();
            exportFilename = dtView.data($('.dt-view').find('tr.shown')).proveedor;
            $(tableDetallePagar.api().buttons()[1].node).trigger('click')
        });

        $template.find('.btnPrintDetalleCtasCobrar').on('click', function (e) {
            e.preventDefault();
            exportFilename = dtView.data($('.dt-view').find('tr.shown')).proveedor;
            $(tableDetallePagar.api().buttons()[2].node).trigger('click')
        });

        tableDetallePagar = $template
            .find('table')
            .attr('id', tableId)
            .DataTableEx({
                data: rowData,
                buttons: [
                    {
                        filename: function () { return exportFilename; },
                        extend: "excel",
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        filename: function () { return exportFilename; },
                        extend: "pdfHtml5",
                        exportOptions: {
                            columns: ':visible'
                        },
                        pageSize: 'A4'
                    },
                    {
                        filename: function () { return exportFilename; },
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
                    { data: 'comprobante', width: '30%', orderable: false },
                    { data: 'sucursal', visible: false },
                    { data: 'numero', visible: false },
                    { data: null, width: '40%', orderable: false, render: numeroComprobante },
                    { data: 'saldo', width: '20%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency }
                ]
            });

        var div = '<div class="table-detalle">Aguarde por favor...</div>';
        var $row = $(div);

        return $row.html($template);
    }

    function numeroComprobante(data, type, row) {

        if (row.idComprobante != -1)
            return `${String("0000" + row.sucursal).slice(-4)}-${String("00000000" + row.numero).slice(-8)}`
        return "";

    }

});

$(function () {
    ReportResumenCtaCteProView.init();
});
