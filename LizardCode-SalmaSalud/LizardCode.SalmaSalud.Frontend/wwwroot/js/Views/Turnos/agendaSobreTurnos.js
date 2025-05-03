/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var AgendaSobreTurnosView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var calendar;

    calendarInitialized = false;

    this.init = function () {

        buildControls();
        bindControlEvents();
        AgendaSobreTurnosView.getSobreTurnos();
    }


    function buildControls() {

    }

    function bindControlEvents() {

        $('[data-toggle="tooltip"]').tooltip();
    }

    this.getSobreTurnos = function (initialDate) {

        var calendarElement = document.getElementById('AgendaSobreTurnos');

        var inicio = $('#AGENDA_ST_HoraInicio').val();
        var fin = $('#AGENDA_ST_HoraFin').val();
        var intervalo = $('#AGENDA_ST_IntervaloTurnos').val();

        AgendaSobreTurnosView.calendar = new FullCalendar.Calendar(calendarElement, {
            locale: 'es',
            timeZone: 'UTC',
            slotMinTime: inicio + ':00',
            slotMaxTime: fin + ':00',
            slotDuration: '00:15:00', //TODO: Probar si vamos por acá
            allDaySlot: false,
            allDayContent: '',

            initialDate: initialDate || new Date(),
            validRange: {
                start: (new Date()).toISOString().split('T')[0]
            },

            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'timeGridWeek,timeGridDay'
            },
            initialView: 'timeGridWeek',
            windowResizeDelay: 100,
            editable: false,
            navLinks: true,
            events: function (info, successCallback, failureCallback) {
                Utils.modalLoader();
                $.getJSON('/TurnosSalaEspera/GetAgendaSobreTurnos', getParametersAgenda(info))
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
                        //$('.fc-btnCopiarDia-button').hide();
                        calendarInitialized = true;
                    }

                    Utils.modalClose();
                }
            },
            eventsSet: function (events) {
                //Nada...

                $('[data-toggle="tooltip"]').tooltip();
            },
            //dayMaxEventRows: true,
            //views: {
            //    dayGridMonth: {
            //        dayMaxEventRows: 5
            //    },
            //    timeGrid: {
            //        dayMaxEventRows: 50
            //    },
            //    dayGrid: {
            //        //weekday: 'long'
            //    }
            //},
            //hiddenDays: [0],
            eventClick: function (info) {

                console.log(info.event);

                if ((new Date(info.event.extendedProps.fecha)) <= (new Date()))
                    return;

                if (info.event.extendedProps.idProfesionalTurno == 0) {

                    return;
                } else {
                    Utils.modalLoader();

                    var idProfesionalTurno = info.event._def.extendedProps.idProfesionalTurno;
                    //var action = '/Turnos/AsignarSobreTurnoView?idProfesionalTurno=' + idProfesionalTurno;
                    //var widthClass = 'modal-70';

                    //Modals.loadAnyModal('asignarDialog', widthClass, action, function () { }, function () { });

                    window.location = '/Turnos/AsignarSobreTurnoPage?idProfesionalTurno=' + idProfesionalTurno;
                }
            },

            eventDidMount: function (info) {
                if (info.event.extendedProps.paciente) {

                    $(info.el).attr('data-toggle', 'tooltip');
                    $(info.el).attr('data-placement', 'top');
                    $(info.el).attr('title', info.event.extendedProps.paciente + (info.event.extendedProps.descripcion ? ' - ' + info.event.extendedProps.descripcion : ''));
                }
            },
        });

        //console.log('render');
        AgendaSobreTurnosView.calendar.render();
    }

    function getParametersAgenda(info) {

        return {
            start: info.startStr,
            end: info.endStr
        };
    }
});

$(function () {
    AgendaSobreTurnosView.init();
});

//# sourceURL=agendaSobreTurnos.js