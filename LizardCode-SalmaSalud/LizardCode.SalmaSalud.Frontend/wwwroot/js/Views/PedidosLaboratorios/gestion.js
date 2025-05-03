/* Script AsignarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */
var PedidosLaboratoriosGestionView = new (function () {
    var self = this;

    this.init = function () {

        MaestroLayout.init();

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        //Ver de embellecer la grilla?        
        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: null, render: renderCheckBox, class: 'text-center', width: '5%' },
            { data: 'idPedidoLaboratorio', width: '8%' },
            { data: 'fecha', render: DataTableEx.renders.date, width: '9%' },
            { data: 'idPresupuesto', width: '8%' },
            { data: 'paciente', width: '11%' },
            { data: 'laboratorio', width: '22%' },
            { data: null, render: renderEstado, class: 'text-center', width: '8%' },
            { data: 'fechaEnvio', render: DataTableEx.renders.date, width: '8%' },
            { data: 'numeroSobre', width: '5%' },
            { data: null, render: renderFechaEstimada, width: '8%' },
            { data: 'fechaRecepcion', render: DataTableEx.renders.date, width: '8%' },
            { data: null, render: renderHistorial, class: 'text-center', width: '8%' }
        ];

        var order = [[2, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idPedidoLaboratorio', columns, order);
    }

    function renderCheckBox(data, type, row, meta) {
        if (data.idEstadoPedidoLaboratorio == enums.EstadoPedidoLaboratorio.Pendiente
            || data.idEstadoPedidoLaboratorio == enums.EstadoPedidoLaboratorio.Envviado)            
        return '<input type="checkbox" class="chkItem chkItem_' + data.idPresupuesto + '_' + data.id + '"" data-id-pedido="' + data.idPedidoLaboratorio + '" data-id-estado="' + data.idEstadoPedidoLaboratorio + '" data-id-bloque="0" />';
        else
            return '';
    }

    function renderEstado(data, type, row, meta) {
        return '<span class="badge badge-pills badge-' + data.estadoClase + ' font10">' + data.estado + '</span>';
    }

    function renderHistorial(data, type, row, meta) {
        return '<span type="button" class="btn badge badge-light btHistorial" title="HISTORIAL" data-ajax-action="/PedidosLaboratorios/PedidoLaboratorioHistorialView?idPedidoLaboratorio=' + data.idPedidoLaboratorio + '" data-toggle="tooltip" data-placement="top" title="HISTORIAL"><i class="fa fa-list"></i></span>';
    }

    function renderFechaEstimada(data, type, row, meta) {
        var currentTime = new Date();

        if (data.fechaRecepcion && new Date(data.fechaEstimada) < new Date(data.fechaRecepcion)) {
            return '<span class="btn badge badge-danger" data-toggle="tooltip" data-placement="top" title="PEDIDO ATRASADO">' + DataTableEx.utils.date(data.fechaEstimada) + '</span>';
        }

        if (!data.fechaRecepcion && data.fechaEstimada && new Date(data.fechaEstimada.replace('T00:00:00', 'T23:59:59')) < currentTime) {
            return '<span class="btn badge badge-danger" data-toggle="tooltip" data-placement="top" title="PEDIDO ATRASADO">' + DataTableEx.utils.date(data.fechaEstimada) + '</span>';
        }

        return DataTableEx.utils.date(data.fechaEstimada);
    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > span.btHistorial', doAction);
        dtView.table().on('click', 'tbody > tr > td > input.chkItem', validarChecks);

        $('.btRecibido').click(function () {

            var idBloque = $(this).data('id-bloque');

            var checkeds = $('.chkItem[data-id-bloque="' + idBloque + '"]:checkbox:checked');
            var ids = new Array();
            checkeds.each(function () {
                ids.push($(this).data('id-pedido'));
            });

            var action = '/PedidosLaboratorios/MarcarRecibidoView?idsPedidos=' + ids.join(',');

            Modals.loadAnyModal('pedidosGestionMarcarDialog', 'modal-50', action, function () { }, function () { });
        });

        $('.btEnviado').click(function () {

            var idBloque = $(this).data('id-bloque');

            var checkeds = $('.chkItem[data-id-bloque="' + idBloque + '"]:checkbox:checked');
            var ids = new Array();
            checkeds.each(function () {
                ids.push($(this).data('id-pedido'));
            });

            var action = '/PedidosLaboratorios/MarcarEnviadoView?idsPedidos=' + ids.join(',');

            Modals.loadAnyModal('pedidosGestionMarcarDialog', 'modal-50', action, function () { }, function () { });
        });
    }

    function doAction($btn) {

        var action = $($btn.target).closest('span').data('ajax-action');
        var widthClass = $($btn.target).closest('span').data('width-class');
        //var action = $(this).closest('span').data('ajax-action');
        //var widthClass = $(this).closest('span').data('width-class');

        Modals.loadAnyModal('actionsDialog', widthClass, action, function () { }, function () { dtView.reload(); });
    }

    function validarChecks($chk) {
        //var idBloque = $chk.data('id-bloque');
        var idBloque = $($chk.target).data('id-bloque');

        $('.btRecibido[data-id-bloque="' + idBloque + '"]').prop('disabled', true);
        $('.btEnviado[data-id-bloque="' + idBloque + '"]').prop('disabled', true);

        var checkeds = $('.chkItem[data-id-bloque="' + idBloque + '"]:checkbox:checked');
        var enviados = 0;
        var recibidos = 0;
        var pendientes = 0;

        checkeds.each(function () {
            var estado = $(this).data('id-estado');

            if (estado == enums.EstadoPedidoLaboratorio.Pendiente)
                pendientes++;

            if (estado == enums.EstadoPedidoLaboratorio.Envviado)
                enviados++;

            if (estado == enums.EstadoPedidoLaboratorio.Recibido)
                recibidos++;
        });

        if (recibidos)
            return;

        if (enviados && !pendientes && !recibidos) {
            $('.btRecibido[data-id-bloque="' + idBloque + '"]').prop('disabled', false);
        }

        if (pendientes && !enviados && !recibidos) {
            $('.btEnviado[data-id-bloque="' + idBloque + '"]').prop('disabled', false);
        }
        
    }
});

$(function () {
    PedidosLaboratoriosGestionView.init();
});

//# sourceURL=gestion.js