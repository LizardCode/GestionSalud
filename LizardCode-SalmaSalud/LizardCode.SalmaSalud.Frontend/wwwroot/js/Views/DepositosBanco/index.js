/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/masterDetail.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMDepositosBancoView = new (function () {

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

        MaestroLayout.errorTooltips = false;
        MaestroLayout.disabledFields = true;

        var columns = [
            { data: 'idDepositoBanco' },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'banco' },
            { data: 'descripcion' },
            { data: 'moneda' },
            {
                data: 'importe',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'desc']];
        var pageLength = 30;

        dtView = MaestroLayout.initializeDatatable('idDepositoBanco', columns, order, pageLength);

    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.asiento', retrieveAsientoDetails);

        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;

        $('select[name^="Detalle.IdBanco"]').select2({ allowClear: true });

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

        $('select[name^="IdBanco"]').on('change', function (e) {
            if ($(this).val() == "")
                $('input[name^=Descripcion]').val("");
            else
                $('input[name^=Descripcion]').val(`DEPOSITO ${$(this).select2('data').text}`).valid();
        });

        $('select[name="Detalle.IdTipoDeposito"]').on('change', function (e) {

            var idTipoDeposito = $(this).val();

            $('.dvTipoDeposito').removeClass('show').addClass('hide');
            $('.modalMasterItems .select2-field.validate').removeClass('validate');

            $('input[name="Detalle.Descripcion"]').val($('#Detalle_IdTipoDeposito :selected').text());
            $('input[name="Detalle.Importe"]').prop('readonly', false);

            switch (parseInt(idTipoDeposito)) {
                case enums.TipoDeposito.Efectivo:
                    $('.dvEfectivo').addClass('show');
                    $('.modalMasterItems .dvEfectivo .select2-field').addClass('validate');
                    break;
                case enums.TipoDeposito.ChequeComun:
                    $('.dvChequeComun').addClass('show');
                    $('.modalMasterItems .dvChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoDeposito.EChequeComun:
                    $('.dvEChequeComun').addClass('show');
                    $('.modalMasterItems .dvEChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoDeposito.ChequeDiferido:
                    $('.dvChequeDiferido').addClass('show');
                    $('.modalMasterItems .dvChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoDeposito.EChequeDiferido:
                    $('.dvEChequeDiferido').addClass('show');
                    $('.modalMasterItems .dvEChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoDeposito.ChequeTerceros:
                    $('.dvChequeTerceros').addClass('show');
                    $('.modalMasterItems .dvChequeTerceros .select2-field').addClass('validate');
                    $('input[name="Detalle.Importe"]').prop('readonly', true);
                    break;
                case enums.TipoDeposito.Transferencia:
                    $('.dvTransferencia').addClass('show');
                    $('.modalMasterItems .dvTransferencia .select2-field').addClass('validate');
                    break;
            }

            $('.dvImporte').addClass('show');

        });

        $('input[name="Detalle.NumeroCheque"]').on('blur', function (e) {
            var nroCheque = $(this).val().replaceAll("_", "");
            $(this).val(String("0000000000" + nroCheque).slice(-10));
            $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoDeposito :selected').text()} ${$(this).val()}`);
        });

        $('input[name="Detalle.ImporteEfectivo"], input[name="Detalle.ImporteCheque"], input[name="Detalle.ImporteTransferencia"]').on('blur', function (e) {
            var importe = AutoNumeric.isManagedByAutoNumeric(this) ? AutoNumeric.getNumber(this) : $(this).val();
            $('input[name="Detalle.Importe"]').val(importe);
        });

        $('select[name="Detalle.IdBancoChequeComun"]')
            .on('change', function (e) {

                if ($(this).select2('val') == "") {
                    $('input[name="Detalle.NumeroChequeComun"]').val('');
                    $('input[name="Detalle.Descripcion"]').val('');
                    return;
                }

                var iconObj = $('input[name="Detalle.NumeroChequeComun"]').closest('div').find('i');

                var action = 'DepositosBanco/GetPrimerChequeDisponible';
                var params = {
                    idBanco: $(this).select2('val'),
                    idTipoCheque: enums.TipoCheque.Comun
                };

                iconLoader($(iconObj), true);

                Ajax.GetJson(action, params)
                    .done(function (cheque) {
                        $('input[name="Detalle.NumeroChequeComun"]').val(cheque.nroCheque);
                        $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoDeposito :selected').text()} ${cheque.nroCheque}`);
                    })
                    .fail(Ajax.ShowError)
                    .always(function () {
                        iconLoader($(iconObj), false);
                    });
            });

        $('select[name="Detalle.IdBancoChequeDiferido"]')
            .on('change', function (e) {

                if ($(this).select2('val') == "") {
                    $('input[name="Detalle.NumeroChequeDiferido"]').val('');
                    $('input[name="Detalle.Descripcion"]').val('');
                    return;
                }

                var iconObj = $('input[name="Detalle.NumeroChequeDiferido"]').closest('div').find('i');

                var action = 'DepositosBanco/GetPrimerChequeDisponible';
                var params = {
                    idBanco: $(this).select2('val'),
                    idTipoCheque: enums.TipoCheque.Diferido
                };

                iconLoader($(iconObj), true);

                Ajax.GetJson(action, params)
                    .done(function (cheque) {
                        $('input[name="Detalle.NumeroChequeDiferido"]').val(cheque.nroCheque);
                        $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoDeposito :selected').text()} ${cheque.nroCheque}`);
                    })
                    .fail(Ajax.ShowError)
                    .always(function () {
                        iconLoader($(iconObj), false);
                    });
            });

        $('select[name="Detalle.IdBancoEChequeComun"]')
            .on('change', function (e) {

                if ($(this).select2('val') == "") {
                    $('input[name="Detalle.NumeroEChequeComun"]').val('');
                    $('input[name="Detalle.Descripcion"]').val('');
                    return;
                }

                var iconObj = $('input[name="Detalle.NumeroEChequeComun"]').closest('div').find('i');

                var action = 'DepositosBanco/GetPrimerChequeDisponible';
                var params = {
                    idBanco: $(this).select2('val'),
                    idTipoCheque: enums.TipoCheque.EChequeComun
                };

                iconLoader($(iconObj), true);

                Ajax.GetJson(action, params)
                    .done(function (cheque) {
                        $('input[name="Detalle.NumeroEChequeComun"]').val(cheque.nroCheque);
                        $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoDeposito :selected').text()} ${cheque.nroCheque}`);
                    })
                    .fail(Ajax.ShowError)
                    .always(function () {
                        iconLoader($(iconObj), false);
                    });
            });

        $('select[name="Detalle.IdBancoEChequeDiferido"]')
            .on('change', function (e) {

                if ($(this).select2('val') == "") {
                    $('input[name="Detalle.NumeroEChequeDiferido"]').val('');
                    $('input[name="Detalle.Descripcion"]').val('');
                    return;
                }

                var iconObj = $('input[name="Detalle.NumeroEChequeDiferido"]').closest('div').find('i');

                var action = 'DepositosBanco/GetPrimerChequeDisponible';
                var params = {
                    idBanco: $(this).select2('val'),
                    idTipoCheque: enums.TipoCheque.EChequeDiferido
                };

                iconLoader($(iconObj), true);

                Ajax.GetJson(action, params)
                    .done(function (cheque) {
                        $('input[name="Detalle.NumeroEChequeDiferido"]').val(cheque.nroCheque);
                        $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoDeposito :selected').text()} ${cheque.nroCheque}`);
                    })
                    .fail(Ajax.ShowError)
                    .always(function () {
                        iconLoader($(iconObj), false);
                    });
            });

        $('input[name="Detalle.NumeroChequeComun"]').on('blur', function (e) {
            var nroCheque = $(this).val();
            if (nroCheque == "")
                return;
            $(this).val(String("0000000000" + nroCheque).slice(-10));
        });
        $('input[name="Detalle.NumeroChequeDiferido"]').on('blur', function (e) {
            var nroCheque = $(this).val();
            if (nroCheque == "")
                return;
            $(this).val(String("0000000000" + nroCheque).slice(-10));
        });
        $('input[name="Detalle.NumeroEChequeComun"]').on('blur', function (e) {
            var nroCheque = $(this).val();
            if (nroCheque == "")
                return;
            $(this).val(String("0000000000" + nroCheque).slice(-10));
        });
        $('input[name="Detalle.NumeroEChequeDiferido"]').on('blur', function (e) {
            var nroCheque = $(this).val();
            if (nroCheque == "")
                return;
            $(this).val(String("0000000000" + nroCheque).slice(-10));
        });

        $('input[name="Detalle.NumeroChequeComun"]').on('change', validarNroChequeDisponible);
        $('input[name="Detalle.NumeroChequeDiferido"]').on('change', validarNroChequeDisponible);
        $('input[name="Detalle.NumeroEChequeComun"]').on('change', validarNroChequeDisponible);
        $('input[name="Detalle.NumeroEChequeDiferido"]').on('change', validarNroChequeDisponible);

        $('input[name="Detalle.IdChequeTerceros"]')
            .Select2Ex({ url: '/DepositosBanco/GetChequesCartera', allowClear: true, placeholder: 'Buscar...' })
            .on('change', function (e) {

                if (e.added != undefined) {
                    $('input[name="Detalle.NumeroChequeTerceros"]').val(e.added.nroCheque);
                    $('input[name="Detalle.BancoChequeTerceros"]').val(e.added.banco);
                    $('input[name="Detalle.FechaChequeTerceros"]').val(moment(e.added.fecha, 'YYYY-MM-DD').format('DD/MM/YYYY'));
                    $('input[name="Detalle.FechaDiferidoChequeTerceros').val(moment(e.added.fechaVto, 'YYYY-MM-DD').format('DD/MM/YYYY'));
                    AutoNumeric.set($('input[name="Detalle.Importe"]')[0], e.added.importe);

                    $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoDeposito :selected').text()} ${e.added.nroCheque}`);
                }
                else {
                    $('input[name="Detalle.NumeroChequeTerceros"]').val("");
                    $('input[name="Detalle.BancoChequeTerceros"]').val("");
                    $('input[name="Detalle.FechaChequeTerceros"]').val("");
                    $('input[name="Detalle.FechaDiferidoChequeTerceros').val("");
                    $('input[name="Detalle.Importe"]').val("");

                    $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoDeposito :selected').text()}`);
                }
            });

        $('select[name^=IdMoneda]', modalNew).on('change', function (e) {

            var $form = $(e.target).closest('form');

            if ($('input[name^=Fecha]', modalNew).val() == "")
                return;

            if ($('select[name^=IdMoneda]', modalNew).val() == "")
                return;

            var params = {
                idMoneda1: $('select[name="IdMoneda"]', modalNew).val(),
                fecha: moment($('input[name^=Fecha]', modalNew).val(), 'DD/MM/YYYY').format('YYYY-MM-DD')
            };

            Ajax.Execute('/DepositosBanco/GetFechaCambio/', params)
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

    }

    function validarNroChequeDisponible(e) {

        var objCheque = e.target;
        var nroCheque = String("0000000000" + $(objCheque).val()).slice(-10);

        if (nroCheque == "")
            return;

        var nameBanco = $(objCheque).attr('name').replace('Numero', 'IdBanco');
        var idTipoCheque;
        var tipoCheque = $(objCheque).attr('name').replace('Detalle.Numero', '');

        switch (tipoCheque) {
            case enums.DescripcionTipoCheque.ChequeComun:
                idTipoCheque = enums.TipoCheque.Comun;
                break;
            case enums.DescripcionTipoCheque.ChequeDiferido:
                idTipoCheque = enums.TipoCheque.Diferido;
                break;
            case enums.DescripcionTipoCheque.EChequeComun:
                idTipoCheque = enums.TipoCheque.EChequeComun;
                break;
            case enums.DescripcionTipoCheque.EChequeDiferido:
                idTipoCheque = enums.TipoCheque.EChequeDiferido;
                break;
        }

        var params = {
            idBanco: $('select[name="' + nameBanco + '"]').select2('val'),
            idTipoCheque: idTipoCheque,
            nroCheque: nroCheque
        };

        Ajax.Execute('DepositosBanco/VerificarChequeDisponible/', params)
            .done(function (response) {
                if (response.status == enums.AjaxStatus.OK && response.detail) {
                    $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoPago :selected').text()} ${nroCheque}`);
                }
                else {
                    Utils.alertInfo("El Número de Cheque no Existe o no esta Disponible");
                    $(objCheque).val('');
                    $('input[name="Detalle.Descripcion"]').val('');
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Utils.modalError('Error', jqXHR.responseJSON.detail);
            });
    }

    function iconLoader(icon, flag) {

        if (flag) {
            icon
                .data('normal-icons', icon.attr('class'))
                .prop('disabled', true)
                .prop('class', 'far fa-cog fa-spin');
        }
        else {
            icon
                .prop('disabled', false)
                .prop('class', icon.data('normal-icons'));
        }

    }

    function newDialogOpening(dialog, $form) {

        var idEjercicio = $form.find('#IdEjercicio option:eq(1)').val()
        $form.find('#IdEjercicio').select2('val', idEjercicio);
        $form.find('#Fecha').val(moment().format(enums.FormatoFecha.DefaultFormat));
        $form.find('#Importe').prop('disabled', true);
        AutoNumeric.set($form.find('#Importe')[0], 0.0);

        $form.find('.dvItems .master-detail-component').MasterDetail()
            .clear();
    }

    function editDialogOpening($form, entity) {

        $form.find('#Importe').prop('disabled', true);

        $form.find('#IdDepositoBanco_Edit').val(entity.idDepositoBanco);
        $form.find('#IdEjercicio_Edit').select2('val', entity.idEjercicio);
        $form.find('#Fecha_Edit').val(moment(entity.fecha).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#IdMoneda_Edit').select2('val', entity.moneda);
        $form.find('#Cotizacion_Edit').val(entity.cotizacion);
        $form.find('#IdBanco_Edit').select2('val', entity.idBanco);
        $form.find('#Descripcion_Edit').val(entity.descripcion);

        AutoNumeric.set($form.find('#Importe_Edit')[0], entity.importe);

        $form.find('.dvItems .master-detail-component').MasterDetail()
            .source(entity.items);
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

    // #region Detalle de Items y Asiento Contable

    function formatRowDetail(rowData) {

        if (rowData == null || rowData.items == null || rowData.items.length == 0)
            return '<div class="text-center"><span>El Depósito Bancario no presenta Items en el Detalle</span></div>';

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

            Ajax.Execute('/DepositosBanco/Obtener/' + row.data().idDepositoBanco, null, null, 'GET')
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

            Ajax.Execute('/DepositosBanco/ObtenerAsiento/' + row.data().idDepositoBanco, null, null, 'GET')
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


    // #region Acciones de Items Detalle y Retenciones

    function masterDetailInitializationItem() {
        console.log('MasterDetail inicializado');
    }

    function masterDetailDialogOpeningItem(e, dialog, $form, $item) {

        if ($item != undefined) {

            $('.dvTipoDeposito').removeClass('show').addClass('hide');
            $('input[name="Detalle.Importe"]').prop('readonly', false);
            $('.modalMasterItems .select2-field').removeClass('validate');

            switch (parseInt($item.IdTipoDeposito.value)) {
                case enums.TipoDeposito.Efectivo:
                    $('.dvEfectivo').addClass('show');
                    $('.modalItemsFormaPago .dvEfectivo .select2-field').addClass('validate');
                    break;
                case enums.TipoDeposito.ChequeComun:
                    $('.dvChequeComun').addClass('show');
                    $('.modalMasterItems .dvChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoDeposito.EChequeComun:
                    $('.dvEChequeComun').addClass('show');
                    $('.modalMasterItems .dvEChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoDeposito.ChequeDiferido:
                    $('.dvChequeDiferido').addClass('show');
                    $('.modalMasterItems .dvChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoDeposito.EChequeDiferido:
                    $('.dvEChequeDiferido').addClass('show');
                    $('.modalMasterItems .dvEChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoDeposito.ChequeTerceros:
                    $('.dvChequeTerceros').addClass('show');
                    $('.modalMasterItems .dvChequeTerceros .select2-field').addClass('validate');
                    $('input[name="Detalle.Importe"]').prop('readonly', true);
                    break;
                case enums.TipoDeposito.Transferencia:
                    $('.dvTransferencia').addClass('show');
                    $('.modalMasterItems .dvTransferencia .select2-field').addClass('validate');
                    break;
            }

            $('select[name="Detalle.IdTipoDeposito"]').prop('disabled', true);
            $('.dvImporte').addClass('show');
        }
        else {
            $('.dvTipoDeposito').removeClass('show').addClass('hide');
            $('select[name="Detalle.IdTipoDeposito"]').prop('disabled', false);
            $('.dvImporte').removeClass('show').addClass('hide');
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
        calculaTotalDepositoBancario(e);
    }

    function masterDetailRowEditedItem(e, item, items, $row, $rows) {
        console.log('MasterDetail item editado', item);
        calculaTotalDepositoBancario(e);
    }

    function masterDetailRowRemovedItem(e, item, items, $row, $rows) {
        calculaTotalDepositoBancario(e);
        console.log('MasterDetail item eliminado', item, items);
    }

    function masterDetailSourceLoadedItem(e, items, $rows) {
        calculaTotalDepositoBancario(e);
        console.log('MasterDetail lista de items cargada', items, $rows);
    }

    function masterDetailEmptyItem(e) {
        calculaTotalDepositoBancario(e);
        console.log('MasterDetail items eliminados');
    }

    // #endregion

    // #region Funcion de Totales para Deposito Bancario

    function calculaTotalDepositoBancario(e) {

        var $form = $(e.target).closest('form');

        var total = 0;
        var items = $form.find('.dvItems .master-detail-component').MasterDetail().getItems();

        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            total += parseFloat(item.Importe.value);
        }

        AutoNumeric.set($form.find('input[name^=Importe]')[0], total);

    }

    // #endregion

});

$(function () {
    ABMDepositosBancoView.init();
});
