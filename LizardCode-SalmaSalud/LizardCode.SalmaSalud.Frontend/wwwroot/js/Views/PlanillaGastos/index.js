/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMPlanillaGastosView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var editMode = false;

    var modalProcesarExcel;
    var btEdit;
    var btRemove;
    var btProcesarExcel;


    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        modalProcesarExcel = $('.modal.modalProcesarExcel', mainClass);
        btProcesarExcel = $('button.btProcesarExcel', mainClass);
        btEdit = $('.toolbar-actions button.btEdit', mainClass);
        btRemove = $('.toolbar-actions button.btRemove', mainClass);

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
            .on('repeater:source:loaded', repeaterSourceLoaded)
            .Repeater();

        MaestroLayout.disabledFields = true;
        MaestroLayout.errorTooltips = true;

        var columns = [
            { data: 'idPlanillaGastos' },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'descripcion' },
            {
                data: 'importeTotal',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-warning">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { data: 'idEstadoPlanilla', render: renderEstadoPlanilla },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'asc']];
        var pageLength = 30;

        dtView = MaestroLayout.initializeDatatable('idPlanillaGastos', columns, order, pageLength);

    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.asiento', retrieveAsientoDetails);

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.mainTableRowSelected = mainTableRowSelected;

        $('select[name^="IdMoneda"]').on('change', function (e) {

            var dialog = $(e.target).parents('.modal');

            var monedaVal = $(this).val();
            var annoMesVal = $('input[name^="AnnoMes"]', dialog).val();
            var numeroVal = $('input[name^="Numero"]', dialog).val();

            if (annoMesVal == "" || annoMesVal == undefined || annoMesVal == "__/____")
                return;
            if (annoMesVal.replaceAll("_", "").length != 7)
                return;

            if (numeroVal == "" || numeroVal == undefined || numeroVal == "___")
                return;

            if (monedaVal == "" || monedaVal == undefined)
                return;

            var params = {
                annoMes: annoMesVal,
                numero: numeroVal,
                moneda: monedaVal
            };

            getItemsGastos(params, $('select[name^="Item"]', dialog));

        });

        $('.modal', mainClass).on('blur', 'input[name$=".NoGravado"]', setTotal);
        $('.modal', mainClass).on('blur', 'input[name$=".Subtotal"]', setTotal);
        $('.modal', mainClass).on('blur', 'input[name$=".Subtotal2"]', setTotal);
        $('.modal', mainClass).on('change', 'select[name$=".IdAlicuota"]', setTotal);
        $('.modal', mainClass).on('change', 'select[name$=".IdAlicuota2"]', setTotal);
        $('.modal', mainClass).on('blur', 'input[name$=".Percepcion"]', setTotal);
        $('.modal', mainClass).on('blur', 'input[name$=".Percepcion2"]', setTotal);
        $('.modal', mainClass).on('blur', 'input[name$=".ImpuestosInternos"]', setTotal);

        $('.modal .repeater-table', mainClass).on('blur', 'input[name$=".CUIT"]', function (e) {

            var tr = $(e.target).closest('tr');
            var proveedor = tr.find('input[name$=".Proveedor"]');
            var cuit = $(this).val();

            if (cuit == "")
                return;

            var action = 'PlanillaGastos/GetProveedorByCUIT/';
            var params = {
                cuit: cuit
            };

            Ajax.Execute(action, params)
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            proveedor.val(data);
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        });

        btProcesarExcel.on('click', function () {
            $('#FileExcel').val('');
            $('#lblFileExcel').text('').hide();
            $('.btOkProcesarExcel').prop('disabled', true);
            modalProcesarExcel.modal({ backdrop: 'static', keyboard: false, modalOverflow: true });
        });

        $('#FileExcel').on('change', async function (e) {

            var fE = document.getElementById('FileExcel');
            console.log('Selected file: ' + fE.files.item(0).name);

            $('#lblFileExcel').text(fE.files.item(0).name).show();
            $('.btOkProcesarExcel').prop('disabled', false);
        });
    }

    function getItemsGastos($params, obj) {

        Ajax.Execute('/PlanillaGastos/GetItemsGastos/', $params)
            .done(function (response) {
                if (response.status == enums.AjaxStatus.OK) {
                    $(obj).Select2Ex().source(response.detail.map(function (obj) { return { id: obj.id, text: obj.text }; }));
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Utils.modalError('Error', jqXHR.responseJSON.detail);
                $(obj).select2('data', null);
            });

    }

    function mainTableRowSelected(dataArray, api) {

        btEdit.prop('disabled', true);
        btRemove.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        if (dataArray.length == 1) {
            if (dataArray[0].idEstadoPlanilla == enums.EstadoPlanillaGastos.Ingresada) {
                btEdit.prop('disabled', false);
                btRemove.prop('disabled', false);
            }
        }

    }

    function newDialogOpening(dialog, $form) {

        editMode = false;
        var idEjercicio = $form.find('#IdEjercicio option:eq(1)').val()
        $form.find('#IdEjercicio').select2('val', idEjercicio);
        $form.find('#ImporteTotal').prop('disabled', true);
        $form.find('#ImporteTotal').val(0);

        $('input[name^=Fecha]', $form).val(moment().format('DD/MM/YYYY'));
        $('select[name^="IdCuentaContable"]').closest('.field').addClass('hide');

        $form.find('.repeater-component').Repeater()
            .clear();

    }

    function editDialogOpening($form, entity) {

        editMode = true;
        $form.find('#IdPlanillaGastos_Edit').val(entity.idPlanillaGastos);
        $form.find('#IdEjercicio_Edit').select2('val', entity.idEjercicio);
        $form.find('#Fecha_Edit').val(moment(entity.fecha).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#IdTipoPlanilla_Edit').select2('val', entity.idTipoPlanilla);
        $form.find('#IdMoneda_Edit').select2('val', entity.moneda);
        $form.find('#AnnoMes_Edit').val(`${entity.anno}/${entity.mes}`);
        $form.find('#Numero_Edit').val(entity.numero);
        $form.find('#Item_Edit').select2('data', { id: entity.item, text: entity.item });
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#IdCuentaContable_Edit').select2('val', entity.idCuentaContable);
        $form.find('#ImporteTotal_Edit').val(accounting.formatMoney(entity.importeTotal));

        $('select[name^="IdCuentaContable"]', $form).closest('.field').removeClass('hide');
        $('select[name^="IdCuentaContable"].select2-field', $form).addClass('validate');

        if (entity.items.length > 0)
            $form.find('.repeater-component').Repeater()
                .source(entity.items);
        else
            $form.find('.repeater-component').Repeater()
                .clear();

    }

    function renderEstadoPlanilla(data, type, row) {
        switch (row.idEstadoPlanilla) {
            case enums.EstadoPlanillaGastos.Ingresada:
                return '<span class="badge badge-warning"> Ingresada </span>';
            case enums.EstadoPlanillaGastos.Pagada:
                return '<span class="badge badge-success"> Pagada </span>';
        }
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
            return '<div class="text-center"><span>La Planilla de Gastos no presenta Items en el Detalle</span></div>';

        var tableTemplate = 
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Item</th>
                            <th>Proveedor</th>
                            <th>Fecha</th>
                            <th>Comprobante</th>
                            <th>Número</th>
                            <th>No Gravado</th>
                            <th>Subtotal</th>
                            <th>Alícuota</th>
                            <th>Subtotal 2</th>
                            <th>Alícuota 2</th>
                            <th>Percepción</th>
                            <th>Percepción 2</th>
                            <th>Imp. Internos</th>
                            <th>Total</th>
                            <th>CAE</th>
                            <th>Vto. CAE</th>
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
                <td>[@Proveedor]</td>
                <td>[@Fecha]</td>
                <td>[@Comprobante]</td>
                <td>[@Numero]</td>
                <td>[@NoGravado]</td>
                <td>[@Subtotal]</td>
                <td>[@Alicuota]</td>
                <td>[@Subtotal2]</td>
                <td>[@Alicuota2]</td>
                <td>[@Percepcion]</td>
                <td>[@Percepcion2]</td>
                <td>[@ImpuestosInternos]</td>
                <td>[@Total]</td>
                <td>[@CAE]</td>
                <td>[@VtoCAE]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData.items) {
            var item = rowData.items[i];
            var row = rowTemplate
                .replace('[@Item]', item.item)
                .replace('[@Proveedor]', item.proveedor)
                .replace('[@Fecha]', moment(item.fecha).format(enums.FormatoFecha.DefaultFormat))
                .replace('[@Comprobante]', item.comprobante)
                .replace('[@Numero]', item.numeroComprobante)
                .replace('[@NoGravado]', accounting.formatMoney(item.noGravado))
                .replace('[@Subtotal]', accounting.formatMoney(item.subtotal))
                .replace('[@Alicuota]', accounting.formatMoney(item.alicuota, { symbol: '%', format: '%v %s' }))
                .replace('[@Subtotal2]', accounting.formatMoney(item.subtotal2))
                .replace('[@Alicuota2]', accounting.formatMoney(item.alicuota2, { symbol: '%', format: '%v %s' }))
                .replace('[@Percepcion]', accounting.formatMoney(item.percepcion))
                .replace('[@Percepcion2]', accounting.formatMoney(item.percepcion2))
                .replace('[@ImpuestosInternos]', accounting.formatMoney(item.impuestosInternos))
                .replace('[@Total]', accounting.formatMoney(item.total))
                .replace('[@CAE]', item.cae || "")
                .replace('[@VtoCAE]', !item.vencimientoCAE ? "" : moment(item.vencimientoCAE).format(enums.FormatoFecha.DefaultFormat));

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

            Ajax.Execute('/PlanillaGastos/Obtener/' + row.data().idPlanillaGastos, null, null, 'GET')
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

            Ajax.Execute('/PlanillaGastos/ObtenerAsiento/' + row.data().idPlanillaGastos, null, null, 'GET')
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

    function rebuildItemNumbers($rows) {

        for (var i = 0; i < $rows.length; i++) {

            var $row = $rows.eq(i);
            var $item = $row.find('> td input.repeater-control[name$=".Item"]');

            $item.val(i + 1);

        }
    }

    function repeaterInitialization() { }

    function repeaterRowAdded(e, $newRow, $rows) {

        rebuildItemNumbers($rows);
        ConstraintsMaskEx.init();

        $('input[name$=".NumeroComprobante"]', $newRow).alphanum({
            allowNumeric: true,
            allow: '-',
            allowSpace: false,
            allowUpper: false,
            allowLower: false
        })
        .on('blur', function (e) {

            var numero = $(this).val().replaceAll('_', '');

            if (numero == "")
                return;

            if (numero.indexOf('-') == -1) {
                $(this).val('');
                return;
            }

            var [suc, nro] = numero.split('-');

            suc = `${String("00000" + suc).slice(-5)}`
            nro = `${String("00000000" + nro).slice(-8)}`

            $(this).val(`${suc}-${nro}`);
        });

    }

    function repeaterRowRemoved(e, $removedRow, $rows) {

        rebuildItemNumbers($rows);
        setTotal({ target: $('tbody', e.target) });

    }

    function repeaterFieldFocus(e, data) { }

    function repeaterFieldChange(e, data) { }

    function repeaterSourceLoaded(e) {

        setTotalPlanilla(e);

    }

    function setTotal(e) {

        var tr = $(e.target).closest('tr');
        var total = tr.find('input[name$=".Total"]');

        total.val(0);

        var importeTotal = 0;
        var noGravado = AutoNumeric.isManagedByAutoNumeric(tr.find('input[name$=".NoGravado"]')[0]) ? AutoNumeric.getNumber(tr.find('input[name$=".NoGravado"]')[0]) : tr.find('input[name$=".NoGravado"]').val();
        var subtotal = AutoNumeric.isManagedByAutoNumeric(tr.find('input[name$=".Subtotal"]')[0]) ? AutoNumeric.getNumber(tr.find('input[name$=".Subtotal"]')[0]) : tr.find('input[name$=".Subtotal"]').val();
        var subtotal2 = AutoNumeric.isManagedByAutoNumeric(tr.find('input[name$=".Subtotal2"]')[0]) ? AutoNumeric.getNumber(tr.find('input[name$=".Subtotal2"]')[0]) : tr.find('input[name$=".Subtotal2"]').val();
        var alicuota = tr.find('select[name$=".IdAlicuota"] option:selected').text();
        var alicuota2 = tr.find('select[name$=".IdAlicuota2"] option:selected').text();
        var percepcion = AutoNumeric.isManagedByAutoNumeric(tr.find('input[name$=".Percepcion"]')[0]) ? AutoNumeric.getNumber(tr.find('input[name$=".Percepcion"]')[0]) : tr.find('input[name$=".Percepcion"]').val();
        var percepcion2 = AutoNumeric.isManagedByAutoNumeric(tr.find('input[name$=".Percepcion2"]')[0]) ? AutoNumeric.getNumber(tr.find('input[name$=".Percepcion2"]')[0]) : tr.find('input[name$=".Percepcion2"]').val();
        var impuestosInternos = AutoNumeric.isManagedByAutoNumeric(tr.find('input[name$=".ImpuestosInternos"]')[0]) ? AutoNumeric.getNumber(tr.find('input[name$=".ImpuestosInternos"]')[0]) : tr.find('input[name$=".ImpuestosInternos"]').val();
        
        if (!isNaN(parseFloat(subtotal)) && !isNaN(parseFloat(alicuota))) {
            var importe = parseFloat(subtotal) * (1 + (parseFloat(alicuota) / 100));
            var importe2 = parseFloat(subtotal2) * (1 + (parseFloat(alicuota2) / 100)) | 0;
            var percepcion = percepcion | 0;
            var percepcion2 = percepcion2 | 0;

            importeTotal = noGravado + importe + importe2 + percepcion + percepcion2 + impuestosInternos;

            if (AutoNumeric.isManagedByAutoNumeric(total.get(0)))
                AutoNumeric.set(total.get(0), importeTotal);
            else
                total.val(accounting.formatMoney(importeTotal));
        }
        
        setTotalPlanilla({ target: e.target });
    }

    function setTotalPlanilla(e) {

        var dialog = $(e.target).closest('.modal');
        var tbody = $(e.target).closest('tbody');
        if (tbody.length == 0)
            tbody = $(e.target).find('tbody');
        var totalPlanilla = 0;
        var trs = tbody.find('> tr');

        trs.each(function () {
            var tr = $(this);
            var total = tr.find('> td > input[name$=".Total"]');
            var totalf = parseFloat(accounting.unformat(total.val(), ",") | 0);

            totalPlanilla += totalf;
        });

        dialog.find('input[name^="ImporteTotal"]').val(accounting.formatMoney(totalPlanilla));
    }

    this.ajaxProcesarExcelBegin = function () {
        Utils.modalLoader('Procesando...');
    }

    this.ajaxProcesarExcelSuccess = function (response) {
        Utils.modalClose();

        modalProcesarExcel.modal('hide');

        if (response.status == 'OK') {
            if (response.detail) {

                if (editMode) {
                    $('.modalEdit .repeater-component').Repeater()
                        .clear();

                    $('.modalEdit .repeater-component').Repeater()
                        .source(response.detail);
                } else {
                    $('.modalNew .repeater-component').Repeater()
                        .clear();

                    $('.modalNew .repeater-component').Repeater()
                        .source(response.detail);
                }
            }
            else {
                Utils.alertError('No se encontraron registros.');
            }
        } else {
            Utils.alertError(response.detail);
        }
    }

    this.ajaxProcesarExcelFailure = function (jqXHR, textStatus, errorThrown) {
        Utils.modalClose();

        Ajax.ShowError(jqXHR, textStatus, errorThrown);
    }
});

$(function () {
    ABMPlanillaGastosView.init();
});
