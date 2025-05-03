/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/masterDetail.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMFacturacionArticulosView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var btImprimir;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);

        btImprimir = $('.toolbar-actions button.btImprimir', mainClass);
        btImprimir.prop('disabled', true);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }

    function buildControls() {

        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');

        $('.modal .master-detail-component', mainClass)
            .on('master-detail:init', masterDetailInitialization)
            .on('master-detail:dialog-opened', masterDetailDialogOpened)
            .on('master-detail:row:added', masterDetailRowAdded)
            .on('master-detail:row:edited', masterDetailRowEdited)
            .on('master-detail:row:removed', masterDetailRowRemoved)
            .on('master-detail:source:loaded', masterDetailSourceLoaded)
            .on('master-detail:empty', masterDetailEmpty)
            .MasterDetail({
                modalSelector: mainClass + ' .modal.modalMasterDetail',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'idComprobanteVenta', visible: false },
            { data: 'comprobante', width: '10%' },
            { data: 'sucursal', visible: false },
            { data: 'numero', visible: false },
            {
                data: null,
                width: '10%',
                render: function (data, type, row, meta) {
                    return `${String("00000" + row.sucursal).slice(-5)}-${String("00000000" + row.numero).slice(-8)}`
                }
            },
            { data: 'fecha', width: '5%', render: DataTableEx.renders.date },
            { data: 'cliente', width: '40%', render: DataTableEx.renders.ellipsis },
            { data: 'moneda', width: '5%' },
            { data: 'subtotal', visible: false },
            {
                data: 'total',
                width: '10%',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-warning">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { data: 'idEstadoAFIP', orderable: false, searchable: false, width: '5%', class: 'text-center', render: renderEstadoAFIP },
            { data: 'cae', orderable: false, searchable: false, width: '10%', class: 'text-center' },
            { data: 'vencimientoCAE', orderable: false, searchable: false, width: '5%', class: 'text-center', render: DataTableEx.renders.date },
            { data: null, orderable: false, searchable: false, class: 'text-center', width: '5%', render: renderViewDetails }
        ];

        var order = [[0, 'desc']];
        var pageLength = 30;

        dtView = MaestroLayout.initializeDatatable('idComprobanteVenta', columns, order, pageLength);

    }

    function renderEstadoAFIP(data, type, row) {

        switch (data) {
            case enums.EstadoAFIP.Inicial:
                return `<i class="far fa-question-circle amarillo" title="Comprobante Pendiente de Autorización"></i>`;
                break;
            case enums.EstadoAFIP.Observado:
                return `<i class="far fa-exclamation-circle azul" title="Comprobante Observado"></i>`;
                break;
            case enums.EstadoAFIP.Error:
                return `<i class="far fa-times-circle rojo" title="Comprobate con Error"></i>`;
                break;
            case enums.EstadoAFIP.Autorizado:
                return `<i class="far fa-check-circle verde" title="Comprobate Autorizado"></i>`;
                break;
        }
    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.asiento', retrieveAsientoDetails);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.validar', retrieveValidarComprobante);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.obtener', retrieveCAEComprobante);

        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.newDialogOpening = newDialogOpening;

        $('input[name^=Numero]', modalNew).on('blur', function (e) {
            if ($(this).val().replaceAll('_', '') == "")
                return;
            $(this).val(`${String("000" + $(this).val().replaceAll('_','')).slice(-3)}`)
        });

        $('select[name^=IdSucursal]', modalNew).on('change', function (e) {

            if ($(this).val() == "" || $(this).val() == undefined)
                $('#Sucursal', modalNew).val("");
            else
                $('#Sucursal', modalNew).val($(this).text().substring(1, 7));
        });

        $('select[name^=IdMonedaComprobante]', modalNew).on('change', function (e) {

            if ($(this).val() == "" || $(this).val() == undefined)
                $('#Moneda', modalNew).val("");
            else
                $('#Moneda', modalNew).val($(this).val());
        });

        $('select[name^=IdComprobante]', modalNew).on('change', function (e) {

            var $form = $(e.target).closest('form');

            if ($('.percepcionAGIP').length > 0 || $('.percepcionARBA').length > 0) {

                params = {
                    idCliente: $('select[name^=IdCliente]', $form).select2('val'),
                    idComprobante: $(this).select2('val'),
                    fecha: moment($('input[name^=Fecha]').val(), 'DD/MM/YYYY').format('YYYY-MM-DD')
                };

                Ajax.Execute('/FacturacionAutomatica/GetPercepcionesByCliente/', params)
                    .done(function (response) {
                        if (response.status == enums.AjaxStatus.OK) {
                            if ($('.percepcionAGIP').length > 0) {
                                $('.percepcionAGIP').data('porc', response.detail.percepcionAGIP);
                            }

                            if ($('.percepcionARBA').length > 0) {
                                $('.percepcionARBA').data('porc', response.detail.percepcionARBA);
                            }
                        }

                        let items = $form.find('.master-detail-component').MasterDetail()
                            .getItems();

                        calculaTotalesComprobante(items);
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        Utils.modalError('Error', jqXHR.responseJSON.detail);
                    });
            }

        });

        $('select[name^=IdCliente]', modalNew).on('change', function (e) {

            var $form = $(e.target).closest('form');

            if ($(this).val() == "" || $(this).val() == undefined) {
                $('select[name^=IdComprobante]', $form).select2('val', null).empty();
                return;
            }

            var params = {
                idCliente: $(this).select2('val')
            };

            Ajax.Execute('/FacturacionArticulos/GetComprobantesByCliente/', params)
                .done(function (response) {
                    $('select[name^=IdComprobante]', $form).Select2Ex()
                        .source(response.detail.map(function (obj) { return { id: obj.idComprobante, text: obj.descripcion }; }));
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    Utils.modalError('Error', jqXHR.responseJSON.detail);
                });

            if ($('.percepcionAGIP', $form).length > 0 || $('.percepcionARBA', $form).length > 0) {

                params = {
                    idCliente: $(this).select2('val'),
                    idComprobante: $('select[name^=IdComprobante]', $form).select2('val'),
                    fecha: moment($('input[name^=Fecha]', $form).val(), 'DD/MM/YYYY').format('YYYY-MM-DD')
                };

                Ajax.Execute('/FacturacionManual/GetPercepcionesByCliente/', params)
                    .done(function (response) {
                        if (response.status == enums.AjaxStatus.OK) {
                            if ($('.percepcionAGIP', $form).length > 0) {
                                $('.percepcionAGIP', $form).data('porc', response.detail.percepcionAGIP);
                            }

                            if ($('.percepcionARBA', $form).length > 0) {
                                $('.percepcionARBA', $form).data('porc', response.detail.percepcionARBA);
                            }
                        }

                        let items = $form.find('.master-detail-component').MasterDetail()
                            .getItems();

                        calculaTotalesComprobante(items);
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        Utils.modalError('Error', jqXHR.responseJSON.detail);
                    });
            }
        });

        $('select[name^=IdMonedaComprobante]', modalNew).on('change', function (e) {

            var $form = $(e.target).closest('form');

            if ($('input[name^=Fecha]', modalNew).val() == "")
                return;

            if ($('select[name^=IdMonedaComprobante]', modalNew).val() == "")
                return;

            var params = {
                idMoneda1: $('select[name="IdMonedaComprobante"]', modalNew).val(),
                fecha: moment($('input[name^=Fecha]', modalNew).val(), 'DD/MM/YYYY').format('YYYY-MM-DD')
            };

            Ajax.Execute('/FacturacionArticulos/GetFechaCambio/', params)
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
                dateFormat: "d/m/Y",
                allowInput: true,
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
                        idMoneda1: $('select[name="IdMonedaComprobante"]', modalNew).val(),
                        fecha: moment(dateStr, 'DD/MM/YYYY').format('YYYY-MM-DD')
                    };

                    Ajax.Execute('/FacturacionManual/GetFechaCambio/', params)
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


        $('select[name="Detalle.IdArticulo"]').on('change', function (e) {
            $('textarea[name="Detalle.Descripcion"]').text(`${$('#Detalle_IdArticulo :selected').text()}`);

            var params = {
                idArticulo: $(this).val()
            };

            Ajax.Execute('/FacturacionArticulos/GetPrecio/', params)
                .done(function (response) {
                    if (response.status == enums.AjaxStatus.OK) {
                        AutoNumeric.set($('input[name="Detalle.PrecioUnitario"]')[0], response.detail);
                    }
                    else {
                        AutoNumeric.set($('input[name="Detalle.PrecioUnitario"]')[0], 0);
                    }

                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    Utils.modalError('Error', jqXHR.responseJSON.detail);
                });
        
        });

        $('input[name="Detalle.Cantidad"], input[name="Detalle.PrecioUnitario"]').on('blur', function (e) {
            var cantidad = AutoNumeric.getNumber(this);
            var precio = AutoNumeric.getNumber($('input[name="Detalle.PrecioUnitario"]')[0]);

            AutoNumeric.set($('input[name="Detalle.Importe"]')[0], cantidad * precio);
        });

        btImprimir.on('click', imprimir);

        $('select[name^=IdCondicion]', modalNew).on('change', function (e) {

            if ($(this).val() && $('input[name^=Fecha]', modalNew).val()) {

                var params = {
                    idCondicion: $(this).val(),
                    fecha: $('input[name^=Fecha]', modalNew).val()
                };

                Ajax.Execute('/FacturacionArticulos/GetVencimiento/', params)
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
    }

    function mainTableRowSelected(dataArray, api) {

        btImprimir.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        if (dataArray.length == 1) {
            btImprimir.prop('disabled', false);
        }
        else if (dataArray.length > 1) {
            btImprimir.prop('disabled', true);
        }
    }

    function newDialogOpening(dialog, $form) {

        var idEjercicio = $form.find('#IdEjercicio option:eq(1)').val()
        $form.find('#IdEjercicio').select2('val', idEjercicio);

        var idMonedaLocal = $form.find('#IdMonedaComprobante option:eq(1)').val()
        $form.find('#IdMonedaComprobante').select2('val', idMonedaLocal);
        AutoNumeric.set($('input[name^=Cotizacion]', $form)[0], 1);

        $('input[name^=Fecha]', $form).val(moment().format('DD/MM/YYYY'));
        $('input[name^=Vto]', $form).val('');

        $form.find('.master-detail-component').MasterDetail()
            .clear();

        limpiaTotalesComprobante();

    }

    function renderViewDetails(data, type, row) {

        var btnDetalle = "", btnAsiento = "", btnValidate = "", btnObtenerCAE = "";

        btnValidate = '<i class="far fa-file-check validar" title="Validar Comprobante"></i>';
        btnDetalle = '<i class="far fa-search-plus details" title="Desplegar detalle de Items"></i>';
        btnAsiento = '<i class="far fa-file-invoice asiento" title="Asiento Contable"></i>';
        btnObtenerCAE = '<i class="far fa-file-import obtener" title="Obtener CAE"></i>';

        if (parseInt(row.idEstadoAFIP) == enums.EstadoAFIP.Autorizado) {
            btnValidate = "";
            btnObtenerCAE = "";
        }
        
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
                <li>
                    ${btnObtenerCAE}
                </li>
            </ul>`;
    }

    function formatRowDetail(rowData) {

        if (rowData == null || rowData.items == null || rowData.items.length == 0)
            return '<div class="text-center"><span>El Comprobante no presenta Items en el Detalle</span></div>';

        var tableTemplate = 
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Item</th>
                            <th>Descripción</th>
                            <th>Precio Unitario</th>
                            <th>Cantidad</th>
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

        var rowTemplate =
            `<tr>
                <td>[@Item]</td>
                <td>[@Descripcion]</td>
                <td>[@Precio]</td>
                <td>[@Cantidad]</td>
                <td>[@Importe]</td>
                <td>[@Impuestos]</td>
                <td>[@Alicuota]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData.items) {
            var item = rowData.items[i];
            var row = rowTemplate
                .replace('[@Item]', item.item)
                .replace('[@Descripcion]', item.descripcion)
                .replace('[@Precio]', accounting.formatMoney(item.precio))
                .replace('[@Cantidad]', item.cantidad)
                .replace('[@Importe]', accounting.formatMoney(item.importe))
                .replace('[@Impuestos]', accounting.formatMoney(item.impuestos))
                .replace('[@Alicuota]', accounting.formatMoney(item.alicuota, { symbol: '%', format: '%v %s' }));

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

            Ajax.Execute('/FacturacionArticulos/Obtener/' + row.data().idComprobanteVenta, null, null, 'GET')
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

            Ajax.Execute('/FacturacionArticulos/ObtenerAsiento/' + row.data().idComprobanteVenta, null, null, 'GET')
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
            idComprobanteVenta: row.data().idComprobanteVenta
        };

        iconLoader($(tr.prevObject), true);

        Ajax.Execute('/FacturacionArticulos/ValidarComprobantesById/', params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {
                        if (data.error)
                            Utils.modalError("Error", data.error);
                        if (data.observacion)
                            Utils.modalInfo("Información", data.observacion);
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

    function retrieveCAEComprobante() {
        var tr = $(this).closest('tr');
        var row = dtView.api().row(tr);

        var params = {
            idComprobanteVenta: row.data().idComprobanteVenta
        };

        iconLoader($(tr.prevObject), true);

        Ajax.Execute('/FacturacionArticulos/ObtenerCAEComprobantesById/', params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {
                        if (data.error)
                            Utils.modalError("Error", data.error);
                        if (data.observacion)
                            Utils.modalInfo("Información", data.observacion);
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

        calculaTotalesComprobante(items);
    }

    function masterDetailRowEdited(e, item, items, $row, $rows) {
        console.log('MasterDetail item editado', item);
        calculaTotalesComprobante(items);
    }

    function masterDetailRowRemoved(e, item, items, $row, $rows) {
        console.log('MasterDetail item eliminado', item, items);
        calculaTotalesComprobante(items);
    }

    function masterDetailSourceLoaded(e, items, $rows) {
        console.log('MasterDetail lista de items cargada', items, $rows);
        calculaTotalesComprobante(items);
    }

    function masterDetailEmpty(e) {
        console.log('MasterDetail items eliminados');
    }

    function limpiaTotalesComprobante() {
        $('.impuestos').hide();
        $('.subtotal').text(`${accounting.formatMoney(0)}`);
        $('.total').text(`${accounting.formatMoney(0)}`);

        if ($('.percepcionAGIP').length > 0 || $('.percepcionARBA').length > 0) {
            $('.percepcionAGIP').text(`${accounting.formatMoney(0)}`).data('porc', 0);
            $('.percepcionARBA').text(`${accounting.formatMoney(0)}`).data('porc', 0);
        }
    }

    function calculaTotalesComprobante(items) {

        $('.impuestos').hide();

        var subtotal = 0, impuestos = 0, total = 0, percepcionAGIP = 0, percepcionARBA = 0;
        var impuestos = items.map(function (obj) { return { IdAlicuota: obj.IdAlicuota.value, Alicuota: obj.IdAlicuota.text, Importe: 0 }; });
        impuestos = impuestos.filter((value, index, self) => self.findIndex((m) => m.Alicuota === value.Alicuota) === index);

        for (var i = 0; i < items.length; i++) {

            var item = items[i];
            subtotal += parseFloat(item.Importe.value);

            var impuesto = $.grep(impuestos, function (n, i) {
                return n.IdAlicuota == item.IdAlicuota.value;
            });
            if (impuesto[0] != undefined)
                impuesto[0].Importe += parseFloat(item.Importe.value) * (parseFloat(item.IdAlicuota.text.replace(',', '.')) / 100);
        }

        if ($('.percepcionAGIP').length > 0 || $('.percepcionARBA').length > 0) {
            var porcCABA = parseFloat($('.percepcionAGIP').data('porc') || 0);
            var porcGBA = parseFloat($('.percepcionARBA').data('porc') || 0);

            percepcionAGIP = parseFloat(subtotal) * (porcCABA / 100);
            percepcionARBA = parseFloat(subtotal) * (porcGBA / 100);
        }

        total = subtotal;

        for (var i = 0; i < impuestos.length; i++) {
            $('.text_iva_' + impuestos[i].IdAlicuota).parent().show();
            $('.iva_' + impuestos[i].IdAlicuota).text(`${accounting.formatMoney(impuestos[i].Importe)}`);
            $('.iva_' + impuestos[i].IdAlicuota).parent().show();

            total += impuestos[i].Importe;
        }

        if ($('.percepcionAGIP').length > 0 || $('.percepcionARBA').length > 0) {
            $('.percepcionARBA').text(`${accounting.formatMoney(percepcionARBA)}`);
            $('.percepcionAGIP').text(`${accounting.formatMoney(percepcionAGIP)}`);

            total += percepcionARBA + percepcionAGIP;
        }

        $('.subtotal').text(`${accounting.formatMoney(subtotal)}`);
        $('.total').text(`${accounting.formatMoney(total)}`);
        
    }

    function imprimir() {

        var id = dtView.selected()[0].idComprobanteVenta;
        var action = $(this).data('ajax-action') + id;

        window.open(action);

    }

});

$(function () {
    ABMFacturacionArticulosView.init();
});
