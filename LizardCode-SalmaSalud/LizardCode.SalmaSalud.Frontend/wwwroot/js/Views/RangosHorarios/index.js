/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMRangosHorariosView = new (function () {

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
            { data: 'idRangoHorario', width: '10%' },
            { data: 'especialidad', width: '20%' },
            { data: 'descripcion', width: '70%' }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idRangoHorario', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;

    }


    function editDialogOpening($form, entity) {

        $form.find('#IdRangoHorario_Edit').val(entity.idRangoHorario);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#IdEspecialidad_Edit').select2('val', entity.idEspecialidad);
    }    
});

$(function () {
    ABMRangosHorariosView.init();
});
