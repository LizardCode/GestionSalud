/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/masterDetail.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMSaldoInicioBancoView = new (function () {

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

        $('.modal .dvItemsAnticiposClientes .master-detail-component', mainClass)
            .on('master-detail:init', masterDetailInitializationItem)
            .on('master-detail:row:added', masterDetailRowAddedItem)
            .on('master-detail:row:edited', masterDetailRowEditedItem)
            .on('master-detail:row:removed', masterDetailRowRemovedItem)
            .on('master-detail:source:loaded', masterDetailSourceLoadedItem)
            .on('master-detail:empty', masterDetailEmptyItem)
            .on('master-detail:row:adding', masterDetailRowAddingItem)
            .MasterDetail({
                modalSelector: mainClass + ' .modal.modalItemsAnticiposClientes',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });

        $('.modal .dvItemsAnticiposProveedores .master-detail-component', mainClass)
            .on('master-detail:init', masterDetailInitializationItem)
            .on('master-detail:row:added', masterDetailRowAddedItem)
            .on('master-detail:row:edited', masterDetailRowEditedItem)
            .on('master-detail:row:removed', masterDetailRowRemovedItem)
            .on('master-detail:source:loaded', masterDetailSourceLoadedItem)
            .on('master-detail:empty', masterDetailEmptyItem)
            .on('master-detail:row:adding', masterDetailRowAddingItem)
            .MasterDetail({
                modalSelector: mainClass + ' .modal.modalItemsAnticiposProveedores',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });

        $('.modal .dvItemsCheques .master-detail-component', mainClass)
            .on('master-detail:init', masterDetailInitializationItem)
            .on('master-detail:dialog-add-opening', masterDetailDialogOpeningItem)
            .on('master-detail:dialog-edit-opening', masterDetailDialogOpeningItem)
            .on('master-detail:row:added', masterDetailRowAddedItem)
            .on('master-detail:row:edited', masterDetailRowEditedItem)
            .on('master-detail:row:removed', masterDetailRowRemovedItem)
            .on('master-detail:source:loaded', masterDetailSourceLoadedItem)
            .on('master-detail:empty', masterDetailEmptyItem)
            .on('master-detail:row:adding', masterDetailRowAddingItem)
            .MasterDetail({
                modalSelector: mainClass + ' .modal.modalItemsCheques',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });


        MaestroLayout.errorTooltips = true;
        MaestroLayout.disabledFields = true;

        var columns = [
            { data: 'idSaldoInicioBanco' },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'descripcion' },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'desc']];

        dtView = MaestroLayout.initializeDatatable('idSaldoInicioBanco', columns, order);

    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);
        
        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;
        
        $('select[name="Cheque.IdBancoChequeComun"], select[name="Cheque.IdBancoChequeDiferido"], select[name="Cheque.IdBancoEChequeDiferido"], select[name="Cheque.IdBancoEChequeComun"]')
                .select2({ allowClear: true });

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

        $('select[name^="Cheque.IdTipoCheque"]').on('change', function (e) {

            var idTipoCheque = $(this).val();

            $('.dvTipoSdoInicio').removeClass('show').addClass('hide');
            $('.modalItemsCheques .select2-field.validate').removeClass('validate');

            $('input[name="Cheque.Descripcion"]').val($('#Cheque_IdTipoCheque :selected').text());
            $('input[name="Cheque.Importe"]').prop('readonly', false);

            switch (parseInt(idTipoCheque)) {
                case enums.TipoSdoInicioBanco.ChequeComun:
                    $('.dvChequeComun').addClass('show');
                    $('.modalItemsCheques .dvChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoSdoInicioBanco.EChequeComun:
                    $('.dvEChequeComun').addClass('show');
                    $('.modalItemsCheques .dvEChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoSdoInicioBanco.ChequeDiferido:
                    $('.dvChequeDiferido').addClass('show');
                    $('.modalItemsCheques .dvChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoSdoInicioBanco.EChequeDiferido:
                    $('.dvEChequeDiferido').addClass('show');
                    $('.modalItemsCheques .dvEChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoSdoInicioBanco.ChequeTerceros:
                    $('.dvChequeTerceros').addClass('show');
                    $('.modalItemsCheques .dvChequeTerceros .select2-field').addClass('validate');
                    break;
            }

            $('.dvImporte').addClass('show');

        });

        // #region Eventos Detalle de Items

        $('input[name^="Cheque.NumeroCheque"]').on('blur', function (e) {
            var nroCheque = $(this).val().replaceAll("_", "");
            $(this).val(String("0000000000" + nroCheque).slice(-10));
            $('input[name="Cheque.Descripcion"]').val(`${$('#Cheque_IdTipoCheque :selected').text()} ${$(this).val()}`);
        });

        $('input[name="Cheque.NumeroChequeComun"]').on('blur', function (e) {
            var nroCheque = $(this).val();
            if (nroCheque == "")
                return;
            $(this).val(String("0000000000" + nroCheque).slice(-10));
        });
        $('input[name="Cheque.NumeroChequeDiferido"]').on('blur', function (e) {
            var nroCheque = $(this).val();
            if (nroCheque == "")
                return;
            $(this).val(String("0000000000" + nroCheque).slice(-10));
        });
        $('input[name="Cheque.NumeroEChequeComun"]').on('blur', function (e) {
            var nroCheque = $(this).val();
            if (nroCheque == "")
                return;
            $(this).val(String("0000000000" + nroCheque).slice(-10));
        });
        $('input[name="Cheque.NumeroEChequeDiferido"]').on('blur', function (e) {
            var nroCheque = $(this).val();
            if (nroCheque == "")
                return;
            $(this).val(String("0000000000" + nroCheque).slice(-10));
        });

        $('input[name="Cheque.NumeroChequeTerceros"]').on('blur', function (e) {
            var nroCheque = $(this).val();
            if (nroCheque == "")
                return;
            $(this).val(String("0000000000" + nroCheque).slice(-10));
        });

        // #endregion 

    }

    function newDialogOpening(dialog, $form) {

        $('input[name^=Fecha]', $form).val(moment().format('DD/MM/YYYY'));

        $('.modal .dvItemsCheques .master-detail-component', mainClass).MasterDetail()
            .clear()

        $('.modal .dvItemsAnticiposClientes .master-detail-component', mainClass).MasterDetail()
            .clear()

        $('.modal .dvItemsAnticiposProveedores .master-detail-component', mainClass).MasterDetail()
            .clear()
    }

    function editDialogOpening($form, entity) {

        $form.find('#IdSaldoInicioBanco_Edit').val(entity.idSaldoInicioBanco);
        $form.find('#Fecha_Edit').val(moment(entity.fecha).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#Descripcion_Edit').val(entity.descripcion);

        $form.find('.dvItemsCheques .master-detail-component', mainClass).MasterDetail()
            .source(entity.cheques);

        $form.find('.dvItemsAnticiposClientes .master-detail-component', mainClass).MasterDetail()
            .source(entity.anticiposClientes);

        $form.find('.dvItemsAnticiposProveedores .master-detail-component', mainClass).MasterDetail()
            .source(entity.anticiposProveedores);

    }

    function renderViewDetails(data, type, row) {

        var btnDetalle = '<i class="far fa-search-plus details" title="Desplegar Detalle de Items"></i>';

        return `
            <ul class="table-controls">
                <li>
                    ${btnDetalle}
                </li>
            </ul>`;
    }

    // #region Detalle de Items, Pagos y Asiento Contable

    function formatRowDetail(rowData) {

        if (rowData == null)
            return '<div class="text-center"><span>El Saldo Inicio no presenta Items en el Detalle</span></div>';

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Banco</th>
                            <th>Tipo de Cheque</th>
                            <th>Nro. Cheque</th>
                            <th>Fecha</th>
                            <th>Fecha Diferido</th>
                            <th>Importe</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@RowsCheques]
                    </tbody>
                </table>

                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Fecha</th>
                            <th>Cliente</th>
                            <th>Descripción</th>
                            <th>Moneda</th>
                            <th>Importe</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@RowsAnticiposClientes]
                    </tbody>
                </table>

                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Fecha</th>
                            <th>Proveedor</th>
                            <th>Descripción</th>
                            <th>Moneda</th>
                            <th>Importe</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@RowsAnticiposProveedores]
                    </tbody>
                </table>
            </div>`;

        var rowTemplateCheques =
            `<tr>
                <td>[@Banco]</td>
                <td>[@TipoCheque]</td>
                <td>[@NumeroCheque]</td>
                <td>[@Fecha]</td>
                <td>[@FechaDiferido]</td>
                <td>[@Importe]</td>
            </tr>`;

        var rowTemplateAnticiposClientes =
            `<tr>
                <td>[@Fecha]</td>
                <td>[@Cliente]</td>
                <td>[@Descripcion]</td>
                <td>[@Moneda]</td>
                <td>[@Importe]</td>
            </tr>`;

        var rowTemplateAnticiposProveedores =
            `<tr>
                <td>[@Fecha]</td>
                <td>[@Proveedor]</td>
                <td>[@Descripcion]</td>
                <td>[@Moneda]</td>
                <td>[@Importe]</td>
            </tr>`;

        var rowsCheques = '';
        var rowsAnticiposClientes = '';
        var rowsAnticiposProveedores = '';

        for (var i in rowData.cheques) {
            var cheque = rowData.cheques[i];

            var fecha = "";
            var fechaDiferido = "";
            
            switch (cheque.idTipoCheque) {
                case enums.TipoCheque.Comun:
                case enums.TipoCheque.EChequeComun:
                    fecha = cheque.fechaChequeComun || cheque.fechaEChequeComun;
                    break;
                case enums.TipoCheque.Diferido:
                case enums.TipoCheque.EChequeDiferido:
                    fecha = cheque.fechaChequeDiferido || cheque.fechaEChequeDiferido;
                    fechaDiferido = cheque.fechaDiferidoChequeDiferido || cheque.fechaDiferidoEChequeDiferido || "";
                    break;
                case enums.TipoCheque.Terceros:
                    fecha = cheque.fechaChequeTerceros;
                    fechaDiferido = cheque.fechaDiferidoChequeTerceros || "";
                    break;
            }

            var row = rowTemplateCheques
                .replace('[@Banco]', cheque.bancoChequeComun || cheque.bancoEChequeComun || cheque.bancoChequeDiferido || cheque.bancoEChequeDiferido)
                .replace('[@TipoCheque]', cheque.tipoCheque)
                .replace('[@NumeroCheque]', cheque.numeroChequeComun || cheque.numeroChequeDiferido || cheque.numeroEChequeComun || cheque.numeroEChequeDiferido)
                .replace('[@Fecha]', fecha == "" ? fecha : moment(fecha, enums.FormatoFecha.DatabaseFullFormat).format(enums.FormatoFecha.DefaultFormat))
                .replace('[@FechaDiferido]', fechaDiferido == "" ? fechaDiferido : moment(fechaDiferido, enums.FormatoFecha.DatabaseFullFormat).format(enums.FormatoFecha.DefaultFormat))
                .replace('[@Importe]', accounting.formatMoney(cheque.importe));

            rowsCheques += row;
        }

        for (var i in rowData.anticiposClientes) {
            var anticipo = rowData.anticiposClientes[i];
            var row = rowTemplateAnticiposClientes
                .replace('[@Fecha]', moment(anticipo.fecha, enums.FormatoFecha.DatabaseFullFormat).format(enums.FormatoFecha.DefaultFormat))
                .replace('[@Cliente]', anticipo.cliente)
                .replace('[@Descripcion]', anticipo.descripcion)
                .replace('[@Moneda]', anticipo.moneda)
                .replace('[@Importe]', accounting.formatMoney(anticipo.importe));

            rowsAnticiposClientes += row;
        }

        for (var i in rowData.anticiposProveedores) {
            var anticipo = rowData.anticiposProveedores[i];
            var row = rowTemplateAnticiposProveedores
                .replace('[@Fecha]', moment(anticipo.fecha, enums.FormatoFecha.DatabaseFullFormat).format(enums.FormatoFecha.DefaultFormat))
                .replace('[@Proveedor]', anticipo.proveedor)
                .replace('[@Descripcion]', anticipo.descripcion)
                .replace('[@Moneda]', anticipo.moneda)
                .replace('[@Importe]', accounting.formatMoney(anticipo.importe));

            rowsAnticiposProveedores += row;
        }

        return tableTemplate
            .replace('[@RowsCheques]', rowsCheques)
            .replace('[@RowsAnticiposClientes]', rowsAnticiposClientes)
            .replace('[@RowsAnticiposProveedores]', rowsAnticiposProveedores);
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

            Ajax.Execute('/SaldoInicioBanco/Obtener/' + row.data().idSaldoInicioBanco, null, null, 'GET')
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

    // #endregion


    // #region Acciones de Items Detalle y Retenciones

    function masterDetailInitializationItem() {
        console.log('MasterDetail inicializado');
    }

    function masterDetailDialogOpeningItem(e, dialog, $form, $item) {

        if ($item != undefined) {

            $('.dvTipoSdoInicio').removeClass('show').addClass('hide');
            $('input[name="Cheque.Importe"]').prop('readonly', false);

            switch (parseInt($item.IdTipoCheque.value)) {
                case enums.TipoSdoInicioBanco.ChequeComun:
                    $('.dvChequeComun').addClass('show');
                    $('.modalItemsCheques .dvChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoSdoInicioBanco.EChequeComun:
                    $('.dvEChequeComun').addClass('show');
                    $('.modalItemsCheques .dvEChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoSdoInicioBanco.ChequeDiferido:
                    $('.dvChequeDiferido').addClass('show');
                    $('.modalItemsCheques .dvChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoSdoInicioBanco.EChequeDiferido:
                    $('.dvEChequeDiferido').addClass('show');
                    $('.modalItemsCheques .dvEChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoSdoInicioBanco.ChequeTerceros:
                    $('.dvChequeTerceros').addClass('show');
                    $('.modalItemsCheques .dvChequeTerceros .select2-field').addClass('validate');
                    break;
            }

            $('select[name="Cheque.IdTipoCheque"]').prop('disabled', true);
            $('.dvImporte').addClass('show');
        }
        else {
            $('.dvTipoSdoInicio').removeClass('show').addClass('hide');
            $('select[name="Cheque.IdTipoCheque"]').prop('disabled', false);
            $('.dvImporte').removeClass('show').addClass('hide');
        }

    }

    // #endregion


    // #region Eventos Master Detail Items Detalle

    function masterDetailRowAddingItem(e, item, items, $row, $rows) {

    }

    function masterDetailRowAddedItem(e, item, items, $row, $rows) {

    }

    function masterDetailRowEditedItem(e, item, items, $row, $rows) {

    }

    function masterDetailRowRemovedItem(e, item, items, $row, $rows) {

    }

    function masterDetailSourceLoadedItem(e, items, $rows) {

    }

    function masterDetailEmptyItem(e) {

    }

    // #endregion

});

$(function () {
    ABMSaldoInicioBancoView.init();
});
