/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/masterDetail.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMRecibosView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var modalImputaciones;
    var modalImputacionesSave;
    var btImputaciones;

    var btEdit;
    var btImprimir;
    var btRemove;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        modalImputaciones = $('.modal.modalImputaciones', mainClass);
        modalImputacionesSave = $('.modal-footer button.btConfirm', modalImputaciones);
        btImputaciones = $('.toolbar-actions button.btImputaciones', mainClass);
        btImprimir = $('.toolbar-actions button.btImprimir', mainClass);
        btImprimir.prop('disabled', true);

        btEdit = $('.toolbar-actions button.btEdit', mainClass);
        btRemove = $('.toolbar-actions button.btRemove', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }

    function buildControls() {

        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');
        modalImputaciones.find('> .modal-dialog').addClass('modal-70');

        $('.modal .dvItems .master-detail-component', mainClass)
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
                modalSelector: mainClass + ' .modal.modalMasterItems',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });

        $('.modal .dvAnticipos .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
            .on('repeater:row:added', repeaterAnticiposRowAdded)
            .on('repeater:field:blur', repeaterAnticiposFieldBlur)
            .Repeater({
                autoAddRow: false,
                readonly: false,
                disableRemove: true,
                disableAdd: true,
                disableEdit: true
            });

        $('.modal .dvRetenciones  .master-detail-component', mainClass)
            .on('master-detail:init', masterDetailInitializationRetencion)
            .on('master-detail:dialog-add-opening', masterDetailDialogRetencionesOpening)
            .on('master-detail:dialog-edit-opening', masterDetailDialogRetencionesOpening)
            .on('master-detail:row:added', masterDetailRowAddedRetencion)
            .on('master-detail:row:edited', masterDetailRowEditedRetencion)
            .on('master-detail:row:removed', masterDetailRowRemovedRetencion)
            .on('master-detail:source:loaded', masterDetailSourceLoadedRetencion)
            .on('master-detail:empty', masterDetailEmptyRetencion)
            .on('master-detail:row:adding', masterDetailRowAddingRetencion)
            .MasterDetail({
                modalSelector: mainClass + ' .modal.modalMasterRetenciones',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });

        $('.modal .dvImputaciones .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
            .on('repeater:row:added', repeaterImputacionesRowAdded)
            .on('repeater:row:removed', repeaterImputacionesRowRemoved)
            .on('repeater:field:blur', repeaterFieldBlur)
            .Repeater({
                readonly: false,
                autoAddRow: false,
                disableRemove: true,
                disableAdd: true,
                disableEdit: true
            });

        MaestroLayout.errorTooltips = false;
        MaestroLayout.disabledFields = true;

        var columns = [
            { data: 'idRecibo' },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'idTipoRecibo', render: renderTipoRecibo },
            { data: 'cliente', render: DataTableEx.renders.ellipsis },
            { data: 'descripcion', render: DataTableEx.renders.ellipsis },
            { data: 'moneda' },
            {
                data: 'total',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { data: 'idEstadoRecibo', orderable: false, searchable: false, class: 'text-center', render: renderEstadoRecibo },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'desc']];
        var pageLength = 30;

        dtView = MaestroLayout.initializeDatatable('idRecibo', columns, order, pageLength);

    }

    function renderTipoRecibo(data, type, row) {

        switch (row.idTipoRecibo) {
            case enums.TipoRecibo.Comun:
                return '<span class="badge badge-success"> Común </span>';
            case enums.TipoRecibo.Anticipo:
                return '<span class="badge badge-info"> Anticipo </span>';
        }
    }

    function renderEstadoRecibo(data, type, row) {

        switch (row.idEstadoRecibo) {
            case enums.EstadoRecibo.Ingresado:
                return '<span class="badge badge-danger"> Ingresado </span>';
            case enums.EstadoRecibo.Finalizado:
                return '<span class="badge badge-success"> Finalizado </span>';
            case enums.EstadoRecibo.Anulado:
                return '<span class="badge badge-dark"> Anulado </span>';
        }

    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.asiento', retrieveAsientoDetails);

        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.mainTableDraw = mainTableDraw;

        $('select[name="Detalle.IdBanco"]').select2({ allowClear: true });

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

        $('select[name^="IdCliente"]').on('change', function (e) {
            if ($(this).val() == "")
                $('input[name^=Descripcion]').val("");
            else
                $('input[name^=Descripcion]').val(`COBRO ${$(this).select2('data').text}`).valid();


        });

        $('select[name="Retencion.IdCategoria"]').on('change', function (e) {

            var idCategoria = $(this).val();

            var objConteinerCuentaContable = $('select[name="Retencion.IdCuentaContable"]').closest('div').parent();

            $(objConteinerCuentaContable).addClass('hide');
            $(objConteinerCuentaContable).find('.select2-field').removeClass('validate');

            switch (parseInt(idCategoria)) {
                case enums.CategoriaRetencion.IngresosBrutos:
                    $(objConteinerCuentaContable).removeClass('hide');
                    $(objConteinerCuentaContable).find('.select2-field').addClass('validate');
                    break;

                default:
                    break;
            }

        });
        $('select[name="Retencion.IdCuentaContable"]').closest('div').parent().addClass('hide');

        $('select[name="Detalle.IdTipoCobro"]').on('change', function (e) {

            var idTipoCobro = $(this).val();

            $('.dvTipoCobro').removeClass('show').addClass('hide');
            $('.modalMasterItems .select2-field.validate').removeClass('validate');

            $('input[name="Detalle.Descripcion"]').val($('#Detalle_IdTipoCobro :selected').text());

            switch (parseInt(idTipoCobro)) {
                case enums.TipoCobro.Efectivo:
                    $('.dvEfectivo').addClass('show');
                    $('.modalMasterItems .dvEfectivo .select2-field').addClass('validate');
                    break;
                case enums.TipoCobro.Cheque:
                    $('.dvCheque').addClass('show');
                    $('.modalMasterItems .dvCheque .select2-field').addClass('validate');
                    break;
                case enums.TipoCobro.Transferencia:
                    $('.dvTransferencia').addClass('show');
                    $('.modalMasterItems .dvTransferencia .select2-field').addClass('validate');
                    break;
                case enums.TipoCobro.Documento:
                    $('.dvDocumento').addClass('show');
                    $('.modalMasterItems .dvDocumento .select2-field').addClass('validate');
                    break;
            }

            $('.dvImporte').addClass('show');

        });

        $('input[name="Detalle.NroCheque"]').on('blur', function (e) {
            var nroCheque = $(this).val().replaceAll("_", "");
            $(this).val(String("0000000000" + nroCheque).slice(-10));
            $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoCobro :selected').text()} ${$(this).val()}`);
        });

        $('input[name="Detalle.NroDocumento"]').on('blur', function (e) {
            var nroDocumento = $(this).val().replaceAll("_", "");
            $(this).val(String("0000000000" + nroDocumento).slice(-10));
            $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoCobro :selected').text()} ${$(this).val()}`);
        });

        $('input[name="Detalle.ImporteEfectivo"], input[name="Detalle.ImporteCheque"], input[name="Detalle.ImporteTransferencia"], input[name^="Detalle.ImporteDocumento"]').on('blur', function (e) {
            var importe = AutoNumeric.isManagedByAutoNumeric(this) ? AutoNumeric.getNumber(this) : $(this).val();
            $('input[name="Detalle.Importe"]').val(importe);
        });

        $('select[name^=IdMonedaCobro]', modalNew).on('change', function (e) {

            var $form = $(e.target).closest('form');

            if ($('input[name^=Fecha]', modalNew).val() == "")
                return;

            if ($('select[name^=IdMonedaCobro]', modalNew).val() == "")
                return;

            var params = {
                idMoneda1: $('select[name="IdMonedaCobro"]', modalNew).val(),
                fecha: moment($('input[name^=Fecha]', modalNew).val(), 'DD/MM/YYYY').format('YYYY-MM-DD')
            };

            Ajax.Execute('/Recibos/GetFechaCambio/', params)
                .done(function (response) {
                    if (response.status == enums.AjaxStatus.OK) {
                        AutoNumeric.set($('input[name^=Cotizacion]', $form)[0], response.detail);
                    }
                    else {
                        AutoNumeric.set($('input[name^=Cotizacion]', $form)[0], 1);
                    }

                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    Utils.modalError('Error', jqXHR.responseJSON.detail);
                });

        });

        $('.btnCargaAnticipos').on('click', function (e) {

            var $form = $(e.target).closest('form');
            var dialog = $(e.target).parents('.modal');

            var idCliente = $('select[name^=IdCliente]', dialog).select2('val');
            var idMoneda = $('select[name^=IdMoneda]', dialog).select2('val');

            if (idCliente == "") {
                Utils.alertInfo("Seleccione un Cliente");
                return false;
            }

            if (idMoneda == "") {
                Utils.alertInfo("Seleccione una Moneda para el Recibo");
                return false;
            }

            var action = '/Recibos/ObtenerAnticiposImputar';
            var params = {
                idCliente: idCliente,
                idMoneda: idMoneda
            };

            $form.find('.dvAnticipos .repeater-component').Repeater()
                .clear()

            Ajax.GetJson(action, params)
                .done(function (anticipos) {

                    $form.find('.dvAnticipos .repeater-component').Repeater()
                        .clear()
                        .source(anticipos);
                })
                .fail(Ajax.ShowError);

        });

        //Modal de Imputaciones
        modalImputaciones
            .on('shown.bs.modal', function () {
                var $form = $('form', this);
                var action = '/Recibos/ObtenerComprobantesImputar';
                var params = {
                    id: dtView.selected()[0].idRecibo
                };

                $('input[name="IdRecibo"]', this).val(params.id);
                AutoNumeric.set($('input[name="Redondeo"]')[0], 0);
                AutoNumeric.set($('.ImporteComposicionPago')[0], 0);

                $form.find('.dvImputaciones .repeater-component').Repeater()
                    .clear()

                Ajax.GetJson(action, params)
                    .done(function (comprobantes) {

                        $form.find('.dvImputaciones .repeater-component').Repeater()
                            .clear()
                            .source(comprobantes);
                    })
                    .fail(Ajax.ShowError);
            })
            .on('hidden.bs.modal', function () {
                var form = $('form', this);
                Utils.resetValidator(form);
            });

        modalImputacionesSave
            .on('click', function () {
                $('form', modalImputaciones).on('submit', function (e) {
                    $(this).find('input[type=text]').each(function () {
                        if (AutoNumeric.isManagedByAutoNumeric(this))
                            AutoNumeric.getAutoNumericElement(this).unformat();
                    });
                });

                $('form', modalImputaciones).submit();
            });

        btImputaciones.on('click', showImputaciones);
        btImprimir.on('click', imprimir);

    }

    function newDialogOpening(dialog, $form) {

        var idEjercicio = $form.find('#IdEjercicio option:eq(1)').val();
        $form.find('#IdEjercicio').select2('val', idEjercicio);
        $form.find('#Fecha').val(moment().format(enums.FormatoFecha.DefaultFormat));
        
        $form.find('.dvItems .master-detail-component').MasterDetail()
            .clear();

        $form.find('.dvRetenciones .master-detail-component').MasterDetail()
            .clear();

        $form.find('.dvAnticipos .repeater-component').Repeater()
            .clear();

        $form.find('#Total').prop('disabled', true);
        AutoNumeric.set($form.find('#Total')[0], 0.0);

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdRecibo_Edit').val(entity.idRecibo);
        $form.find('#IdEjercicio_Edit').select2('val', entity.idEjercicio);
        $form.find('#Fecha_Edit').val(moment(entity.fecha).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#IdTipoRecibo_Edit').select2('val', entity.idTipoRecibo);
        $form.find('#IdMoneda_Edit').select2('val', entity.moneda);
        $form.find('#IdMonedaCobro_Edit').select2('val', entity.monedaCobro);
        $form.find('#Cotizacion_Edit').val(entity.cotizacion);
        $form.find('#IdCliente_Edit').select2('val', entity.idCliente);
        $form.find('#Descripcion_Edit').val(entity.descripcion);

        $form.find('.dvItems .master-detail-component').MasterDetail()
            .source(entity.items);

        $form.find('.dvRetenciones .master-detail-component').MasterDetail().clear();
        if (entity.retenciones.length > 0)
            $form.find('.dvRetenciones .master-detail-component').MasterDetail()
                .source(entity.retenciones);

        $form.find('.dvAnticipos .repeater-component').Repeater().clear();
        if (entity.anticipos.length > 0)
            $form.find('.dvAnticipos .repeater-component').Repeater()
                .source(entity.anticipos);

        $form.find('#Total').prop('disabled', true);
        AutoNumeric.set($form.find('#Total_Edit')[0], entity.total);

    }

    function mainTableRowSelected(dataArray, api) {

        btImprimir.prop('disabled', true);
        btImputaciones.prop('disabled', true);
        btEdit.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        btImputaciones.prop('disabled', !dataArray.length == 1 || dataArray[0].idTipoRecibo == enums.TipoRecibo.Anticipo);
        btEdit.prop('disabled', dataArray[0].idEjercicio == null);
        btImprimir.prop('disabled', dataArray[0].idEjercicio == null);
        btRemove.prop('disabled', dataArray[0].idEjercicio == null);

        //Para Recibos Finalizados
        btImputaciones.prop('disabled', dataArray[0].idEstadoRecibo == enums.EstadoRecibo.Finalizado);
        btEdit.prop('disabled', dataArray[0].idEstadoRecibo == enums.EstadoRecibo.Finalizado);

    }

    function mainTableDraw() {

        var data = dtView.selected();

        btImputaciones.prop('disabled', true);

        if (data === undefined || data === null)
            return;

        btImputaciones.prop('disabled', !data.length == 1);

    }

    function renderViewDetails(data, type, row) {

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

    // #region Eventos del Repeater de Anticipos

    function repeaterInitialization() {
        console.log('repeater iniciado');
    }

    function repeaterAnticiposRowAdded(e, $newRow, $rows) {
        inicializarAnticiposSeleccion(e, $newRow);
        calculaTotalRecibo(e);
    }

    function repeaterAnticiposFieldBlur(e, data) {
        calculaTotalRecibo(e);
    }

    // #endregion

    // #region Detalle de Items y Asiento Contable

    function formatRowDetail(rowData) {

        if (rowData == null || rowData.items == null || rowData.items.length == 0)
            return '<div class="text-center"><span>El Recibo no presenta Items en el Detalle</span></div>';

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Descripción</th>
                            <th>Importe</th>
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
                <td>[@Importe]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData.items) {
            var item = rowData.items[i];
            var row = rowTemplate
                .replace('[@Descripcion]', item.descripcion)
                .replace('[@Importe]', accounting.formatMoney(item.importe));

            rows += row;
        }

        for (var i in rowData.retenciones) {
            var retencion = rowData.retenciones[i];
            var row = rowTemplate
                .replace('[@Descripcion]', `${retencion.categoria.toUpperCase()} NRO. ${retencion.nroRetencion}`)
                .replace('[@Importe]', accounting.formatMoney(retencion.importe));

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

            Ajax.Execute('/Recibos/Obtener/' + row.data().idRecibo, null, null, 'GET')
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

            Ajax.Execute('/Recibos/ObtenerAsiento/' + row.data().idRecibo, null, null, 'GET')
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

    function formatRowAsientoDetail(rowData) {

        if (rowData == null || rowData.length == 0)
            return '<div class="text-center"><span>El Recibo no presenta un Asiento Contable</span></div>';

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

    // #endregion

    // #region Eventos para Imputacion de Comprobantes

    function showImputaciones(e) {

        var selectedData = dtView.selected()[0];

        if (selectedData.idEstadoRecibo == enums.EstadoRecibo.Finalizado) {
            Utils.alertInfo("El Recibo se encuentra Finalizado");
            return;
        }

        $('.importeRecibo', modalImputaciones)
            .text(accounting.formatMoney(selectedData.total));

        modalImputaciones.modal({ backdrop: 'static' });

    }

    this.modalImputacionesajaxFormBegin = function (context, arguments) {

        var dialog = $(this).parents('.modal');
        var btConfirm = dialog.find('.modal-footer button.btConfirm');

        MaestroLayout.buttonLoader(btConfirm, true, 'Guardando');
    }

    this.modalImputacionesajaxFormSuccess = function (context) {

        Utils.alertSuccess('Guardado correctamente.');
        $(modalImputaciones).modal('hide');

        dtView.reload();

        var dialog = $(this).parents('.modal');
        var btConfirm = dialog.find('.modal-footer button.btConfirm')

        MaestroLayout.buttonLoader(btConfirm, false);
    }

    this.modalImputacionesajaxFormFailure = function (context) {

        Utils.ajaxFormFailure(context);

        var dialog = $(this).parents('.modal');
        var btConfirm = dialog.find('.modal-footer button.btConfirm')

        MaestroLayout.buttonLoader(btConfirm, false);
    }

    // #endregion

    // #region Acciones de Items Detalle y Retenciones

    function masterDetailInitializationItem() {
        console.log('MasterDetail inicializado');
    }

    function masterDetailDialogRetencionesOpening(e, dialog, $form, $item) {

        var objConteinerCuentaContable = $('select[name="Retencion.IdCuentaContable"]').closest('div').parent();

        if ($item != undefined) {

            $(objConteinerCuentaContable).addClass('hide');
            $(objConteinerCuentaContable).find('.select2-field').removeClass('validate');

            switch (parseInt($item.IdCategoria.value)) {
                case enums.CategoriaRetencion.IngresosBrutos:
                    $(objConteinerCuentaContable).removeClass('hide');
                    $(objConteinerCuentaContable).find('.select2-field').addClass('validate');
                    break;

                default:
                    break;
            }
        }
        else {
            $(objConteinerCuentaContable).addClass('hide');
        }

    }

    function masterDetailDialogOpeningItem(e, dialog, $form, $item) {

        if ($item != undefined) {
            $('.dvTipoCobro').removeClass('show').addClass('hide');
            $('.modalMasterItems .select2-field').removeClass('validate');
            switch (parseInt($item.IdTipoCobro.value)) {
                case enums.TipoCobro.Efectivo:
                    $('.dvEfectivo').addClass('show');
                    $('.modalMasterItems .dvEfectivo .select2-field').addClass('validate');
                    break;
                case enums.TipoCobro.Cheque:
                    $('.dvCheque').addClass('show');
                    $('.modalMasterItems .dvCheque .select2-field').addClass('validate');
                    break;
                case enums.TipoCobro.Transferencia:
                    $('.dvTransferencia').addClass('show');
                    $('.modalMasterItems .dvTransferencia .select2-field').addClass('validate');
                    break;
                case enums.TipoCobro.Documento:
                    $('.dvDocumento').addClass('show');
                    $('.modalMasterItems .dvDocumento .select2-field').addClass('validate');
                    break;
            }

            $('select[name="Detalle.IdTipoCobro"]').prop('disabled', true);
            $('.dvImporte').addClass('show');
        }
        else {
            $('.dvTipoCobro').removeClass('show').addClass('hide');
            $('select[name="Detalle.IdTipoCobro"]').prop('disabled', false);
        }

        console.log('MasterDetail Dialog Opening');

    }



    // #endregion

    // #region Eventos Master Detail Items Detalle

    function masterDetailRowAddingItem(e, item, items, $row, $rows) {
        console.log('MasterDetail item agregando', item);
    }

    function masterDetailRowAddedItem(e, item, items, $row, $rows) {
        console.log('MasterDetail item agregado', item);
        calculaTotalRecibo(e);
    }

    function masterDetailRowEditedItem(e, item, items, $row, $rows) {
        console.log('MasterDetail item editado', item);
        calculaTotalRecibo(e);
    }

    function masterDetailRowRemovedItem(e, item, items, $row, $rows) {
        calculaTotalRecibo(e);
        console.log('MasterDetail item eliminado', item, items);
    }

    function masterDetailSourceLoadedItem(e, items, $rows) {
        calculaTotalRecibo(e);
        console.log('MasterDetail lista de items cargada', items, $rows);
    }

    function masterDetailEmptyItem(e) {
        calculaTotalRecibo(e);
        console.log('MasterDetail items eliminados');
    }

    // #endregion

    // #region Eventos Master Detail Items Detalle

    function masterDetailInitializationRetencion() {
        console.log('MasterDetail inicializado');
    }

    function masterDetailRowAddingRetencion(e, item, items, $row, $rows) {
        console.log('MasterDetail item agregando', item);
    }

    function masterDetailRowAddedRetencion(e, item, items, $row, $rows) {
        console.log('MasterDetail item agregado', item);
        calculaTotalRecibo(e);
    }

    function masterDetailRowEditedRetencion(e, item, items, $row, $rows) {
        calculaTotalRecibo(e);
        console.log('MasterDetail item editado', item);
    }

    function masterDetailRowRemovedRetencion(e, item, items, $row, $rows) {
        calculaTotalRecibo(e);
        console.log('MasterDetail item eliminado', item, items);
    }

    function masterDetailSourceLoadedRetencion(e, items, $rows) {
        calculaTotalRecibo(e);
        console.log('MasterDetail lista de items cargada', items, $rows);
    }

    function masterDetailEmptyRetencion(e) {
        calculaTotalRecibo(e);
        console.log('MasterDetail items eliminados');
    }

    // #endregion

    // #region Eventos del Repeater de Imputaciones
    function repeaterInitialization() {
        console.log('repeater iniciado');
    }

    function repeaterImputacionesRowAdded(e, $newRow, $rows) {
        inicializarSeleccion(e, $newRow);
    }

    function repeaterImputacionesRowRemoved(e, $removedRow, $rows) {

    }

    function repeaterFieldBlur(e, data) {
        calculaTotalImputaciones(e);
    }

    // #endregion

    // #region Funcion de Totales para Recibo

    function calculaTotalRecibo(e) {

        var $form = $(e.target).closest('form');

        var total = 0, reten = 0, anticipos = 0;
        var items = $form.find('.dvItems .master-detail-component').MasterDetail().getItems();
        var retenciones = $form.find('.dvRetenciones .master-detail-component').MasterDetail().getItems();
        var trsAnticipos = $form.find('.dvAnticipos .repeater-component tbody > tr');


        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            total += parseFloat(item.Importe.value);
        }

        for (var i = 0; i < retenciones.length; i++) {
            var retencion = retenciones[i];
            reten += parseFloat(retencion.Importe.value);
        }

        trsAnticipos.each(function () {

            var tr = $(this);

            var anticipo = tr.find('> td > input[name$=".Importe"]');
            var anticipof = AutoNumeric.isManagedByAutoNumeric(anticipo.get(0)) ? AutoNumeric.getNumber(anticipo.get(0)) : $(anticipo).val();

            anticipos += anticipof;
        });

        AutoNumeric.set($form.find('input[name^=Total]')[0], total + reten + anticipos);

    }

    // #endregion

    // #region Funcion para inicializar los checkbox de Selección y Totales

    function inicializarSeleccion(e, $newRow) {

        var checkbox = $('input[type=checkbox][name$=".Seleccionar"]', $newRow);
        var importe = $('input[type=text][name$=".Importe"]', $newRow);

        importe.prop('readonly', true);

        checkbox.on('change', function () {

            importe
                .prop('readonly', !this.checked)
                .focus();

            if (this.checked) {
                AutoNumeric.getAutoNumericElement(importe[0]).update({
                    maximumValue: maxImporte,
                    minimumValue: minImporte
                });
            }

            if (!this.checked) {
                if (AutoNumeric.isManagedByAutoNumeric(importe.get(0)))
                    AutoNumeric.set(importe.get(0), 0.0);
                else
                    importe.val(0.0);

                checkbox.focus();
            }

        });
    }

    function inicializarAnticiposSeleccion(e, $newRow) {

        var checkbox = $('input[type=checkbox][name$=".Seleccionar"]', $newRow);
        var importe = $('input[type=text][name$=".Importe"]', $newRow);

        importe.prop('readonly', !checkbox.is(":checked"));

        checkbox.on('change', function () {

            importe
                .prop('readonly', !this.checked)
                .focus();

            if (!this.checked) {
                if (AutoNumeric.isManagedByAutoNumeric(importe.get(0)))
                    AutoNumeric.set(importe.get(0), 0.0);
                else
                    importe.val(0.0);

                checkbox.focus();
            }

        });
    }

    function calculaTotalImputaciones(e) {

        var $form = $(e.target).closest('form');
        var trs = $(e.target).find('tbody > tr');
        var total = 0;

        trs.each(function () {

            var tr = $(this);

            var importe = tr.find('> td > input[name$=".Importe"]');
            var importef = AutoNumeric.isManagedByAutoNumeric(importe.get(0)) ? AutoNumeric.getNumber(importe.get(0)) : $(importe).val();

            total += importef;
        });

        $form.find('.ImporteComprobantes').text(accounting.formatMoney(total));

    }

    // #endregion

    function imprimir() {

        var id = dtView.selected()[0].idRecibo;
        var action = $(this).data('ajax-action') + id;

        window.open(action);

    }

});

$(function () {
    ABMRecibosView.init();
});
