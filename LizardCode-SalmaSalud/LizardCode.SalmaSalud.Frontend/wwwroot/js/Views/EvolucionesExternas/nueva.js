/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMEvolucionesView = new (function () {

    this.init = function () {

        $('.dvPrestaciones .repeater-component')
            .on('repeater:init', repeaterPrestacionesInitialization)
            .on('repeater:row:added', repeaterPrestacionesRowAdded)
            .on('repeater:row:removed', repeaterPrestacionesRowRemoved)
            .on('repeater:field:focus', repeaterPrestacionesFieldFocus)
            .on('repeater:field:change', repeaterPrestacionesFieldChange)
            .on('repeater:field:blur', repeaterPrestacionesFieldBlur)
            .Repeater({
                dropdownAutoWidth: false,
                select2Width: 'element',
            })
            .comboSource('IdPrestacion', []);

        $('.dvOtrasPrestaciones .repeater-component')
            .on('repeater:init', repeaterOtrasPrestacionesInitialization)
            .on('repeater:row:added', repeaterOtrasPrestacionesRowAdded)
            .on('repeater:row:removed', repeaterOtrasPrestacionesRowRemoved)
            .on('repeater:field:focus', repeaterOtrasPrestacionesFieldFocus)
            .on('repeater:field:change', repeaterOtrasPrestacionesFieldChange)
            .on('repeater:field:blur', repeaterOtrasPrestacionesFieldBlur)
            .Repeater({
                dropdownAutoWidth: false,
                select2Width: 'element'
            });

        $('.dvRecetas .master-detail-component')
            .on('master-detail:init', masterDetailInitialization)
            .on('master-detail:dialog-add-opening', masterDetailDialogAddOpening)
            .on('master-detail:dialog-opened', masterDetailDialogOpened)
            .on('master-detail:row:added', masterDetailRowAdded)
            .on('master-detail:row:edited', masterDetailRowEdited)
            .on('master-detail:row:removed', masterDetailRowRemoved)
            .on('master-detail:source:loaded', masterDetailSourceLoaded)
            .on('master-detail:empty', masterDetailEmpty)
            .MasterDetail({
                modalSelector: '.modalMasterDetail',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });

        $('.dvOrdenes .master-detail-component')
            .on('master-detail:init', masterDetailOrdenesInitialization)
            .on('master-detail:dialog-opened', masterDetailOrdenesDialogOpened)
            .on('master-detail:row:added', masterDetailOrdenesRowAdded)
            .on('master-detail:row:edited', masterDetailOrdenesRowEdited)
            .on('master-detail:row:removed', masterDetailOrdenesRowRemoved)
            .on('master-detail:source:loaded', masterDetailOrdenesSourceLoaded)
            .on('master-detail:empty', masterDetailOrdenesEmpty)
            .MasterDetail({
                modalSelector: '.modalMasterDetailOrdenes',
                readonly: false,
                disableRemove: false,
                disableAdd: false,
                disableEdit: false
            });

        buildControls();
        bindControlEvents();
    }


    function buildControls() {

        Utils.rebuildValidator($('.frmNuevaEvolucion'), ':hidden:not(.validate):not(.validate :hidden), .no-validate');
    }

    function bindControlEvents() {
        $('.ABMEvolucionesView').on('change', 'select[name$=".IdPrestacion"]', setPrestacion);
        $('.ABMEvolucionesView').on('change', 'select[name$=".IdOtraPrestacion"]', setOtraPrestacion);

        $('#Fecha')
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

        $('.btSave')
            .on('click', function () {
                guardar($('.frmNuevaEvolucion'));
            });

        $('.frmNuevaEvolucion').on('click', '.fly-remove', function () {

            $('.pacienteDocumento').val('');
            $('.pacienteDocumento').removeAttr('readonly');
            setPacienteData($('.pacienteDocumento'), null);

            $('.pacienteDocumento')
                .closest('.controls')
                .find('span.fly-remove')
                .remove();
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

        $('select[name$="IdFinanciadorPlan"]').on('change', function (e) {
            changeFinanciadorPlan(e);
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

        $('.btnBusqueda')
                .on('click', function () {
                    var action = $(this).closest('span').data('ajax-action');
                    var widthClass = $(this).closest('span').data('width-class');

                    Modals.loadAnyModal('busquedaDialog', widthClass, action, function () {
                        BUSQUEDA_DOCUMENTO_PACIENTE = 0;
                    }, function () {
                        if (BUSQUEDA_DOCUMENTO_PACIENTE) {

                            $('.pacienteDocumento').val(BUSQUEDA_DOCUMENTO_PACIENTE);
                            pacienteLookup($('.pacienteDocumento'));
                            BUSQUEDA_DOCUMENTO_PACIENTE = 0;
                        }
                    });
                });
    }

    function repeaterPrestacionesInitialization() {
        console.log('repeater iniciado');
    }

    function repeaterPrestacionesRowAdded(e, $newRow, $rows) {

        //rebuildItemNumbers($rows);
        ConstraintsMaskEx.init();
        setTotalPrestaciones(e);
    }

    function repeaterPrestacionesRowRemoved(e, $removedRow, $rows) {

        //rebuildItemNumbers($rows);
        setTotalPrestaciones(e);

    }

    function repeaterPrestacionesFieldFocus(e, data) {

    }

    function repeaterPrestacionesFieldChange(e, data) {

        setTotalPrestaciones(e);
    }

    function repeaterPrestacionesFieldBlur(e, data) {

        setTotalPrestaciones(e);
    }

    function repeaterOtrasPrestacionesInitialization() {
        console.log('repeater iniciado');
    }

    function repeaterOtrasPrestacionesRowAdded(e, $newRow, $rows) {

        //rebuildItemNumbers($rows);
        ConstraintsMaskEx.init();
        setTotalOtrasPrestaciones(e);

    }

    function repeaterOtrasPrestacionesRowRemoved(e, $removedRow, $rows) {

        //rebuildItemNumbers($rows);
        setTotalOtrasPrestaciones(e);

    }

    function repeaterOtrasPrestacionesFieldFocus(e, data) {
        setTotalOtrasPrestaciones(e);

    }

    function repeaterOtrasPrestacionesFieldChange(e, data) {

        setTotalOtrasPrestaciones(e);
    }

    function repeaterOtrasPrestacionesFieldBlur(e, data) {

        setTotalOtrasPrestaciones(e);
    }

    function setPrestacion(e) {

        var tr = $(e.target).closest('tr');
        var prestacion = tr.find('select[name$=".IdPrestacion"]');
        var action = '/Financiadores/ObtenerPrestacion';
        var params = {
            id: prestacion.val()
        };

        Ajax.GetJson(action, params)
            .done(function (response) {

                var codigo = tr.find('input[name$=".Codigo"]');
                codigo.val(response.codigo);

                var valor = tr.find('input[name$=".CoPago"]');
                AutoNumeric.set(valor.get(0), response.coPago);

                var valor = tr.find('input[name$=".Valor"]');
                AutoNumeric.set(valor.get(0), response.valor);

                setTotalPrestaciones({ target: tr.parents('table') });

            }).fail(Ajax.ShowError);
    }

    function setOtraPrestacion(e) {

        var tr = $(e.target).closest('tr');
        var prestacion = tr.find('select[name$=".IdOtraPrestacion"]');
        var action = '/Prestaciones/Obtener';
        var params = {
            id: prestacion.val()
        };

        Ajax.GetJson(action, params)
            .done(function (response) {

                var codigo = tr.find('input[name$=".Codigo"]');
                codigo.val(response.codigo);

                var valor = tr.find('input[name$=".Valor"]');
                AutoNumeric.set(valor[0], response.valor);

                setTotalOtrasPrestaciones({ target: tr.parents('table') });

            }).fail(Ajax.ShowError);
    }

    function guardar($form) {

        //!$form.valid();
        if (!$form.valid())
            return;

        Utils.modalQuestion("Guardar Evolución", "¿Confirma el guardado de la evolución?.",
            function (confirm) {
                if (confirm) {
                    Utils.modalLoader();

                    //Borro los hiddens que haya...
                    $('.hiddensZonas').html('');

                    var j = 0;                    
                    $.each(OdontogramaView.piezas || [], function (i, pieza) {
                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', 'Piezas[' + j.toString() + '].Pieza')
                            .attr('value', pieza.pieza)
                            .appendTo(".hiddensZonas");
                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', 'Piezas[' + j.toString() + '].Caries')
                            .attr('value', pieza.caries)
                            .appendTo(".hiddensZonas");

                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', 'Piezas[' + j.toString() + '].Corona')
                            .attr('value', pieza.corona)
                            .appendTo(".hiddensZonas");

                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', 'Piezas[' + j.toString() + '].PrFija')
                            .attr('value', pieza.prFija)
                            .appendTo(".hiddensZonas");

                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', 'Piezas[' + j.toString() + '].PrRemovible')
                            .attr('value', pieza.prRemovible)
                            .appendTo(".hiddensZonas");

                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', 'Piezas[' + j.toString() + '].Amalgama')
                            .attr('value', pieza.amalgama)
                            .appendTo(".hiddensZonas");

                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', 'Piezas[' + j.toString() + '].Ausente')
                            .attr('value', pieza.ausente)
                            .appendTo(".hiddensZonas");

                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', 'Piezas[' + j.toString() + '].Ortodoncia')
                            .attr('value', pieza.ortodoncia)
                            .appendTo(".hiddensZonas");

                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', 'Piezas[' + j.toString() + '].Extraccion')
                            .attr('value', pieza.extraccion)
                            .appendTo(".hiddensZonas");

                        $('<input>')
                            .attr('type', 'hidden')
                            .attr('name', 'Piezas[' + j.toString() + '].Observaciones')
                            .attr('value', pieza.observaciones)
                            .appendTo(".hiddensZonas");

                        $.each(pieza.zonas || [], function (k, zona) {

                            $('<input>')
                                .attr('type', 'hidden')
                                .attr('name', 'Piezas[' + j.toString() + '].Zonas[' + k.toString() + '].Zona')
                                .attr('value', zona.zona)
                                .appendTo(".hiddensZonas");

                            $('<input>')
                                .attr('type', 'hidden')
                                .attr('name', 'Piezas[' + j.toString() + '].Zonas[' + k.toString() + '].TipoTrabajo')
                                .attr('value', zona.tipoTrabajo)
                                .appendTo(".hiddensZonas");

                        });

                        j++;
                    });

                    $form.submit();
                }
            }, "GUARDAR", "Cancelar", true);

    };

    this.ajaxNuevaEvolucionBegin = function (context, arguments) {

        //Utils.modalLoader();
    }

    this.ajaxNuevaEvolucionSuccess = function (context) {
        Utils.modalClose();

        //$('.actionsDialog').modal('hide');

        Utils.modalInfo('Evolución Ingresada', 'Se ha ingresado la EVOLUCIÓN de manera correcta', 5000, undefined, function () { });
        setTimeout(function () { window.location = '/EvolucionesExternas' }, 1000);
    }

    this.ajaxNuevaEvolucionFailure = function (context) {
        Utils.modalClose();

        Ajax.ShowError(context);
    }

    function setTotalPrestaciones(e) {

        var total = 0;
        var totalCoPagos = 0;
        var trs = $(e.target).find('tbody > tr');

        trs.each(function () {
            var tr = $(this);
            //var importe = tr.find('> td > input[name$=".Valor"]');
            //var importef = AutoNumeric.getNumber(importe.get(0));
            //total += importef;

            var importe = tr.find('> td > input[name$=".CoPago"]');
            var importef = AutoNumeric.getNumber(importe.get(0));
            totalCoPagos += importef;
        });

        //$('.totalPrestaciones').text(`${accounting.formatMoney(total)}`);
        $('.totalCoPagos').text(`${accounting.formatMoney(totalCoPagos)}`);
    }

    function setTotalOtrasPrestaciones(e) {

        //var total = 0;
        //var trs = $(e.target).find('tbody > tr');

        //trs.each(function () {
        //    var tr = $(this);
        //    var importe = tr.find('> td > input[name$=".Valor"]');
        //    var importef = AutoNumeric.getNumber(importe.get(0));

        //    total += importef;
        //});

        //$('.totalOtrasPrestaciones').text(`${accounting.formatMoney(total)}`);
    }

    function masterDetailInitialization() {
        console.log('MasterDetail inicializado');
    }

    function masterDetailDialogAddOpening(e, dialog, $form) {

        $('input[name^="Receta.IdVademecum"]')
            .Select2Ex({ url: '/TurnosSalaEspera/GetMedicamentos', allowClear: true, placeholder: 'Buscar...', minimumInputLength: 1 });

    }

    function masterDetailDialogOpened(e, dialog, $form) {
        console.log('MasterDetail Dialog Opened');
    }

    function masterDetailRowAdded(e, item, items, $row, $rows) {
        console.log('MasterDetail item agregado', item);
    }

    function masterDetailRowEdited(e, item, items, $row, $rows) {
        console.log('MasterDetail item editado', item);
    }

    function masterDetailRowRemoved(e, item, items, $row, $rows) {
        console.log('MasterDetail item eliminado', item, items);
    }

    function masterDetailSourceLoaded(e, items, $rows) {
        console.log('MasterDetail lista de items cargada', items, $rows);
    }

    function masterDetailEmpty(e) {
        console.log('MasterDetail items eliminados');
    }



    function masterDetailOrdenesInitialization() {
        console.log('MasterDetailOrdenes inicializado');
    }

    function masterDetailOrdenesDialogOpened(e, dialog, $form) {
        console.log('MasterDetailOrdenes Dialog Opened');
    }

    function masterDetailOrdenesRowAdded(e, item, items, $row, $rows) {
        console.log('MasterDetailOrdenes item agregado', item);
    }

    function masterDetailOrdenesRowEdited(e, item, items, $row, $rows) {
        console.log('MasterDetailOrdenes item editado', item);
    }

    function masterDetailOrdenesRowRemoved(e, item, items, $row, $rows) {
        console.log('MasterDetailOrdenes item eliminado', item, items);
    }

    function masterDetailOrdenesSourceLoaded(e, items, $rows) {
        console.log('MasterDetailOrdenes lista de items cargada', items, $rows);
    }

    function masterDetailOrdenesEmpty(e) {
        console.log('MasterDetailOrdenes items eliminados');
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

                        planes.select2('val', selection, true);
                    },
                    Ajax.ShowError
                );
            })
            .fail(Ajax.ShowError);
    }

    function changeFinanciadorPlan(event, source) {
        var idFinanciadorPlan = $('select[name$="IdFinanciadorPlan"]').val() || 0;

        //Vacío el repeater
        $('.dvPrestaciones .repeater-component').Repeater()
            .clear();
        
        $('.dvPrestaciones .repeater-component')
            .on('repeater:init', repeaterOtrasPrestacionesInitialization)
            .on('repeater:row:added', repeaterOtrasPrestacionesRowAdded)
            .on('repeater:row:removed', repeaterOtrasPrestacionesRowRemoved)
            .on('repeater:field:focus', repeaterOtrasPrestacionesFieldFocus)
            .on('repeater:field:change', repeaterOtrasPrestacionesFieldChange)
            .on('repeater:field:blur', repeaterOtrasPrestacionesFieldBlur)
            .Repeater({
                dropdownAutoWidth: false,
                select2Width: 'element'
            })
            .comboSource('IdPrestacion', []);

        //Voy al server a completar el maestro
        $.get("/EvolucionesExternas/GetPrestacionesByFinanciadorPlan?idFinanciadorPlan=" + idFinanciadorPlan, function (response) {
            Ajax.ParseResponse(response,
                function (data) {
                    if (data && data.length) {

                        $('.dvPrestaciones .repeater-component')
                            .on('repeater:init', repeaterOtrasPrestacionesInitialization)
                            .on('repeater:row:added', repeaterOtrasPrestacionesRowAdded)
                            .on('repeater:row:removed', repeaterOtrasPrestacionesRowRemoved)
                            .on('repeater:field:focus', repeaterOtrasPrestacionesFieldFocus)
                            .on('repeater:field:change', repeaterOtrasPrestacionesFieldChange)
                            .on('repeater:field:blur', repeaterOtrasPrestacionesFieldBlur)
                            .Repeater({
                                dropdownAutoWidth: false,
                                select2Width: 'element'
                            })
                            .comboSource('IdPrestacion', data, true);
                    }
                },
                Ajax.ShowError
            );
        });
    }
});

$(function () {
    ABMEvolucionesView.init();
});

//# sourceURL=nuevaEvolucion.js