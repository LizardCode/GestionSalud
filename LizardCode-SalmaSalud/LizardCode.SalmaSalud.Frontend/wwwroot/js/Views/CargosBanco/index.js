/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMCargosBancoView = new (function () {

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

        $('.modal .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
            .on('repeater:row:added', repeaterRowAdded)
            .on('repeater:row:removed', repeaterRowRemoved)
            .on('repeater:field:focus', repeaterFieldFocus)
            .on('repeater:field:change', repeaterFieldChange)
            .Repeater();

        MaestroLayout.errorTooltips = true;

        var columns = [
            { data: 'idCargoBanco' },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'fechaReal', render: DataTableEx.renders.date },
            { data: 'descripcion' },
            { data: 'banco' },
            { data: 'total', render: DataTableEx.renders.currency },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'asc']];
        var pageLength = 30;

        dtView = MaestroLayout.initializeDatatable('idCargoBanco', columns, order, pageLength);

    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.asiento', retrieveAsientoDetails);

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;

        $('.modal', mainClass).on('blur', 'input[name$=".Importe"]', setTotal);
        $('.modal', mainClass).on('change', 'select[name$=".IdAlicuota"]', setTotal);

        $('input[name^=Fecha], input[name^=fechaReal]')
            .inputmask("99/99/9999")
            .flatpickr({
                locale: "es",
                allowInput: true,
                defaultDate: "today",
                dateFormat: "d/m/Y",
                onClose: function (selectedDates, dateStr, instance) {
                    if (dateStr == "")
                        instance.setDate(moment().format(enums.FormatoFecha.DefaultFormat));
                    else
                        instance.setDate(dateStr);
                }
            });
    }

    function newDialogOpening(dialog, $form) {

        var idEjercicio = $form.find('#IdEjercicio option:eq(1)').val()
        $form.find('#IdEjercicio').select2('val', idEjercicio);

        $form.find('.repeater-component').Repeater()
            .clear();

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdCargoBanco_Edit').val(entity.idCargoBanco);
        $form.find('#Fecha_Edit').val(moment(entity.fecha).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#FechaReal_Edit').val(moment(entity.fechaReal).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#IdEjercicio_Edit').select2('val', entity.idEjercicio);
        $form.find('#IdBanco_Edit').select2('val', entity.idBanco);
        $form.find('#Descripcion_Edit').val(entity.descripcion);

        if (entity.items.length > 0)
            $form.find('.repeater-component').Repeater()
                .source(entity.items);
        else
            $form.find('.repeater-component').Repeater()
                .clear();

    }

    function renderViewDetails() {

        var btnDetalle = "", btnAsiento = "";

        btnDetalle = '<i class="far fa-search-plus details" title="Desplegar detalle de Items"></i>';
        btnAsiento = '<i class="far fa-file-invoice asiento" title="Asiento Contable"></i>';

        return `
            <ul class="table-controls">
                <li>
                    ${btnDetalle}
                </li>
                <li>
                    ${btnAsiento}
                </li>
            </ul>`;
    }

    function formatRowDetail(rowData) {

        if (rowData == null || rowData.items == null || rowData.items.length == 0)
            return '<div class="text-center"><span>El Cargo Bancario no presenta Items en el Detalle</span></div>';

        var tableTemplate = 
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Item</th>
                            <th>Código</th>
                            <th>Cuenta</th>
                            <th>Detalle</th>
                            <th>Importe</th>
                            <th>Alícuota</th>
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
                <td>[@Item]</td>
                <td>[@Codigo]</td>
                <td>[@Cuenta]</td>
                <td>[@Detalle]</td>
                <td>[@Importe]</td>
                <td>[@Alicuota]</td>
                <td>[@Total]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData.items) {
            var item = rowData.items[i];
            var row = rowTemplate
                .replace('[@Item]', item.item)
                .replace('[@Codigo]', item.codigo)
                .replace('[@Cuenta]', item.cuenta)
                .replace('[@Detalle]', item.detalle)
                .replace('[@Importe]', accounting.formatMoney(item.importe))
                .replace('[@Alicuota]', accounting.formatMoney(item.alicuota, { symbol: '%', format: '%v %s' }))
                .replace('[@Total]', accounting.formatMoney(item.total));

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

            Ajax.Execute('/CargosBanco/Obtener/' + row.data().idCargoBanco, null, null, 'GET')
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowDetail(data));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }
    }

    function formatRowAsientoDetail(rowData) {

        if (rowData == null || rowData.length == 0)
            return '<div class="text-center"><span>El Comprobante no presenta un Asiento Contable</span></div>';

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Item</th>
                            <th>Fecha</th>
                            <th>Cuenta Contable</th>
                            <th>Detalle</th>
                            <th>Débitos</th>
                            <th>Créditos</th>
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
                <td>[@Fecha]</td>
                <td>[@IdCuentaComtable]</td>
                <td>[@Detalle]</td>
                <td>[@Debitos]</td>
                <td>[@Creditos]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData) {
            var item = rowData[i];
            var row = rowTemplate
                .replace('[@Item]', item.item)
                .replace('[@Fecha]', moment(item.fecha, enums.FormatoFecha.DatabaseFullFormat).format(enums.FormatoFecha.DefaultFormat))
                .replace('[@IdCuentaComtable]', item.codigo)
                .replace('[@Detalle]', item.detalle)
                .replace('[@Debitos]', item.debitos == 0 ? "" : accounting.formatMoney(item.debitos))
                .replace('[@Creditos]', item.creditos == 0 ? "" : accounting.formatMoney(item.creditos));

            rows += row;
        }

        return tableTemplate.replace('[@Rows]', rows);
    }

    function retrieveAsientoDetails() {
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

            Ajax.Execute('/CargosBanco/ObtenerAsiento/' + row.data().idCargoBanco, null, null, 'GET')
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowAsientoDetail(data));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }
    }

    function setTotal(e) {

        var tr = $(e.target).closest('tr');
        var total = tr.find('input[name$=".Total"]');

        total.val(0);

        var subtotal = AutoNumeric.isManagedByAutoNumeric(tr.find('input[name$=".Importe"]')[0]) ? AutoNumeric.getNumber(tr.find('input[name$=".Importe"]')[0]) : tr.find('input[name$=".Importe"]').val();
        var alicuota = tr.find('select[name$=".IdAlicuota"]').val();

        if (!isNaN(parseFloat(subtotal)) && !isNaN(parseFloat(alicuota))) {
            var importe = parseFloat(subtotal) * (1 + (parseFloat(alicuota) / 100));
            if (AutoNumeric.isManagedByAutoNumeric(total.get(0)))
                AutoNumeric.set(total.get(0), importe);
            else
                total.val(accounting.formatMoney(importe));
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
        ConstraintsMaskEx.init();

    }

    function repeaterRowRemoved(e, $removedRow, $rows) {

        rebuildItemNumbers($rows);
        setTotal({ target: $('tbody', e.target) });

    }

    function repeaterFieldFocus(e, data) {

        console.log(data);

    }

    function repeaterFieldChange(e, data) {

        console.log(data);

    }

});

$(function () {
    ABMCargosBancoView.init();
});
