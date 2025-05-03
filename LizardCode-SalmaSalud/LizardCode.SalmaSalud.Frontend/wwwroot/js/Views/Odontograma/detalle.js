var OdontogramaDetalleView = new (function () {

    var self = this;
    var pieza = {};

    this.init = function () {

        buildControls();
        bindEvents();

    }

    function buildControls() {
        if ($('.ODONTOGRAMA_DETALLE_hdnIdEvolucion').val() > 0) { 

            $('*[data-modal-id-evolucion="' + $('.ODONTOGRAMA_DETALLE_hdnIdEvolucion').val() + '"]')
                .find('input, select, textarea').attr('readonly', 'readonly').attr('disabled', 'disabled');
        }
    }

    function bindEvents() {

        if ($('.ODONTOGRAMA_DETALLE_hdnIdEvolucion').val() == 0) {
            $('.btOdontogramaDetalleGuardar').click(function () {
                guardar();
            });

            $('.zona').click(function () {
                var valor = $(this).data('tipo-trabajo');
                $(this).removeClass('background-' + valor);
                valor++;
                if (valor > 2) {
                    valor = 0;
                }
                $(this).data('tipo-trabajo', valor);
                $(this).addClass('background-' + valor);
            });
        }
    }


    this.updateOdontograma = function (pieza) {
        if (!pieza)
            return;

        OdontogramaDetalleView.pieza = pieza;

        $('.chkCaries').prop('checked', pieza.caries);
        $('.chkCorona').prop('checked', pieza.corona);
        $('.chkPrFija').prop('checked', pieza.prFija);
        $('.chkPrRemovible').prop('checked', pieza.prRemovible);
        $('.chkAmalgama').prop('checked', pieza.amalgama);
        $('.chkAusente').prop('checked', pieza.ausente);
        $('.chkOrtodoncia').prop('checked', pieza.ortodoncia);
        $('.chkExtraccion').prop('checked', pieza.extraccion);

        pieza.zonas.forEach((zona) => {
            var clase = zona.zona;
            $('.zona_' + clase).addClass('background-' + zona.tipoTrabajo);
            $('.zona_' + clase).data('tipo-trabajo', zona.tipoTrabajo);
        });
        $('.piezaObservaciones').val(pieza.observaciones);
    }


    function guardar() {
        var pieza = {
            pieza: $('.ODONTOGRAMA_DETALLE_hdnPieza').val(),
            caries: $('.chkCaries').is(':checked'),
            corona: $('.chkCorona').is(':checked'),
            prFija: $('.chkPrFija').is(':checked'),
            prRemovible: $('.chkPrRemovible').is(':checked'),
            amalgama: $('.chkAmalgama').is(':checked'),
            ausente: $('.chkAusente').is(':checked'),
            ortodoncia: $('.chkOrtodoncia').is(':checked'),
            extraccion: $('.chkExtraccion').is(':checked'),
            observaciones: $('.piezaObservaciones').val(),
            zonas: []
        };

         $('.zona').each(function () {
             pieza.zonas.push({
                 zona: $(this).data('zona'),
                 tipoTrabajo: $(this).data('tipo-trabajo')
            });
        });
    
        OdontogramaView.updatePiezas(pieza, $('.ODONTOGRAMA_DETALLE_hdnPieza').val());
        $('.detalleOdontogramaDialog').modal('hide')
    }

});

$(function () {

    OdontogramaDetalleView.init();

});

//# sourceURL=odontogramaDetalle.js