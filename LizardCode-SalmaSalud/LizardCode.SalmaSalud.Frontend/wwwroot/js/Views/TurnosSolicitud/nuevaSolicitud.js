var NuevaSolicitudView = new (function () {

    //#region Init

    this.init = function () {

        buildControls();
        bindControlsEvents();

    };

    //#endregion

    function buildControls() {
        Utils.rebuildValidator($('.frmMisDatos'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');

        $('[data-toggle="tooltip"]').tooltip();

        $('#IdEspecialidad').select2();
        $('#Dias').select2();
        $('#RangosHorarios').select2();

        $('select[name$="IdEspecialidad"]').on('change', function (e) {
            reloadDias(e);
            reloadRangos(e);
        });
    }

    function bindControlsEvents() {

        //$('.btnCancelarTurno').on('click', function () {
        //    var action = $(this).data('ajax-action');
        //    var widthClass = $(this).data('width-class');

        //    Modals.loadAnyModal('actionsDialog', widthClass, action, function () { }, function () { dtView.reload(); });
        //});
    }

    function reloadDias(e, element, selection) {

        var target = (e == null ? element : e.target);
        var idEspecialidad = $(target).val();
        var dias = $('#Dias');

        var action = 'TurnosSolicitud/GetDiasByEspecialidadId';
        var params = {
            id: idEspecialidad,
        };

        dias.find('option').remove();

        Ajax.Execute(action, params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {

                        dias.find('option').remove();

                        for (var i in data) {
                            var option = data[i];

                            dias.append(
                                $('<option />')
                                    .val(option.idTipoDia)
                                    .text(option.descripcion)
                            );
                        }

                        dias.select2('val', selection);
                    },
                    Ajax.ShowError
                );
            })
            .fail(Ajax.ShowError);
    }

    function reloadRangos(e, element, selection) {

        var target = (e == null ? element : e.target);
        var idEspecialidad = $(target).val();
        var rangos = $('#RangosHorarios');

        var action = 'TurnosSolicitud/GetRangosHorariosByEspecialidadId';
        var params = {
            id: idEspecialidad,
        };

        rangos.find('option').remove();

        Ajax.Execute(action, params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {

                        rangos.find('option').remove();

                        for (var i in data) {
                            var option = data[i];

                            rangos.append(
                                $('<option />')
                                    .val(option.idRangoHorario)
                                    .text(option.descripcion)
                            );
                        }

                        rangos.select2('val', selection);
                    },
                    Ajax.ShowError
                );
            })
            .fail(Ajax.ShowError);
    }

    this.ajaxBegin = function (context, arguments) {

        Utils.modalLoader();
    }

    this.ajaxSuccess = function (context) {
        Utils.modalClose();

        Utils.modalInfo('Turno Solicitado', 'Se ha solicitado el turno exitosamente.', 5000);
        setTimeout(function () { window.location = '/portal-pacientes/turnos' }, 1000);

    }

    this.ajaxFailure = function (context) {
        Utils.modalClose();

        Ajax.ShowError(context);
    }
});

$(function () {

    NuevaSolicitudView.init();
});

//# sourceURL=nuevaSolicitud.js