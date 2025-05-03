/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMFinanciadoresView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var editMode = false;
    var btPrestaciones;
    var btPadron;

    var autocomplete;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);

        btPrestaciones = $('button.btPrestaciones', mainClass);
        btPadron = $('button.btPadron', mainClass);

        $('.modal .dvPlanes .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
            .on('repeater:row:added', repeaterRowAdded)
            .on('repeater:row:removed', repeaterRowRemoved)
            .on('repeater:field:focus', repeaterFieldFocus)
            .on('repeater:field:change', repeaterFieldChange)
            .on('repeater:field:blur', repeaterFieldBlur)
            .Repeater();

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }


    function buildControls() {

        MaestroLayout.errorTooltips = false;

        modalNew.find('> .modal-dialog').addClass('modal-70');
        modalEdit.find('> .modal-dialog').addClass('modal-70');

        var columns = [
            { data: 'idFinanciador' },
            { data: 'nombre' },
            { data: 'cuit' },
            { data: 'nroFinanciador' },
            { data: 'telefono' },
            { data: 'email' }
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idFinanciador', columns, order);

        $('input[name^=Provincia]').prop('readonly', true);
        $('input[name^=Localidad]').prop('readonly', true);
        $('input[name^=CodigoPostal]').prop('readonly', true);
    }

    function bindControlEvents() {

        btPrestaciones.on('click', goToPrestaciones);
        btPadron.on('click', goToPadron);

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.tabChanged = tabChanged;

        $('input[name^=Direccion]').on('focus', function () {
            $('input[name^=Direccion]').attr('autocomplete', 'new-password');
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

            Ajax.Execute('/Financiadores/Padron/', params)
                .done(function (response) {
                    var detail = response.detail;

                    if (detail.razonSocial == null) {
                        $('input[name^="Nombre"]').val(detail.nombre + " " + detail.apellido);
                    }
                    else {
                        $('input[name^="Nombre"]').val(detail.razonSocial);
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

                            $('select[name^=Empresas]', $form).select2('val', 1);
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

    function mainTableRowSelected(dataArray, api) {
                
        btPrestaciones.prop('disabled', true);
        btPadron.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        if (dataArray.length == 1) {
            btPrestaciones.prop('disabled', false);
            btPadron.prop('disabled', false);
        }
    }

    function newDialogOpening(dialog, $form) {

        editMode = false;

        $form.find('.dvPlanes .repeater-component').Repeater()
            .clear();
    }

    function editDialogOpening($form, entity) {

        editMode = true;

        $form.find('#IdFinanciador_Edit').val(entity.idFinanciador);
        $form.find('#NroFinanciador_Edit').val(entity.nroFinanciador);
        $form.find('#Capita_Edit').prop('checked', entity. capita);
        $form.find('#Nombre_Edit').val(entity.nombre);
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

        if (entity.items.length > 0)
            $form.find('.dvPlanes .repeater-component').Repeater()
                .source(entity.items);
        else
            $form.find('.dvPlanes .repeater-component').Repeater()
                .clear();
    }

    function tabChanged(dialog, $form, index, name) {

        if (index >= 1) {

            dialog.addClass('modal-70');

            if (index != 1) { 

                dialog.find('.modal-footer .btLimpiar').remove();
            }
        } else {

            dialog.removeClass('modal-70');
            dialog.find('.modal-footer .btLimpiar').remove();
        }

        if (index == 1) {

            var clean = $('<button />')
                .attr('type', 'button')
                .addClass('btn btn-danger btLimpiar')
                .html('<i class="far fa-eraser"></i> Limpiar')
                .on('click', function (e) {
                    cleanAddress(dialog, index, e);
                });

            dialog.find('.modal-footer').prepend(clean);
        }
    }


    this.initAutocomplete = function () {

        var forms = $('.modal form');

        forms.each(function (idx, form) {

            address1Field = $('input[name^=Direccion]', form);

            // Create the autocomplete object, restricting the search predictions to
            // addresses in the US and Canada.
            autocomplete = new google.maps.places.Autocomplete(address1Field.get(0), {
                fields: ["place_id", "formatted_address", "address_components", "geometry"],
                types: ["address"]
            });

            autocomplete.setOptions({ strictBounds: true });

            address1Field.focus();

            // When the user selects an address from the drop-down, populate the
            // address fields in the form.
            autocomplete.addListener("place_changed", function () {
                fillInAddress(autocomplete.getPlace(), form);
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

    function repeaterInitialization() {
        console.log('repeater iniciado');
    }

    function repeaterRowAdded(e, $newRow, $rows) {

        //rebuildItemNumbers($rows);
        ConstraintsMaskEx.init();

    }

    function repeaterRowRemoved(e, $removedRow, $rows) {

        //rebuildItemNumbers($rows);

    }

    function repeaterFieldFocus(e, data) {

    }

    function repeaterFieldChange(e, data) {

    }

    function repeaterFieldBlur(e, data) {

    }

    function goToPrestaciones() {
        var selectedRows = dtView.selected();
        var idFinanciador = selectedRows[0].idFinanciador;

        window.location = '/PrestacionesFinanciador/?idFinanciador=' + idFinanciador; 
    }

    function goToPadron() {
        var selectedRows = dtView.selected();
        var idFinanciador = selectedRows[0].idFinanciador;

        window.location = '/FinanciadoresPadron/?idFinanciador=' + idFinanciador;
    }

});

$(function () {
    ABMFinanciadoresView.init();
});
