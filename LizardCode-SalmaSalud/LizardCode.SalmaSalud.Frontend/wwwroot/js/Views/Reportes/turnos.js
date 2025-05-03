/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var TurnosView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    this.init = function () {

        ReportLayout.init();

        var columns = [
            { data: 'idTurno', visible: false, orderable: false, searchable: false },
            { data: 'idTipoTurno', class: 'text-center', width: "10%", orderable: false, searchable: false, render: renderTipo },
            { data: 'fecha', width: "5%", orderable: false, searchable: false },
            { data: 'hora', class: 'text-center', width: "5%", orderable: false, searchable: false, render: renderHoraTurno },
            { data: 'profesional', width: "20%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'paciente', width: "20%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'financiador', width: "30%", render: DataTableEx.renders.ellipsis, orderable: false, searchable: false },
            { data: 'estado', class: 'text-center', width: "10%", orderable: false, searchable: false, render: renderEstado },
        ];

        var order = [[0, 'asc']];

        dtView = ReportLayout.initializeDatatable('idTurno', columns, order, 30, false);
    }
    function renderTipo(data, type, row) {

        return '<p class="td-sub-field"><b>' + row.tipoTurnoDescripcion + '</b></p>';
    }

    function renderHoraTurno(data, type, row) {

        return '<div class="avatar avatar-sm"><span class="avatar-title rounded-circle azul-avatar">' + row.hora.substring(0, 5) + '</span></div>';
    }

    function renderEstado(data, type, row) {

        return '<span class="badge badge-pills badge-' + row.estadoClase + ' font10">' + row.estado + '</span>';
    }
});

$(function () {
    TurnosView.init();
});
