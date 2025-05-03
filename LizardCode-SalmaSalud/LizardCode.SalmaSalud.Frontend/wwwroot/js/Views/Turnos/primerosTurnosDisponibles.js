var TurnosDisponiblesView = new (function () {

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
            var hora = $(this).data('hora-turno');

            var fecha = $(this).data('fecha-turno');
            var action = '/Turnos/AsignarView?fecha=' + fecha + '&hora=' + hora + '&idProfesionalTurno=' + id;
            var widthClass = 'modal-70';

            Modals.loadAnyModal('asignarDialog', widthClass, action, function () { }, function () { });
        });
    }
});

$(function () {
    TurnosDisponiblesView.init();
});

//# sourceURL=primerosTurnosDisponibles.js