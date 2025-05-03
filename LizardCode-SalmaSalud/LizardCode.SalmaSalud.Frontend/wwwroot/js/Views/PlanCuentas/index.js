/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/repeater.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var ABMPlanCuentasView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    this.init = function () {

        buildControls();
        bindControlEvents();

        $('.file-tree-folder').addClass('open');
        $('.widget-content-area ul').css('display', 'block');

    }

    function buildControls() {

        $('#IdCuentaContable').prop('disabled', true);
        $('#RubroContable').prop('disabled', true);
        $('#CodigoCuenta').prop('disabled', true);
        $('#Descripcion').prop('disabled', true);

    }

    function bindControlEvents() {

        $('.nodo-cuenta').on('click', function (e) {
            var idCuenta = $(this).data('id');

            Ajax.Execute('/PlanCuentas/Detalle/' + idCuenta, null, null, 'GET')
                .done(function (response) {
                    Ajax.ParseResponse(response,
                        function (data) {
                            $('#IdCuentaContable').val(data.idCuentaContable);
                            $('#RubroContable').val(data.rubroContable);
                            $('#CodigoCuenta').val(data.codigoCuenta);
                            $('#Descripcion').val(data.descripcion);
                            
                        },
                        Ajax.ShowError
                    );
                })
                .fail(Ajax.ShowError);
        });
    }

});

$(function () {
    ABMPlanCuentasView.init();
});
