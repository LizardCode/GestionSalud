/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMTiposAsientosView = new (function () {

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

        modalNew.find('> .modal-dialog').addClass('modal-70');
        modalEdit.find('> .modal-dialog').addClass('modal-70');

        $('.modal .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
            .on('repeater:row:added', repeaterRowAdded)
            .on('repeater:row:removed', repeaterRowRemoved)
            .on('repeater:field:focus', repeaterFieldFocus)
            .on('repeater:field:change', repeaterFieldChange)
            .Repeater({ min: 1, max: 50 });

        var columns = [
            { data: 'idTipoAsiento' },
            { data: 'descripcion' }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idTipoAsiento', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.newDialogOpening = newDialogOpening;
    }

    function newDialogOpening(dialog, $form) {

        editMode = false;

        $form.find('.repeater-component').Repeater()
            .clear();
    }

    function editDialogOpening($form, entity) {

        $form.find('#IdTipoAsiento_Edit').val(entity.idTipoAsiento);
        $form.find('#Descripcion_Edit').val(entity.descripcion);

        if (entity.items && entity.items.length > 0)
            $form.find('.repeater-component').Repeater()
                .source(entity.items);
        else
            $form.find('.repeater-component').Repeater();
    }

    function rebuildItemNumbers($rows) {

        for (var i = 0; i < $rows.length; i++) {

            var $row = $rows.eq(i);
            var $item = $row.find('> td input.repeater-control[name$=".Item"]');

            $item.val(i + 1);
        }
    }

    function repeaterInitialization() {

        console.log('repeater iniciado');
    }

    function repeaterRowAdded(e, $newRow, $rows) {

        rebuildItemNumbers($rows);
        ConstraintsMaskEx.init();
    }

    function repeaterRowRemoved(e, $removedRow, $rows) {

        rebuildItemNumbers($rows);
    }

    function repeaterFieldFocus(e, data) {

        console.log(data);
    }

    function repeaterFieldChange(e, data) {

        console.log(data);
    }

});

$(function () {
    ABMTiposAsientosView.init();
});
