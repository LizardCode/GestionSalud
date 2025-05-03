/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ProfesionalesAgendaView = new (function () {

    var self = this;
    //var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var calendar;

    calendarInitialized = false;

    this.init = function () {

        buildControls();
        bindControlEvents();

        ProfesionalesAgendaView.getAgenda();
    }


    function buildControls() {

    }

    function bindControlEvents() {

        $('[data-toggle="tooltip"]').tooltip();
    }

    //function getAgenda(initialDate) {
    this.getAgenda = function (initialDate) {

        var calendarElement = document.getElementById('AgendaProfesional');

        var inicio = $('#AGENDA_HoraInicio').val();
        var fin = $('#AGENDA_HoraFin').val();
        var intervalo = $('#AGENDA_IntervaloTurnos').val();

        ProfesionalesAgendaView.calendar = new FullCalendar.Calendar(calendarElement, {
            locale: 'es',
            timeZone: 'UTC',
            slotMinTime: inicio + ':00',
            slotMaxTime: fin + ':00',
            slotDuration: '00:' + intervalo + ':00',
            allDaySlot: false,
            allDayContent: '',

            views: {
                timeGrid: {
                    type: 'timeGrid',
                    displayEventEnd: false
                }
            },

            initialDate: initialDate || new Date(),
            validRange: {
                start: (new Date()).toISOString().split('T')[0]
            },

            customButtons: {
                btnCopiarSemana: {
                    text: 'COPIAR A SIG. SEMANA',
                    click: function () {
                        copiarSemana();
                    }
                },
                btnCopiarDia: {
                    text: 'COPIAR A SIG. DÍA',
                    click: function () {
                        copiarDia();
                    }
                }
            },

            headerToolbar: {
                left: 'prev,next today btnCopiarSemana btnCopiarDia',
                center: 'title',
                right: 'timeGridWeek,timeGridDay'
            },
            initialView: 'timeGridWeek',
            windowResizeDelay: 100,
            editable: false,
            navLinks: true,
            events: function (info, successCallback, failureCallback) {
                Utils.modalLoader();
                $.getJSON('/Profesionales/GetAgenda', getParametersAgenda(info))
                    .done(function (data) {
                        Utils.modalClose();
                        successCallback(data);
                    })
                    .fail(function (data) {
                        Utils.modalClose();
                        failureCallback(data);
                    });
            },
            loading: function (bool) {
                if (bool) {
                    Utils.modalLoader();
                } else {

                    if (!calendarInitialized) {
                        $('.fc-btnCopiarDia-button').hide();
                        calendarInitialized = true;
                    }

                    Utils.modalClose();

                    $('.fc-timeGridWeek-button').click(function (e) {
                        $('.fc-btnCopiarDia-button').hide();
                        $('.fc-btnCopiarSemana-button').show();
                    });

                    $('.fc-timeGridDay-button').click(function (e) {
                        $('.fc-btnCopiarSemana-button').hide();
                        $('.fc-btnCopiarDia-button').show();
                    });

                    $('.fc-col-header-cell-cushion').click(function (e) {
                        $('.fc-btnCopiarSemana-button').hide();
                        $('.fc-btnCopiarDia-button').show();
                    });
                }
            },
            eventsSet: function (events) {
                //Nada...
            },

            eventClick: function (info) {

                console.log(info.event);

                if ((new Date(info.event.extendedProps.fecha)) <= (new Date()) || info.event.extendedProps.idProfesionalTurno == -1)
                    return;

                Utils.modalLoader();

                if (info.event.extendedProps.idProfesionalTurno == 0) {
                    var idProfesional = $('#AGENDA_IdProfesional').val();

                    var params = {
                        desde: moment(info.event._instance.range.start).format('YYYY-MM-DDTHH:mm:SS'),
                        //hasta: moment(info.event._instance.range.end).format('YYYY-MM-DDTHH:mm:SS'),
                        IdProfesional: idProfesional
                    };

                    $.post("/Profesionales/HabilitarAgenda", params, function () {
                        Utils.modalClose();
                    })
                        .done(function (data) {
                            //alert("second success");
                            ProfesionalesAgendaView.calendar.refetchEvents();
                        })
                        .fail(function () {
                            //alert("error");
                            Utils.modalError('Error Habilitando Agenda', 'Error');
                        })
                        .always(function () {
                            //alert("finished");
                        });
                } else {
                    Utils.modalLoader();

                    var params = {
                        idProfesionalTurno: info.event._def.extendedProps.idProfesionalTurno
                    };

                    $.post("/Profesionales/DesHabilitarAgenda", params, function () {
                        Utils.modalClose();
                    })
                        .done(function (data) {
                            //alert("second success");
                            ProfesionalesAgendaView.calendar.refetchEvents();
                        })
                        .fail(function () {
                            //alert("error");
                            Utils.modalError('Error DesHabilitando Agenda', 'Error');
                        })
                        .always(function () {
                            //alert("finished");
                        });
                }
            }
        });

        //console.log('render');
        ProfesionalesAgendaView.calendar.render();
    }

    function getParametersAgenda(info) {
        var idProfesional = $('#AGENDA_IdProfesional').val();

        return {
            start: info.startStr,
            end: info.endStr,
            IdProfesional: idProfesional
        };
    }

    function copiarSemana() {

        Utils.modalQuestion('Copiar agenda', '¿Desea replicar la agenda en la semana próxima?', function (confirm) {
            if (confirm) {
                Utils.modalLoader();
                var desde = ProfesionalesAgendaView.calendar.view.activeEnd.toISOString().split('T')[0];
                var idProfesional = $('#AGENDA_IdProfesional').val();

                var params = {
                    desde: desde,
                    IdProfesional: idProfesional
                };

                $.post("/Profesionales/CopiarSemana", params, function () {
                    Utils.modalClose();
                })
                    .done(function (data) {
                        $('.fc-next-button.fc-button.fc-button-primary').click();
                        ProfesionalesAgendaView.calendar.refetchEvents();
                    })
                    .fail(function () {
                        //alert("error");
                        Utils.modalError('Error Copiando Agenda', 'Error');
                    })
                    .always(function () {
                        //alert("finished");
                    });
            }
        }, 'REPLICAR', 'CANCELAR');
    }

    function copiarDia() {

        Utils.modalQuestion('Copiar agenda', '¿Desea replicar la agenda en el próximo día?', function (confirm) {
            if (confirm) {
                Utils.modalLoader();
                var desde = ProfesionalesAgendaView.calendar.view.activeEnd.toISOString().split('T')[0];
                var idProfesional = $('#AGENDA_IdProfesional').val();

                var params = {
                    desde: desde,
                    IdProfesional: idProfesional
                };

                $.post("/Profesionales/CopiarDia", params, function () {
                    Utils.modalClose();
                })
                    .done(function (data) {
                        $('.fc-next-button.fc-button.fc-button-primary').click();
                        ProfesionalesAgendaView.calendar.refetchEvents();
                    })
                    .fail(function () {
                        //alert("error");
                        Utils.modalError('Error Copiando Agenda', 'Error');
                    })
                    .always(function () {
                        //alert("finished");
                    });
            }
        }, 'REPLICAR', 'CANCELAR');
    }
});

$(function () {
    ProfesionalesAgendaView.init();
});

//# sourceURL=agenda.js