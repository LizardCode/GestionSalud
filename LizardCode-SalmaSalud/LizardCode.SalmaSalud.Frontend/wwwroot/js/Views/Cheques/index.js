/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMChequesView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var btDebitar;


    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        modalDebitar = $('.modal.modalDebitar', mainClass);

        btDebitar = $('.toolbar-actions button.btDebitar', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }


    function buildControls() {

        modalDebitar.find('> .modal-dialog').addClass('modal-80');

        MaestroLayout.errorTooltips = false;

        $('.dvChequesADebitar .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
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

        var columns = [
            { data: 'idCheque' },
            { data: 'idTipoCheque', visible: false },
            { data: 'tipoCheque', render: renderTipoCheque },
            { data: 'nroCheque' },
            { data: 'fechaEmision', render: DataTableEx.renders.date },
            { data: 'fechaVto', render: DataTableEx.renders.date },
            { data: 'banco' },
            { data: 'idEstadoCheque', visible: false },
            { data: 'idAsiento', visible: false },
            { data: 'estadoCheque', render: renderEstadoCheque },
            { data: 'importe', render: DataTableEx.renders.currency },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[3, 'asc']];
        var pageLength = 30;
        var multipleSelection = true;

        dtView = MaestroLayout.initializeDatatable('idCheque', columns, order, pageLength, multipleSelection);

    }

    function renderViewDetails(data, type, row) {

        var btnReversar = "", btnAsiento = "", btnDetalle = "";
        
        btnReversar = '<i class="far fa-history reverse" title="Reversar Debito"></i>';
        btnAsiento = '<i class="far fa-file-invoice asiento" title="Asiento Contable"></i>';

        btnDetalle = '<i class="far fa-search-plus details" title="Desplegar detalle del Cheque"></i>';

        if (row.idAsiento != null) {
            return `
                <ul class="table-controls">
                    <li>
                        ${btnDetalle}
                    </li>
                    <li>
                        ${btnReversar}
                    </li>
                    <li>
                        ${btnAsiento}
                    </li>
                </ul>`;
        }

        return `
                <ul class="table-controls">
                    <li>
                        ${btnDetalle}
                    </li>
                </ul>`;
    }

    function renderTipoCheque(data, type, row) {
        return '<span class="badge badge-dark"> ' + data + ' </span>';
    }

    function renderEstadoCheque(data, type, row) {
        switch (row.idEstadoCheque) {
            case enums.EstadoCheque.SinLibrar:
                return '<span class="badge badge-success"> Sin Librar </span>';
                break;
            case enums.EstadoCheque.Librado:
                return '<span class="badge badge-warning"> Librado </span>';
                break;
            case enums.EstadoCheque.DebitadoDepositado:
                return '<span class="badge badge-success"> Debitado/Depositado </span>';
                break;
            case enums.EstadoCheque.Anulado:
                return '<span class="badge badge-danger"> Anulado </span>';
                break;
            case enums.EstadoCheque.Rechazado:
                return '<span class="badge badge-danger"> Rechazado </span>';
                break;
            case enums.EstadoCheque.DebitadoRechazado:
                return '<span class="badge badge-danger"> Debitado / Rechazado </span>';
                break;
            case enums.EstadoCheque.EnCartera:
                return '<span class="badge badge-dark"> En Cartera </span>';
                break;
            case enums.EstadoCheque.Entregado:
                return '<span class="badge badge-dark"> Entregado </span>';
                break;
        }
    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.reverse', reverseRow);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.asiento', retrieveAsientoDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);

        $('#IdBanco, #IdTipoCheque').on('change', function (e) {
            $('#NroDesde, #NroHasta').val('');
        });

        $('#NroDesde').on('change', function (e) {
            if ($(this).val() == "")
                return;
            var desde = "0000000000" + $(this).val();
            $(this).val(desde.substr(desde.length - 10));
        });

        $('#NroHasta').on('change', function (e) {
            if ($(this).val() == "")
                return;
            var hasta = "0000000000" + $(this).val();
            $(this).val(hasta.substr(hasta.length - 10));
        });

        $('select[name^="IdBancoDebitar"]', modalDebitar).on('change', function (e) {
            var $form = $(e.target).closest('form');

            $form.find('.dvChequesADebitar .repeater-component').Repeater()
                .clear();

            if ($(this).val() == "")
                return;

            var idBanco = $(this).val();

            var action = 'Cheques/ObtenerChequesADebitar';
            var params = {
                idBanco: idBanco
            };

            Ajax.GetJson(action, params)
                .done(function (cheques) {

                    $form.find('.dvChequesADebitar .repeater-component').Repeater()
                        .source(cheques);
                })
                .fail(Ajax.ShowError);


        });

        $('.btConfirm', modalDebitar)
            .on('click', function () {
                $('form', modalDebitar).on('submit', function (e) {
                    $(this).find('input[type=text]').each(function () {
                        if (AutoNumeric.isManagedByAutoNumeric(this))
                            AutoNumeric.getAutoNumericElement(this).unformat();
                    });
                });

                $('form', modalDebitar).submit();
            });

        btDebitar.on('click', showDebitar);
    }

    function editDialogOpening($form, entity) {

        $form.find('#IdCheque_Edit').val(entity.idCheque);
        $form.find('#IdBanco_Edit').select2('val', entity.idBanco);
        $form.find('#IdTipoCheque_Edit').select2('val', entity.idTipoCheque);
        $form.find('#NroDesde_Edit').val(entity.nroDesde);
        $form.find('#NroHasta_Edit').val(entity.nroHasta);

    }

    function reverseRow() {
        var tr = $(this).closest('tr');
        var row = dtView.api().row(tr);

        var params = {
            idCheque: row.data().idCheque
        };

        iconLoader($(tr.prevObject), true);

        Ajax.Execute('/Cheques/ReverseById/', params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {
                        dtView.reload();
                    },
                    Ajax.ShowError
                );
            })
            .fail(Ajax.ShowError)
            .always(function () {
                iconLoader($(tr.prevObject), false);
            });
    }

    function iconLoader(btAction, flag) {

        if (flag) {
            btAction
                .data('normal-icons', btAction.attr('class'))
                .prop('disabled', true)
                .prop('class', 'far fa-cog fa-spin');
        }
        else {
            btAction
                .prop('disabled', false)
                .prop('class', btAction.data('normal-icons'));
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

            Ajax.Execute('/Cheques/ObtenerAsiento/' + row.data().idCheque, null, null, 'GET')
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
            return '<div class="text-center"><span>El Cheque no presenta un Asiento Contable</span></div>';

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Item</th>
                            <th>Fecha</th>
                            <th>Código de Contable</th>
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
                <td>[@IdCuentaContable]</td>
                <td>[@CuentaContable]</td>
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
                .replace('[@IdCuentaContable]', item.codigo)
                .replace('[@CuentaContable]', item.cuenta)
                .replace('[@Detalle]', item.detalle)
                .replace('[@Debitos]', item.debitos == 0 ? "" : accounting.formatMoney(item.debitos))
                .replace('[@Creditos]', item.creditos == 0 ? "" : accounting.formatMoney(item.creditos));

            rows += row;
        }

        return tableTemplate.replace('[@Rows]', rows);
    }

    function showDebitar(e) {

        $('.dvChequesADebitar .repeater-component').Repeater()
            .clear();

        $('#IdBancoDebitar').select2('val', null);
        var idEjercicio = $('#IdEjercicio option:eq(1)').val();
        $('#IdEjercicio').select2('val', idEjercicio);
        $('#Fecha').val(moment().format(enums.FormatoFecha.DefaultFormat));

        modalDebitar.modal({ backdrop: 'static' });

    }

    // #region Eventos del Repeater de Cheques a Debitar

    function repeaterInitialization() {
        console.log('repeater iniciado');
    }

    function repeaterImputacionesRowRemoved(e, $removedRow, $rows) {
    }

    function repeaterFieldFocus(e, data) {

    }

    function repeaterFieldChange(e, data) {

    }

    function repeaterFieldBlur(e, data) {

    }

    // #endregion

    this.modalDebitarajaxFormBegin = function () {
        Utils.modalLoader('Procesando...');
    }

    this.modalDebitarajaxFormSuccess = function (response) {
        Utils.modalClose();

        modalDebitar.modal('hide');

        if (response.status == 'OK') {
            Utils.alertSuccess('Cheques debitados correctamente.');
            dtView.reload();
        }
        else {
            Utils.alertError(response.detail);
        }
    }

    this.modalDebitarajaxFormFailure = function (jqXHR, textStatus, errorThrown) {
        Utils.modalClose();
        Ajax.ShowError(jqXHR, textStatus, errorThrown);
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

            Ajax.Execute('/Cheques/Detalle/' + row.data().idCheque, null, null, 'GET')
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowDetail(data, row.data().idTipoCheque, row.data().idEstadoCheque));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }
    }

    function formatRowDetail(rowData, tipoCheque, estadoCheque) {

        switch (tipoCheque) {
            case enums.TipoCheque.Diferido:
            case enums.TipoCheque.Comun:
                switch (estadoCheque) {
                    case enums.EstadoCheque.SinLibrar:
                        return '<div class="text-center"><span>El Cheque esta <strong>Sin Librar.</strong></span></div>';
                        break;
                    case enums.EstadoCheque.Entregado:
                    case enums.EstadoCheque.Librado:
                        var tableCheque =
                            `<div class="table-responsive">
                                <table class="table table-bordered mb-4">
                                    <thead>
                                        <tr>
                                            <th>Orden de Pago N°</th>
                                            <th>Proveedor</th>
                                            <th>CUIT</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        [@Rows]
                                    </tbody>
                                </table>
                            </div>`;

                        var rowTemplate =
                            `<tr>
                                <td>[@OrdenPago]</td>
                                <td>[@Proveedor]</td>
                                <td>[@CUIT]</td>
                            </tr>`;

                        var rows = '';
                        for (var i in rowData) {
                            var item = rowData[i];
                            var row = rowTemplate
                                .replace('[@OrdenPago]', item.IdOrdenPago)
                                .replace('[@Proveedor]', item.Proveedor)
                                .replace('[@CUIT]', item.CUIT);

                            rows += row;
                        }

                        return tableCheque.replace('[@Rows]', rows);

                        break;
                    case enums.EstadoCheque.DebitadoDepositado:
                        return '<div class="text-center"><span>El Cheque fue Debitado por la Entidad Bancaria.</span></div>';
                        break;
                    case enums.EstadoCheque.Anulado:
                        return '<div class="text-center"><span>El Cheque fue <strong>Anulado.</strong></span></div>';
                        break;
                    case enums.EstadoCheque.Rechazado:
                        return '<div class="text-center"><span>El Cheque fue Rechazado por la Entidad Bancaria.</span></div>';
                        break;
                    case enums.EstadoCheque.DebitadoRechazado:
                        return '<div class="text-center"><span>El Cheque fue Debitado y Rechazado por la Entidad Bancaria.</span></div>';
                        break;                    
                }
                break;
            case enums.TipoCheque.Terceros:
                switch (estadoCheque) {
                    case enums.EstadoCheque.SinLibrar:
                        return '<div class="text-center"><span>El Cheque esta <strong>Sin Librar.</strong></span></div>';
                        break;
                    case enums.EstadoCheque.Librado:
                        break;
                    case enums.EstadoCheque.DebitadoDepositado:
                        var tableCheque =
                            `<div class="table-responsive">
                                <table class="table table-bordered mb-4">
                                    <thead>
                                        <tr>
                                            <th>Deposito N°</th>
                                            <th>Banco</th>
                                            <th>CUIT</th>
                                            <th>Nro. de Cuenta</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        [@Rows]
                                    </tbody>
                                </table>
                            </div>`;

                        var rowTemplate =
                            `<tr>
                                <td>[@IdDepositoBanco]</td>
                                <td>[@Banco]</td>
                                <td>[@CUIT]</td>
                                <td>[@NroCuenta]</td>
                            </tr>`;

                        var rows = '';
                        for (var i in rowData) {
                            var item = rowData[i];
                            var row = rowTemplate
                                .replace('[@IdDepositoBanco]', item.IdDepositoBanco)
                                .replace('[@Banco]', item.Banco)
                                .replace('[@CUIT]', item.CUIT)
                                .replace('[@NroCuenta]', item.NroCuenta);

                            rows += row;
                        }

                        return tableCheque.replace('[@Rows]', rows);
                        break;
                    case enums.EstadoCheque.Anulado:
                        return '<div class="text-center"><span>El Cheque fue <strong>Anulado.</strong></span></div>';
                        break;
                    case enums.EstadoCheque.Rechazado:
                        return '<div class="text-center"><span>El Cheque fue Rechazado por la Entidad Bancaria.</span></div>';
                        break;
                    case enums.EstadoCheque.DebitadoRechazado:
                        return '<div class="text-center"><span>El Cheque fue Debitado y Rechazado por la Entidad Bancaria.</span></div>';
                        break;
                    case enums.EstadoCheque.EnCartera:
                        return '<div class="text-center"><span>El Cheque se encuentra <strong>En Cartera</strong> para ser Depositado o Entregado.</span></div>';
                        break;
                    case enums.EstadoCheque.Entregado:
                        var tableCheque =
                            `<div class="table-responsive">
                                <table class="table table-bordered mb-4">
                                    <thead>
                                        <tr>
                                            <th>Orden de Pago N°</th>
                                            <th>Proveedor</th>
                                            <th>CUIT</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        [@Rows]
                                    </tbody>
                                </table>
                            </div>`;

                        var rowTemplate =
                            `<tr>
                                <td>[@OrdenPago]</td>
                                <td>[@Proveedor]</td>
                                <td>[@CUIT]</td>
                            </tr>`;

                        var rows = '';
                        for (var i in rowData) {
                            var item = rowData[i];
                            var row = rowTemplate
                                .replace('[@OrdenPago]', item.IdOrdenPago)
                                .replace('[@Proveedor]', item.Proveedor)
                                .replace('[@CUIT]', item.CUIT);

                            rows += row;
                        }

                        return tableCheque.replace('[@Rows]', rows);
                        break;
                }
                break;
        }


    }
});

$(function () {
    ABMChequesView.init();
});
