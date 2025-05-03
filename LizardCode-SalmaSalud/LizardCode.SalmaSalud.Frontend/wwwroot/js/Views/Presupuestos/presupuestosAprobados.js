var PresupuestosAprobadosView = new (function () {

    var self = this;
    var dtPresupuestosAprobados = null;

    this.init = function () {

        buildControls();
        bindControlEvents();

        $('.btPresupuestosAprobadosSeleccionar').prop('disabled', true);
    }


    function buildControls() {
        dtPresupuestosAprobados = $('.dtPresupuestosAprobados')
            .DataTableEx({
                processing: true,
                serverSide: false,
                pageLength: 10,
                lengthChange: false,
                searching: false,
                //columns: [
                //    { data: null },
                //    { data: 'idPresupuesto' },
                //    { data: 'idPresupuesto' },
                //    { data: 'material', orderable: false, width: '100%' },
                //    { data: 'idMaterial', orderable: false, visible: false }
                //],
                onDraw: function () { feather.replace(); },
                order: [1, 'asc']
            });
    }

    function bindControlEvents() {
        $('.chkSeleccionarPresupuestoAll').click(function () {
            var checked = $(this).is(':checked');

            $('.chkSeleccionarPresupuesto').each(function () {
                $(this).prop('checked', checked).trigger('change');
            });
        });

        $('.chkSeleccionarPresupuesto').on('change', function () {
            var checkedCount = $('.chkSeleccionarPresupuesto:checked').length;

            $('.btPresupuestosAprobadosSeleccionar').prop('disabled', checkedCount > 0 ? false : true);
        });

        $('.verDetallePresupuestos').on('click', function () { retrieveRowDetails($(this)); });

        $('.btPresupuestosAprobadosSeleccionar').on('click', function () {
            var presupuestos = [];
            $('.chkSeleccionarPresupuesto').each(function () {
                var checked = $(this).is(':checked');
                if (checked) {
                    presupuestos.push($(this).data('id-presupuesto').toString());
                }
            });
            $('.hdnPresupuestos').val(presupuestos.join(','));

            $('.presupuestosDialog').modal('hide');
        });
    }

    function retrieveRowDetails(jObject) {

        var idPresupuesto = $(jObject).data('id-presupuesto');

        var tr = $(jObject).closest('tr');
        var row = dtPresupuestosAprobados.api().row(tr);
        var loader =
            '<div class="text-center">' +
            '<div class="loader-sm spinner-grow text-success"></div>' +
            '</div>';

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
            $(jObject).find('i').removeClass('fa-minus');
            $(jObject).find('i').addClass('fa-plus');
        }
        else {
            row.child(loader).show();
            tr.addClass('shown');
            $(jObject).find('i').removeClass('fa-plus');
            $(jObject).find('i').addClass('fa-minus');

            $.get('/Presupuestos/PresupuestosAprobadosDetalleView?idPresupuesto=' + idPresupuesto, { }, function (content) {
                row.child(content);
            });
        }
    }
});

$(function () {
    PresupuestosAprobadosView.init();
});

//# sourceURL=presupuestosAprobados.js