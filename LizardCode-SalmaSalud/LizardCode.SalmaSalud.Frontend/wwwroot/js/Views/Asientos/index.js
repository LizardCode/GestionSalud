/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMAsientosView = new (function () {

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
            .on('repeater:field:blur', repeaterFieldBlur)
            .Repeater();

        MaestroLayout.errorTooltips = true;

        var columns = [
            { data: 'idAsiento' },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'descripcion' },
            { data: 'tipoAsiento', render: renderTipo },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'asc']];
        var pageLength = 30;

        dtView = MaestroLayout.initializeDatatable('idAsiento', columns, order, pageLength);

    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > i.details', retrieveRowDetails);

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;

        $('input[name=Fecha]', modalNew)
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

        $('select[name^=IdTipoAsiento]', modalNew).on('change', function (e) {

            var $form = $(e.target).closest('form');

            var idTipoAsiento = $(this).select2('val');
            var params = {
                idTipoAsiento: idTipoAsiento
            };

            Ajax.GetJson(RootPath + 'Asientos/GetCuentasByTipoAsiento', params)
                .done(function (cuentas) {

                    if (cuentas.length > 0)
                        $form.find('.repeater-component').Repeater()
                            .source(cuentas);
                })
                .fail(Ajax.ShowError);

        });
    }

    function newDialogOpening(dialog, $form) {

        var idEjercicio = $form.find('#IdEjercicio option:eq(1)').val()
        $form.find('#IdEjercicio').select2('val', idEjercicio);
        $form.find('.repeater-component').Repeater().clear();

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdAsiento_Edit').val(entity.idAsiento);
        $form.find('#Fecha_Edit').val(moment(entity.fecha).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#IdEjercicio_Edit').select2('val', entity.idEjercicio);
        $form.find('#IdTipoAsiento_Edit').select2('val', entity.idTipoAsiento);
        $form.find('#Descripcion_Edit').val(entity.descripcion);

        if (entity.items.length > 0)
            $form.find('.repeater-component').Repeater()
                .source(entity.items);
        else
            $form.find('.repeater-component').Repeater()
                .clear();

    }

    function renderTipo(data, type, row) {
        if (data == "")
            return "";
        return '<span class="badge badge-success"> ' + data + ' </span>';

    }

    function renderViewDetails() {
        return '<i class="details far fa-search-plus" title="Desplegar detalle"></i>';
    }

    function formatRowDetail(rowData) {

        if (rowData == null || rowData.items == null || rowData.items.length == 0)
            return '<div class="text-center"><span>El Asiento Manual no presenta Items en el Detalle</span></div>';

        var tableTemplate = 
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Item</th>
                            <th>Fecha</th>
                            <th>Código</th>
                            <th>Cuenta</th>
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
                <td>[@Codigo]</td>
                <td>[@Cuenta]</td>
                <td>[@Detalle]</td>
                <td>[@Debitos]</td>
                <td>[@Creditos]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData.items) {
            var item = rowData.items[i];
            var row = rowTemplate
                .replace('[@Item]', item.item)
                .replace('[@Fecha]', moment(item.fecha, enums.FormatoFecha.DatabaseFullFormat).format(enums.FormatoFecha.DefaultFormat))
                .replace('[@Codigo]', item.codigo)
                .replace('[@Cuenta]', item.cuenta)
                .replace('[@Detalle]', item.detalle)
                .replace('[@Debitos]', accounting.formatMoney(item.debitos))
                .replace('[@Creditos]', accounting.formatMoney(item.creditos));

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

            Ajax.Execute('/Asientos/Obtener/' + row.data().idAsiento, null, null, 'GET')
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
        calculaTotalesAsiento(e);

    }

    function repeaterFieldFocus(e, data) {

    }

    function repeaterFieldChange(e, data) {

    }

    function repeaterFieldBlur(e, data) {

        calculaTotalesAsiento(e);
    }

    function calculaTotalesAsiento(e) {

        var $form = $(e.target).closest('form');
        var trs = $(e.target).find('tbody > tr');
        var totalDebitos = 0;
        var totalCreditos = 0;

        trs.each(function () {

            var tr = $(this);

            var debito = tr.find('> td > input[name$=".Debitos"]');
            var credito = tr.find('> td > input[name$=".Creditos"]');

            var importeDeb = AutoNumeric.isManagedByAutoNumeric(debito.get(0)) ? AutoNumeric.getNumber(debito.get(0)) : $(debito).val();
            var importeCre = AutoNumeric.isManagedByAutoNumeric(credito.get(0)) ? AutoNumeric.getNumber(credito.get(0)) : $(credito).val();

            totalDebitos += importeDeb;
            totalCreditos += importeCre;
        });

        $form.find('.TotalDebitos').text(accounting.formatMoney(totalDebitos));
        $form.find('.TotalCreditos').text(accounting.formatMoney(totalCreditos));
    }

});

$(function () {
    ABMAsientosView.init();
});
