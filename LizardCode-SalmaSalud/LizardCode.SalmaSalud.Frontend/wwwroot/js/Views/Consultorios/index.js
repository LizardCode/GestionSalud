/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMConsultoriosView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');


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
            { data: 'descripcion' },
            { data: 'edificio' },
            { data: 'piso' }
        ];

        var order = [[0, 'desc']];

        dtView = MaestroLayout.initializeDatatable('idConsultorio', columns, order);

    }

    function bindControlEvents() {
        modalNew.find('> .modal-dialog').addClass('modal-50');
        modalEdit.find('> .modal-dialog').addClass('modal-50');

        MaestroLayout.editDialogOpening = editDialogOpening;
    }

    function editDialogOpening($form, entity) {

        $form.find('#IdConsultorio_Edit').val(entity.idConsultorio);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#Edificio_Edit').val(entity.edificio);
        $form.find('#Piso_Edit').val(entity.piso);

    }

});

$(function () {
    ABMConsultoriosView.init();
});
