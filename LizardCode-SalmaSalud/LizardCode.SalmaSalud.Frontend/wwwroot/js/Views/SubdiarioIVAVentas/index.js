/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ReportSubdiarioIVAVentasView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var btDownloadCitiVentas;
    var btDownloadCitiVentasAli;
    var btTotalesAlicuotas;
    var modalDetalleAlicuotas;

    var dtAlicuotas;


    this.init = function () {

        btDownloadCitiVentas = $('.toolbar-actions button.btDownloadCitiVentas', mainClass);
        btDownloadCitiVentasAli = $('.toolbar-actions button.btDownloadCitiVentasAli', mainClass);
        btTotalesAlicuotas = $('.toolbar-actions button.btTotalesAlicuotas', mainClass);

        modalDetalleAlicuotas = $('.modal.modalDetalleAlicuotas', mainClass);

        ReportLayout.init();

        var columns = [
            { data: 'idComprobanteVenta', visible: false },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'comprobante' },
            { data: 'sucursal' },
            { data: 'numero' },
            { data: 'cliente' },
            { data: 'cuit' },
            { data: 'tipoIVA' },
            { data: 'neto', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' },
            { data: 'noGravado', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' },
            { data: 'alicuota', render: DataTableEx.renders.percentage, className: 'dt-body-right dt-body-nowrap' },
            { data: 'importeAlicuota', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' },
            { data: 'importeTotal', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' }
        ];

        var order = [[0, 'asc']];
        var columnsFooterTotal = [8, 9, 11, 12];

        dtView = ReportLayout.initializeDatatable('idComprobanteVenta', columns, order, 30, false, columnsFooterTotal);

        btDownloadCitiVentas.on('click', retrieveFileCitiVentas);
        btDownloadCitiVentasAli.on('click', retrieveFileCitiVentasAlicuotas);
        btTotalesAlicuotas.on('click', retrieveTotalesAlicuotas);

        //Modal Detalle
        modalDetalleAlicuotas.find('> .modal-dialog').addClass('modal-40');
        modalDetalleAlicuotas
            .on('shown.bs.modal', function () {


            })
            .on('hidden.bs.modal', function () {
                var form = $('form', this);
                Utils.resetValidator(form);
            });

        $('.btExcelAlicuotas').on('click', function () {
            $(dtAlicuotas.api().buttons()[0].node).trigger('click');
        });

    }

    function retrieveTotalesAlicuotas() {

        var fechaDesde = $('#Filter_FechaDesde').val();
        var fechaHasta = $('#Filter_FechaHasta').val();
        var idCliente = $('#Filter_IdCliente').val();

        var params = {
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta,
            IdCliente: idCliente
        };

        Ajax.Execute('/SubdiarioIVAVentas/DetalleAlicuotas/', params)
            .done(function (response) {
                dtAlicuotas = $('.tableDetailAlicuotas').DataTableEx({
                    buttons: [
                        {
                            extend: "excel",
                            footer: true,
                            exportOptions: {
                                columns: ':visible'
                            }
                        }
                    ],
                    data: response,
                    info: false,
                    paging: false,
                    searching: false,
                    autoWidth: false,
                    destroy: true,
                    ordering: false,
                    columns: [
                        { data: 'alicuota', width: '40%', class: 'text-center' },
                        { data: 'netoGravado', width: '15%', class: 'text-right', render: DataTableEx.renders.currency },
                        { data: 'noGravado', width: '15%', class: 'text-right', render: DataTableEx.renders.currency },
                        { data: 'importeAlicuota', width: '15%', class: 'text-right', render: DataTableEx.renders.currency },
                        { data: 'total', width: '15%', class: 'text-right', render: DataTableEx.renders.currency }
                    ]
                });
            })
            .fail(Ajax.ShowError);

        modalDetalleAlicuotas.modal({ backdrop: 'static' });
    }

    function retrieveFileCitiVentas() {

        var fechaDesde = $('#Filter_FechaDesde').val();
        var fechaHasta = $('#Filter_FechaHasta').val();

        if (fechaDesde == "") {
            Utils.alertInfo("Ingrese Fecha Desde");
            return false;
        }
        if (fechaHasta == "") {
            Utils.alertInfo("Ingrese Fecha Hasta");
            return false;
        }

        var filters = {
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta
        };

        Utils.download('/SubdiarioIVAVentas/DescargarCITIVentas/', decodeURIComponent($.param(filters)), 'POST');

    }

    function retrieveFileCitiVentasAlicuotas() {

        var fechaDesde = $('#Filter_FechaDesde').val();
        var fechaHasta = $('#Filter_FechaHasta').val();

        if (fechaDesde == "") {
            Utils.alertInfo("Ingrese Fecha Desde");
            return false;
        }
        if (fechaHasta == "") {
            Utils.alertInfo("Ingrese Fecha Hasta");
            return false;
        }

        var filters = {
            FechaDesde: $('#Filter_FechaDesde').val(),
            FechaHasta: $('#Filter_FechaHasta').val()
        };

        Utils.download('/SubdiarioIVAVentas/DescargarCITIVentasAli/', decodeURIComponent($.param(filters)), 'POST');

    }

});

$(function () {
    ReportSubdiarioIVAVentasView.init();
});
