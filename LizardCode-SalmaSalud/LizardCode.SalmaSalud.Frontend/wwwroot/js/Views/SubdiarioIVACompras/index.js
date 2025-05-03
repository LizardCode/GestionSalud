/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ReportSubdiarioIVAComprasView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var btDownloadCitiCompras;
    var btDownloadCitiComprasAli;
    var btTotalesAlicuotas;
    var modalDetalleAlicuotas;

    var dtAlicuotas;

    this.init = function () {

        btDownloadCitiCompras = $('.toolbar-actions button.btDownloadCitiCompras', mainClass);
        btDownloadCitiComprasAli = $('.toolbar-actions button.btDownloadCitiComprasAli', mainClass);
        btTotalesAlicuotas = $('.toolbar-actions button.btTotalesAlicuotas', mainClass);

        modalDetalleAlicuotas = $('.modal.modalDetalleAlicuotas', mainClass);

        ReportLayout.init();

        var columns = [
            { data: 'idComprobanteCompra', visible: false },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'comprobante' },
            { data: 'sucursal' },
            { data: 'numero' },
            { data: 'proveedor' },
            { data: 'cuit' },
            { data: 'tipoIVA' },
            { data: 'neto', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' },
            { data: 'noGravado', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' },
            { data: 'alicuota', render: DataTableEx.renders.percentage, className: 'dt-body-right dt-body-nowrap' },
            { data: 'importeAlicuota', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' },
            { data: 'importeTotal', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' },
            { data: 'percepcionIVA', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' },
            { data: 'percepcionIB', render: DataTableEx.renders.currency, className: 'dt-body-right dt-body-nowrap' }
        ];

        var order = [[0, 'asc']];
        var columnsFooterTotal = [8, 9, 11, 12, 13, 14];

        dtView = ReportLayout.initializeDatatable('idComprobanteCompras', columns, order, 30, false, columnsFooterTotal);

        btDownloadCitiCompras.on('click', retrieveFileCitiCompras);
        btDownloadCitiComprasAli.on('click', retrieveFileCitiComprasAlicuotas);
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
        var idProveedor = $('#Filter_IdProveedor').val();

        var params = {
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta,
            IdProveedor: idProveedor
        };

        Ajax.Execute('/SubdiarioIVACompras/DetalleAlicuotas/', params)
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

    function retrieveFileCitiCompras() {

        var fechaDesde = $('#Filter_FechaDesde').val();
        var fechaHasta = $('#Filter_FechaDesde').val();

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

        Utils.download('/SubdiarioIVACompras/DescargarCITICompras/', decodeURIComponent($.param(filters)), 'POST');

    }

    function retrieveFileCitiComprasAlicuotas() {

        var fechaDesde = $('#Filter_FechaDesde').val();
        var fechaHasta = $('#Filter_FechaDesde').val();

        if (fechaDesde == "") {
            Utils.alertInfo("Ingrese Fecha Desde");
            return false;
        }
        if (fechaHasta == "") {
            Utils.alertInfo("Ingrese Fecha Hasta");
            return false;
        }

        var filters = {
            FechaDesde: fechaHasta,
            FechaHasta: fechaDesde
        };

        Utils.download('/SubdiarioIVACompras/DescargarCITIComprasAli/', decodeURIComponent($.param(filters)), 'POST');

    }

});

$(function () {
    ReportSubdiarioIVAComprasView.init();
});
