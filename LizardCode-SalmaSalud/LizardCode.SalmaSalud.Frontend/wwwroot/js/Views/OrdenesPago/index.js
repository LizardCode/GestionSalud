/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/masterDetail.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMOrdenesPagoView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var modalFormaPago;
    var modalFormaPagoSave;
    var btPagar;

    var btImprimir;
    var modalPrint;
    var btRemove;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        modalFormaPago = $('.modal.modalFormaPago', mainClass);
        modalPrint = $('.modal.modalPrint', mainClass);

        //Custom Buttons
        btPagar = $('.toolbar-actions button.btPagar', mainClass);
        btImprimir = $('.toolbar-actions button.btImprimir', mainClass);
        btImprimir.prop('disabled', true);
        btRemove = $('.toolbar-actions button.btRemove', mainClass);

        //Modal Pagar
        modalFormaPagoSave = $('.modal-footer button.btConfirm', modalFormaPago);
        
        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }

    function buildControls() {

        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');
        modalFormaPago.find('> .modal-dialog').addClass('modal-70');

        $('.modal .dvImputaciones .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
            .on('repeater:row:added', repeaterImputacionesRowAdded)
            .on('repeater:row:removed', repeaterImputacionesRowRemoved)
            .on('repeater:field:focus', repeaterFieldFocus)
            .on('repeater:field:change', repeaterFieldChange)
            .on('repeater:field:blur', repeaterFieldBlur)
            .Repeater({
                autoAddRow: false,
                readonly: false,
                disableRemove: true,
                disableAdd: true,
                disableEdit: true
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

        $('.modal .dvPlanillaGastos .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
            .on('repeater:row:added', repeaterPlanillaGastosRowAdded)
            .Repeater({
                autoAddRow: false,
                readonly: false,
                disableRemove: true,
                disableAdd: true,
                disableEdit: true
            });

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
                modalSelector: mainClass + ' .modal.modalItemsFormaPago',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });


        MaestroLayout.errorTooltips = true;
        MaestroLayout.disabledFields = true;

        var columns = [
            { data: 'idOrdenPago' },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'idTipoOrdenPago', render: renderTipoOrdenPago },
            { data: 'proveedor', render: DataTableEx.renders.ellipsis },
            { data: 'descripcion', render: DataTableEx.renders.ellipsis },
            { data: 'monedaPago' },
            {
                data: 'importe',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { data: 'idEstadoOrdenPago', orderable: false, searchable: false, class: 'text-center', render: renderEstadoOrdenPago },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'desc']];
        var pageLength = 30;

        dtView = MaestroLayout.initializeDatatable('idOrdenPago', columns, order, pageLength);

    }

    function renderTipoOrdenPago(data, type, row) {
        switch (row.idTipoOrdenPago) {
            case enums.TipoOrdenPago.Proveedores:
                return '<span class="badge badge-primary"> Proveedores </span>';
            case enums.TipoOrdenPago.Gastos:
                return '<span class="badge badge-secondary"> Gastos </span>';
            case enums.TipoOrdenPago.Anticipo:
                return '<span class="badge badge-info"> Anticipo </span>';
            case enums.TipoOrdenPago.Varios:
                return '<span class="badge badge-success"> Varios </span>';
        }
    }

    function renderEstadoOrdenPago(data, type, row) {

        switch (row.idEstadoOrdenPago) {
            case enums.EstadoOrdenPago.Ingresada:
                return '<span class="badge badge-danger"> Ingresada </span>';
            case enums.EstadoOrdenPago.Pagada:
                return '<span class="badge badge-success"> Pagada </span>';
        }
    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.asiento', retrieveAsientoDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.pagos', retrieveComposicionPago);
        
        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.mainTableDraw = mainTableDraw;

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

        $('select[name^="IdTipoOrdenPago"]').on('change', function (e) {

            $('.areaImputaciones').hide();
            $('.areaAnticipos').hide();
            $('.areaPlanillaGastos').hide();
            $('.btnCargaItems').hide();
            $('#IdProveedor').closest('div').parent().hide();
            $('#IdCuentaContable').closest('div').parent().hide();
            $('#Importe').closest('.form-group').hide();
            $('#ImporteAnticipo').closest('.form-group').hide();
            $('#ImporteVarios').closest('.form-group').hide();

            switch (parseInt($(this).val())) {
                case enums.TipoOrdenPago.Proveedores:
                    $('.btnCargaItems').show();
                    $('.areaAnticipos').show();
                    $('.areaImputaciones').show();
                    $('#Importe').closest('.form-group').show();
                    $('#IdProveedor').closest('div').parent().show();
                    break;
                case enums.TipoOrdenPago.Gastos:
                    $('.btnCargaItems').show();
                    $('.areaPlanillaGastos').show();
                    $('#Importe').closest('.form-group').show();
                    break;
                case enums.TipoOrdenPago.Anticipo:
                    $('#IdProveedor').closest('div').parent().show();
                    $('#ImporteAnticipo').closest('.form-group').show();
                    break;
                case enums.TipoOrdenPago.Varios:
                    $('#IdCuentaContable').closest('div').parent().show();
                    $('#ImporteVarios').closest('.form-group').show();
                    break;
            }
        });

        $('select[name^="IdProveedor"]').on('change', function (e) {

            var $form = $(e.target).closest('form');

            if ($(this).val() == "") {
                $('input[name^=Descripcion]').val("");

                return;
            }

            var idTipoOrdenPago = $('select[name^="IdTipoOrdenPago"]', $form).select2('val')

            switch (parseInt(idTipoOrdenPago)) {
                case enums.TipoOrdenPago.Proveedores:
                case enums.TipoOrdenPago.Gastos:
                    $('input[name^=Descripcion]').val(`PAGO ${$(this).select2('data').text}`).valid();
                    break;
                case enums.TipoOrdenPago.Anticipo:
                    $('input[name^=Descripcion]').val(`ANTICIPO ${$(this).select2('data').text}`).valid();
                    break;
            }
        });

        $('select[name="Detalle.IdTipoPago"]').on('change', function (e) {

            var idTipoPago = $(this).val();

            $('.dvTipoPago').removeClass('show').addClass('hide');
            $('.modalItemsFormaPago .select2-field.validate').removeClass('validate');

            $('input[name="Detalle.Descripcion"]').val($('#Detalle_IdTipoPago :selected').text());
            $('input[name="Detalle.Importe"]').prop('readonly', false);

            switch (parseInt(idTipoPago)) {
                case enums.TipoPago.Efectivo:
                    $('.dvEfectivo').addClass('show');
                    $('.modalItemsFormaPago .dvEfectivo .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.ChequeComun:
                    $('.dvChequeComun').addClass('show');
                    $('.modalItemsFormaPago .dvChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.EChequeComun:
                    $('.dvEChequeComun').addClass('show');
                    $('.modalItemsFormaPago .dvEChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.ChequeDiferido:
                    $('.dvChequeDiferido').addClass('show');
                    $('.modalItemsFormaPago .dvChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.EChequeDiferido:
                    $('.dvEChequeDiferido').addClass('show');
                    $('.modalItemsFormaPago .dvEChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.ChequeTerceros:
                    $('.dvChequeTerceros').addClass('show');
                    $('.modalItemsFormaPago .dvChequeTerceros .select2-field').addClass('validate');
                    $('input[name="Detalle.Importe"]').prop('readonly', true);
                    break;
                case enums.TipoPago.Transferencia:
                    $('.dvTransferencia').addClass('show');
                    $('.modalItemsFormaPago .dvTransferencia .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.CuentaContable:
                    $('.dvCuentaContable').addClass('show');
                    $('.modalItemsFormaPago .dvCuentaContable .select2-field').addClass('validate');
                    break;
            }

            $('.dvImporte').addClass('show');

        });

        $('select[name^=IdMonedaPago]', modalNew).on('change', function (e) {

            var $form = $(e.target).closest('form');

            if ($('input[name^=Fecha]', modalNew).val() == "")
                return;

            if ($('select[name^=IdMonedaPago]', modalNew).val() == "")
                return;

            var params = {
                idMoneda1: $('select[name="IdMonedaPago"]', modalNew).val(),
                fecha: moment($('input[name^=Fecha]', modalNew).val(), 'DD/MM/YYYY').format('YYYY-MM-DD')
            };

            Ajax.Execute('/OrdenesPago/GetFechaCambio/', params)
                .done(function (response) {
                    if (response.status == enums.AjaxStatus.OK) {
                        AutoNumeric.set($('input[name^=Cotizacion]', $form)[0], response.detail || 0);
                    }
                    else {
                        AutoNumeric.set($('input[name^=Cotizacion]', $form)[0], 1);
                    }
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    Utils.modalError('Error', jqXHR.responseJSON.detail);
                });

        });

        $('.btnCargaItems', modalNew).on('click', function (e) {

            var $form = $(e.target).closest('form');
            var dialog = $(e.target).parents('.modal');

            switch (parseInt($('select[name^=IdTipoOrdenPago]', dialog).val())) {
                case enums.TipoOrdenPago.Proveedores:

                    var idProveedor = $('select[name^=IdProveedor]', dialog).select2('val');
                    var idMoneda = $('select[name^=IdMoneda]', dialog).select2('val');
                    var idMonedaPago = $('select[name^=IdMonedaPago]', dialog).val();
                    var cotizacion = AutoNumeric.getNumber($('input[name^=Cotizacion]', dialog)[0]);

                    if (idProveedor == "") {
                        Utils.alertInfo("Seleccione un Proveedor");
                        return false;
                    }

                    if (idMoneda == "") {
                        Utils.alertInfo("Seleccione una Moneda para la selección de Comprobantes");
                        return false;
                    }

                    if (idMonedaPago == "") {
                        Utils.alertInfo("Seleccione una Moneda para el Pago");
                        return false;
                    }

                    if (cotizacion == "") {
                        Utils.alertInfo("Ingrese una Cotización");
                        return false;
                    }

                    var action = '/OrdenesPago/ObtenerComprobantesImputar';
                    var params = {
                        id: idProveedor,
                        IdMoneda: idMoneda,
                        IdMonedaPago: idMonedaPago,
                        Cotizacion: cotizacion
                    };

                    $form.find('.dvImputaciones .repeater-component').Repeater()
                        .clear()

                    Ajax.GetJson(action, params)
                        .done(function (comprobantes) {

                            $form.find('.dvImputaciones .repeater-component').Repeater()
                                .source(comprobantes);
                        })
                        .fail(Ajax.ShowError);


                    var action = '/OrdenesPago/ObtenerAnticiposImputar';
                    var params = {
                        idProveedor: idProveedor,
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

                    break;

                case enums.TipoOrdenPago.Gastos:

                    var idMoneda = $('select[name^=IdMoneda]', dialog).select2('val');
                    var idMonedaPago = $('select[name^=IdMonedaPago]', dialog).val();
                    var cotizacion = AutoNumeric.getNumber($('input[name^=Cotizacion]', dialog)[0]);

                    if (idMoneda == "") {
                        Utils.alertInfo("Seleccione una Moneda para la selección de Comprobantes");
                        return false;
                    }

                    if (idMonedaPago == "") {
                        Utils.alertInfo("Seleccione una Moneda para el Pago");
                        return false;
                    }

                    if (cotizacion == "") {
                        Utils.alertInfo("Ingrese una Cotización");
                        return false;
                    }

                    var action = 'OrdenesPago/ObtenerPlanillasImputar';
                    var params = {
                        IdMoneda: $('select[name^=IdMoneda]', dialog).select2('val'),
                        IdMonedaPago: $('select[name^=IdMonedaPago]', dialog).select2('val'),
                        Cotizacion: cotizacion
                    };

                    $form.find('.dvPlanillaGastos .repeater-component').Repeater()
                        .clear()

                    Ajax.GetJson(action, params)
                        .done(function (planillas) {

                            $form.find('.dvPlanillaGastos .repeater-component').Repeater()
                                .source(planillas);
                        })
                        .fail(Ajax.ShowError);
                    
                    break;

                case enums.TipoOrdenPago.Anticipo:
                    break;
            }

        });

        // #region Eventos Detalle de Items

        $('input[name="Detalle.NumeroCheque"]').on('blur', function (e) {
            var nroCheque = $(this).val().replaceAll("_", "");
            $(this).val(String("0000000000" + nroCheque).slice(-10));
            $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoOrdenPago :selected').text()} ${$(this).val()}`);
        });

        $('input[name="Detalle.ImporteEfectivo"], input[name="Detalle.ImporteCheque"], input[name="Detalle.ImporteTransferencia"], input[name="Detalle.ImporteDocumento"]').on('blur', function (e) {
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

                var action = 'OrdenesPago/GetPrimerChequeDisponible';
                var params = {
                    idBanco: $(this).select2('val'),
                    idTipoCheque: enums.TipoCheque.Comun
                };

                iconLoader($(iconObj), true);

                Ajax.GetJson(action, params)
                    .done(function (cheque) {
                        if (cheque != null) {
                            $('input[name="Detalle.NumeroChequeComun"]').val(cheque.nroCheque);
                            $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoPago :selected').text()} ${cheque.nroCheque}`);
                        }
                        else {
                            $('input[name="Detalle.NumeroChequeComun"]').val('');
                            $('input[name="Detalle.Descripcion"]').val('');
                            Utils.alertInfo("No Existen Cheques Disponibles con esa Configuración");
                        }
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

                var action = 'OrdenesPago/GetPrimerChequeDisponible';
                var params = {
                    idBanco: $(this).select2('val'),
                    idTipoCheque: enums.TipoCheque.Diferido
                };

                iconLoader($(iconObj), true);

                Ajax.GetJson(action, params)
                    .done(function (cheque) {
                        if (cheque != null) {
                            $('input[name="Detalle.NumeroChequeDiferido"]').val(cheque.nroCheque);
                            $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoPago :selected').text()} ${cheque.nroCheque}`);
                        }
                        else {
                            $('input[name="Detalle.NumeroChequeDiferido"]').val('');
                            $('input[name="Detalle.Descripcion"]').val('');
                            Utils.alertInfo("No Existen Cheques Disponibles con esa Configuración");
                        }
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

                var action = 'OrdenesPago/GetPrimerChequeDisponible';
                var params = {
                    idBanco: $(this).select2('val'),
                    idTipoCheque: enums.TipoCheque.EChequeComun
                };

                iconLoader($(iconObj), true);

                Ajax.GetJson(action, params)
                    .done(function (cheque) {
                        if (cheque != null) {
                            $('input[name="Detalle.NumeroEChequeComun"]').val(cheque.nroCheque);
                            $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoPago :selected').text()} ${cheque.nroCheque}`);
                        }
                        else {
                            $('input[name="Detalle.NumeroEChequeComun"]').val('');
                            $('input[name="Detalle.Descripcion"]').val('');
                            Utils.alertInfo("No Existen Cheques Disponibles con esa Configuración");
                        }
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

                var action = 'OrdenesPago/GetPrimerChequeDisponible';
                var params = {
                    idBanco: $(this).select2('val'),
                    idTipoCheque: enums.TipoCheque.EChequeDiferido
                };

                iconLoader($(iconObj), true);

                Ajax.GetJson(action, params)
                    .done(function (cheque) {
                        if (cheque != null) {
                            $('input[name="Detalle.NumeroEChequeDiferido"]').val(cheque.nroCheque);
                            $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoPago :selected').text()} ${cheque.nroCheque}`);
                        }
                        else {
                            $('input[name="Detalle.NumeroEChequeDiferido"]').val('');
                            $('input[name="Detalle.Descripcion"]').val('');
                            Utils.alertInfo("No Existen Cheques Disponibles con esa Configuración");
                        }
                    })
                    .fail(Ajax.ShowError)
                    .always(function () {
                        iconLoader($(iconObj), false);
                    });
            });

        $('select[name="Detalle.IdCuentaContable"]')
            .on('change', function (e) {

                if ($(this).select2('val') == "") {
                    $('input[name="Detalle.Descripcion"]').val('');
                    return;
                }

                $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoPago :selected').text()} ${$('#Detalle_IdCuentaContable :selected').text() }`);
                
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
            .Select2Ex({ url: '/OrdenesPago/GetChequesCartera', allowClear: true, placeholder: 'Buscar...' })
            .on('change', function (e) {

                if (e.added != undefined) {
                    $('input[name="Detalle.NumeroChequeTerceros"]').val(e.added.nroCheque);
                    $('input[name="Detalle.BancoChequeTerceros"]').val(e.added.banco);
                    $('input[name="Detalle.FechaChequeTerceros"]').val(moment(e.added.fecha, 'YYYY-MM-DD').format('DD/MM/YYYY'));
                    $('input[name="Detalle.FechaDiferidoChequeTerceros').val(moment(e.added.fechaVto, 'YYYY-MM-DD').format('DD/MM/YYYY'));
                    AutoNumeric.set($('input[name="Detalle.Importe"]')[0], e.added.importe);

                    $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoOrdenPago :selected').text()} ${e.added.nroCheque}`);
                }
                else {
                    $('input[name="Detalle.NumeroChequeTerceros"]').val("");
                    $('input[name="Detalle.BancoChequeTerceros"]').val("");
                    $('input[name="Detalle.FechaChequeTerceros"]').val("");
                    $('input[name="Detalle.FechaDiferidoChequeTerceros').val("");
                    $('input[name="Detalle.Importe"]').val("");

                    $('input[name="Detalle.Descripcion"]').val(`${$('#Detalle_IdTipoOrdenPago :selected').text()}`);
                }
            });

        // #endregion 

        //Modal de Pagar
        modalFormaPago
            .on('shown.bs.modal', function () {
                $('input[name="IdOrdenPago"]', this).val(dtView.selected()[0].idOrdenPago);
            })
            .on('hidden.bs.modal', function () {
                var form = $('form', this);
                Utils.resetValidator(form);
            });

        modalFormaPagoSave
            .on('click', function () {
                $('form', modalFormaPago).on('submit', function (e) {
                    $(this).find('input[type=text]').each(function () {
                        if (AutoNumeric.isManagedByAutoNumeric(this))
                            AutoNumeric.getAutoNumericElement(this).unformat();
                    });
                });

                $('form', modalFormaPago).submit();
            });

        btPagar.on('click', showPagar);
        //btImprimir.on('click', imprimir);
        btImprimir.on('click', printDialog);

        modalPrint.find('button.btPrintOrdenPago').on('click', imprimir);
        modalPrint.find('button.btPrintGanancias').on('click', imprimirRetencionesGanancias);
        modalPrint.find('button.btPrintIVA').on('click', imprimirRetencionesIVA);
        modalPrint.find('button.btPrintIIBB').on('click', imprimirRetencionesIIBB);
        modalPrint.find('button.btPrintSUSS').on('click', imprimirRetencionesSUSS);

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

        Ajax.Execute('OrdenesPago/VerificarChequeDisponible/', params)
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
        $form.find('#Importe').prop('disabled', true);

        $('input[name^=Fecha]', $form).val(moment().format('DD/MM/YYYY'));
        $('input[name^=Vto]', $form).val('');

        $('.areaImputaciones').hide();
        $('.areaAnticipos').hide();
        $('.areaPlanillaGastos').hide();
        $('.btnCargaItems').hide();
        $('#IdProveedor').closest('div').parent().hide();
        $('#IdCuentaContable').closest('div').parent().hide();
        $('#Importe').closest('.form-group').hide();
        $('#Importe').attr('disabled', true);
        $('#ImporteAnticipo').closest('.form-group').hide();
        $('#ImporteVarios').closest('.form-group').hide();

        $form.find('.dvImputaciones .repeater-component').Repeater()
            .clear();

        $form.find('.dvAnticipos .repeater-component').Repeater()
            .clear();

        $form.find('.dvPlanillaGastos .repeater-component').Repeater()
            .clear();

    }

    function mainTableRowSelected(dataArray, api) {

        btImprimir.prop('disabled', true);
        btPagar.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        btPagar.prop('disabled', !dataArray.length == 1);
        btRemove.prop('disabled', dataArray[0].idEjercicio == null);
        btImprimir.prop('disabled', !dataArray.length == 1);

    }

    function mainTableDraw() {

        var data = dtView.selected();

        btPagar.prop('disabled', true);

        if (data === undefined || data === null)
            return;

        btPagar.prop('disabled', !data.length == 1);

    }

    function renderViewDetails(data, type, row) {

        var btnDetalle = "", btnPago = "", btnAsiento = "";

        btnDetalle = '<i class="far fa-search-plus details" title="Desplegar detalle de Items"></i>';
        btnPago = '<i class="far fa-file-invoice-dollar pagos" title="Desplegar Detalle de Pago"></i>';
        btnAsiento = '<i class="far fa-file-invoice asiento" title="Asiento Contable"></i>';

        return `
            <ul class="table-controls">
                <li>
                    ${btnDetalle}
                </li>
                <li>
                    ${btnPago}
                </li>
                <li>
                    ${btnAsiento}
                </li>
            </ul>`;
    }

    // #region Detalle de Items, Pagos y Asiento Contable

    function formatRowDetail(rowData) {

        if (rowData == null || rowData.imputaciones == null || rowData.imputaciones.length == 0)
            return '<div class="text-center"><span>La Orden de Pago no presenta Comprobantes en el Detalle</span></div>';

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Fecha Real</th>
                            <th>Tipo de Comprobante</th>
                            <th>Comprobante</th>
                            <th>Importe</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@RowsDetalle]
                    </tbody>
                </table>

                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Descripción Anticipo</th>
                            <th>Importe</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@RowsAnticipos]
                    </tbody>
                </table>

                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Fecha</th>
                            <th>Tipo de Retención</th>
                            <th>Nro. Retención</th>
                            <th>Base Imponible</th>
                            <th>Importe</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@RowsRetenciones]
                    </tbody>
                </table>
            </div>`;

        var rowTemplate =
            `<tr>
                <td>[@Fecha]</td>
                <td>[@TipoComprobante]</td>
                <td>[@Comprobante]</td>
                <td>[@Importe]</td>
            </tr>`;

        var rowTemplateAnticipos =
            `<tr>
                <td>[@Descripcion]</td>
                <td>[@Importe]</td>
            </tr>`;

        var rowTemplateRetenciones =
            `<tr>
                <td>[@Fecha]</td>
                <td>[@TipoRetencion]</td>
                <td>[@NroRetencion]</td>
                <td>[@BaseImponible]</td>
                <td>[@Importe]</td>
            </tr>`;

        var rows = '';
        var rowsRetencion = '';
        var rowsAnticipos = '';

        for (var i in rowData.imputaciones) {
            var imputacion = rowData.imputaciones[i];
            var row = rowTemplate
                .replace('[@Fecha]', moment(imputacion.fecha, enums.FormatoFecha.DefaultFullFormat).format(enums.FormatoFecha.DefaultFormat))
                .replace('[@TipoComprobante]', imputacion.tipoComprobante)
                .replace('[@Comprobante]', imputacion.numeroComprobante)
                .replace('[@Importe]', accounting.formatMoney(imputacion.importe));

            rows += row;
        }

        for (var i in rowData.anticipos) {
            var anticipo = rowData.anticipos[i];
            var row = rowTemplateAnticipos
                .replace('[@Descripcion]', anticipo.descripcion)
                .replace('[@Importe]', accounting.formatMoney(anticipo.importe));

            rowsAnticipos += row;
        }

        for (var i in rowData.retenciones) {
            var retencion = rowData.retenciones[i];
            var row = rowTemplateRetenciones
                .replace('[@Fecha]', moment(retencion.fecha, enums.FormatoFecha.DatabaseFullFormat).format(enums.FormatoFecha.DefaultFormat))
                .replace('[@TipoRetencion]', retencion.tipoRetencion)
                .replace('[@NroRetencion]', String("000000" + retencion.nroRetencion).slice(-6))
                .replace('[@BaseImponible]', accounting.formatMoney(retencion.baseImponible))
                .replace('[@Importe]', accounting.formatMoney(retencion.importe));

            rowsRetencion += row;
        }

        return tableTemplate
            .replace('[@RowsDetalle]', rows)
            .replace('[@RowsAnticipos]', rowsAnticipos)
            .replace('[@RowsRetenciones]', rowsRetencion);
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

            Ajax.Execute('/OrdenesPago/Obtener/' + row.data().idOrdenPago, null, null, 'GET')
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

            Ajax.Execute('/OrdenesPago/ObtenerAsiento/' + row.data().idOrdenPago, null, null, 'GET')
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
            return '<div class="text-center"><span>La Orden de Pago no presenta un Asiento Contable</span></div>';

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

    function retrieveComposicionPago() {
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

            Ajax.Execute('/OrdenesPago/ObtenerDetallePago/' + row.data().idOrdenPago, null, null, 'GET')
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowComposicionPagoDetail(data));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }
    }

    function formatRowComposicionPagoDetail(rowData) {

        if (rowData == null || rowData.length == 0)
            return '<div class="text-center"><span>La Orden de Pago no presenta Detalle de Pagos</span></div>';

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Item</th>
                            <th>Detalle</th>
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
                <td>[@Item]</td>
                <td>[@Detalle]</td>
                <td>[@Importe]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData) {
            var item = rowData[i];
            var row = rowTemplate
                .replace('[@Item]', parseInt(i) + 1)
                .replace('[@Detalle]', item.descripcion)
                .replace('[@Importe]', accounting.formatMoney(item.importe));

            rows += row;
        }

        return tableTemplate.replace('[@Rows]', rows);
    }

    // #endregion


    // #region Eventos para Pagar la Orden de Pago

    function showPagar(e) {

        var selectedData = dtView.selected()[0];

        if (selectedData.idEstadoOrdenPago == enums.EstadoOrdenPago.Pagada) {
            Utils.alertInfo("La Orden de Pago se encuentra Pagada");
            return;
        }

        $('.importeOrdenPago', modalFormaPago)
            .text(accounting.formatMoney(selectedData.importe));

        $('.modal .dvItems .master-detail-component', mainClass).MasterDetail()
            .clear()

        modalFormaPago.modal({ backdrop: 'static' });

    }

    this.modalFormaPagoajaxFormBegin = function (context, arguments) {

        var dialog = $(this).parents('.modal');
        var btConfirm = dialog.find('.modal-footer button.btConfirm')

        MaestroLayout.buttonLoader(btConfirm, true, 'Guardando');
    }

    this.modalFormaPagoajaxFormSuccess = function (context) {

        Utils.alertSuccess('Guardado correctamente.');
        $(modalFormaPago).modal('hide');

        dtView.reload();

        var dialog = $(this).parents('.modal');
        var btConfirm = dialog.find('.modal-footer button.btConfirm')

        MaestroLayout.buttonLoader(btConfirm, false);
    }

    this.modalFormaPagoajaxFormFailure = function (context) {

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

    function masterDetailDialogOpeningItem(e, dialog, $form, $item) {

        if ($item != undefined) {

            $('.dvTipoPago').removeClass('show').addClass('hide');
            $('input[name="Detalle.Importe"]').prop('readonly', false);

            switch (parseInt($item.IdTipoPago.value)) {
                case enums.TipoPago.Efectivo:
                    $('.dvEfectivo').addClass('show');
                    $('.modalItemsFormaPago .dvEfectivo .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.ChequeComun:
                    $('.dvChequeComun').addClass('show');
                    $('.modalItemsFormaPago .dvChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.EChequeComun:
                    $('.dvEChequeComun').addClass('show');
                    $('.modalItemsFormaPago .dvEChequeComun .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.ChequeDiferido:
                    $('.dvChequeDiferido').addClass('show');
                    $('.modalItemsFormaPago .dvChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.EChequeDiferido:
                    $('.dvEChequeDiferido').addClass('show');
                    $('.modalItemsFormaPago .dvEChequeDiferido .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.ChequeTerceros:
                    $('.dvChequeTerceros').addClass('show');
                    $('.modalItemsFormaPago .dvChequeTerceros .select2-field').addClass('validate');
                    $('input[name="Detalle.Importe"]').prop('readonly', true);
                    break;
                case enums.TipoPago.Transferencia:
                    $('.dvTransferencia').addClass('show');
                    $('.modalItemsFormaPago .dvTransferencia .select2-field').addClass('validate');
                    break;
                case enums.TipoPago.CuentaContable:
                    $('.dvCuentaContable').addClass('show');
                    $('.modalItemsFormaPago .dvCuentaContable .select2-field').addClass('validate');
                    break;
            }

            $('select[name="Detalle.IdTipoPago"]').prop('disabled', true);
            $('.dvImporte').addClass('show');
        }
        else {
            $('.dvTipoPago').removeClass('show').addClass('hide');
            $('select[name="Detalle.IdTipoPago"]').prop('disabled', false);
            $('.dvImporte').removeClass('show').addClass('hide');
        }

        console.log('MasterDetail Dialog Opening');

    }

    // #endregion


    // #region Eventos Master Detail Items Detalle

    function masterDetailRowAddingItem(e, item, items, $row, $rows) {
        calculaTotalComposicionPago(e)
    }

    function masterDetailRowAddedItem(e, item, items, $row, $rows) {
        calculaTotalComposicionPago(e)
    }

    function masterDetailRowEditedItem(e, item, items, $row, $rows) {
        calculaTotalComposicionPago(e)
    }

    function masterDetailRowRemovedItem(e, item, items, $row, $rows) {
        calculaTotalComposicionPago(e);
    }

    function masterDetailSourceLoadedItem(e, items, $rows) {
        calculaTotalComposicionPago(e)
    }

    function masterDetailEmptyItem(e) {
        calculaTotalComposicionPago(e)
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
        calculaTotalOrdenPago(e);
    }

    function repeaterFieldFocus(e, data) {
        
    }

    function repeaterFieldChange(e, data) {

    }

    function repeaterFieldBlur(e, data) {
        calculaTotalOrdenPago(e);
    }

    // #endregion

    // #region Eventos del Repeater de Planilla de Gastos

    function repeaterPlanillaGastosRowAdded(e, $newRow, $rows) {
        inicializarPlanillaGastosSeleccion(e, $newRow);
    }

    // #endregion

    // #region Eventos del Repeater de Anticipos

    function repeaterAnticiposRowAdded(e, $newRow, $rows) {
        inicializarAnticiposSeleccion(e, $newRow);
        calculaTotalOrdenPago(e);
    }

    function repeaterAnticiposFieldBlur(e, data) {
        calculaTotalOrdenPago(e);
    }

    // #endregion

    // #region Funcion de Totales para la Orden de Pago

    function calculaTotalOrdenPago(e) {

        var total = 0, anticipos = 0;
        var $form = $(e.target).closest('form');

        var trs = $form.find('.dvImputaciones .repeater-component tbody > tr');
        var trsAnticipos = $form.find('.dvAnticipos .repeater-component tbody > tr');

        trs.each(function () {

            var tr = $(this);

            var importe = tr.find('> td > input[name$=".Importe"]');
            var importef = AutoNumeric.isManagedByAutoNumeric(importe.get(0)) ? AutoNumeric.getNumber(importe.get(0)) : $(importe).val();

            total += importef;
        });

        trsAnticipos.each(function () {

            var tr = $(this);

            var anticipo = tr.find('> td > input[name$=".Importe"]');
            var anticipof = AutoNumeric.isManagedByAutoNumeric(anticipo.get(0)) ? AutoNumeric.getNumber(anticipo.get(0)) : $(anticipo).val();

            anticipos += anticipof;
        });



        AutoNumeric.set($form.find('input[name^=Importe]')[0], total - anticipos);

    }

    function calculaTotalComposicionPago(e) {

        var $form = $(e.target).closest('form');

        var total = 0;
        var items = $form.find('.dvItems .master-detail-component').MasterDetail().getItems();

        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            total += parseFloat(item.Importe.value);
        }

        $form.find('.ImporteComposicionPago').text(accounting.formatMoney(total));

    }

    function calculaTotalOrdenPagoPlanillasGastos(e) {

        var $form = $(e.target).closest('form');
        var trs = $form.find('tbody > tr');
        var total = 0;

        trs.each(function () {

            var tr = $(this);

            var checkbox = tr.find('> td > input[type=checkbox][name$=".Seleccionar"]');

            if (checkbox.prop('checked')) {
                var importe = tr.find('> td > input[name$=".Importe"]');
                var importef = AutoNumeric.isManagedByAutoNumeric(importe.get(0)) ? AutoNumeric.getNumber(importe.get(0)) : $(importe).val();

                total += importef;
            }
        });

        AutoNumeric.set($form.find('input[name^=Importe]')[0], total);

    }

    // #endregion

    // #region Funcion para inicializar los checkbox de Selección

    function inicializarPlanillaGastosSeleccion(e, $newRow) {

        var checkbox = $('input[type=checkbox][name$=".Seleccionar"]', $newRow);
        checkbox.on('change', calculaTotalOrdenPagoPlanillasGastos);

    }

    function inicializarSeleccion(e, $newRow) {

        var checkbox = $('input[type=checkbox][name$=".Seleccionar"]', $newRow);
        var importe = $('input[type=text][name$=".Importe"]', $newRow);
        var maxImporte = $('input[type=text][name$=".Saldo"]', $newRow);
        var minImporte = 0;

        maxImporte = AutoNumeric.getNumber(maxImporte[0]);
        if (maxImporte < 0) {
            minImporte = maxImporte;
            maxImporte = 0;
        }

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
                if (AutoNumeric.isManagedByAutoNumeric(importe.get(0))) {
                    AutoNumeric.set(importe.get(0), 0.0);
                }
                else
                    importe.val(0.0);

                checkbox.focus();
            }

        });

    }

    function inicializarAnticiposSeleccion(e, $newRow) {

        var checkbox = $('input[type=checkbox][name$=".Seleccionar"]', $newRow);
        var importe = $('input[type=text][name$=".Importe"]', $newRow);
        var maxImporte = $('input[type=text][name$=".Saldo"]', $newRow);
        var minImporte = 0;

        maxImporte = AutoNumeric.getNumber(maxImporte[0]);
        if (maxImporte < 0) {
            minImporte = maxImporte;
            maxImporte = 0;
        }

        importe.prop('readonly', !checkbox.is(":checked"));

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
                if (AutoNumeric.isManagedByAutoNumeric(importe.get(0))) {
                    AutoNumeric.set(importe.get(0), 0.0);
                }
                else
                    importe.val(0.0);

                checkbox.focus();
            }

        });
    }

    // #endregion

    // #region Print Dialog

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

    // #endregion

});

$(function () {
    ABMOrdenesPagoView.init();
});
