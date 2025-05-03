var DashboardView = new (function () {

    //#region Init

    var defaultDom =
        "<'dt--top-section'<l><f>>" +
        "<'table-responsive'tr>" +
        "<'dt--bottom-section d-flex flex-row-reverse'<'dt--pages-count  mb-sm-0 mb-3'i><'dt--pagination'p>>";

    this.init = function () {

        buildControls();
        bindControlsEvents();

        initTotales();
        initDataTables();
    };

    function initTotales() {
        Ajax.GetJson(RootPath + 'FacturacionManual/GetCantidadFacturasVenta')
            .done(function (data) {

                $('.pFacturas').html($('.pFacturas').html().replace('[VALUE]', data.cantidad));
                $('#FacturasLoader').hide();
                $('.pFacturas').show();

                $('[data-toggle="tooltip"]').tooltip();
            })
            .fail(Ajax.ShowError);

        Ajax.GetJson(RootPath + 'FacturacionManual/GetCantidadFacturasCompra')
            .done(function (data) {

                $('.pFacturasProveedores').html($('.pFacturasProveedores').html().replace('[VALUE]', data.cantidad));
                $('#FacturasProveedoresLoader').hide();
                $('.pFacturasProveedores').show();

                $('[data-toggle="tooltip"]').tooltip();
            })
            .fail(Ajax.ShowError);

        Ajax.GetJson(RootPath + 'FacturacionManual/GetIVADashboard')
            .done(function (data) {

                $('.pPosicionIVA').html($('.pPosicionIVA').html().replace('[VALUE]', accounting.formatMoney(data.importe)));
                $('#PosicionIVALoader').hide();
                $('.pPosicionIVA').show();

                $('[data-toggle="tooltip"]').tooltip();
            })
            .fail(Ajax.ShowError);

        Ajax.GetJson(RootPath + 'ResumenCtaCteCli/GetResumenCtaCteCliDashboard')
            .done(function (data) {

                $('.pSaldoCuentas').html($('.pSaldoCuentas').html().replace('[VALUE]', accounting.formatMoney(data.importe)));
                $('#SaldoCuentasLoader').hide();
                $('.pSaldoCuentas').show();

                $('[data-toggle="tooltip"]').tooltip();
            })
            .fail(Ajax.ShowError);


        $('[data-toggle="tooltip"]').tooltip();

        $('.bEmpresas').click(function () {
            location.href = '/Empresas';
        })

        $('.bUsuarios').click(function () {
            location.href = '/Usuarios';
        })

        $('.bClientes').click(function () {
            location.href = '/Clientes';
        })

        $('.bProveedores').click(function () {
            location.href = '/Proveedores';
        })
    };

    function initDataTables() {
        dtTiposDeCambio = $('.dtTiposDeCambio')
            .DataTableEx({

                ajax: {
                    url: RootPath + '/Monedas/ObtenerTiposDeCambioDashboard',
                    type: 'POST',
                    error: function (xhr, ajaxOptions, thrownError) {
                        Ajax.ShowError(xhr, xhr.statusText, thrownError);
                    },
                    callback: function (xhr) {
                        Ajax.ShowError(xhr, xhr.statusText, '');
                    }
                },
                processing: true,
                serverSide: true,
                pageLength: 10,
                lengthChange: false,
                dom: defaultDom,
                columns: [
                    { data: 'simbolo', width: '15%', class: 'text-center' },
                    { data: 'descripcion', width: '45%' },
                    { data: 'fecha', width: '25%', class: 'text-center', render: DataTableEx.renders.date },
                    { data: 'cotizacion', width: '15%', class: 'text-right', render: DataTableEx.renders.currency }
                ],
                order: [[1, 'ASC']],
                onDraw: datatableDraw
            });
    };

    //#endregion

    //#region Funciones

    function buildControls() {

    }

    function bindControlsEvents() {

    }

    function datatableDraw() {

        feather.replace();
    }

    //#endregion

});

$(function () {

    DashboardView.init();
});