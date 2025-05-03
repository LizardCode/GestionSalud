var TurnosDetalleView = new (function () {

    var self = this;

    this.init = function () {

        buildControls();
        bindControlEvents();
    }


    function buildControls() {
    }

    function bindControlEvents() {

        $('.agendarTurno').click(function () {
            var id = $(this).data('id-profesional-turno');

            //var action = '/Turnos/AsignarView?idProfesionalTurno=' + id;
            //var widthClass = 'modal-70';

            //Modals.loadAnyModal('asignarDialog', widthClass, action, function () { }, function () { TurnosView.calendar.refetchEvents(); });

            window.location = '/Turnos/AsignarPage?idProfesionalTurno=' + id;
        });
    }
});

$(function () {
    TurnosDetalleView.init();
});

//# sourceURL=detalle.js