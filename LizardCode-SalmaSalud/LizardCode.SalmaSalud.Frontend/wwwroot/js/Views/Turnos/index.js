/* Script Turnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */

var TurnosView = new (function () {
    
    //#region Init

	let initialized = false;
    var calendar;

    this.init = function () {

        $('.filtros').click();

        buildControls();
        bindControlsEvents();

        GetEventosCalendario();
    };

    //#endregion

    //#region Funciones

    function buildControls() {
        $('select').Select2Ex({ allowClear : true });

        //$('.s2Profesionales').select2('enable', false);
    }

    function bindControlsEvents() {
        $('.sidebarCollapse').click(function () {
            if (typeof TurnosView !== 'undefined') { 
                setTimeout(function () {
                    TurnosView.calendar.updateSize();
                }, 300);
            }
        });

        //$('.chkProfesionales').click(function () {
        //    if ($(this).is(':checked')) {
        //        $('.s2Profesionales').select2('enable', false);
        //    } else {
        //        $('.s2Profesionales').select2('enable', true);
        //    }
        //});

        $('.btFiltros').click(function () {

            var especialidad = getParametervalue('Especialidades');
            var profesional = getParametervalue('Profesionales');

            if (especialidad == 0 && profesional == 0) {
                Utils.modalInfo('Filtros', 'Seleccione al menos uno de los dos filtros.');
                return;
            }

            $('.filtros').click();

            if ($('.chkMostarPrimerosDsiponibles').is(':checked'))
                showPrimerosTurnosDisponibles();

            TurnosView.calendar.refetchEvents();
        });

        $('.btnPrimerTurnoAgendar').click(function () {

            var id = $(this).data('id-profesional-turno');

            window.location = '/Turnos/AsignarPage?idProfesionalTurno=' + id;
        });
    }

    function GetEventosCalendario() {

        var calendarElement = document.getElementById('CalendarioTurnos');

        TurnosView.calendar = new FullCalendar.Calendar(calendarElement, {
            //views: {
            //    dayGridMonth: {
            //        titleFormat: {
            //            year: 'numeric', month: 'long', day: 'numeric', weekday: 'short'
            //        },
            //    },
            //},
            dateClick: function (info) {
                console.log(info);
                return false;
            },
            validRange: {
                start: (new Date()).toISOString().split('T')[0]
            },
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth'
            },
            initialView: 'dayGridMonth',
            windowResizeDelay: 300,
            editable: false,
            initialDate: new Date(),
            locale: 'es',
            dayMaxEvents: true, // allow "more" link when too many events            
            events: function (info, successCallback, failureCallback) {
                Utils.modalLoader();
                $.getJSON('/Turnos/GetTurnosDisponiblesPorDia', getParameters(info))
                    .done(function (data) {
                        Utils.modalClose();
                        successCallback(data);

                        let cantidad = 0;
                        data.forEach(item => {
                            cantidad += item.disponibles || 0;
                        });
                        getInfoBusqueda(getParameters(info), cantidad);
                    })
                    .fail(function (data) {
                        Utils.modalClose();
                        failureCallback(data);
                    });
            },
            loading: function (bool) {
                if (bool) {
                    //Utils.modalWait('Obteniendo eventos...');
                    Utils.modalLoader();
                } else {
                    if (!initialized) {
                        //$('.sidebarCollapse').click();
                        initialized = true;
                    }

                    Utils.modalClose();
                }
            },
            eventsSet: function (events) {

                //Nada...
            },
            eventClick: function (info) {

                if (info.event.extendedProps.disponibles > 0) {
                    //console.log(info);
                    Utils.modalLoader();

                    var fecha = moment(info.event._instance.range.end).format('YYYY-MM-DD');
                    //var action = '/Turnos/Detalle?fecha=' + fecha + '&idEspecialidad=' + getParametervalue('Especialidades') + '&idProfesional=' + getParametervalue('Profesionales');
                    //var widthClass = 'modal-50';

                    //Modals.loadAnyModal('turnosDialog', widthClass, action, function () { }, function () { });

                    window.location = '/Turnos/DetallePage?fecha=' + fecha + '&idEspecialidad=' + getParametervalue('Especialidades') + '&idProfesional=' + getParametervalue('Profesionales');
                }
            }
        });

        TurnosView.calendar.render();
    }

    function getParameters(info) {

        return {
            start: info.startStr,
            end: info.endStr,
            IdEspecialidad: getParametervalue('Especialidades'),
            IdProfesional: getParametervalue('Profesionales')
        };
    }

    function getParametervalue(filterName) {
        if ($('.chk' + filterName).is(':checked'))
            return 0;
        else {
            return $('.s2' + filterName).select2('val');
        }
    }

    function getInfoBusqueda(parameters, cantidad) {
        $.getJSON('/Turnos/ObtenerPrimerTurnoDisponible', parameters)
            .done(function (data) {

                $('.btnPrimerTurnoAgendar').data('id-profesional-turno', 0);

                if (data) {
                    $('.btnPrimerTurnoAgendar').data('id-profesional-turno', data.idProfesionalTurno);
                    $('.bInfoBusquedaProximoTurno').html('El próximo turno disponible es el [DIA] a las [HORA] con el profesional: [PROFESIONAL]. Especialidad: [ESPECIALIDAD]'.replace('[DIA]', data.fecha).replace('[HORA]', data.hora).replace('[ESPECIALIDAD]', data.especialidad).replace('[PROFESIONAL]', data.profesional));
                    $('.dvInfoBusquedaProximoTurno').show();
                } else {
                    $('.dvInfoBusquedaProximoTurno').hide();
                }

                $('.bInfoBusqueda').html('Existen [CANTIDAD] turnos disponibles para el mes de [MES].'.replace('[CANTIDAD]', cantidad).replace('[MES]', $('.fc-toolbar-title').text()));
                $('.dvInfoBusqueda').show();

                $('[data-toggle="tooltip"]').tooltip();
            })
            .fail(function (data) {
                Utils.ShowError('Error');
            });
    }

    function showPrimerosTurnosDisponibles() {
        Utils.modalLoader();

        var action = '/Turnos/PrimerosTurnosDisponibles?idEspecialidad=' + getParametervalue('Especialidades') + '&idProfesional=' + getParametervalue('Profesionales');
        var widthClass = 'modal-50';

        Modals.loadAnyModal('turnosDialog', widthClass, action, function () {
            if (!$('#hdnPrimerosTurnosDisponiblesCantidad').val()) {
                $('.turnosDialog').modal('hide');
            }
        }, function () { });
    }

});

$(function () {

    TurnosView.init();
});