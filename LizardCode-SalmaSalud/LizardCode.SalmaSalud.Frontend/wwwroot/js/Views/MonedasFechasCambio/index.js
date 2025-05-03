/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMMonedasFechasCambioView = new (function () {

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
            { data: 'idMoneda' },
            { data: 'descripcion' },
            { data: 'simbolo' },
            { data: 'codigo' },
            { data: null, orderable: false, searchable: false, class: 'text-center', render: renderViewDetails }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idMoneda', columns, order);

    }

    function bindControlEvents() {

        dtView.table().on('click', 'tbody > tr > td > i.details', retrieveRowDetails);

        MaestroLayout.editDialogOpening = editDialogOpening;

        $('#Cotizacion_Edit').on('blur', function (e) {
            var $form = $(e.currentTarget).parents('form');
            $form.submit();
        })
        .on('focus', function (e) {
            $(this).select();
        });       

    }

    function editDialogOpening($form, entity) {

        $('.flatpicker', $form)
            .inputmask("99/99/9999")
            .flatpickr({
                inline: true,
                allowInput: true,
                dateFormat: "d/m/Y",
                defaultDate: "today",
                locale: "es",
                onClose: function (selectedDates, dateStr, instance) {
                    if (dateStr == "")
                        instance.setDate(moment().format(enums.FormatoFecha.DefaultFormat));
                    else
                        instance.setDate(dateStr);
                },
                onChange: function (selectedDates, dateStr, instance) {

                    var params = {
                        idMoneda: $('#IdMoneda_Edit').val(),
                        fecha: dateStr
                    };

                    Ajax.Execute('/MonedasFechasCambio/GetFechaCambio/', params)
                        .done(function (response) {
                            if (response.status == enums.AjaxStatus.OK) {
                                $('#Cotizacion_Edit').val(response.detail);
                            }
                            else {
                                $('#Cotizacion_Edit').val(0);
                            }

                        })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            Utils.modalError('Error', jqXHR.responseJSON.detail);
                        });
                }
            });

        $('.flatpickr-calendar.inline', $form).css('margin', 'auto');

        $('#Fecha_Edit', $form).css('display', 'none');
        $('#IdMoneda_Edit', $form).val(entity.idMoneda);

        $('.btn-default').html("<i class='fas fa-door-open'></i> Salir");
        $('.btSave').hide();

        var params = {
            idMoneda: entity.idMoneda,
            fecha: moment().format('YYYY-MM-DD')
        };

        Ajax.Execute('/MonedasFechasCambio/GetFechaCambio/', params)
            .done(function (response) {
                if (response.status == enums.AjaxStatus.OK) {
                    $('#Cotizacion_Edit').val(response.detail);
                }
                else {
                    $('#Cotizacion_Edit').val(0);
                }

            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Utils.modalError('Error', jqXHR.responseJSON.detail);
            });

    }

    function renderViewDetails() {
        return '<i class="details far fa-search-plus" title="Desplegar Cotizaciones"></i>';
    }

    function formatRowDetail(rowData) {

        if (rowData == null || rowData.length == 0)
            return '<div class="text-center"><span>La Moneda no presenta Cotizaciones en el Detalle</span></div>';

        var tableTemplate =
            `<div class="table-responsive">
                <table class="table table-bordered mb-4">
                    <thead>
                        <tr>
                            <th>Fecha</th>
                            <th>Cotización</th>
                        </tr>
                    </thead>
                    <tbody>
                        [@Rows]
                    </tbody>
                </table>
            </div>`;

        var rowTemplate =
            `<tr>
                <td>[@Fecha]</td>
                <td>[@Cotizacion]</td>
            </tr>`;

        var rows = '';

        for (var i in rowData) {
            var item = rowData[i];
            var row = rowTemplate
                .replace('[@Fecha]', DataTableEx.utils.date(item.fecha))
                .replace('[@Cotizacion]', accounting.formatMoney(item.cotizacion));

            rows += row;
        }

        return tableTemplate.replace('[@Rows]', rows);
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

            Ajax.Execute('/MonedasFechasCambio/ObtenerCotizaciones/' + row.data().idMoneda, null, null, 'GET')
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            row.child(formatRowDetail(data));
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        }
    }

});

$(function () {
    ABMMonedasFechasCambioView.init();
});
