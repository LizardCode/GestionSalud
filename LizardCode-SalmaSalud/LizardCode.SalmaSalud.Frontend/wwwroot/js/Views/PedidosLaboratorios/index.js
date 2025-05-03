/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMPedidosLaboratoriosView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var slPresupuesto;
    var slLaboratorio;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        btRemove = $('.toolbar-actions button.btRemove', mainClass);

        slPresupuesto = $('select#IdPresupuesto, select#IdPresupuesto_Edit', mainClass);
        slLaboratorio = $('select#IdLaboratorio, select#IdLaboratorio_Edit', mainClass);

        $('.modal .dvServicios .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
            .on('repeater:row:added', repeaterRowAdded)
            .on('repeater:row:removed', repeaterRowRemoved)
            .on('repeater:field:focus', repeaterFieldFocus)
            .on('repeater:field:change', repeaterFieldChange)
            .on('repeater:field:blur', repeaterFieldBlur)
            .Repeater();

        MaestroLayout.init();

        buildControls();
        bindControlEvents();
    }


    function buildControls() {

        MaestroLayout.errorTooltips = false;

        modalNew.find('> .modal-dialog').addClass('modal-50');

        var columns = [
            { data: 'idPedidoLaboratorio', width: '7%' },
            { data: 'fecha', render: DataTableEx.renders.date, width: '9%' },
            { data: 'idPresupuesto', width: '7%' },
            { data: 'paciente', width: '20%' },
            { data: 'pacienteDocumento', width: '10%' },
            { data: 'laboratorio', width: '22%' },
            {
                data: 'valor',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                },
                width: '10%'
            },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderEstado, width: '10%' },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderAcciones, width: '5%' }
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idPedidoLaboratorio', columns, order);
    }

    function bindControlEvents() {


        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);

        $('.modal').on('change', 'select[name$=".IdLaboratorioServicio"]', setServicio);
        //$('.modal').on('change', 'select[name$=".IdOtraPrestacion"]', setOtraPrestacion);

        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.tabChanged = tabChanged;

        slPresupuesto.on('change', changePresupuesto);
        slLaboratorio.on('change', changeLaboratorio);
    }

    function mainTableRowSelected(dataArray, api) {

        btRemove.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        btRemove.prop('disabled', !dataArray.length == 1 || dataArray[0].idEstadoPedidoLaboratorio != enums.EstadoPedidoLaboratorio.Pendiente);
    }

    function renderEstado(data, type, row) {

        return '<span class="badge badge-' + data.estadoClase + '">' + data.estado + '</span>';
    }

    function renderAcciones(data, type, row, meta) {

        //var btnAprobar = '<span type="button" class="btn badge badge-success btnAprobar btAction" title="MARCAR APROBADO" data-id-presupuesto="' + data.idPresupuesto + '" data-width-class="modal-70" data-ajax-action="/Presupuestos/Aprobar"><i class="fa fa-check"></i></span>';
        //var btnRechazar = '<span type="button" class="btn badge badge-danger btnRechazar btAction" title="MARCAR RECHAZADO" data-id-presupuesto="' + data.idPresupuesto + '" data-width-class="modal-70" data-ajax-action="/Presupuestos/Rechazar"><i class="fa fa-times"></i></span>';

        //var sReturn = '';
        //if (data.idEstadoPresupuesto == enums.EstadoPresupuesto.Abierto)
        //    sReturn += btnAprobar + '&nbsp;' + btnRechazar;
        //return sReturn;

        var btnDetalle = '<i class="far fa-search-plus details" title="Desplegar detalle de Servicios"></i>';
    
        return `
            <ul class="table-controls">
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

            Ajax.Execute('/PedidosLaboratorios/Obtener/' + row.data().idPedidoLaboratorio, null, null, 'GET')
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
                            <th>Servicio</th>
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
                <td>[@Servicio]</td>
                <td class="text-right">[@Valor]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData.servicios) {
            var item = rowData.servicios[i];
            var row = rowTemplate
                .replace('[@Servicio]', item.servicio)
                .replace('[@Valor]', accounting.formatMoney(item.valor));

            rows += row;
        }

        return tableTemplate.replace('[@Rows]', rows);
    }

    function newDialogOpening(dialog, $form) {

        reloadMaestroPresupuestos();

        $(".resumenView").html('');

        $form.find('.dvServicios .repeater-component').Repeater()
            .clear();
    }

    function tabChanged(dialog, $form, index, name) {

     }

    function repeaterInitialization() {
        console.log('repeater iniciado');
    }

    function repeaterRowAdded(e, $newRow, $rows) {

        ConstraintsMaskEx.init();
        setTotal(e);
    }

    function repeaterRowRemoved(e, $removedRow, $rows) {

        setTotal(e);
    }

    function repeaterFieldFocus(e, data) {

        setTotal(e);
    }

    function repeaterFieldChange(e, data) {

        setTotal(e);
    }

    function repeaterFieldBlur(e, data) {

        setTotal(e);
    }

    function setServicio(e) {

        var tr = $(e.target).closest('tr');
        var servicio = tr.find('select[name$=".IdLaboratorioServicio"]');
        var action = 'Laboratorios/ObtenerServicio';
        var params = {
            id: servicio.val()
        };

        Ajax.GetJson(action, params)
            .done(function (response) {

                //var codigo = tr.find('input[name$=".Descripcion"]');
                //codigo.val(response.descripcion);

                var valor = tr.find('input[name$=".Valor"]');
                AutoNumeric.set(valor.get(0), response.valor);

                setTotal({ target: tr.parents('table') });

            }).fail(Ajax.ShowError);
    }

    function setTotal(e) {

        var total = 0;
        var trs = $(e.target).find('tbody > tr');

        trs.each(function () {
            var tr = $(this);
            var importe = tr.find('> td > input[name$=".Valor"]');
            var importef = AutoNumeric.getNumber(importe.get(0));

            total += importef;
        });

        $('.totalServicios').text(`${accounting.formatMoney(total)}`);
    }

    function changePresupuesto(event, source) {
        var idPresupuesto = $('.modalNew select[name$="IdPresupuesto"]').val();

        Ajax.Execute('/Presupuestos/Obtener/' + idPresupuesto, null, null, 'GET')
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {

                        var idPaciente = data.idPaciente;
                        $.get('/Pacientes/ResumenView?id=' + idPaciente + '&showNombre=true&showButton=false', function (content) {
                            $(".resumenView").html(content);
                        });         
                    },
                    Ajax.ShowError
                );
            })
            .fail(Ajax.ShowError);        
    }

    function changeLaboratorio(event, source) {
        var idLaboratorio = $('.modalNew select[name$="IdLaboratorio"]').val();

        //Vacío el repeater
        $('.modalNew .dvServicios .repeater-component').Repeater()
            .clear();


        $('.modalNew .dvServicios .repeater-component').Repeater()
            .comboSource('IdLaboratorioServicio', []);

        //Voy al server a completar el maestro
        $.get("/Laboratorios/GetServiciosByLaboratorio?idLaboratorio=" + idLaboratorio, function (response) {
            Ajax.ParseResponse(response,
                function (data) {
                    if (data.length) {
                        $('.modalNew .dvServicios .repeater-component').Repeater()
                            .comboSource('IdLaboratorioServicio', data, true);
                    }
                },
                Ajax.ShowError
            );
        });
    }

    function reloadMaestroPresupuestos() {
        var combo = modalNew.find('select[name$="IdPresupuesto"]');

        combo.find('option').remove();

        //Voy al server a completar el maestro
        $.get("/Presupuestos/GetPresupuestosAprobadosDisponibles", function (response) {
            Ajax.ParseResponse(response,
                function (data) {
                    if (data.length) {
                        for (var i in data) {
                            var option = data[i];

                            combo.append(
                                $('<option>')
                                    .attr('value', option.value)
                                    .text(option.text)
                            );
                        }
                    }
                },
                Ajax.ShowError
            );
        });
    }
});

$(function () {
    ABMPedidosLaboratoriosView.init();
});
