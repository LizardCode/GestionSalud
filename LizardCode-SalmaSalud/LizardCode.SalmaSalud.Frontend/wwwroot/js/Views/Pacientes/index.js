/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMPacientesView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var modalNew;
    var modalEdit;
    var btNew;
    var btEdit;
    var btRemove;

    var btHistoriaClinica;

    var openingEditDialog = false;
    var isViewDialog = false;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);

        btNew = $('.toolbar-actions button.btNew', mainClass);
        btEdit = $('.toolbar-actions button.btEdit', mainClass);
        btRemove = $('.toolbar-actions button.btRemove', mainClass);
        btHistoriaClinica = $('.toolbar-actions button.btHistoriaClinica', mainClass);
        btView = $('.toolbar-actions button.btView', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

        var tipoUsuario = $('#hidLoggedUserTipo').val();
        btNew.prop('disabled', tipoUsuario == enums.TipoUsuario.Profesional);
        if (tipoUsuario == enums.TipoUsuario.Recepcion) {
            btHistoriaClinica.hide();
        } else if (tipoUsuario == enums.TipoUsuario.Profesional) {
            btNew.hide();
            btEdit.hide();
            btRemove.hide();
        }
    }

    function buildControls() {

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'idPaciente' },
            { data: 'nombre' },
            { data: 'documento' },
            { data: 'financiadorNro' },
            { data: 'telefono' },
            { data: 'email' },
            { data: null, render: renderHabilitado, class: 'text-center' },
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idPaciente', columns, order, 20);

        $('input[name^=Provincia]').prop('readonly', true);
        $('input[name^=Localidad]').prop('readonly', true);
        $('input[name^=CodigoPostal]').prop('readonly', true);

        $('select[name^="IdNacionalidad"]').select2({ allowClear: true });
    }

    function bindControlEvents() {

        btHistoriaClinica.on('click', historiaClinicaDialog);
        btView.on('click', viewDialog);

        $('select[name$="IdFinanciador"]').on('change', function (e) {
            reloadPlanes(e);
        });

        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.tabChanged = tabChanged;

        //$('input[name^=SinCobertura]').change(function () {
        //    var checked = $(this).is(':checked');

        //    $('.financiador').prop('disabled', checked);
        //    //$('select[name^="IdFinanciador"]').prop('disabled', checked);
        //    $('.financiadorPlan').prop('disabled', checked);
        //    //$('select[name^="IdFinanciadorPla"]').prop('disabled', checked);
        //    $('.financiadorNro').prop('disabled', checked);
        //});
    }

    function mainTableRowSelected(dataArray, api) {

        //btNew.prop('disabled', tipoUsuario == enums.TipoUsuario.Profesional);
        var tipoUsuario = $('#hidLoggedUserTipo').val();

        btHistoriaClinica.prop('disabled', true);
        btEdit.prop('disabled', true);
        btRemove.prop('disabled', true);
        btView.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        if (dataArray.length == 1) {
            btHistoriaClinica.prop('disabled', tipoUsuario == enums.TipoUsuario.Recepcion);    
            btEdit.prop('disabled', tipoUsuario == enums.TipoUsuario.Profesional);
            btRemove.prop('disabled', tipoUsuario != enums.TipoUsuario.Administrador);
            btView.prop('disabled', false);
        }
    }

    function editDialogOpening($form, entity) {

        openingEditDialog = true;

        $form.find('#IdPaciente_Edit').val(entity.idPaciente);
        $form.find('#Nombre_Edit').val(entity.nombre);
        $form.find('#Documento_Edit').val(entity.documento);
        $form.find('#FechaNacimiento_Edit').val(moment(entity.fechaNacimiento).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#Nacionalidad_Edit').val(entity.nacionalidad);

        $form.find('#IdFinanciador_Edit').select2('val', entity.idFinanciador);
        $form.find('#FinanciadorNro_Edit').val(entity.financiadorNro);

        $form.find('#Telefono_Edit').val(entity.telefono);
        $form.find('#Email_Edit').val(entity.email);
        $form.find('#Habilitado_Edit').prop('checked', entity.habilitado);
        $form.find('#IdNacionalidad_Edit').select2('val', entity.idNacionalidad);

        reloadPlanes(null, $form.find('#IdFinanciador_Edit'), entity.idFinanciadorPlan);

        if (!entity.idFinanciador) {
            $('#SinCobertura_Edit').prop('checked', true).trigger('change');
        }

        if (isViewDialog) {
            $form.find('input[type=text]').attr("disabled", true);
            $form.find('input[type=checkbox]').attr("disabled", true);
            $form.find('select').attr("disabled", true);
            $('.btSave').hide();
        } else {
            $form.find('input[type=text]').attr("disabled", false);
            $form.find('input[type=checkbox]').attr("disabled", false);
            $form.find('select').attr("disabled", false);

            $("#Documento_Edit").attr("disabled", true);
            $("#IdPaciente_Edit").attr("disabled", true);
            $('.btSave').show();
        }

        isViewDialog = false;
    }

    function renderHabilitado(data, type, row) {

        if (row.habilitado)
            return '<span class="badge badge-pills badge-success font10"><i class="fa fa-check"></i></span>';
        else
            return '<span class="badge badge-pills badge-danger font10"><i class="fa fa-times"></i></span>';
    }

    function renderFinanciador(data, type, row) {

        if (row.financiador)
            return row.financiador + ' - ' + row.financiadorPlan;
        else
            return '';
    }

    function reloadPlanes(e, element, selection) {

        var target = (e == null ? element : e.target);
        var form = $(target).closest('form');
        var idPlan = $(target).val();
        var planes = form.find('select[name$="IdFinanciadorPlan"]');

        var action = 'Financiadores/GetPlanesByFinanciadorId';
        var params = {
            id: idPlan,
        };

        Ajax.Execute(action, params)
            .done(function (response) {
                Ajax.ParseResponse(response,
                    function (data) {

                        var selectedValue;
                        if (openingEditDialog) {
                            selectedValue = selection;
                            openingEditDialog = false;
                        }

                        planes.find('option').remove();

                        //planes.append(
                        //    $('<option />').val('')
                        //);

                        for (var i in data) {
                            var option = data[i];

                            planes.append(
                                $('<option />')
                                    .val(option.idFinanciadorPlan)
                                    .text(option.nombre)
                            );
                        }

                        if (selectedValue) {
                            planes.select2('val', selectedValue)
                        }
                    },
                    Ajax.ShowError
                );
            })
            .fail(Ajax.ShowError);
    }

    function historiaClinicaDialog() {

        var selectedRows = dtView.selected();
        var idPaciente = selectedRows[0].idPaciente;
        var action = '/Pacientes/HistoriaClinicaView?id=' + idPaciente + '&showResumenPaciente=true';

        Modals.loadAnyModal('historiaClinicaDialog', 'modal-95', action, function () {}, function () {});
    }

    function viewDialog() {

        isViewDialog = true;
        $('.btEdit').click();
    }

    function tabChanged(dialog, $form, index, name) {

        dialog.addClass('modal-60');
    }
});

$(function () {
    ABMPacientesView.init();
});
