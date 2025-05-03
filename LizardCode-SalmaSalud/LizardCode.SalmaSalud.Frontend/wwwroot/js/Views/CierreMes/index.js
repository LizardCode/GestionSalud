/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMCierreMesView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    this.init = function () {

        buildControls();
        bindControlEvents();

    }

    function buildControls() {

        $('#IdEjercicio').Select2Ex();

    }

    function bindControlEvents() {

        $('#IdEjercicio').on('click', function (e) {
            var idEjercicio = $(this).select2('val');

            Ajax.Execute('/CierreMes/Detalle/' + idEjercicio, null, null, 'GET')
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (rowData) {
                            for (var i in rowData) {
                                var item = rowData[i];
                                $('#mes-' + item.mes).text(`${item.nombreMes}-${item.anno}`);

                                if (item.modulo == enums.Modulos.Clientes) {
                                    $('#mes-' + item.mes + '-clientes').data("mes", item.mes);
                                    $('#mes-' + item.mes + '-clientes').data("anno", item.anno);
                                    if (item.cierre == enums.Common.Si)
                                        $('#mes-' + item.mes + '-clientes').text("Mes Cerrado").addClass("btn-danger");
                                    else
                                        $('#mes-' + item.mes + '-clientes').text("Mes Abierto").addClass("btn-success");
                                }

                                if (item.modulo == enums.Modulos.Proveedores) {
                                    $('#mes-' + item.mes + '-proveedores').data("mes", item.mes);
                                    $('#mes-' + item.mes + '-proveedores').data("anno", item.anno);
                                    if (item.cierre == enums.Common.Si)
                                        $('#mes-' + item.mes + '-proveedores').text("Mes Cerrado").addClass("btn-danger");
                                    else
                                        $('#mes-' + item.mes + '-proveedores').text("Mes Abierto").addClass("btn-success");
                                }

                                if (item.modulo == enums.Modulos.CajaBanco) {
                                    $('#mes-' + item.mes + '-cajaBanco').data("mes", item.mes);
                                    $('#mes-' + item.mes + '-cajaBanco').data("anno", item.anno);
                                    if (item.cierre == enums.Common.Si)
                                        $('#mes-' + item.mes + '-cajaBanco').text("Mes Cerrado").addClass("btn-danger");
                                    else
                                        $('#mes-' + item.mes + '-cajaBanco').text("Mes Abierto").addClass("btn-success");
                                }

                            }
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        });

        $('.cierre').on('click', function (e) {
            e.preventDefault();

            var obj = this;
            $(obj).prop('disabled', true);
            var params = {
                idEjercicio: $("#IdEjercicio").select2('val'),
                anno: $(obj).data('anno'),
                mes: $(obj).data('mes'),
                modulo: $(obj).data('modulo')
            };

            Ajax.Execute('/CierreMes/Cierre/', params)
                .done(function (response) {
                    if (response.status == enums.AjaxStatus.OK) {
                        if ($(obj).hasClass('btn-success'))
                            $(obj).text("Mes Cerrado").removeClass('btn-success').addClass("btn-danger");
                        else
                            $(obj).text("Mes Abierto").removeClass('btn-success').addClass("btn-success");
                    }
                    else
                        Utils.modalError('Error', `Error al Cerrar el Mes/Año ${params.mes}/${params.anno}`);
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    Utils.modalError('Error', jqXHR.responseJSON.detail);
                })
                .always(function () {
                    $(obj).prop('disabled', false);
                });

        });
    }

});

$(function () {
    ABMCierreMesView.init();
});
