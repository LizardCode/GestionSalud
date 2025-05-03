/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMGuardiasView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);

        btRemove = $('.toolbar-actions button.btRemove', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();
    }

    function buildControls() {

        MaestroLayout.errorTooltips = false;

        modalNew.find('> .modal-dialog').addClass('modal-80');

        var columns = [
            { data: 'idGuardia' },
            { data: 'fecha', render: DataTableEx.renders.date },
            { data: 'desde', render: DataTableEx.renders.dateTime },
            { data: 'hasta', render: DataTableEx.renders.dateTime },
            { data: 'profesional' },
            //{
            //    data: null,
            //    class: 'text-center',
            //    render: function (data, type, row, meta) {
            //        return `<span class="badge badge-${data.estadoClase}">${data.estado}</span>`;
            //    }
            //},
            {
                data: 'total',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${DataTableEx.renders.currency(data)}</span>`;
                }
            }
        ];

        var order = [[0, 'desc']];

        dtView = MaestroLayout.initializeDatatable('idGuardia', columns, order);
    }

    function bindControlEvents() {

        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.tabChanged = tabChanged;

        $('input[name^=Desde], input[name^=Hasta]', modalNew)
            .inputmask("99/99/9999 99:99")
            .flatpickr({
                locale: "es",
                defaultDate: "today",
                dateFormat: "d/m/Y H:i",
                allowInput: true
            });
    }

    function mainTableRowSelected(dataArray, api) {

        btRemove.prop('disabled', true);

        if (dataArray === undefined || dataArray === null)
            return;

        btRemove.prop('disabled', !dataArray.length == 1);
    }

    function newDialogOpening(dialog, $form) {

        editMode = false;

        $('input[name^=Desde]', $form).val(moment().format('DD/MM/YYYY'));
        $('input[name^=Hasta]', $form).val(moment().format('DD/MM/YYYY'));
    }

    function tabChanged(dialog, $form, index, name) {

    }
});

$(function () {
    ABMGuardiasView.init();
});
