/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMEspecialidadesView = new (function () {

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
            { data: 'idEspecialidad' },
            { data: 'descripcion' }
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idEspecialidad', columns, order);

    }

    function bindControlEvents() {
        modalNew.find('> .modal-dialog').addClass('modal-50');
        modalEdit.find('> .modal-dialog').addClass('modal-50');

        MaestroLayout.editDialogOpening = editDialogOpening;

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdEspecialidad_Edit').val(entity.idEspecialidad);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
    }

});

$(function () {
    ABMEspecialidadesView.init();
});
