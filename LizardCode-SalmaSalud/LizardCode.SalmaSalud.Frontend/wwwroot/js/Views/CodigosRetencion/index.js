/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMCodigosRetencionView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');


    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }

    function buildControls() {

        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');

        $('.modal .dvGanancias .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
            .on('repeater:row:added', repeaterRowAdded)
            .on('repeater:row:removed', repeaterRowRemoved)
            .on('repeater:field:focus', repeaterFieldFocus)
            .on('repeater:field:change', repeaterFieldChange)
            .Repeater();

        MaestroLayout.errorTooltips = true;

        var columns = [
            { data: 'idCodigoRetencion' },
            { data: 'descripcion' },
            { data: 'regimen' },
            { data: 'idTipoRetencion', visible: false },
            { data: 'tipoRetencion', render: renderTipo },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idCodigoRetencion', columns, order);

        $('.nav-item').not(":nth-child(1)").hide();

    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > i.details', retrieveRowDetails);

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.tabChanged = tabChanged;

        $('.modal select[name="IdTipoRetencion"]', mainClass)
            .on('change', setTipoRetencion);

    }

    function setTipoRetencion(e) {

        var $form = $(e.target).closest('form');
        var val = parseInt($(e.target).select2('val'));

        $('.nav-item').not(":nth-child(1)").hide();

        switch (val) {
            case enums.TiposRetencion.Ganancias:
                $('.nav-item', $form).filter(':nth-child(2)').show()
                break;
            case enums.TiposRetencion.IVA:
                $('.nav-item', $form).filter(':nth-child(3)').show()
                break;
            case enums.TiposRetencion.IngresosBrutos:
                $('.nav-item', $form).filter(':nth-child(4)').show()
                break;
            case enums.TiposRetencion.SUSS:
                $('.nav-item', $form).filter(':nth-child(5)').show()
                break;
            case enums.TiposRetencion.IVAMonotributo:
                $('.nav-item', $form).filter(':nth-child(6)').show()
                break;
            case enums.TiposRetencion.GananciasMonotributo:
                $('.nav-item', $form).filter(':nth-child(7)').show()
                break;
        }
    }

    function tabChanged(dialog, $form, index, name) {

        dialog
            .removeClass('modal-95')
            .removeClass('modal-40');

        switch (index) {
            case 0:
                dialog.addClass('modal-40');
                break;
            case 1:
                dialog.addClass('modal-80');
                break;
            default:
                dialog.addClass('modal-60');
                break;
        }

    }

    function newDialogOpening(dialog, $form) {

        $('.nav-item').not(":nth-child(1)").hide();
        $form.find('.dvGanancias .repeater-component').Repeater()
            .clear();

    }

    function editDialogOpening($form, entity) {

        $('.nav-item').not(":nth-child(1)").hide();
        $form.find('.dvGanancias .repeater-component').Repeater()
            .clear();

        $form.find('#IdCodigoRetencion_Edit').val(entity.idCodigoRetencion);
        $form.find('#IdTipoRetencion_Edit').select2('val', entity.idTipoRetencion);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#Regimen_Edit').val(entity.regimen);

        switch (entity.idTipoRetencion) {
            case enums.TiposRetencion.Ganancias:
                $('.nav-item', $form).filter(':nth-child(2)').show()
                break;
            case enums.TiposRetencion.IVA:
                $('.nav-item', $form).filter(':nth-child(3)').show()
                break;
            case enums.TiposRetencion.IngresosBrutos:
                $('.nav-item', $form).filter(':nth-child(4)').show()
                break;
            case enums.TiposRetencion.SUSS:
                $('.nav-item', $form).filter(':nth-child(5)').show()
                break;
            case enums.TiposRetencion.IVAMonotributo:
                $('.nav-item', $form).filter(':nth-child(6)').show()
                break;
            case enums.TiposRetencion.GananciasMonotributo:
                $('.nav-item', $form).filter(':nth-child(7)').show()
                break;
        }
        
        switch (entity.idTipoRetencion) {
            case enums.TiposRetencion.Ganancias:
                AutoNumeric.set($form.find('#ImporteNoSujetoGanancias_Edit')[0], entity.importeNoSujeto);
                AutoNumeric.set($form.find('#ImporteMinimoRetencionGanancias_Edit')[0], entity.importeMinimoRetencion);
                $form.find('#AcumulaPagos_Edit').prop('checked', entity.acumulaPagos);
                if (entity.items != null && entity.items.length > 0)
                    $form.find('.dvGanancias .repeater-component').Repeater().source(entity.items);
                else
                    $form.find('.dvGanancias .repeater-component').Repeater();
                break;
            case enums.TiposRetencion.GananciasMonotributo:
                AutoNumeric.set($form.find('#ImporteNoSujetoGanMonotributo_Edit')[0], entity.importeNoSujeto);
                AutoNumeric.set($form.find('#PorcentajeRetencionGanMonotributo_Edit')[0], entity.porcentajeRetencion);
                AutoNumeric.set($form.find('#CantidadMesesGanMonotributo_Edit')[0], entity.cantidadMeses);
                break;
            case enums.TiposRetencion.IngresosBrutos:
                AutoNumeric.set($form.find('#ImporteNoSujetoIngBrutos_Edit')[0], entity.importeNoSujeto);
                AutoNumeric.set($form.find('#PorcentajeRetencionIngBrutos_Edit')[0], entity.porcentajeRetencion);
                $form.find('#PadronRetencionAGIP_Edit').prop('checked', entity.padronRetencionAGIP);
                $form.find('#PadronRetencionARBA_Edit').prop('checked', entity.padronRetencionARBA);

                break;
            case enums.TiposRetencion.IVA:
                AutoNumeric.set($form.find('#ImporteNoSujetoIVA_Edit')[0], entity.importeNoSujeto);
                AutoNumeric.set($form.find('#PorcentajeRetencionIVA_Edit')[0], entity.porcentajeRetencion);
                break;
            case enums.TiposRetencion.IVAMonotributo:
                AutoNumeric.set($form.find('#ImporteNoSujetoIVAMonotributo_Edit')[0], entity.importeNoSujeto);
                AutoNumeric.set($form.find('#PorcentajeRetencionIVAMonotributo_Edit')[0], entity.porcentajeRetencion);
                AutoNumeric.set($form.find('#CantidadMesesIVAMonotributo_Edit')[0], entity.cantidadMeses);
                break;
            case enums.TiposRetencion.SUSS:
                AutoNumeric.set($form.find('#ImporteNoSujetoSUSS_Edit')[0], entity.importeNoSujeto);
                AutoNumeric.set($form.find('#PorcentajeRetencionSUSS_Edit')[0], entity.porcentajeRetencion);
                break;
        }
        
    }

    function renderTipo(data, type, row) {
        if (data == "")
            return "";
        return '<span class="badge badge-success"> ' + data + ' </span>';

    }

    function renderViewDetails(data, type, row) {
        if (row.idTipoRetencion != enums.TiposRetencion.Ganancias)
            return "";
        return '<i class="details far fa-search-plus" title="Desplegar detalle"></i>';
    }

    function formatRowDetailGanancias(rowData) {

        if (rowData == null || rowData.items == null || rowData.items.length == 0)
            return '<div class="text-center"><span>El Códigos de Retención no presenta Items en el Detalle</span></div>';

        var tableTemplate = 
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Item</th>
                            <th>Importe Desde</th>
                            <th>Importe Hasta</th>
                            <th>Importe Retención</th>
                            <th>Mas %</th>
                            <th>Sobre Excedente</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@Rows]
                    </tbody>
                </table>
            </div>`;

        var rowTemplate =
            `<tr>
                <td>[@Item]</td>
                <td>[@ImporteDesde]</td>
                <td>[@ImporteHasta]</td>
                <td>[@ImporteRetencion]</td>
                <td>[@MasPorcentaje]</td>
                <td>[@SobreExcedente]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData.items) {
            var item = rowData.items[i];
            var row = rowTemplate
                .replace('[@Item]', item.item)
                .replace('[@ImporteDesde]', accounting.formatMoney(item.importeDesde))
                .replace('[@ImporteHasta]', accounting.formatMoney(item.importeHasta))
                .replace('[@ImporteRetencion]', accounting.formatMoney(item.importeRetencion))
                .replace('[@MasPorcentaje]', accounting.formatMoney(item.masPorcentaje, { symbol: '%', format: '%v %s' }))
                .replace('[@SobreExcedente]', accounting.formatMoney(item.sobreExcedente));

            rows += row;
        }

        return tableTemplate.replace('[@Rows]', rows);
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

            Ajax.Execute('/CodigosRetencion/Obtener/' + row.data().idCodigoRetencion, null, null, 'GET')
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowDetailGanancias(data));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }
    }

    function rebuildItemNumbers($rows) {

        for (var i = 0; i < $rows.length; i++) {

            var $row = $rows.eq(i);
            var $item = $row.find('> td input.repeater-control[name$=".Item"]');

            $item.val(i + 1);

        }

    }

    function repeaterInitialization() {
        console.log('repeater iniciado');
    }

    function repeaterRowAdded(e, $newRow, $rows) {

        rebuildItemNumbers($rows);

    }

    function repeaterRowRemoved(e, $removedRow, $rows) {

        rebuildItemNumbers($rows);

    }

    function repeaterFieldFocus(e, data) {

        console.log(data);

    }

    function repeaterFieldChange(e, data) {

        console.log(data);

    }

});

$(function () {
    ABMCodigosRetencionView.init();
});
