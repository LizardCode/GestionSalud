/* Script HomeCalendario */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */

var TurnoHistorialView = new (function () {
    
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

        var idTurno = $('.hdnIdTurno').val();

        $('.dtHistorial_' + idTurno)
                    .DataTableEx({

                        ajax: {
                            url: RootPath + '/Turnos/ObtenerHistorial?idTurno=' + idTurno,
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
                        pageLength: 12,
                        lengthChange: false,
                        dom: defaultDom,

                        /*select: { style: selectionStyle, info: (selectionStyle === 'os') },*/
                        columns: [

                            { data: 'fecha', render: DataTableEx.renders.date },
                            { data: 'usuario' },
                            { data: 'estado', render: estadoRender },
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

    TurnoHistorialView.init();
});

//# sourceURL=historial.js