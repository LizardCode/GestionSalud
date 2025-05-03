var BusquedaView = new (function () {
    var HC_dtBusqueda;

    //#region Init

    var defaultDom =
        "<'dt--top-section'<l><f>>" +
        "<'table-responsive'tr>" +
        "<'dt--bottom-section d-sm-flex justify-content-sm-between text-center'<'dt--pages-count  mb-sm-0 mb-3'i><'dt--pagination'p>>";

    this.init = function () {

        buildControls();
        bindControlsEvents();

        initDataTables();
    };

    //#endregion

    function buildControls() {
        $('[data-toggle="tooltip"]').tooltip();
    }

    function bindControlsEvents() {

        $('.btSeleccionar').click(function () {

            seleccionar();
        });
    }

    function initDataTables() {

        HC_dtBusqueda = $('.HC_dtBusqueda')
            .DataTableEx({

                ajax: {
                    url: RootPath + '/Pacientes/ObtenerTodos',
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
                pageLength: 15,
                lengthChange: false,
                dom: defaultDom,

                select: { style: 'single', info: false },

                /*select: { style: selectionStyle, info: (selectionStyle === 'os') },*/
                columns: [                   
                    { data: 'nombre', width: '50%' },
                    { data: 'documento', width: '10%' },                    
                    { data: 'financiador', width: '40%' },                    
                    { data: 'idPaciente', visible: false }
                ],
                order: [[0, 'ASC']],

                onDoubleClick: datatableDoubleClickedRow,
                onSelected: datatableSelectedRow
            });
    }

    function datatableDraw() {

        feather.replace();
        $('[data-toggle="tooltip"]').tooltip();
        $('.historiasClinicasModalBody .dt--pages-count').hide();
    }

    function datatableSelectedRow(dataArray, api) {

        $('.btSeleccionar').prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        if (dataArray.length == 1) {
            $('.btSeleccionar').prop('disabled', false);
            //seleccionar();
        }
    }

    function datatableDoubleClickedRow() {

        seleccionar();
    }

    function seleccionar() {

        var data = HC_dtBusqueda.selected()[0];
        BUSQUEDA_DOCUMENTO_PACIENTE = data.documento;

        $('.busquedaDialog').modal('hide');
    }
});

$(function () {

    BusquedaView.init();
});

//# sourceURL=Busqueda.js