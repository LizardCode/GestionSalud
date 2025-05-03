var TurnoReAgendarView = new (function () {

    var self = this;
    var dtReAgendarTurno;

    var defaultDom =
        "<'dt--top-section'<l><f>>" +
        "<'table-responsive'tr>" +
        "<'dt--bottom-section d-sm-flex justify-content-sm-between text-center'<'dt--pages-count  mb-sm-0 mb-3'i><'dt--pagination'p>>";

    this.init = function () {

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        var idEspecialidad = $('.hdnReAgendarIdEspecialidad').val();
        var idProfesional = $('.hdnReAgendarIdProfesional').val();

        dtReAgendarTurno = $('.dtReAgendarTurno')
            .DataTableEx({

                ajax: {
                    url: RootPath + '/Turnos/ObtenerTurnosReAgendar?idEspecialidad=' + idEspecialidad + '&idProfesional=' + idProfesional,
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
                    { data: 'fecha', width: '50%' },
                    { data: 'hora', width: '30%', class: 'text-center' },
                    { data: null, class: 'text-center', width: '20%', render: renderAcciones },
                    { data: 'fechaInicio', visible: false }
                ],
                order: [[3, 'ASC']],

                //onSelected: datatableSelectedRow,
                onDraw: datatableDraw,
                //onInit: datatableInit
            });

        $('.dtReAgendarTurno tbody').on("click", ".btReAgendar", function (e) {

            var action = $(this).data('ajax-action');
            var id = $(this).data('id-turno');

            Utils.modalQuestion("Confirmar Turno", "¿Confirma Re-Agendar el turno?.",
                function (confirm) {
                    if (confirm) {
                        Utils.modalLoader();

                        var params = {
                            idTurno: $('.hdnReAgendarIdTurno').val(),
                            idProfesionalTurno: id
                        };

                        $.post(action, params, function () {
                            Utils.modalClose();
                        })
                            .done(function (data) {
                                Utils.modalInfo('Turno RE-AGENDADO', 'Se ha RE-AGENDADO el turno de manera correcta', 5000, undefined, function () {

                                    $('.actionsDialog').modal('hide');
                                });
                            })
                            .fail(function () {
                                Utils.modalError('Error RE-AGENDANDO TURNO', 'Error');
                            })
                            .always(function () {

                            });
                    }
                }, "RE-AGENDAR", "Cancelar", true);
        });
    }

    function datatableDraw() {

        feather.replace();
    }

    function bindControlEvents() {

    }

    function renderAcciones(data, type, row, meta) {

        var btnReAgendar = '<span type="button" class="btn badge badge-warning btReAgendar" title="RE-AGENDAR" data-id-turno="' + data.idProfesionalTurno + '" data-ajax-action="/Turnos/ReAgendar"><i class="fa fa-calendar"></i></span>';

        return btnReAgendar;
    }
});

$(function () {
    TurnoReAgendarView.init();
});

//# sourceURL=reAgendar.js