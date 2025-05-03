/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/masterDetail.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMCargaManualView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var itemsComprobante = [];
    var percepcionesComprobante = [];
    var btUploadCSV;
    var modalUploadCSV;


    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        modalUploadCSV = $('.modal.modalUploadCSV', mainClass);
        btUploadCSV = $('button.btUploadMisComprobantes', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }

    function buildControls() {

        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');

        $('.modal .dvItems .master-detail-component', mainClass)
            .on('master-detail:init', masterDetailInitialization)
            .on('master-detail:dialog-opened', masterDetailDialogOpened)
            .on('master-detail:row:added', masterDetailRowAdded)
            .on('master-detail:row:edited', masterDetailRowEdited)
            .on('master-detail:row:removed', masterDetailRowRemoved)
            .on('master-detail:source:loaded', masterDetailSourceLoaded)
            .on('master-detail:empty', masterDetailEmpty)
            .MasterDetail({
                modalSelector: mainClass + ' .modal.modalMasterDetailItems',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });

        $('.modal .dvPercepciones .master-detail-component', mainClass)
            .on('master-detail:init', masterDetailInitialization)
            .on('master-detail:dialog-opened', masterDetailDialogOpened)
            .on('master-detail:row:added', masterDetailPercepRowAdded)
            .on('master-detail:row:edited', masterDetailPercepRowEdited)
            .on('master-detail:row:removed', masterDetailPercepRowRemoved)
            .on('master-detail:source:loaded', masterDetailPercepSourceLoaded)
            .on('master-detail:empty', masterDetailEmpty)
            .MasterDetail({
                modalSelector: mainClass + ' .modal.modalMasterDetailPercep',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'idComprobanteCompra', visible: false },
            { data: 'comprobante', width: '10%' },
            { data: 'numero', width: '10%' },
            { data: 'fecha', width: '5%', render: DataTableEx.renders.date },
            { data: 'proveedor', width: '40%', render: DataTableEx.renders.ellipsis },
            { data: 'moneda', width: '5%' },
            { data: 'idTipoComprobante', orderable: false, searchable: false, class: 'text-center', width: '5%', render: renderTipoComprobante},
            { data: 'subtotal', visible: false },
            {
                data: 'total',
                width: '10%',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { data: 'idEstadoAFIP', orderable: false, searchable: false, class: 'text-center', width: '5%', render: renderEstadoAFIP },
            { data: 'cae', orderable: false, searchable: false, width: '5%', class: 'text-center' },
            { data: 'vencimientoCAE', orderable: false, searchable: false, width: '5%', class: 'text-center', render: DataTableEx.renders.date },
            { data: null, orderable: false, searchable: false, width: '5%', class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'desc']];
        var pageLength = 30;

        dtView = MaestroLayout.initializeDatatable('idComprobanteCompra', columns, order, pageLength);

    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.asiento', retrieveAsientoDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.validar', retrieveValidarComprobante);

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;

        $('input[name^=Numero]', modalNew).alphanum({
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

            $(this).val(suc + '-' + nro);
        });

        $('select[name^=IdMonedaComprobante]', modalNew).on('change', function (e) {

            if ($(this).val() == "" || $(this).val() == undefined)
                $('#Moneda', modalNew).val("");
            else
                $('#Moneda', modalNew).val($(this).val());
        });

        $('#IdTipoInterfaz').on('change', function (e) {

            switch (parseInt($(this).val())) {
                case enums.TipoInterfaz.MisComprobantes:
                    $('#dvConteinerMisComp').show();
                    break;
                case enums.TipoInterfaz.DawaSoft:
                    $('#dvConteinerMisComp').hide();
                    break;
            }

        });

        $('select[name^=IdProveedor]', modalNew).on('change', function (e) {

            var dialog = $(e.target).parents('.modal');

            if ($(this).val() == "" || $(this).val() == undefined) {
                $('select[name^=IdComprobante]', dialog).select2('val', null).empty();
                return;
            }

            
            var params = {
                idProveedor: $(this).select2('val')
            };

            Ajax.Execute('/CargaManual/GetComprobantesByProveedor/', params)
                .done(function (response) {
                    $('select[name^=IdComprobante]', dialog).Select2Ex().source(response.detail.map(function (obj) { return { id: obj.idComprobante, text: obj.descripcion }; }));
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    Utils.modalError('Error', jqXHR.responseJSON.detail);
                });
        });

        $('select[name^= IdMonedaComprobante]', modalNew).on('change', function (e) {

            var $form = $(e.target).closest('form');

            if ($('input[name=Fecha]', modalNew).val() == "")
                return;

            if ($('select[name^=IdMonedaComprobante]', modalNew).val() == "")
                return;

            var params = {
                idMoneda1: $('select[name="IdMonedaComprobante"]', modalNew).val(),
                fecha: moment($('input[name=Fecha]', modalNew).val(), 'DD/MM/YYYY').format('YYYY-MM-DD')
            };

            Ajax.Execute('/CargaManual/GetFechaCambio/', params)
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

        $('input[name^=Fecha]', modalNew)
            .inputmask("99/99/9999")
            .flatpickr({
                locale: "es",
                defaultDate: "today",
                allowInput: true,
                dateFormat: "d/m/Y",
                onClose: function (selectedDates, dateStr, instance) {
                    if (dateStr == "")
                        instance.setDate(moment().format(enums.FormatoFecha.DefaultFormat));
                    else
                        instance.setDate(dateStr);
                },
                onChange: function (selectedDates, dateStr, e) {

                    if (dateStr == "")
                        return;

                    if ($('select[name^=IdMonedaComprobante]', modalNew).val() == "")
                        return;

                    var params = {
                        idMoneda1: $('select[name="IdMonedaEstimado"]', modalNew).val(),
                        idMoneda2: $('select[name="IdMonedaComprobante"]', modalNew).val(),
                        fecha: moment(dateStr, 'DD/MM/YYYY').format('YYYY-MM-DD')
                    };

                    Ajax.Execute('/CargaManual/GetFechaCambio/', params)
                        .done(function (response) {
                            if (response.status == enums.AjaxStatus.OK) {
                                AutoNumeric.set($('input[name^=Cotizacion]', modalNew)[0], response.detail);
                            }
                            else {
                                AutoNumeric.set($('input[name^=Cotizacion]', modalNew)[0], 1);
                            }

                        })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            Utils.modalError('Error', jqXHR.responseJSON.detail);
                        });
                }

            });

        $('select[name^=IdCondicion]', modalNew).on('change', function (e) {

            if ($(this).val() && $('input[name^=Fecha]', modalNew).val()) {

                var params = {
                    idCondicion: $(this).val(),
                    fecha: $('input[name^=Fecha]', modalNew).val()
                };

                Ajax.Execute('/CargaManual/GetVencimiento/', params)
                    .done(function (response) {

                        if (response.status == enums.AjaxStatus.OK) {
                            $('input[name^=Vto]', modalNew).val(moment(response.detail).format('DD/MM/YYYY'));
                        }
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        Utils.modalError('Error', jqXHR.responseJSON.detail);
                    });
            }
        });

        $('select[name^=IdCondicion]', modalEdit).on('change', function (e) {

            if ($(this).val() && $('input[name^=Fecha]', modalEdit).val()) {

                var params = {
                    idCondicion: $(this).val(),
                    fecha: $('input[name^=Fecha]', modalEdit).val()
                };

                Ajax.Execute('/CargaManual/GetVencimiento/', params)
                    .done(function (response) {

                        if (response.status == enums.AjaxStatus.OK) {
                            $('input[name^=Vto]', modalEdit).val(moment(response.detail).format('DD/MM/YYYY'));
                        }
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        Utils.modalError('Error', jqXHR.responseJSON.detail);
                    });
            }
        });

        btUploadCSV.on('click', function () {

            var idEjercicio = $('#IdEjercicioInterfaz option:eq(1)').val()
            $('#IdEjercicioInterfaz').select2('val', idEjercicio);

            var idCuentaContable = $('#IdCuentaContable option:eq(1)').val()
            $('#IdCuentaContable').select2('val', idCuentaContable);

            $('input[name^=FechaInterfaz]').val(moment().format(enums.FormatoFecha.DefaultFormat));

            $('#FileCSV').val('');
            $('#lblFileCSV').text('').hide();
            $('.btOkProcesarCSV').prop('disabled', true);

            modalUploadCSV.modal({ backdrop: 'static', keyboard: false, modalOverflow: true });
        });

        $('#FileCSV').on('change', async function (e) {

            var fE = document.getElementById('FileCSV');
            console.log('Selected file: ' + fE.files.item(0).name);

            $('#lblFileCSV').text(fE.files.item(0).name).show();
            $('.btOkProcesarCSV').prop('disabled', false);
        });
    }

    function newDialogOpening(dialog, $form) {

        var idEjercicio = $form.find('#IdEjercicio option:eq(1)').val()
        $form.find('#IdEjercicio').select2('val', idEjercicio);

        $('input[name^=Fecha]', $form).val(moment().format(enums.FormatoFecha.DefaultFormat));
        $('input[name^=FechaReal], input[name^=Vto], input[name^=VenciminetoCAE]', $form).val('');

        $form.find('.dvItems .master-detail-component').MasterDetail()
            .clear();

        $form.find('.dvPercepciones .master-detail-component').MasterDetail()
            .clear();

        limpiaTotalesComprobante();

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdComprobanteCompra_Edit').val(entity.idComprobanteCompra);
        $form.find('#IdEjercicio_Edit').select2('val', entity.idEjercicio);
        $form.find('#FechaReal_Edit').val(moment(entity.fechaReal).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#Fecha_Edit').val(moment(entity.fecha).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#IdMonedaComprobante_Edit').select2('val', entity.moneda);
        AutoNumeric.set($form.find('#Cotizacion_Edit')[0], entity.cotizacion);
        $form.find('#Vto_Edit').val(moment(entity.fechaVto).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#IdProveedor_Edit').select2('val', entity.idProveedor);
        $form.find('#IdCentroCosto_Edit').select2('val', entity.idCentroCosto);
        $form.find('#IdComprobante_Edit').select2('data', { id: entity.idComprobante, text: entity.comprobante });
        $form.find('#IdCondicion_Edit').select2('val', entity.idCondicion);
        $form.find('#NumeroComprobante_Edit').val(`${entity.sucursal}-${entity.numero}`);
        $form.find('#CAE_Edit').val(entity.cae);
        if (entity.vencimientoCAE != "")
            $form.find('#VenciminetoCAE_Edit').val(moment(entity.vencimientoCAE).format(enums.FormatoFecha.DefaultFormat));

        $form.find('.dvItems .master-detail-component').MasterDetail()
            .source(entity.items);

        if (entity.listaPercepciones)
            $form.find('.dvPercepciones .master-detail-component').MasterDetail()
                .source(entity.listaPercepciones);

        calculaTotalesComprobante();

    }

    function renderViewDetails(data, type, row) {

        var btnDetalle = "", btnAsiento = "", btnValidate = "";

        btnValidate = '<i class="far fa-file-check validar" title="Validar Comprobante"></i>';
        btnDetalle = '<i class="far fa-search-plus details" title="Desplegar detalle de Items"></i>';
        if (row.idTipoComprobante != enums.TipoComprobante.GastosProveedores)
            btnAsiento = '<i class="far fa-file-invoice asiento" title="Asiento Contable"></i>';
        
        return `
            <ul class="table-controls">
                <li>
                    ${btnValidate}
                </li>
                <li>
                    ${btnDetalle}
                </li>
                <li>
                    ${btnAsiento}
                </li>
            </ul>`;
    }

    function renderEstadoAFIP(data, type, row) {

        switch (data) {
            case enums.EstadoAFIP.Inicial:
                return `<i class="far fa-question-circle amarillo" title="Comprobante Pendiente de Validación"></i>`;
                break;
            case enums.EstadoAFIP.Observado:
                return `<i class="far fa-exclamation-circle azul" title="Comprobante Observado"></i>`;
                break;
            case enums.EstadoAFIP.Error:
                return `<i class="far fa-times-circle rojo" title="Comprobate con Error"></i>`;
                break;
            case enums.EstadoAFIP.Autorizado:
                return `<i class="far fa-check-circle verde" title="Comprobate Verificado"></i>`;
                break;
            case enums.EstadoAFIP.DocumentoSinCAE:
                return `<i class="far fa-check-circle verde" title="Documento Sin C.A.E."></i>`;
                break;
        }
    }

    function renderTipoComprobante(data, type, row) {

        switch (data) {
            case enums.TipoComprobante.ManualProveedores:
                return `<span class="badge badge-danger">Manual</span>`;
                break;
            case enums.TipoComprobante.GastosProveedores:
                return `<span class="badge badge-secondary">Gastos</span>`;
                break;
            case enums.TipoComprobante.InterfazAFIP:
                return `<span class="badge badge-info">Interfaz</span>`;
                break;
            default:
                return `<span class="badge badge-warning">Manual</span>`;
                break;
        }

    }

    function formatRowDetail(rowData) {

        if (rowData == null || rowData.items == null || rowData.items.length == 0)
            return '<div class="text-center"><span>El Comprobante no presenta Items en el Detalle</span></div>';

        var templateResult = "";

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Item</th>
                            <th>Descripción</th>
                            <th>Importe</th>
                            <th>Impuestos</th>
                            <th>Alícuota</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@Rows]
                    </tbody>
                </table>
            </div>`;

        var tableTemplatePagos =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Id Orden Pago</th>
                            <th>Fecha</th>
                            <th>Descripción</th>
                            <th>Importe</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@RowsDetalleOrdenPago]
                    </tbody>
                </table>`;

        var rowTemplate =
            `<tr>
                <td>[@Item]</td>
                <td>[@Descripcion]</td>
                <td>[@Importe]</td>
                <td>[@Impuestos]</td>
                <td>[@Alicuota]</td>
            </tr>`;

        var rowTemplatePago =
            `<tr>
                <td>[@IdOrdenPago]</td>
                <td>[@Fecha]</td>
                <td>[@Descripcion]</td>
                <td>[@Importe]</td>
            </tr>`;

        var rows = '';
        var rowsPagos = '';

        for (var i in rowData.items) {
            var item = rowData.items[i];
            var row = rowTemplate
                .replace('[@Item]', item.item)
                .replace('[@Descripcion]', item.descripcion)
                .replace('[@Importe]', accounting.formatMoney(item.importe))
                .replace('[@Impuestos]', accounting.formatMoney(item.impuestos))
                .replace('[@Alicuota]', accounting.formatMoney(item.alicuota, { symbol: '%', format: '%v %s' }));

            rows += row;
        }

        templateResult += tableTemplate.replace('[@Rows]', rows);

        if (rowData.pagos.length > 0) {
            for (var i in rowData.pagos) {
                var pago = rowData.pagos[i];
                var row = rowTemplatePago
                    .replace('[@IdOrdenPago]', pago.idOrdenPago)
                    .replace('[@Fecha]', moment(pago.fecha).format(enums.FormatoFecha.DefaultFormat))
                    .replace('[@Descripcion]', pago.descripcion)
                    .replace('[@Importe]', accounting.formatMoney(pago.importe));

                rowsPagos += row;
            }

            templateResult += tableTemplatePagos.replace('[@RowsDetalleOrdenPago]', rowsPagos);
        }

        return templateResult;
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

            Ajax.Execute('/CargaManual/ObtenerItems/' + row.data().idComprobanteCompra, null, null, 'GET')
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

            Ajax.Execute('/CargaManual/ObtenerAsiento/' + row.data().idComprobanteCompra, null, null, 'GET')
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

    function retrieveValidarComprobante() {
        var tr = $(this).closest('tr');
        var row = dtView.api().row(tr);

        var params = {
            idComprobanteCompra: row.data().idComprobanteCompra
        };

        iconLoader($(tr.prevObject), true);

        Ajax.Execute('/CargaManual/ValidarComprobantesById/', params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {
                        if (data.error)
                            Utils.modalError("Error", data.error);
                        if (data.observacion)
                            Utils.modalError("Error", data.observacion);
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

    function masterDetailInitialization() {
        console.log('MasterDetail inicializado');
    }

    function masterDetailDialogOpened(e, dialog, $form) {
        console.log('MasterDetail Dialog Opened');
    }

    function masterDetailRowAdded(e, item, items, $row, $rows) {
        console.log('MasterDetail item agregado', item);

        var itemVal = 0;
        if (item.Item == undefined) {
            var $rowLast = $rows.eq($rows.length - 2);
            var $cellItemValue = $rowLast.find('> td span.master-detail-cell[name$=".Item"]');
            itemVal = parseInt($cellItemValue.text()) + 1;
        }
        else {
            itemVal = item.Item.value;
        }

        var $row = $rows.eq($rows.length - 1);
        var $cell = $row.find('> td input.master-detail-cell-value[name$=".Item"]');
        var $cellValue = $row.find('> td span.master-detail-cell[name$=".Item"]');

        $cell.val(itemVal);
        $cellValue.text(itemVal);

        itemsComprobante = items;
        calculaTotalesComprobante();
    }

    function masterDetailRowEdited(e, item, items, $row, $rows) {
        console.log('MasterDetail item editado', item);
        itemsComprobante = items;
        calculaTotalesComprobante();
    }

    function masterDetailRowRemoved(e, item, items, $row, $rows) {
        console.log('MasterDetail item eliminado', item, items);
        itemsComprobante = items;
        calculaTotalesComprobante();
    }

    function masterDetailSourceLoaded(e, items, $rows) {
        console.log('MasterDetail lista de items cargada', items, $rows);
        itemsComprobante = items;
        calculaTotalesComprobante();
    }

    function masterDetailEmpty(e) {
        console.log('MasterDetail items eliminados');
    }

    function masterDetailPercepRowAdded(e, item, items, $row, $rows) {
        console.log('MasterDetail item agregado', item);

        var itemVal = 0;
        if (item.Item == undefined) {
            var $rowLast = $rows.eq($rows.length - 2);
            var $cellItemValue = $rowLast.find('> td span.master-detail-cell[name$=".Item"]');
            itemVal = parseInt($cellItemValue.text()) + 1;
        }
        else {
            itemVal = item.Item.value;
        }

        var $row = $rows.eq($rows.length - 1);
        var $cell = $row.find('> td input.master-detail-cell-value[name$=".Item"]');
        var $cellValue = $row.find('> td span.master-detail-cell[name$=".Item"]');

        $cell.val(itemVal);
        $cellValue.text(itemVal);

        percepcionesComprobante = items;
        calculaTotalesComprobante();
    }

    function masterDetailPercepRowEdited(e, item, items, $row, $rows) {
        console.log('MasterDetail item editado', item);
        percepcionesComprobante = items;
        calculaTotalesComprobante();
    }

    function masterDetailPercepRowRemoved(e, item, items, $row, $rows) {
        console.log('MasterDetail item eliminado', item, items);
        percepcionesComprobante = items;
        calculaTotalesComprobante();
    }

    function masterDetailPercepSourceLoaded(e, items, $rows) {
        console.log('MasterDetail lista de items cargada', items, $rows);
        percepcionesComprobante = items;
        calculaTotalesComprobante();
    }

    function limpiaTotalesComprobante() {
        $('.impuestos').hide();
        $('.subtotal').text(`${accounting.formatMoney(0)}`);
        $('.percepciones').text(`${accounting.formatMoney(0)}`);
        $('.total').text(`${accounting.formatMoney(0)}`);
    }

    function calculaTotalesComprobante() {

        $('.impuestos').hide();

        var subtotal = 0, impuestos = 0, percepciones = 0, total = 0;
        var impuestos = itemsComprobante.map(function (obj) { return { IdAlicuota: obj.IdAlicuota.value, Alicuota: obj.IdAlicuota.text, Importe: 0 }; });
        impuestos = impuestos.filter((value, index, self) => self.findIndex((m) => m.IdAlicuota === value.IdAlicuota) === index);

        for (var i = 0; i < itemsComprobante.length; i++) {

            var item = itemsComprobante[i];
            subtotal += parseFloat(item.Importe.value);

            var impuesto = $.grep(impuestos, function (n, i) {
                return n.IdAlicuota == item.IdAlicuota.value;
            });
            if (impuesto[0] != undefined)
                impuesto[0].Importe += parseFloat(item.Importe.value) * (parseFloat(item.IdAlicuota.text.replace(',', '.')) / 100);
        }

        for (var i = 0; i < percepcionesComprobante.length; i++) {

            var percepcion = percepcionesComprobante[i];
            percepciones += parseFloat(percepcion.Importe.value);

        }

        total = subtotal + percepciones;

        for (var i = 0; i < impuestos.length; i++) {
            $('[class="text_iva_' + impuestos[i].IdAlicuota + '"]').parent().show();
            $('[class="iva_' + impuestos[i].IdAlicuota + '"]').text(`${accounting.formatMoney(impuestos[i].Importe)}`);
            $('[class="iva_' + impuestos[i].IdAlicuota + '"]').parent().show();

            total += impuestos[i].Importe;
        }

        $('.subtotal').text(`${accounting.formatMoney(subtotal)}`);
        $('.percepciones').text(`${accounting.formatMoney(percepciones)}`);
        $('.total').text(`${accounting.formatMoney(total)}`);
        
    }

    this.ajaxProcesarCSVBegin = function () {
        Utils.modalLoader('Procesando...');
    }

    this.ajaxProcesarCSVSuccess = function (response) {
        Utils.modalClose();

        modalUploadCSV.modal('hide');

        if (response.status == 'OK') {
            if (response.detail) {

                if (response.detail.length > 0) {

                    var htmlResult = "";
                    for (const info of response.detail)
                        htmlResult += `<li class="list-group-item">${info}</li>`;

                    const wrapper = document.createElement('div');
                    wrapper.classList.add("swal-li-result");
                    wrapper.innerHTML = `<ul class="list-group">${htmlResult}</ul>`;

                    swal({
                        title: "Error en la Carga de Comprobantes. Detalle:",
                        allowOutsideClick: false,
                        allowEscapeKey: false,
                        content: wrapper,
                        icon: "error",
                        button: "Aceptar"
                    });
                }
                else {
                    Utils.alertSuccess("El Proceso de Upload de Mis Comprobantes AFIP se Termino Satisfactoriamente.");
                }

                dtView.reload();
            }
            else {
                Utils.alertError('Error al Procesar el archivo CSV Mis Comprobantes.');
            }
        } else {
            Utils.alertError(response.detail);
        }
    }

    this.ajaxProcesarCSVFailure = function (jqXHR, textStatus, errorThrown) {
        Utils.modalClose();
        Ajax.ShowError(jqXHR, textStatus, errorThrown);
    }

});

$(function () {
    ABMCargaManualView.init();
});
