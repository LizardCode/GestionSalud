/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMClientesView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var activeFormat;


    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }


    function buildControls() {

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'idCliente' },
            { data: 'razonSocial' },
            { data: 'tipoIVA' },
            { data: 'documentoCUIT' },
            { data: 'telefono' },
            { data: 'email' },
            { data: 'localidad' },
            { data: 'direccion', width: 350, render: DataTableEx.renders.ellipsis }
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idCliente', columns, order);

        $('input[name^=Provincia]').prop('readonly', true);
        $('input[name^=Localidad]').prop('readonly', true);
        $('input[name^=CodigoPostal]').prop('readonly', true);
    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.tabChanged = tabChanged;

        $('input[name^=Direccion]').on('focus', function () {
            $('input[name^=Direccion]').attr('autocomplete', 'new-password');
        });

        $('#IdTipoDocumento').on('change', function (e) {

            $('#CUIT').val('');

            if (activeFormat != null) {
                activeFormat.destroy();
                activeFormat = null;
            }

            switch (parseInt($(this).val())) {
                case enums.TipoDocumento.CUIT:
                case enums.TipoDocumento.CUIL:
                    activeFormat = new Cleave('#CUIT', {
                        blocks: [2, 8, 1],
                        delimiter: '-',
                        numericOnly: true
                    });
                    break;

                default:
                    activeFormat = new Cleave('#CUIT', {
                        blocks: [8],
                        numericOnly: true
                    });
                    break;
            }

        });

        $('#IdTipoDocumento_Edit').on('change', function (e) {

            $('#CUIT_Edit').val('');

            if (activeFormat != null) {
                activeFormat.destroy();
                activeFormat = null;
            }

            switch (parseInt($(this).val())) {
                case enums.TipoDocumento.CUIT:
                case enums.TipoDocumento.CUIL:
                    activeFormat = new Cleave('#CUIT_Edit', {
                        blocks: [2, 8, 1],
                        delimiter: '-',
                        numericOnly: true,
                        onValueChanged: function (e) {
                            // e.target = { value: '5100-1234', rawValue: '51001234' }
                        }
                    });
                    break;

                default:
                    activeFormat = new Cleave('#CUIT_Edit', {
                        blocks: [8],
                        numericOnly: true
                    });
                    break;
            }

        });

        $('.btnCUIT').on('click', function (e) {

            var $form = $(e.target).closest('form');

            var btnObj = this;
            var cuitVal = $('input[name^="CUIT"]').val();
            if (cuitVal == "" || cuitVal == undefined || cuitVal == "__-________-_")
                return;

            if (cuitVal.replace("_", "").length != 13)
                return;

            var params = {
                cuit: cuitVal
            };

            $(btnObj).prop('disabled', true);

            Ajax.Execute('/Clientes/Padron/', params)
                .done(function (response) {
                    var detail = response.detail;

                    if (detail.razonSocial == null) {
                        $('input[name^="RazonSocial"]').val(detail.nombre + " " + detail.apellido);
                        $('input[name^="NombreFantasia"]').val(detail.nombre + " " + detail.apellido)
                    }
                    else {
                        $('input[name^="RazonSocial"]').val(detail.razonSocial);
                        $('input[name^="NombreFantasia"]').val(detail.razonSocial)
                    }

                    $('select[name^="IdTipoIVA"]').select2('val', null);
                    if (detail.idTipoIVA != 0) {
                        $('select[name^="IdTipoIVA"]').select2('val', detail.idTipoIVA);
                    }

                    var address = `${detail.direccion}, ${detail.localidad} ${detail.provincia}`;
                    var geocoder = new google.maps.Geocoder();
                    geocoder.geocode({ 'address': address }, function (results, status) {
                        if (status === google.maps.GeocoderStatus.OK) {
                            $('input[name^=Direccion]', $form).val(results[0].formatted_address);
                            fillInAddress(results[0], $form);
                            $('input[name^=NroIBr]', $form).val(detail.cuit);
                            $('input[name^=Telefono]', $form).val("CAMBIAR");
                            $('input[name^=Email]', $form).val("cambiar@dawasoft.com.ar");

                            $('select[name^="IdTipoIVA"]').trigger('click');
                        }
                    });

                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    Utils.modalError('Error', jqXHR.responseJSON.detail);
                })
                .always(function () {
                    $(btnObj).prop('disabled', false);
                });
        });
    }

    function newDialogOpening(dialog, $form) {

        $('#CUIT').val('');

        if (activeFormat != null) {
            activeFormat.destroy();
            activeFormat = null;
        }

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdCliente_Edit').val(entity.idCliente);
        $form.find('#RazonSocial_Edit').val(entity.razonSocial);
        $form.find('#NombreFantasia_Edit').val(entity.nombreFantasia);
        $form.find('#Documento_Edit').val(entity.documento);
        $form.find('#IdTipoIVA_Edit').select2('val', entity.idTipoIVA);
        $form.find('#CUIT_Edit').val(entity.cuit);
        $form.find('#NroIBr_Edit').val(entity.nroIBr);
        $form.find('#Telefono_Edit').DropDownGroup().val(entity.idTipoTelefono, entity.telefono);
        $form.find('#Email_Edit').val(entity.email);

        $form.find('#Direccion_Edit').val(entity.direccion);
        $form.find('#Piso_Edit').val(entity.piso);
        $form.find('#Departamento_Edit').val(entity.departamento);
        $form.find('#Provincia_Edit').val(entity.provincia);
        $form.find('#Localidad_Edit').val(entity.localidad);
        $form.find('#CodigoPostal_Edit').val(entity.codigoPostal);

        if (activeFormat != null) {
            activeFormat.destroy();
            activeFormat = null;
        }

    }

    function tabChanged(dialog, $form, index, name) {

        if (index == 1) {
            dialog.addClass('modal-60');

            var clean = $('<button />')
                .attr('type', 'button')
                .addClass('btn btn-danger btLimpiar')
                .html('<i class="far fa-eraser"></i> Limpiar')
                .on('click', function (e) {
                    cleanAddress(dialog, index, e);
                });

            dialog.find('.modal-footer').prepend(clean);
        }
        else {
            dialog.removeClass('modal-60');
            dialog.find('.modal-footer .btLimpiar').remove();
        }

    }


    this.initAutocomplete = function () {

        var forms = $('.modal form');

        forms.each(function (idx, form) {

            address1Field = $('input[name^=Direccion]', form);

            // Create the autocomplete object, restricting the search predictions to
            // addresses in the US and Canada.
            var autocomplete = new google.maps.places.Autocomplete(address1Field.get(0), {
                componentRestrictions: { country: ["ar"] },
                fields: ["place_id", "formatted_address", "address_components", "geometry"],
                strictBounds: false,
                types: ["address"],
            });

            autocomplete.setOptions({ strictBounds: true });

            address1Field.focus();

            // When the user selects an address from the drop-down, populate the
            // address fields in the form.
            google.maps.event.addListener(autocomplete, "place_changed", function () {
                fillInAddress(this.getPlace(), form);
            });

        });

    }

    function fillInAddress(place, form) {

        // Get the place details from the autocomplete object.
        var address2Field = $('textarea[name=Referencia]', form);
        var latField = $("input[name^='Latitud']", form);
        var longField = $("input[name^='Longitud']", form);
        var provinceField = $("input[name^='Provincia']", form);
        var cityField = $("input[name^='Localidad']", form);
        var postalField = $("input[name^='CodigoPostal']", form);

        // Get each component of the address from the place details,
        // and then fill-in the corresponding field on the form.
        // place.address_components are google.maps.GeocoderAddressComponent objects
        // which are documented at http://goo.gle/3l5i5Mr

        var cs = place.address_components;
        //var number = getComponent(cs, 'street_number');
        //var route = getComponent(cs, 'route');
        var postal = getComponent(cs, 'postal_code');
        var postal_suffix = getComponent(cs, 'postal_code_suffix');
        var locality = getComponent(cs, 'locality');
        var sublocality = getComponent(cs, 'sublocality_level_1');
        var admin1 = getComponent(cs, 'administrative_area_level_1');
        var admin2 = getComponent(cs, 'administrative_area_level_2');

        var postcode = postal.long_name + postal_suffix.long_name;
        var province = admin1.short_name;
        var city = locality.long_name;

        if (province == 'CABA') {
            if (sublocality.long_name != '')
                city = sublocality.long_name;
            else if (admin2.long_name != '')
                city = admin2.long_name;
        }

        postalField.val(postcode);
        cityField.val(city);
        provinceField.val(province);
        latField.val(place.geometry.location.lat());
        longField.val(place.geometry.location.lng());

        // After filling the form with address components from the Autocomplete
        // prediction, set cursor focus on the second address line to encourage
        // entry of subpremise information such as apartment, unit, or floor number.
        address2Field.focus();
    }

    function getComponent(components, name) {

        var result = components.filter(function (f) {
            return f.types[0] == name;
        });

        if (result.length > 0)
            return result[0];
        else
            return {
                long_name: '',
                short_name: ''
            };

    }

    function cleanAddress(dialog, index, e) {

        var tab = dialog.find('.tab-content .tab-pane.tab-0' + index);

        tab.find('input[type=text], textarea').val('');
        tab.find('input[name=Direccion]').focus();

    }
});

$(function () {
    ABMClientesView.init();
});
