/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMEmpresasView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var autocomplete;
    var btCopiarPlanCtas;
    var modalCopiarPlanCtas;


    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        modalCopiarPlanCtas = $('.modal.modalCopiarPlanCtas', mainClass);

        btCopiarPlanCtas = $('.toolbar-actions button.btCopiar', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }

    function buildControls() {

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'idEmpresa' },
            { data: 'razonSocial' },
            { data: 'tipoIVA' },
            { data: 'cuit' },
            { data: 'email' },
            { data: 'vencimientoCertificado', orderable: false, searchable: false, class: 'text-center', render: renderCertificado },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderAcciones }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idEmpresa', columns, order);

        $('input[name^=Provincia]').prop('readonly', true);
        $('input[name^=Localidad]').prop('readonly', true);
        $('input[name^=CodigoPostal]').prop('readonly', true);

        btCopiarPlanCtas.on('click', function (e) {
            modalCopiarPlanCtas.modal({ backdrop: 'static' });
        });

        modalCopiarPlanCtas.find('> .modal-dialog').addClass('modal-20');
        modalCopiarPlanCtas
            .on('shown.bs.modal', function () {

            })
            .on('hidden.bs.modal', function () {
                var form = $('form', this);
                Utils.resetValidator(form);
            });

        modalCopiarPlanCtas.find('button.btOkCopiar').on('click', function (e) {

            var row = dtView.api().row('.selected');

            var idEmpresaOrigen = $('select[name^=IdEmpresaCopiar]').select2('val');

            if (idEmpresaOrigen == "") {
                Utils.alertInfo("Seleccione una Empresa para Copiar el Plan de Cuentas");
                return false;
            }

            var params = {
                idEmpresaDestino: row.data().idEmpresa,
                idEmpresaOrigen: idEmpresaOrigen
            };

            iconLoader($('.btOkCopiar'), true);

            Ajax.Execute('/Empresas/CopiarPlanCtas/', params)
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function () {
                            Utils.alertSuccess("El Plan de Cuentas se Copió Correctamente.");
                            $(modalCopiarPlanCtas).modal('hide');
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError)
                .always(function () {
                    iconLoader($('.btOkCopiar'), false);
                });

        });
    }

    function iconLoader(btAction, flag) {

        if (flag) {

            var text = btAction.text();

            btAction
                .data('normal-text', btAction.text())
                .prop('disabled', true)
                .html('<i class="fas fa-cog fa-spin"></i> ' + text);
        }
        else {

            btAction
                .prop('disabled', false)
                .html('<i class="fa fa-paste"></i>' + btAction.data('normal-text'));
        }

    }

    function renderCertificado(data, type, row, meta) {

        if (data == null || data == undefined) {
            return '<i class="fas fa-certificate red" title="Empresa Sin Certificado AFIP"></i>';
        }

        if (moment().isAfter(data)) {
            return '<i class="fas fa-certificate yellow" title="Certificado AFIP Vencido"></i>';
        }

        return '<i class="fas fa-certificate green" title="Certificado AFIP Vigente"></i>';
    }

    function renderAcciones(data, type, row, meta) {

        return `
            <ul class="table-controls">
                <li>
                    <i class="fad fa-file-signature csr" title="Descargar CSR"></i>
                </li>
                <li>
                    <i class="fad fa-file-certificate crt" title="Upload Certificado"></i>
                </li>
            </ul>`;

    }

    function retrieveFileCSR() {

        var tr = $(this).closest('tr');
        var row = dtView.api().row(tr);

        Utils.download('/Empresas/DescargarCSR/', `id=${row.data().idEmpresa}`, 'POST');

    }

    function uploadFileCRT() {

        var tr = $(this).closest('tr');
        var row = dtView.api().row(tr);

        $('#fileUpload')
            .data('idEmpresa', row.data().idEmpresa)
            .trigger('click');
    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.csr', retrieveFileCSR);
        dtView.table().on('click', 'tbody > tr > td > ul > li > i.crt', uploadFileCRT);

        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.tabChanged = tabChanged;
        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.mainTableDraw = mainTableDraw;

        $('input[name^=Direccion]').on('focus', function () {
            $('input[name^=Direccion]').attr('autocomplete', 'new-password');
        });

        $('#fileUpload')
            .on('change', function (e) {
                var obj = this;
                var reader = new FileReader();
                reader.onload = function (e) {

                    var params = {
                        idEmpresa: $(obj).data('idEmpresa'),
                        crt: e.target.result.split(',')[1]
                    };

                    Ajax.Execute('/Empresas/UploadCRT/', params)
                        .done(function (response) {
                            Utils.alertSuccess('Certificado Guardado correctamente.');
                            dtView.reload();
                        })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            Utils.modalError('Error', jqXHR.responseJSON.detail);
                        });
                };
                reader.onerror = function (e) {
                    Utils.modalError("Error", "Error en leer el Archivo del Certificado");
                };
                reader.readAsDataURL(this.files[0]);
            });

        $(".recetaLogoInput").change(function () {
            readURL(this);
        });

        $('.imgRecetaLogo-delete-icon').on('click', function (e) {
            removeRecetaLogo();
        });
    }

    function editDialogOpening($form, entity) {

        $form.find('#IdEmpresa_Edit').val(entity.idEmpresa);
        $form.find('#RazonSocial_Edit').val(entity.razonSocial);
        $form.find('#NombreFantasia_Edit').val(entity.nombreFantasia);
        $form.find('#CUIT_Edit').DropDownGroup()
            .readonly(true)
            .val(entity.idTipoIVA, entity.cuit);
        $form.find('#NroIBr_Edit').val(entity.nroIBr);
        $form.find('#Telefono_Edit').DropDownGroup().val(entity.idTipoTelefono, entity.telefono);
        $form.find('#Email_Edit').val(entity.email);
        $form.find('#AgentePercepcionAGIP_Edit').prop('checked', entity.agentePercepcionAGIP);
        $form.find('#AgentePercepcionARBA_Edit').prop('checked', entity.agentePercepcionARBA);
        $form.find('#EnableProdAFIP_Edit').prop('checked', entity.enableProdAFIP);
        $form.find('#FechaInicioActividades_Edit').val(moment(entity.fechaInicioActividades).format(enums.FormatoFecha.DefaultFormat));

        $form.find('#Direccion_Edit').val(entity.direccion);
        $form.find('#Piso_Edit').val(entity.piso);
        $form.find('#Departamento_Edit').val(entity.departamento);
        $form.find('#Provincia_Edit').val(entity.provincia);
        $form.find('#Localidad_Edit').val(entity.localidad);
        $form.find('#CodigoPostal_Edit').val(entity.codigoPostal);

        $form.find('#TurnosHoraInicio_Edit').val(entity.turnosHoraInicio);
        $form.find('#TurnosMinutosInicio_Edit').val(entity.turnosMinutosInicio);
        $form.find('#TurnosHoraFin_Edit').val(entity.turnosHoraFin);
        $form.find('#TurnosMinutosFin_Edit').val(entity.turnosMinutosFin);
        $form.find('#TurnosIntervalo_Edit').val(entity.turnosIntervalo);

        if (entity.uploadedTipoRecetaLogo) {

            $('.imgRecetaLogo').attr('src', 'data:' + entity.uploadedTipoRecetaLogo + ';base64,' + entity.uploadedRecetaLogo);
            $('.recetaLogoPreview').show();
        } else {
            removeRecetaLogo();
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
            dialog.addClass('modal-50');
            dialog.find('.modal-footer .btLimpiar').remove();
        }

    }

    function mainTableRowSelected(dataArray, api) {

        btCopiarPlanCtas.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        btCopiarPlanCtas.prop('disabled', !dataArray.length == 1);

    }

    function mainTableDraw() {

        var data = dtView.selected();

        btCopiarPlanCtas.prop('disabled', true);

        if (data === undefined || data === null)
            return;

        btCopiarPlanCtas.prop('disabled', !data.length == 1);

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

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.imgRecetaLogo').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);
            $('.recetaLogoPreview').show();
        }
    };

    function removeRecetaLogo() {
        $('.removedRecetaLogo').val(true);
        $('.recetaLogoInput').val('');
        $('.recetaLogoPreview').hide();
    }
});

$(function () {
    ABMEmpresasView.init();
});
