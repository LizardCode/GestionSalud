/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/masterDetail.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ReportListadoRetencionesView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var btDownloadSicore;

    this.init = function () {

        btDownloadSicore = $('.toolbar-actions button.btDownloadSicore', mainClass);

        ReportLayout.init();

        var columns = [
            { data: 'id', visible: false },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'comprobante' },
            { data: 'razonSocial' },
            { data: 'cuit' },
            { data: 'numeroComprobante' },
            {
                data: 'baseImponible',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            {
                data: 'importe',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-warning">${DataTableEx.renders.currency(data)}</span>`;
                }
            }
        ];

        var order = [[0, 'desc']];
        var columnsFooterTotal = [6, 7];
        
        dtView = ReportLayout.initializeDatatable('id', columns, order, 30, false, columnsFooterTotal);

        btDownloadSicore.on('click', retrieveFileSicore);

        $('#Filter_IdTipoRetecion').on('change', function (e) {
            e.preventDefault();
            ReportLayout.setExportMessageTop($('#Filter_IdTipoRetecion :selected').text());
        });

    }

    function retrieveFileSicore() {

        var idTipoRetencion = $('#Filter_IdTipoRetecion').select2('val');
        var fechaDesde = $('#Filter_FechaDesde').val();
        var fechaHasta = $('#Filter_FechaHasta').val();

        if (idTipoRetencion == "") {
            Utils.alertInfo("Ingrese el Filtro de Tipo Retención");
            return false;
        }
        if (fechaDesde == "") {
            Utils.alertInfo("Ingrese Fecha Desde");
            return false;
        }
        if (fechaHasta == "") {
            Utils.alertInfo("Ingrese Fecha Hasta");
            return false;
        }

        var filters = {
            IdTipoRetencion: idTipoRetencion,
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta
        };

        Utils.download('/ListadoRetenciones/DescargarSicore/', decodeURIComponent($.param(filters)), 'POST');

    }
});

$(function () {
    ReportListadoRetencionesView.init();
});
