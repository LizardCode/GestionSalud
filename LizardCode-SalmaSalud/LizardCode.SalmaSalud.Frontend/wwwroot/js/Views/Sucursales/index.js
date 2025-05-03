/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMSucursalesView = new (function () {

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
            { data: 'idSucursal' },
            { data: 'descripcion' },
            { data: 'codigoSucursal' },
            {
                data: 'exenta',
                render: function (data, type, row) {
                    if (data == enums.Common.Si)
                        return '<span class="badge badge-success"> SI </span>';
                    else
                        return '<span class="badge badge-danger"> NO </span>';
                },
                class: 'text-center'
            },
            {
                data: 'webservice',
                render: function (data, type, row) {
                    if (data == enums.Common.Si)
                        return '<span class="badge badge-success"> SI </span>';
                    else
                        return '<span class="badge badge-danger"> NO </span>';
                },
                class: 'text-center'
            },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idSucursal', columns, order);

    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > ul > li > i.details', retrieveRowDetails);

        MaestroLayout.editDialogOpening = editDialogOpening;

        $('#CodigoSucursal, #CodigoSucursal_Edit').on('change', function (e) {
            if ($(this).val() == "")
                return;
            $(this).val(String("00000" + ($(this).val().replaceAll("_", ""))).slice(-5));
        });
    }

    function editDialogOpening($form, entity) {

        $form.find('#IdSucursal_Edit').val(entity.idSucursal);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#CodigoSucursal_Edit').val(entity.codigoSucursal);
        $form.find('#Webservice_Edit').select2('val', entity.webservice);
        $form.find('#Exenta_Edit').select2('val', entity.exenta);

    }

    function renderViewDetails(data, type, row) {

        var btnDetalle = "";

        btnDetalle = '<i class="far fa-search-plus details" title="Desplegar detalle de Numeración"></i>';

        return `
            <ul class="table-controls">
                <li>
                    ${btnDetalle}
                </li>
            </ul>`;
    }

    function retrieveRowDetails() {
        var tr = $(this).closest('tr');
        var row = dtView.api().row(tr);
        var loader =
            `<div class="text-center">
                <div class="loader-sm spinner-grow text-success"></div>
                <div class="loader-sm spinner-grow text-success"></div>
                <div class="loader-sm spinner-grow text-success"></div>
            </div>`;

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
            $(this).removeClass('expanded');
        }
        else {
            row.child(loader).show();
            tr.addClass('shown');
            $(this).addClass('expanded');

            Ajax.Execute('/Sucursales/GetSucursalesNumeracionByIdSucursal/' + row.data().idSucursal, null, null, 'GET')
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowDetail(data));

                            var child = row.child();
                            var objsNumeracion = child.find('input.mascaraNumeracion');
                            var btnsConsultaAFIP = child.find('button.btNumeracionAfip');

                            //Agrego la mascara a los Inputs
                            for (var iLoop = 0; iLoop < objsNumeracion.length; iLoop++) {

                                new Cleave(objsNumeracion[iLoop], {
                                    blocks: [8],
                                    numericOnly: true
                                });

                                $(objsNumeracion[iLoop])
                                    .on('blur', function (e) {
                                        var numVal = $(this).val();
                                        $(this).val(String("00000000" + numVal.replaceAll("_", "")).slice(-8));

                                        var params = {
                                            idSucursal: $(this).data('sucursal'),
                                            idComprobante: $(this).data('comprobante'),
                                            numerador: $(this).val()
                                        };

                                        Ajax.Execute('/Sucursales/ActualizaNumeracion/', params)
                                            .done(function (response) {
                                                Ajax.ParseResponse(response,
                                                    function (data) {
                                                        Utils.alertInfo("El Número del Comprobante se Actualizó Correctamente");
                                                    },
                                                    Ajax.ShowError
                                                );
                                            })
                                            .fail(Ajax.ShowError);
                                    });
                            }

                            //Agrego el evento a los Botones de Consulta de AFIP
                            for (var iLoop = 0; iLoop < btnsConsultaAFIP.length; iLoop++) {
                                $(btnsConsultaAFIP[iLoop]).on("click", function () {
                                    var params = {
                                        idSucursal: $(this).data('sucursal'),
                                        idComprobante: $(this).data('comprobante')
                                    };

                                    $(btnsConsultaAFIP[iLoop]).prop('disabled', true);

                                    Ajax.Execute('/Sucursales/AFIPConsultaNumeracion/', params)
                                        .done(function (response) {
                                            Ajax.ParseResponse(response,
                                                function (data) {
                                                    Utils.alertInfo(`El Ultimo Nro. de Comprobante Autorizado en AFIP es: ${data}`);
                                                },
                                                Ajax.ShowError
                                            );
                                        })
                                        .fail(Ajax.ShowError)
                                        .always(function () {
                                            $(btnsConsultaAFIP[iLoop]).prop('disabled', false);
                                        });
                                });
                            }
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }
    }

    function formatRowDetail(rowData) {

        if (rowData == null || rowData.length == 0)
            return '<div class="text-center"><span>La Sucursal no presenta Comprobantes asociados con Numeración</span></div>';

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered numeracion">
                    <thead>
                        <tr>
                            <th>Comprobante</th>
                            <th>Numeración</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@Rows]
                    </tbody>
                </table>
            </div>`;

        var rowTemplate =
            `<tr>
                <td>[@Comprobante]</td>
                <td>
                    <div class="input-group">
                        <div class="input-group-prepend">
                            <span class="input-group-text">#</span>
                        </div>
                        <input type="text" class="form-control mascaraNumeracion" aria-label="Numeración" value="[@Numerador]" data-comprobante="[@IdComprobante]" data-sucursal="[@IdSucursal]">
                    </div>
                </td>
                <td>
                    <button class="btn btn-dark btn-sm btNumeracionAfip" data-comprobante="[@IdComprobante]" data-sucursal="[@IdSucursal]">Numeración AFIP</button>
                </td>
            </tr>`;

        var rows = '';

        for (var i in rowData) {
            var item = rowData[i];
            var row = rowTemplate
                .replace('[@Comprobante]', item.comprobante)
                .replace('[@Numerador]', item.numerador)
                .replaceAll('[@IdComprobante]', item.idComprobante)
                .replaceAll('[@IdSucursal]', item.idSucursal);

            rows += row;
        }

        return tableTemplate.replace('[@Rows]', rows);
    }

});

$(function () {
    ABMSucursalesView.init();
});
