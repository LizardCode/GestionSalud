/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMEvolucionesView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var autocomplete;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        //modalEdit = $('.modal.modalEdit', mainClass);
        //modalAgenda = $('.modal.modalAgenda', mainClass);

        //btEdit = $('.toolbar-actions button.btEdit', mainClass);
        //btRemove = $('.toolbar-actions button.btRemove', mainClass);
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
            { data: 'especialidad', visible: $('#hidLoggedUserTipo').val() != enums.TipoUsuario.Profesional },
            { data: 'profesional', visible: $('#hidLoggedUserTipo').val() != enums.TipoUsuario.Profesional },
            { data: 'paciente' },
            { data: null, render: renderEstado },
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idProfesional', columns, order);
    }

    function bindControlEvents() {

    }

    function renderEstado(data, type, row) {

        return '<span class="badge badge-' + data.estadoClase + '">' + data.estado + '</span>';
    }

});

$(function () {
    ABMEvolucionesView.init();
});
