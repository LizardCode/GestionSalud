var OdontogramaConsultaView = new (function () {

    var self = this;
    var piezas = [];
    //var trabajos = [{ pieza: 12, zona: 1, tipoTrabajo: 2 }, { pieza: 12, zona: 2, tipoTrabajo: 2 }, { pieza: 12, zona: 3, tipoTrabajo: 2 }];

    this.init = function () {
        OdontogramaConsultaView.piezas = [];

        if (JSON_COSO)
            OdontogramaConsultaView.piezas = JSON_COSO;

        buildControls();
        bindEvents();

        //updateOdontograma();
    }

    function buildControls() {

    }

    function bindEvents() {
        $('.dvMainOdontogramaConsulta .pieza').click(function () {
            var id = $('.ODONTOGRAMA_CONSULTA_hdnIdEvolucion').val();
            var nroPieza = $(this).data('pieza');
            var action = '/Evoluciones/OdontogramaDetalleView?id=' + id + '&pieza=' + nroPieza;

            Modals.loadAnyModal('detalleOdontogramaDialog', 'modal-70', action,
                function () {
                    var pieza = OdontogramaConsultaView.piezas.filter(o => o.pieza == nroPieza);
                    OdontogramaDetalleView.updateOdontograma(pieza[0]);
                },
                function () {
                    //updateOdontograma();
                });
        });
    }

    //this.updatePiezas = function (oPieza, nroPieza) {
    //    OdontogramaView.piezas = OdontogramaView.piezas.filter((pieza) => pieza.pieza != nroPieza);

    //    OdontogramaView.piezas.push(oPieza);

    //    updateOdontograma();
    //}

    //function updateOdontograma() {

    //    OdontogramaView.piezas.forEach((pieza) => {

    //        var nPieza = pieza.pieza;
    //        pieza.zonas.forEach((zona) => {
    //            var clase = nPieza + '_' + zona.zona;

    //            $('.piezaZona_' + clase).removeClass('background-0');
    //            $('.piezaZona_' + clase).removeClass('background-1');
    //            $('.piezaZona_' + clase).removeClass('background-2');

    //            $('.piezaZona_' + clase).addClass('background-' + zona.tipoTrabajo);
    //            $('.piezaZona_' + clase).data('tipo-trabajo', zona.tipoTrabajo);
    //        });
    //    });
    //}

});

$(function () {

    OdontogramaConsultaView.init();
});

//# sourceURL=odontogramaConsulta.js