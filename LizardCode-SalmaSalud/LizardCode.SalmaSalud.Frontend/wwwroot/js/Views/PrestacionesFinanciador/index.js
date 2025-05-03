/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMPrestacionesView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    //var modalImportarExcel;
    var btImportarExcel;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        //modalImportarExcel = $('.modal.modalImportarExcel', mainClass);
        btImportarExcel = $('button.btImportar', mainClass);

        $('.modal .dvProfesionales .repeater-component', mainClass)
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

        var columns = [
            { width: '20%', data: 'financiadorPlan' },
            { width: '30%', data: 'descripcion' },
            //{ data: 'valor' },
            {
                width: '20%',
                data: 'valor',
                class: 'text-right',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-info">${DataTableEx.renders.currency(data)}</span>`;
                }
            },
            { width: '15%', data: 'codigo' },
            { width: '15%', data: 'prestacionCodigo' },
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idFinanciadorPrestacion', columns, order);
    }

    function bindControlEvents() {

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.tabChanged = tabChanged;

        btImportarExcel.on('click', function () {

            Modals.loadExcelReaderModal('excelReader', 'modal-50', 'PrestacionesFinanciador', 'ImportarPrestacionesFinanciadorExcel',
                function () {
                    Utils.modalLoader('Procesando...');
                },
                function (response) {
                    Utils.modalClose();

                    $('.excelReader').modal('hide');

                    if (response.status == 'OK') {
                        if (response.detail) {
                            //console.log(response);
                            Utils.alertSuccess(`Procesados: ${response.detail.procesados}, Actualizados: ${response.detail.actualizados}, Nuevos: ${response.detail.nuevos}.`);
                            dtView.reload();
                        }
                        else {
                            Utils.alertError('No se encontraron registros para procesar.');
                        }
                    } else {
                        Utils.alertError(response.detail);
                    }
                },
                function (jqXHR, textStatus, errorThrown) {
                    Utils.modalClose();

                    Ajax.ShowError(jqXHR, textStatus, errorThrown);
                }
            )
        });
    }

    function newDialogOpening(dialog, $form) {

        editMode = false;

        $form.find('.dvProfesionales .repeater-component').Repeater()
            .clear();
    }

    function editDialogOpening($form, entity) {

        $form.find('#IdFinanciadorPrestacion_Edit').val(entity.idFinanciadorPrestacion);

        $form.find('#IdFinanciadorPlan_Edit').select2('val', entity.idFinanciadorPlan);

        $form.find('#Codigo_Edit').val(entity.codigo);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#Valor_Edit').val(entity.valor);

        $form.find('#ValorFijo_Edit').val(entity.valorFijo);
        $form.find('#Porcentaje_Edit').val(entity.porcentaje);
        $form.find('#CoPago_Edit').val(entity.coPago);

        $form.find('#IdPrestacion_Edit').select2('val', entity.idPrestacion);

        if (entity.profesionales.length > 0)
            $form.find('.dvProfesionales .repeater-component').Repeater()
                .source(entity.profesionales);
        else
            $form.find('.dvProfesionales .repeater-component').Repeater()
                .clear();
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

    function tabChanged(dialog, $form, index, name) {

        if (index == 1) {
            dialog.addClass('modal-50');
        }
        else {
            dialog.removeClass('modal-50');
        }

    }
});

$(function () {
    ABMPrestacionesView.init();
});
