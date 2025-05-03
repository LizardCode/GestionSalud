/* Script AsignarTurnos */

/* eslint eqeqeq: 0 */
/* eslint no-extra-parens: 0 */
var TurnosAsignarSobreTurnoView = new (function () {
    var self = this;

    this.init = function () {

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        Utils.rebuildValidator($('.frmAsignar'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');
    }

    function bindControlEvents() {

        $('.btSave')
            .on('click', function () {
                asignar($('.frmAsignar'));
            });

        $('.pacienteDocumento')
            .on('blur', function (e) {
                pacienteLookup(this);
            })
            .on('keydown', function (e) {
                if (e.which == 13)
                    pacienteLookup(this);
                else if (e.which == 8 || e.which == 46) {

                    setPacienteData(this, null);
                }
            })
            .on('keypress', function (e) {
                var charTyped = String.fromCharCode(e.which);

                if (/[a-z\d]/i.test(charTyped))
                    setPacienteData(this, null);
            });

        $('.financiador').select2();
        $('.financiadorPlan').select2();

        $('select[name$="IdFinanciador"]').on('change', function (e) {
            reloadPlanes(e);
        });

        $('.frmAsignar').on('click', '.fly-remove', function () {

            $('.pacienteDocumento').val('');
            $('.pacienteDocumento').removeAttr('readonly');
            setPacienteData($('.pacienteDocumento'), null);

            $('.pacienteDocumento')
                .closest('.controls')
                .find('span.fly-remove')
                .remove();
        });

        $('#SinCobertura').change(function () {
            var checked = $(this).is(':checked');

            $('.financiador').prop('disabled', checked);
            $('.financiadorPlan').prop('disabled', checked);
            $('.financiadorNro').prop('disabled', checked);
        });

        $('#FechaNacimiento')
            .inputmask("99/99/9999")
            .flatpickr({
                locale: "es",
                allowInput: true,
                maxDate: "today",
                defaultDate: "today",
                dateFormat: "d/m/Y",
                onClose: function (selectedDates, dateStr, instance) {
                    if (dateStr == "")
                        instance.setDate(moment().format(enums.FormatoFecha.DefaultFormat));
                    else
                        instance.setDate(dateStr);
                }
            });
    }

    function pacienteLookup(element) {

        var documento = $(element).val();
        var action = 'Pacientes/ObtenerPorDocumento';
        var params = {
            documento: documento
        };

        if (documento === '')
            return;

        Ajax.Execute(action, params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {

                        if (data == null) {
                            setPacienteData(element, null);

                            $(element)
                                .closest('.controls')
                                .find('span.fly-remove')
                                .remove();

                            $(element).removeAttr('readonly');

                            return;
                        }

                        setPacienteData(element, {
                            idPaciente: data.idPaciente,
                            nombre: data.nombre,
                            email: data.email,
                            telefono: data.telefono,
                            tipoTelefono: data.tipoTelefono,
                            documento: data.documento,
                            idFinanciador: data.idFinanciador,
                            idFinanciadorPlan: data.idFinanciadorPlan,
                            financiadorNro: data.financiadorNro,
                            fechaNacimiento: data.fechaNacimiento,
                            nacionalidad: data.nacionalidad
                        });


                        var controls = $(element).closest('.controls');

                        if (controls.find('span.fly-new').length == 0)
                            controls
                                .append(
                                    $('<span />')
                                        .addClass('fly-remove')
                                        .attr('title', 'Nuevo paciente')
                                        .html('<i class="far fa-times"></i>')
                            );

                        $(element).attr('readonly', 'readonly');

                    },
                    Ajax.ShowError
                );
            })
            .fail(Ajax.ShowError);

    }

    function setPacienteData(element, data) {
        $('.aUpdate').hide();
        $('.aNew').hide();

        var $paciente = $('#IdPaciente');
        var $documento = $('#Documento');
        var $nombre = $('#Nombre');
        var $email = $('#Email');
        var $telefono = $('#Telefono');

        var $tipoTelefono = $('#IdTipoTelefono');
        var $financiador = $('#IdFinanciador');
        var $financiadorNro = $('#FinanciadorNro');
        var $fechaNacimiento = $('#FechaNacimiento');
        var $nacionalidad = $('#Nacionalidad');

        if (data === undefined || data === null) {
            $paciente.val(0);
            $nombre.val('');
            $email.val('');
            $telefono.val('');
            $tipoTelefono.select2('val', null);
            $financiador.select2('val', null);
            $financiadorNro.val('');
            reloadPlanes(null, $financiador, null);
            $nacionalidad.val('ARGENTINA');
            $fechaNacimiento.val(moment(null).format(enums.FormatoFecha.DefaultFormat));

            if ($documento.val())
                $('.aNew').show();

            return;
        }

        $('.aUpdate').show();

        $documento.val(accounting.formatNumber(data.documento));

        $paciente.val(data.idPaciente);
        $nombre.val(data.nombre);
        $email.val(data.email);
        $telefono.val(data.telefono);
        $tipoTelefono.select2('val', data.idTipoTelefono);
        $financiador.select2('val', data.idFinanciador);
        reloadPlanes(null, $financiador, data.idFinanciadorPlan);
        $financiadorNro.val(data.financiadorNro);
        $fechaNacimiento.val(moment(data.fechaNacimiento).format(enums.FormatoFecha.DefaultFormat));
        $nacionalidad.val(data.nacionalidad);

        if (!data.idFinanciador && !data.idFinanciadorPlan) {
            $('#SinCobertura').prop('checked', true).trigger('change');
        }

    }

    function reloadPlanes(e, element, selection) {

        var target = (e == null ? element : e.target);
        var idPlan = $(target).val();
        var planes = $('#IdFinanciadorPlan');

        var action = 'Financiadores/GetPlanesByFinanciadorId';
        var params = {
            id: idPlan,
        };

        planes.find('option').remove();

        Ajax.Execute(action, params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {

                        planes.find('option').remove();

                        for (var i in data) {
                            var option = data[i];

                            planes.append(
                                $('<option />')
                                    .val(option.idFinanciadorPlan)
                                    .text(option.nombre)
                            );
                        }

                        planes.select2('val', selection);
                    },
                    Ajax.ShowError
                );
            })
            .fail(Ajax.ShowError);
    }

    function asignar($form) {

        if (!$form.valid())
            return;

        var idPaciente = $('#IdPaciente').val();
        var telefono = $('#Telefono').val();

        var action = RootPath + 'Pacientes/ObtenerPorTelefono';
        var params = {
            idPaciente: idPaciente,
            telefono: telefono
        };

        Ajax.Execute(action, params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {
                        console.log(data);

                        if (data) {
                            Utils.modalQuestion("Teléfono Existente", "Ya se ha ingresado un paciente con el mismo teléfono. Paciente: " + data.nombre + " . DNI: " + data.documento + ". ¿Desea continuar?.",
                                function (confirm) {
                                    if (confirm) {
                                        Utils.modalLoader();
                                        $form.submit();
                                    }
                                }, "CONTINUAR", "Cancelar", true);
                        } else {
                            Utils.modalQuestion("Sobre Turno", "¿Confirma el Sobre-Turno?.",
                                function (confirm) {
                                    if (confirm) {
                                        Utils.modalLoader();
                                        $form.submit();
                                    }
                                }, "CONTINUAR", "Cancelar", true);
                        }

                    },
                    Ajax.ShowError
                );
            })
            .fail(Ajax.ShowError);

    };

    this.ajaxAsignarBegin = function (context, arguments) {

        //Utils.modalLoader();
    }

    this.ajaxAsignarSuccess = function (context) {
        Utils.modalClose();

        //$('.sobreTurnosDialog').modal('hide');
        //$('.asignarDialog').modal('hide');

        Utils.modalInfo('Turno Asignado', 'Se ha agendado el SOBRE TURNO de manera correcta', 5000, undefined, function () { });
        setTimeout(function () { window.location = '/TurnosSalaEspera' }, 1000);
    }

    this.ajaxAsignarFailure = function (context) {
        Utils.modalClose();

        Ajax.ShowError(context);
    }
});

$(function () {
    TurnosAsignarSobreTurnoView.init();
});

//# sourceURL=asignarSobreTurno.js