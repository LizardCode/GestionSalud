/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMPresupuestosView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var slPaciente;
    var editMode = false;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);

        btEdit = $('.toolbar-actions button.btEdit', mainClass);
        btRemove = $('.toolbar-actions button.btRemove', mainClass);


        slPaciente = $('select#IdPaciente, select#IdPaciente_Edit', mainClass);

        $('.modal .dvPrestaciones .repeater-component', mainClass)
            .on('repeater:init', repeaterPrestacionesInitialization)
            .on('repeater:row:added', repeaterPrestacionesRowAdded)
            .on('repeater:row:removed', repeaterPrestacionesRowRemoved)
            .on('repeater:field:focus', repeaterPrestacionesFieldFocus)
            .on('repeater:field:change', repeaterPrestacionesFieldChange)
            .on('repeater:field:blur', repeaterPrestacionesFieldBlur)
            .Repeater();


        $('.modal .dvOtrasPrestaciones .repeater-component', mainClass)
            .on('repeater:init', repeaterOtrasPrestacionesInitialization)
            .on('repeater:row:added', repeaterOtrasPrestacionesRowAdded)
            .on('repeater:row:removed', repeaterOtrasPrestacionesRowRemoved)
            .on('repeater:field:focus', repeaterOtrasPrestacionesFieldFocus)
            .on('repeater:field:change', repeaterOtrasPrestacionesFieldChange)
            .on('repeater:field:blur', repeaterOtrasPrestacionesFieldBlur)
            .Repeater();

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }


    function buildControls() {

        MaestroLayout.errorTooltips = false;

        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');

        var columns = [
            { data: 'idPresupuesto' },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'paciente' },
            { data: 'pacienteDocumento' },
            {
                data: 'totalCoPagos',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-secondary">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            {
                data: 'totalPrestaciones',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-secondary">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            {
                data: 'total',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderEstado },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderAcciones }
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idPresupuesto', columns, order);
    }

    function bindControlEvents() {

        $('.modal').on('change', 'select[name$=".IdPrestacion"]', setPrestacion);
        $('.modal').on('change', 'select[name$=".IdOtraPrestacion"]', setOtraPrestacion);

        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.tabChanged = tabChanged;


        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);

        dtView.table().on("click", ".btnRechazar", function (e) {

            var action = $(this).data('ajax-action');
            var id = $(this).data('id-presupuesto');

            Utils.modalQuestion("RECHAZAR Presupuesto", "¿Confirma RECHAZAR el presupuesto id: " + id + "?.",
                function (confirm) {
                    if (confirm) {
                        Utils.modalLoader();

                        var params = {
                            idPresupuesto: id
                        };

                        $.post(action, params, function () {
                            Utils.modalClose();
                        })
                            .done(function (data) {
                                dtView.reload();
                                Utils.modalInfo('Presupuesto RECHAZADO', 'Se ha RECHAZADO el presupuesto de manera correcta', 5000, undefined, function () {
                                });
                            })
                            .fail(function (jqXHR, textStatus, errorThrown) {
                                //alert("error");
                                Utils.modalError('Error RECHAZANDO presupuesto.', jqXHR.responseJSON.detail);
                            })
                            .always(function () {
                                //alert("finished");
                            });
                    }
                }, "RECHAZAR", "Cancelar", true);
        });

        dtView.table().on("click", ".btnAprobar", function (e) {

            var action = $(this).data('ajax-action');
            var id = $(this).data('id-presupuesto');

            Utils.modalQuestion("APROBAR Presupuesto", "¿Confirma APROBAR el presupuesto id: " + id + "?.",
                function (confirm) {
                    if (confirm) {
                        Utils.modalLoader();

                        var params = {
                            idPresupuesto: id
                        };

                        $.post(action, params, function () {
                            Utils.modalClose();
                        })
                            .done(function (data) {
                                dtView.reload();
                                Utils.modalInfo('Presupuesto APROBADO', 'Se ha APROBADO el presupuesto de manera correcta', 5000, undefined, function () {
                                });
                            })
                            .fail(function (jqXHR, textStatus, errorThrown) {
                                //alert("error");
                                Utils.modalError('Error APROBANDO presupuesto.', jqXHR.responseJSON.detail);
                            })
                            .always(function () {
                                //alert("finished");
                            });
                    }
                }, "APROBAR", "Cancelar", true);
        });

        dtView.table().on("click", ".btnCerrar", function (e) {

            var action = $(this).data('ajax-action');
            var id = $(this).data('id-presupuesto');

            Utils.modalQuestion("CERRAR Presupuesto", "El presupuesto ya no estrá disponible para realizar pedidos a laboratorios. ¿Confirma CERRAR el presupuesto id: " + id + "?.",
                function (confirm) {
                    if (confirm) {
                        Utils.modalLoader();

                        var params = {
                            idPresupuesto: id
                        };

                        $.post(action, params, function () {
                            Utils.modalClose();
                        })
                            .done(function (data) {
                                dtView.reload();
                                Utils.modalInfo('Presupuesto CERRADO', 'Se ha CERRADO el presupuesto de manera correcta', 5000, undefined, function () {
                                });
                            })
                            .fail(function (jqXHR, textStatus, errorThrown) {
                                //alert("error");
                                Utils.modalError('Error CERRANDO presupuesto.', jqXHR.responseJSON.detail);
                            })
                            .always(function () {
                                //alert("finished");
                            });
                    }
                }, "CERRAR", "Cancelar", true);
        });

        slPaciente.on('change', changePaciente);
    }

    function mainTableRowSelected(dataArray, api) {

        btEdit.prop('disabled', true);
        btRemove.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        btEdit.prop('disabled', !dataArray.length == 1 || dataArray[0].idEstadoPresupuesto != enums.EstadoPresupuesto.Abierto);
        btRemove.prop('disabled', !dataArray.length == 1 || dataArray[0].idEstadoPresupuesto != enums.EstadoPresupuesto.Abierto);
    }

    function renderEstado(data, type, row) {

        return '<span class="badge badge-' + data.estadoClase + '">' + data.estado + '</span>';
    }

    function renderAcciones(data, type, row, meta) {

        var btnAprobar = '<span type="button" class="btn badge badge-success btnAprobar btAction" title="MARCAR APROBADO" data-id-presupuesto="' + data.idPresupuesto + '" data-width-class="modal-70" data-ajax-action="/Presupuestos/Aprobar"><i class="fa fa-check"></i></span>';
        var btnRechazar = '<span type="button" class="btn badge badge-danger btnRechazar btAction" title="MARCAR RECHAZADO" data-id-presupuesto="' + data.idPresupuesto + '" data-width-class="modal-70" data-ajax-action="/Presupuestos/Rechazar"><i class="fa fa-times"></i></span>';
        var btnCerrar = '<span type="button" class="btn badge badge-black btnCerrar btAction" title="CERRAR" data-id-presupuesto="' + data.idPresupuesto + '" data-width-class="modal-70" data-ajax-action="/Presupuestos/Cerrar"><i class="fa fa-lock"></i></span>';

        var btnDetalle = '<i class="far fa-search-plus details" title="Desplegar detalle de Pedidos"></i>';
        if (!data.enPedido)
            btnDetalle = '';

        var sReturn = '';
        if (data.idEstadoPresupuesto == enums.EstadoPresupuesto.Abierto)
            sReturn += btnAprobar + '&nbsp;' + btnRechazar;

        if (data.idEstadoPresupuesto == enums.EstadoPresupuesto.Aprobado)
            sReturn += btnCerrar;

        //return sReturn;
        return `
            <ul class="table-controls">
                <li>
                    ${sReturn}
                </li>
                <li>
                    ${btnDetalle}
                </li>
            </ul>`;
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

            Ajax.Execute('/Presupuestos/ObtenerPedidos/' + row.data().idPresupuesto, null, null, 'GET')
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

    function formatRowDetail(rowData) {

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>ID Pedido</th>
                            <th>Fecha</th>
                            <th>Laboratorio</th>
                            <th class="text-center">Estado</th>
                            <th class="text-right">Valor</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@Rows]
                    </tbody>
                </table>
            </div>`;

        var rowTemplate =
            `<tr>
                <td>[@Id]</td>
                <td>[@Fecha]</td>
                <td>[@Laboratorio]</td>
                <td class="text-center"><span class="badge badge-[@EstadoClase]">[@Estado]</span></td>
                <td class="text-right">[@Valor]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData) {
            var item = rowData[i];
            var row = rowTemplate
                .replace('[@Id]', item.idPedidoLaboratorio)
                .replace('[@Fecha]', item.fecha)
                .replace('[@Laboratorio]', item.laboratorio)
                .replace('[@Estado]', item.estado)
                .replace('[@EstadoClase]', item.estadoClase)
                .replace('[@Valor]', accounting.formatMoney(item.valor));

            rows += row;
        }

        return tableTemplate.replace('[@Rows]', rows);
    }

    function newDialogOpening(dialog, $form) {

        editMode = false;

        $(".resumenView").html('');

        $form.find('.dvPrestaciones .repeater-component').Repeater()
            .clear();

        $form.find('.dvOtrasPrestaciones .repeater-component').Repeater()
            .clear();
    }

    function editDialogOpening($form, entity) {

        editMode = true;

        $form.find('#IdPresupuesto_Edit').val(entity.idPresupuesto);
        $form.find('#IdPaciente_Edit').select2('val', entity.idPaciente).trigger('change');
        $form.find('#FechaVencimiento_Edit').val(moment(entity.fechaVencimiento).format(enums.FormatoFecha.DefaultFormat));

        if (entity.prestaciones.length > 0) { 
            //$form.find('.dvPrestaciones .repeater-component').Repeater()
            //    .source(entity.prestaciones);
            changePaciente(null, entity.prestaciones);
        }
        else
            $form.find('.dvPrestaciones .repeater-component').Repeater()
                .clear();

        if (entity.otrasPrestaciones.length > 0)
            $form.find('.dvOtrasPrestaciones .repeater-component').Repeater()
                .source(entity.otrasPrestaciones);
        else
            $form.find('.dvOtrasPrestaciones .repeater-component').Repeater()
                .clear();
    }

    function tabChanged(dialog, $form, index, name) {

     }

    function repeaterPrestacionesInitialization() {
        console.log('repeater iniciado');
    }

    function repeaterPrestacionesRowAdded(e, $newRow, $rows) {

        ConstraintsMaskEx.init();
        setTotalPrestaciones(e);
    }

    function repeaterPrestacionesRowRemoved(e, $removedRow, $rows) {

        setTotalPrestaciones(e);
    }

    function repeaterPrestacionesFieldFocus(e, data) {

        setTotalPrestaciones(e);
    }

    function repeaterPrestacionesFieldChange(e, data) {

        setTotalPrestaciones(e);
    }

    function repeaterPrestacionesFieldBlur(e, data) {

        setTotalPrestaciones(e);
    }

    function repeaterOtrasPrestacionesInitialization() {
        console.log('repeater iniciado');
    }

    function repeaterOtrasPrestacionesRowAdded(e, $newRow, $rows) {

        ConstraintsMaskEx.init();
        setTotalOtrasPrestaciones(e);
    }

    function repeaterOtrasPrestacionesRowRemoved(e, $removedRow, $rows) {

        setTotalOtrasPrestaciones(e);
    }

    function repeaterOtrasPrestacionesFieldFocus(e, data) {

        setTotalOtrasPrestaciones(e);
    }

    function repeaterOtrasPrestacionesFieldChange(e, data) {

        setTotalOtrasPrestaciones(e);
    }

    function repeaterOtrasPrestacionesFieldBlur(e, data) {

        setTotalOtrasPrestaciones(e);
    }

    function setPrestacion(e) {

        var tr = $(e.target).closest('tr');
        var prestacion = tr.find('select[name$=".IdPrestacion"]');
        var action = 'Financiadores/ObtenerPrestacion';
        var params = {
            id: prestacion.val()
        };

        Ajax.GetJson(action, params)
            .done(function (response) {

                var codigo = tr.find('input[name$=".Codigo"]');
                codigo.val(response.codigo);

                var valor = tr.find('input[name$=".Valor"]');
                AutoNumeric.set(valor.get(0), response.valor);

                setTotalPrestaciones({ target: tr.parents('table') });

            }).fail(Ajax.ShowError);
    }

    function setOtraPrestacion(e) {

        var tr = $(e.target).closest('tr');
        var prestacion = tr.find('select[name$=".IdOtraPrestacion"]');
        var action = 'Prestaciones/Obtener';
        var params = {
            id: prestacion.val()
        };

        Ajax.GetJson(action, params)
            .done(function (response) {

                var codigo = tr.find('input[name$=".Codigo"]');
                codigo.val(response.codigo);

                var valor = tr.find('input[name$=".Valor"]');
                AutoNumeric.set(valor[0], response.valor);

                setTotalOtrasPrestaciones({ target: tr.parents('table') });

            }).fail(Ajax.ShowError);
    }

    function setTotalPrestaciones(e) {

        var total = 0;
        var totalCoPagos = 0;
        var trs = $(e.target).find('tbody > tr');

        trs.each(function () {
            var tr = $(this);
            var importe = tr.find('> td > input[name$=".Valor"]');
            var importef = AutoNumeric.getNumber(importe.get(0));

            total += importef;

            importe = tr.find('> td > input[name$=".CoPago"]');
            importef = AutoNumeric.getNumber(importe.get(0));

            totalCoPagos += importef;
        });

        $('.totalPrestaciones').text(`${accounting.formatMoney(total)}`);
        $('.totalCoPagos').text(`${accounting.formatMoney(totalCoPagos)}`);
    }

    function setTotalOtrasPrestaciones(e) {

        var total = 0;
        var trs = $(e.target).find('tbody > tr');

        trs.each(function () {
            var tr = $(this);
            var importe = tr.find('> td > input[name$=".Valor"]');
            var importef = AutoNumeric.getNumber(importe.get(0));

            total += importef;
        });

        $('.totalOtrasPrestaciones').text(`${accounting.formatMoney(total)}`);
    }

    function changePaciente(event, source) {
        var idPaciente = $((editMode ? '.modalEdit' : '.modalNew') + ' select[name$="IdPaciente"]').val();

        $.get('/Pacientes/ResumenView?id=' + idPaciente + '&showNombre=false&showButton=false', function (content) {
            $(".resumenView").html(content);
        });

        //Vacío el repeater
        $((editMode ? '.modalEdit' : '.modalNew') + ' .dvPrestaciones .repeater-component').Repeater()
            .clear();
    
        $((editMode ? '.modalEdit' : '.modalNew') + ' .dvPrestaciones .repeater-component').Repeater()
            .comboSource('IdPrestacion', []);

    
        //Voy al server a completar el maestro
        $.get("/Presupuestos/GetPrestacionesByFinanciadorPaciente?idPaciente=" + idPaciente, function (response) {
            Ajax.ParseResponse(response,
                function (data) {
                    if (data.length) {
                        $((editMode ? '.modalEdit' : '.modalNew') + ' .dvPrestaciones .repeater-component').Repeater()
                            .comboSource('IdPrestacion', data, true);

                        if (source) { 
                            $((editMode ? '.modalEdit' : '.modalNew') + ' .dvPrestaciones .repeater-component').Repeater()
                                .source(source);
                        }
                    }
                },
                Ajax.ShowError
            );
        });
        
    }
});

$(function () {
    ABMPresupuestosView.init();
});
