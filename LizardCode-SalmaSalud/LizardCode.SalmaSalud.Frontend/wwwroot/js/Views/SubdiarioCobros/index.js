/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var SubdiarioCobrosView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var tableDetalle;
    var btTotalesImputacion;
    var modalDetalleImputacion;

    var dtImputaciones;

    this.init = function () {

        btTotalesImputacion = $('.toolbar-actions button.btTotalesImputacion', mainClass);
        modalDetalleImputacion = $('.modal.modalDetalleImputacion', mainClass);

        ReportLayout.init();

        var columns = [
            { data: 'idRecibo', visible: false },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'comprobante' },
            { data: 'numero' },
            { data: 'cliente' },
            { data: 'cuit' },
            { data: 'total', render: DataTableEx.renders.currency, class: 'text-right' },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var columnDefs = [
            { targets: 6, className: 'dt-body-right' }
        ];

        var order = [[0, 'asc']];

        dtView = ReportLayout.initializeDatatable('idRecibo', columns, order, 30, false, columnDefs);

        dtView.table().on('click', 'tbody > tr > td > i.detalle', retrieveDetalle);
        btTotalesImputacion.on('click', retrieveTotalesImputaciones);

        $('.btGetExcel').click(function () {
            var params = {
                idRecibo: 0,
                fechaDesde: $('#Filter_FechaDesde').val(),
                fechaHasta: $('#Filter_FechaHasta').val()
            };

            Utils.descargarDocumento('Generando reporte...', RootPath + 'SubdiarioCobros/GenerarExcel',
                null, RootPath + 'SubdiarioCobros/DescargarExcel', params);
        });

        //Modal Detalle
        modalDetalleImputacion.find('> .modal-dialog').addClass('modal-40');
        modalDetalleImputacion
            .on('shown.bs.modal', function () {


            })
            .on('hidden.bs.modal', function () {
                var form = $('form', this);
                Utils.resetValidator(form);
            });

        $('.btExcelImputacion').on('click', function () {
            $(dtImputaciones.api().buttons()[0].node).trigger('click');
        });
    }

    function retrieveTotalesImputaciones() {

        var fechaDesde = $('#Filter_FechaDesde').val();
        var fechaHasta = $('#Filter_FechaHasta').val();

        var params = {
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta
        };

        Ajax.Execute('/SubdiarioCobros/DetalleImputaciones/', params)
            .done(function (response) {
                dtImputaciones = $('.tableDetailImputacion').DataTableEx({
                    buttons: [
                        {
                            extend: "excel",
                            excelStyles: [
                                { template: 'blue_gray_medium' }
                            ],
                            exportOptions: {
                                columns: ':visible',
                                format: {
                                    body: function (data, row, column, node) {
                                        return [2, 3].includes(column) ?
                                            `$${accounting.unformat(data, ",")}` :
                                            data;
                                    }
                                }
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
                        { data: 'codigoCuenta', width: '20%', class: 'text-center' },
                        { data: 'nombreCuenta', width: '50%', class: 'text-center' },
                        { data: 'debitos', width: '30%', class: 'text-right', render: DataTableEx.renders.currency },
                        { data: 'creditos', width: '30%', class: 'text-right', render: DataTableEx.renders.currency }
                    ]
                });
            })
            .fail(Ajax.ShowError);

        modalDetalleImputacion.modal({ backdrop: 'static' });
    }

    function renderViewDetails(data, type, row) {

        var btnDetalle = '<i class="far fa-search-plus detalle" style="cursor:pointer;" title="Detalle asientos"></i>';

        return btnDetalle;
    }

    function retrieveDetalle() {

        var tr = $(this).closest('tr');
        var row = dtView.api().row(tr);
        var loader =
            `<div class="text-center">
                <div class="loader-sm spinner-grow text-success"></div>
            </div>`;

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
            $(this).removeClass('fa-search-minus');
            $(this).addClass('fa-search-plus');
            $(this).removeClass('expanded');
        }
        else {
            row.child(loader).show();
            tr.addClass('shown');
            $(this).removeClass('fa-search-plus');
            $(this).addClass('fa-search-minus');
            $(this).addClass('expanded');

            var params = {
                idRecibo: row.data().idRecibo,
                fechaDesde: $('#Filter_FechaDesde').val(),
                fechaHasta: $('#Filter_FechaHasta').val()
            };

            Ajax.Execute('/SubdiarioCobros/GetDetalle', params)
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowDetalle(data));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }

    }

    function formatRowDetalle(rowData) {

        var tableId = 'dt_detail';
        var $template = $('.templateDetalle').clone();

        $template.removeAttr('class');
        $template.css('display', 'block');

        tableDetalle = $template
            .find('table')
            .attr('id', tableId)
            .DataTableEx({
                data: rowData,
                info: false,
                paging: false,
                searching: false,
                autoWidth: false,
                ordering: false,
                columns: [
                    
                    { data: 'codigoCuenta', width: '30%' },
                    { data: 'nombreCuenta', width: '50%' },
                    { data: 'importe', width: '20%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency }
                ]
            });

        var div = '<div class="table-detalle">Aguarde por favor...</div>';
        var $row = $(div);

        return $row.html($template);

    }
});

$(function () {
    SubdiarioCobrosView.init();
});
