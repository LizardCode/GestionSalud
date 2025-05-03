/* Script HomeCalendario */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */

var PedidosLaboratoriosGestionHistorialView = new (function () {

    //#region Init

    var dtHistorial = null;

    var defaultDom =
        "<'table-responsive'tr>" +
        "<'dt--bottom-section d-sm-flex justify-content-sm-between text-center'<'dt--pages-count  mb-sm-0 mb-3'i><'dt--pagination'p>>";

    this.init = function () {

        buildControls();
        bindControlsEvents();

        initDataTables();
    };

    function initDataTables() {

        var idPedidoLaboratorio = $('.hdnPedidoLaboratorioHistorialIdPedidoLaboratorio').val();

        $('.dtHistorial_' + idPedidoLaboratorio)
            .DataTableEx({

                ajax: {
                    url: RootPath + '/PedidosLaboratorios/PedidoLaboratorioHistorial?idPedidoLaboratorio=' + idPedidoLaboratorio,
                    type: 'POST',
                    error: function (xhr, ajaxOptions, thrownError) {
                        Ajax.ShowError(xhr, xhr.statusText, thrownError);
                    },
                    callback: function (xhr) {
                        Ajax.ShowError(xhr, xhr.statusText, '');
                    }
                },
                processing: true,
                serverSide: true,
                pageLength: 10,
                lengthChange: false,
                dom: defaultDom,

                /*select: { style: selectionStyle, info: (selectionStyle === 'os') },*/
                columns: [

                    { data: 'fecha', render: DataTableEx.renders.date },
                    //{ data: 'usuario' },
                    { data: 'estado', render: estadoRender, class: 'text-center' },
                    { data: 'fechaEstado', render: DataTableEx.renders.date },
                    { data: 'observaciones', width: '50%', render: observacionesRender }
                ],
                order: [[0, 'asc']],
                onDraw: datatableDraw
            });

    };

    //#endregion

    //#region Funciones

    function buildControls() {

    }

    function bindControlsEvents() {
    }

    function datatableDraw() {

        feather.replace();
    }

    function estadoRender(data, type, row, meta) {

        return '<span class="badge badge-' + row.estadoClase + '">' + data + '</span>';
    }

    function observacionesRender(data, type, row, meta) {

        var data = data || '';

        return data.replace(/\r\n/g, '<br/>');
    }

    //#endregion

});

$(function () {

    PedidosLaboratoriosGestionHistorialView.init();
});

//# sourceURL=historial.js