/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMEvolucionesView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var autocomplete;
    var btImportarExcel;
    var btHistoriaClinica;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        //modalEdit = $('.modal.modalEdit', mainClass);
        //modalAgenda = $('.modal.modalAgenda', mainClass);
        btHistoriaClinica = $('.toolbar-actions button.btHistoriaClinicaTb', mainClass);
        btImportarExcel = $('button.btImportar', mainClass);

        //btEdit = $('.toolbar-actions button.btEdit', mainClass);
        btRemove = $('.toolbar-actions button.btRemove', mainClass);
        //btAgenda = $('.toolbar-actions button.btAgenda', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();
    }


    function buildControls() {

        modalNew.find('> .modal-dialog').addClass('modal-95');

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'idEvolucion', visible: false },
            { data: 'fecha', width: '5%', render: DataTableEx.renders.date },
            { data: 'paciente' },
            { data: null, render: renderEstado },
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idEvolucion', columns, order);
    }

    function bindControlEvents() {

        MaestroLayout.mainTableRowSelected = mainTableRowSelected;

        btHistoriaClinica.on('click', historiaClinicaDialog);

        $('.btNuevaEvolucion').click(function () {
            window.location = '/EvolucionesExternas/NuevaPage';
        });

        btImportarExcel.on('click', function () {

            Modals.loadExcelReaderModal('excelReader', 'modal-50', 'EvolucionesExternas', 'ImportarEvolucionesExcel',
                function () {
                    Utils.modalLoader('Procesando...');
                },
                function (response) {
                    Utils.modalClose();

                    $('.excelReader').modal('hide');

                    if (response.status == 'OK') {
                        if (response.detail) {
                            //console.log(response);
                            Utils.alertSuccess(`Procesados: ${response.detail.procesados}, Error: ${response.detail.error}.`);
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

    function mainTableRowSelected(dataArray, api) {

        btHistoriaClinica.prop('disabled', true);
        btRemove.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        btHistoriaClinica.prop('disabled', dataArray.length != 1);
        btRemove.prop('disabled', dataArray.length != 1);
    }

    function historiaClinicaDialog() {

        var selectedRows = dtView.selected();
        var idPaciente = selectedRows[0].idPaciente;
        var action = '/Pacientes/HistoriaClinicaView?id=' + idPaciente + '&showResumenPaciente=true';

        Modals.loadAnyModal('historiaClinicaDialog', 'modal-95', action, function () { }, function () { });
    }

    function renderEstado(data, type, row) {

        return '<span class="badge badge-' + data.estadoClase + '">' + data.estado + '</span>';
    }

});

$(function () {
    ABMEvolucionesView.init();
});
