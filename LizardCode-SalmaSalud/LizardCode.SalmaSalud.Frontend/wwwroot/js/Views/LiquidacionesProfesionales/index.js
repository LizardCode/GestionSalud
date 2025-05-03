/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMLiquidacionesProfesionalesView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var tableDetalle;
    var exportFilename = null;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);

        btRemove = $('.toolbar-actions button.btRemove', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        MaestroLayout.errorTooltips = false;

        modalNew.find('> .modal-dialog').addClass('modal-80');

        $('.modal .master-detail-component', mainClass)
            .on('master-detail:init', masterDetailInitialization)
            .on('master-detail:dialog-opened', masterDetailDialogOpened)
            .on('master-detail:row:added', masterDetailRowAdded)
            .on('master-detail:row:edited', masterDetailRowEdited)
            .on('master-detail:row:removed', masterDetailRowRemoved)
            .on('master-detail:source:loaded', masterDetailSourceLoaded)
            .on('master-detail:empty', masterDetailEmpty)
            .MasterDetail({
                modalSelector: mainClass + ' .modal.modalMasterDetail',
                readonly: false,
                disableRemove: false,
                disableAdd: true,
                disableEdit: false
            });

        var columns = [
            { data: 'idLiquidacionProfesional' },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'fechaDesde', render: DataTableEx.renders.date },
            { data: 'fechaHasta', render: DataTableEx.renders.date },
            { data: 'profesional' },
            {
                data: null,
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-${data.estadoClase}">${data.estado}</span>`;
                }
            },
            {
                data: 'total',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { data: null, orderable: false, searchable: false, class: 'text-center', width: '5%', render: renderViewDetails }
        ];

        var order = [[0, 'desc']];

        dtView = MaestroLayout.initializeDatatable('idLiquidacionProfesional', columns, order);
    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);

        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.tabChanged = tabChanged;

        $('input[name^=FechaDesde], input[name^=FechaHasta]', modalNew)
            .inputmask("99/99/9999")
            .flatpickr({
                locale: "es",
                defaultDate: "today",
                dateFormat: "d/m/Y",
                allowInput: true
            });

        $('select[name$="IdInstitucion"]').on('change', function (e) {
            reloadProfesionales(e);
        });

        $('.btnCargaItems', modalNew).on('click', function (e) {

            var $form = $(e.target).closest('form');
            var dialog = $(e.target).parents('.modal');
            var btCargaItems = $(this);
            var text = btCargaItems.text();

            var fechaDesde = $('input[name^=FechaDesde]', dialog).val();
            var fechaHasta = $('input[name^=FechaHasta]', dialog).val();
            var idProfesional = $('select[name^=IdProfesional]', dialog).val();

            if (fechaDesde == "" || fechaHasta == "") {
                Utils.alertInfo("Ingrese un rango de fechas válido.");
                return false;
            }

            if (idProfesional == "") {
                Utils.alertInfo("Ingrese el Profesional para la liquidación.");
                return false;
            }

            btCargaItems
                .data('normal-text', text)
                .prop('disabled', true)
                .html('<i class="fas fa-cog fa-spin"></i> ' + text);

            var params = {
                desde: moment(fechaDesde, 'DD/MM/YYYY').format('YYYY-MM-DD'),
                hasta: moment(fechaHasta, 'DD/MM/YYYY').format('YYYY-MM-DD'),
                idProfesional: idProfesional
            };

            Ajax.Execute('/LiquidacionesProfesionales/GetPrestaciones/', params)
                .done(function (response) {

                    text = btCargaItems.data('normal-text');
                    btCargaItems.prop('disabled', false)
                        .html(text)
                        .find('i')
                        .remove();

                    if (response.detail.length == 0) {
                        Utils.modalError("Prestaciones", 'No existen prestaciones/guardias a liquidar, Verifique.');
                        return false;
                    }

                    $form.find('.master-detail-component').MasterDetail()
                        .source(response.detail);

                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    Utils.modalError('Error', jqXHR.responseJSON.detail);

                    text = btCargaItems.data('normal-text');
                    btCargaItems.prop('disabled', false)
                        .html(text)
                        .find('i')
                        .remove();
                });
        });
    }

    function mainTableRowSelected(dataArray, api) {

        btRemove.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        btRemove.prop('disabled', !dataArray.length == 1);
    }

    function newDialogOpening(dialog, $form) {

        editMode = false;

        $('input[name^=FechaDesde]', $form).val(moment().format('DD/MM/YYYY'));
        $('input[name^=FechaHasta]', $form).val(moment().format('DD/MM/YYYY'));

        $form.find('.master-detail-component').MasterDetail()
            .clear();
    }

    function tabChanged(dialog, $form, index, name) {

    }

    function renderViewDetails(data, type, row) {

        var btnDetalle = "";

        btnDetalle = '<i class="far fa-search-plus details" title="Desplegar detalle de Prestaciones"></i>';

        return `
            <ul class="table-controls">
                <li>
                    ${btnDetalle}
                </li>
            </ul>`;
    }

    function formatRowDetail(rowData) {

        if (rowData == null || rowData.prestaciones == null || rowData.prestaciones.length == 0)
            return '<div class="text-center"><span>La liquidación no presenta Prestaciones en el Detalle</span></div>';

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Descripción</th>
                            <th>Valor</th>
                            <th>Valor Fijo</th>
                            <th>Porcentaje</th>
                            <th>Valor Porc.</th>
                            <th>Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@Rows]
                    </tbody>
                </table>
            </div>`;

        var rowTemplate =
            `<tr>
                <td>[@Descripcion]</td>
                <td>[@Valor]</td>
                <td>[@Fijo]</td>
                <td>[@Porcentaje]</td>
                <td>[@ValorPorcentaje]</td>
                <td>[@Total]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData.prestaciones) {
            var item = rowData.prestaciones[i];
            var row = rowTemplate
                .replace('[@Descripcion]', item.descripcion)
                .replace('[@Valor]', accounting.formatMoney(item.valor))
                .replace('[@Fijo]', accounting.formatMoney(item.valorFijo))
                .replace('[@Porcentaje]', accounting.formatMoney(item.porcentaje, { symbol: '%', format: '%v %s' }))
                .replace('[@ValorPorcentaje]', accounting.formatMoney(item.valorPorcentaje))
                .replace('[@Total]', accounting.formatMoney(item.total));

            rows += row;
        }

        return tableTemplate.replace('[@Rows]', rows);
    }

    function formatRowDetalle(rowData) {

        var tableId = 'dt_liquidacionDetail';
        var $template = $('.templateDetalle').clone();

        $template.removeAttr('class');
        $template.css('display', 'block');

        $template.find('.btnExcelDetalle').on('click', function (e) {
            e.preventDefault();
            exportFilename = dtView.data($('.dt-view').find('tr.shown')).profesional;
            $(tableDetalle.api().buttons()[0].node).trigger('click')
        });

        tableDetalle = $template
            .find('table')
            .attr('id', tableId)           
            .DataTableEx({
                data: rowData.prestaciones,
                buttons: [
                    {
                        filename: function () { return exportFilename; },
                        extend: "excel",
                        exportOptions: {
                            columns: ':visible'
                        }
                    }
                ],
                info: false,
                paging: false,
                searching: false,
                autoWidth: false,
                ordering: false,
                columns: [
                    { data: 'descripcion', width: '25%' },                    
                    { data: 'valor', width: '15%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency },
                    { data: 'fijo', width: '15%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency },
                    { data: 'porcentaje', width: '15%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency },
                    { data: 'valorPorcentaje', width: '15%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency },
                    { data: 'total', width: '15%', orderable: false, class: 'text-right', render: DataTableEx.renders.currency }
                ]
            });

        var div = '<div class="table-detalle">Aguarde por favor...</div>';
        var $row = $(div);

        return $row.html($template);
    }

    function retrieveRowDetails() {
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

            Ajax.Execute('/LiquidacionesProfesionales/Obtener/' + row.data().idLiquidacionProfesional, null, null, 'GET')
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            //row.child(formatRowDetail(data));
                            row.child(formatRowDetalle(data));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }
    }

    function masterDetailInitialization() {
        console.log('MasterDetail inicializado');
    }

    function masterDetailDialogOpened(e, dialog, $form) {
        console.log('MasterDetail Dialog Opened');
    }

    function masterDetailRowAdded(e, item, items, $row, $rows) {
        console.log('MasterDetail item agregado', item);
    }

    function masterDetailRowEdited(e, item, items, $row, $rows) {
        console.log('MasterDetail item editado', item);
        calculaTotalesLiquidacion(items);
    }

    function masterDetailRowRemoved(e, item, items, $row, $rows) {
        console.log('MasterDetail item eliminado', item, items);
        calculaTotalesLiquidacion(items);
    }

    function masterDetailSourceLoaded(e, items, $rows) {
        console.log('MasterDetail lista de items cargada', items, $rows);
        calculaTotalesLiquidacion(items);
    }

    function masterDetailEmpty(e) {
        console.log('MasterDetail items eliminados');
    }

    function calculaTotalesLiquidacion(items) {

       

        var subtotal = 0, redondeo = 0, total = 0;
        
        for (var i = 0; i < items.length; i++) {

            var item = items[i];
            subtotal += parseFloat(item.Total.value);
        }

        total = subtotal;

        //$('.subtotal').text(`${accounting.formatMoney(subtotal)}`);
        $('.total').text(`${accounting.formatMoney(total)}`);

    }

    function reloadProfesionales(e, element, selection) {

        $('.modal .master-detail-component').MasterDetail()
            .clear();

        var target = (e == null ? element : e.target);
        var form = $(target).closest('form');
        var idEmpresa = $(target).val();
        var instituciones = form.find('select[name$="IdProfesional"]');

        var action = 'Profesionales/GetProfesionalesByEmpresa';
        var params = {
            idEmpresa: idEmpresa,
        };

        Ajax.Execute(action, params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {

                        instituciones.find('option').remove();

                        for (var i in data) {
                            var option = data[i];

                            instituciones.append(
                                $('<option />')
                                    .val(option.idProfesional)
                                    .text(option.nombre)
                            );
                        }
                    },
                    Ajax.ShowError
                );
            })
            .fail(Ajax.ShowError);
    }
});

$(function () {
    ABMLiquidacionesProfesionalesView.init();
});
