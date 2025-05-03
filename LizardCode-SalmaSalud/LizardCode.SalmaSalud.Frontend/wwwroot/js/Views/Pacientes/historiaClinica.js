var HistoriaClinicaView = new (function () {
    var HC_dtEvoluciones;

    //#region Init

    var defaultDom =
        "<'dt--top-section'<l>>" +
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
        //$('.HC_closingButton').click(function () {
        //    $('#historiaClinicaDialog').modal('hide');
        //}); 
    }

    function initDataTables() {

        var idPaciente = $('#HC_IdPaciente').val();

        HC_dtEvoluciones = $('.HC_dtEvoluciones')
            .DataTableEx({

                ajax: {
                    url: RootPath + '/Evoluciones/ObtenerEvolucionesPorPaciente?idPaciente=' + idPaciente,
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
                dom: "<'dt--top-section'<l>>" +
                    "<'table-responsive'tr>" +
                    "<'dt--bottom-section d-sm-flex justify-content-sm-between text-center'<'dt--pages-count  mb-sm-0 mb-3'i><'dt--pagination'p>>",

                select: { style: 'single', info: false },

                /*select: { style: selectionStyle, info: (selectionStyle === 'os') },*/
                columns: [
                    //{ data: null, width: '10%', orderable: false, class: 'text-center', render: renderTipoTurno },
                    { data: 'fecha', width: '20%', render: DataTableEx.renders.date },                    
                    { data: 'especialidad', width: '80%'},                    
                    { data: 'idEvolucion', visible: false }
                ],
                order: [[2, 'DESC']],

                onSelected: HCdatatableSelectedRow,
                onDraw: datatableDraw,
                //onInit: datatableInit
            });
    }

    function HCdatatableSelectedRow(data) {
        reloadEvolucion(data[0].idEvolucion);
    }

    function datatableDraw() {

        feather.replace();
        $('[data-toggle="tooltip"]').tooltip();
        $('.historiasClinicasModalBody .dt--pages-count').hide();
    }

    //function renderTipoTurno(data, type, row, meta) {

    //    if (data.tipoTurno === 'ST')
    //        return '<i class="fa fa-calendar-plus" data-toggle="tooltip" data-placement="right" title="" data-original-title="SOBRE-TURNO"></i>';
    //    else if (data.tipoTurno === 'DE')
    //        return '<i class="fa fa-question" data-toggle="tooltip" data-placement="right" title="" data-original-title="DEMANDA ESPONTANEA"></i>';
    //    else
    //        return '<i class="fa fa-calendar-alt" data-toggle="tooltip" data-placement="right" title="" data-original-title="TURNO"></i>';
    //}

    function reloadEvolucion(idEvolucion) {

        $.get(RootPath + '/Evoluciones/ResumenView', { id: idEvolucion }, function (content) {
            $("#HC_evolucionView").html(content);
        });
    }
});

$(function () {

    HistoriaClinicaView.init();
});

//# sourceURL=historiaClinica.js