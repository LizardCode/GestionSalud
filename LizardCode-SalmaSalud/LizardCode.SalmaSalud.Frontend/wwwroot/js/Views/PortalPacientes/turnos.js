var TurnosView = new (function () {

    //#region Init

    this.init = function () {

        buildControls();
        bindControlsEvents();

    };

    //#endregion

    function buildControls() {
        $('[data-toggle="tooltip"]').tooltip();
    }

    function bindControlsEvents() {

        $('.btNew').on('click', function () {
            window.location = '/portal-pacientes/nueva-solicitud';

        });

        $('.btnCancelarTurno').on('click', function () {
            var action = $(this).data('ajax-action');
            var idTurnoSolicitud = $(this).data('id-turno-solicitud');
            //var widthClass = $(this).data('width-class');

            //Modals.loadAnyModal('actionsDialog', widthClass, action, function () { }, function () { location.reload(true); });

            Utils.modalQuestion("Cancelar turno", "¿Confirma la cancelación de la Solicitud de Turno?.",
                function (confirm) {
                    if (confirm) {
                        Utils.modalLoader();

                        var params = {
                            idTurnoSolicitud: idTurnoSolicitud
                        };

                        $.post(action, params, function () {
                            Utils.modalClose();
                        })
                            .done(function (data) {

                                Utils.modalInfo('Solicitud CANCELADA', 'Se ha CANCELADO la solicitud de manera correcta', 5000, undefined, function () { });

                                setTimeout(function () {
                                    location.reload(true);
                                }, 2000)
                            })
                            .fail(function () {
                                //alert("error");
                                Utils.modalError('Error CANCELANDO Solicitud', 'Error');
                            })
                            .always(function () {
                                //alert("finished");
                            });
                    }
                }, "CONFIRMAR", "Cancelar", true);
        });

        //$('.btnCconfirmarTurno').on('click', function () {
        //    var id = $(this).data('id-turno');
        //    doConfirmar(id);

        //});
    }

    //function doConfirmar(idTurno) {

    //    var action = '/Turnos/Confirmar';

    //    Utils.modalQuestion("Confirmar Asistencia", "Confirmación de asistencia en el día y hora indicados. ¿Confirma?.",
    //        function (confirm) {
    //            if (confirm) {
    //                Utils.modalLoader();

    //                var params = {
    //                    idTurno: idTurno
    //                };

    //                $.post(action, params, function () {
    //                    Utils.modalClose();
    //                })
    //                    .done(function (data) {

    //                        Utils.modalInfo('Turno CONFIRMANDO', 'Se ha CONFIRMADO el turno de manera correcta', 5000, undefined, function () { });

    //                        setTimeout(function () {
    //                            location.reload(true);
    //                        }, 2000)
    //                    })
    //                    .fail(function () {
    //                        //alert("error");
    //                        Utils.modalError('Error CONFIRMANDO TURNO', 'Error');
    //                    })
    //                    .always(function () {
    //                        //alert("finished");
    //                    });
    //            }
    //        }, "CONFIRMAR", "Cancelar", true);

    //}
});

$(function () {

    TurnosView.init();
});

//# sourceURL=turnos.js