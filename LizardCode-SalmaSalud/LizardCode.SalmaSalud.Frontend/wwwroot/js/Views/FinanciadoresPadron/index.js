/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMPadronView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    //var modalImportarExcel;
    var btImportarExcel;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        //modalImportarExcel = $('.modal.modalImportarExcel', mainClass);
        btImportarExcel = $('button.btImportar', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        MaestroLayout.errorTooltips = false;

        var columns = [
            { width: '20%', data: 'documento' },
            { width: '50%', data: 'nombre' },
            { width: '30%', data: 'financiadorNro' }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idFinanciadorPadron', columns, order);
    }

    function bindControlEvents() {

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;

        btImportarExcel.unbind().on('click', function () {

            Modals.loadExcelReaderModal('excelReader', 'modal-50', 'FinanciadoresPadron', 'ImportarPadronFinanciadorExcel',
                function () {
                    Utils.modalLoader('Procesando...');
                },
                function (response) {
                    Utils.modalClose();

                    $('.excelReader').modal('hide');

                    if (response.status == 'OK') {
                        if (response.detail) {
                            //console.log(response);
                            Utils.alertSuccess(`Procesados: ${response.detail.procesados}, Actualizados: ${response.detail.actualizados}, Nuevos: ${response.detail.nuevos}, Eliminados: ${response.detail.eliminados}.`);
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
    }

    function editDialogOpening($form, entity) {

        $form.find('#IdFinanciadorPadron_Edit').val(entity.idFinanciadorPadron);
        $form.find('#IdFinanciador_Edit').val(entity.idFinanciador);

        $form.find('#Documento_Edit').val(entity.documento);
        $form.find('#FinanciadorNro_Edit').val(entity.financiadorNro);
        $form.find('#Nombre_Edit').val(entity.nombre);
    }
});

$(function () {
    ABMPadronView.init();
});
