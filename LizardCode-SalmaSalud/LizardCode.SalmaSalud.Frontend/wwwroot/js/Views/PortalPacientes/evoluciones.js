var EvolucionesView = new (function () {

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

        $('.btnVerEvolucion').on('click', function () {
            var id = $(this).data('id-evolucion');
            var idx = $(this).data('idx');

            loadEvolucion(id, idx)
        });
    }

    function loadEvolucion(idEvolucion, idx) {

        $("#evolucionBody" + idx).html('<img alt="" src="/img/mini_loader.gif" width="75px;">');

        $.get(RootPath + '/Evoluciones/ResumenView', { id: idEvolucion }, function (content) {
            $("#evolucionBody" + idx).html(content);
        });
    }
});

$(function () {

    EvolucionesView.init();
});

//# sourceURL=evoluciones.js