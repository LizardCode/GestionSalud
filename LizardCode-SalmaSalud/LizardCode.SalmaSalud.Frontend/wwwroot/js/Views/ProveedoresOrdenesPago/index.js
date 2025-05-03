/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ReportProveedoresFacturasView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var modalPrint;
    var btImprimir;

    this.init = function () {

        modalPrint = $('.modal.modalPrint', mainClass);
        btImprimir = $('.toolbar-actions button.btImprimirRetenciones', mainClass);
        btImprimir.prop('disabled', true);

        ReportLayout.init();

        var columns = [
            { data: 'idOrdenPago' },
            { data: 'fecha', width: '5%', render: DataTableEx.renders.date },
            { data: 'moneda' },
            {
                data: 'importe',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { data: 'idEstadoOrdenPago', orderable: false, searchable: false, class: 'text-center', render: renderEstadoOrdenPago }
        ];

        var order = [[0, 'asc']];
        //var columnsFooterTotal = [8, 9, 11, 12];

        dtView = ReportLayout.initializeDatatable('idOrdenPago', columns, order, 50, false, undefined, true); //, columnsFooterTotal);


        ReportLayout.mainTableRowSelected = mainTableRowSelected;

        btImprimir.on('click', printDialog);
        modalPrint.find('button.btPrintOrdenPago').on('click', imprimir);
        modalPrint.find('button.btPrintGanancias').on('click', imprimirRetencionesGanancias);
        modalPrint.find('button.btPrintIVA').on('click', imprimirRetencionesIVA);
        modalPrint.find('button.btPrintIIBB').on('click', imprimirRetencionesIIBB);
        modalPrint.find('button.btPrintSUSS').on('click', imprimirRetencionesSUSS);
    }

    function renderEstadoOrdenPago(data, type, row) {

        switch (row.idEstadoOrdenPago) {
            case enums.EstadoOrdenPago.Ingresada:
                return '<span class="badge badge-danger"> Ingresada </span>';
            case enums.EstadoOrdenPago.Pagada:
                return '<span class="badge badge-success"> Pagada </span>';
        }
    }

    function mainTableRowSelected(dataArray, api) {

        btImprimir.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        btImprimir.prop('disabled', !dataArray.length == 1);
    }

    function printDialog() {

        modalPrint.modal({ backdrop: 'static' });

    }

    function imprimir() {

        var id = dtView.selected()[0].idOrdenPago;
        var action = $(this).data('ajax-action') + id;

        window.open(action);
    }

    function imprimirRetencionesGanancias() {

        var id = dtView.selected()[0].idOrdenPago;
        var params = {
            id: id,
            idTipoRetencion: 1
        };

        Utils.descargarDocumento('Generando...', RootPath + 'OrdenesPago/GenerarRetencionPDF', null, RootPath + 'OrdenesPago/DescargarRetencionPDF', params);
    }

    function imprimirRetencionesIVA() {

        var id = dtView.selected()[0].idOrdenPago;
        var params = {
            id: id,
            idTipoRetencion: 2
        };

        Utils.descargarDocumento('Generando...', RootPath + 'OrdenesPago/GenerarRetencionPDF', null, RootPath + 'OrdenesPago/DescargarRetencionPDF', params);
    }

    function imprimirRetencionesIIBB() {

        var id = dtView.selected()[0].idOrdenPago;
        var params = {
            id: id,
            idTipoRetencion: 3
        };

        Utils.descargarDocumento('Generando...', RootPath + 'OrdenesPago/GenerarRetencionPDF', null, RootPath + 'OrdenesPago/DescargarRetencionPDF', params);
    }

    function imprimirRetencionesSUSS() {

        var id = dtView.selected()[0].idOrdenPago;
        var params = {
            id: id,
            idTipoRetencion: 4
        };

        Utils.descargarDocumento('Generando...', RootPath + 'OrdenesPago/GenerarRetencionPDF', null, RootPath + 'OrdenesPago/DescargarRetencionPDF', params);
    }
});

$(function () {
    ReportProveedoresFacturasView.init();
});
