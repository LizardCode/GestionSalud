/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMSdoCtaCtePrvView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    var editMode = false;

    var modalProcesarExcel;
    var btProcesarExcel;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        modalProcesarExcel = $('.modal.modalProcesarExcel', mainClass);
        btProcesarExcel = $('button.btProcesarExcel', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }

    function buildControls() {
        $('select.select2-field', mainClass).Select2Ex();

        modalNew.find('> .modal-dialog').addClass('modal-95');
        modalEdit.find('> .modal-dialog').addClass('modal-95');

        $('.modal .repeater-component', mainClass)
            .on('repeater:init', repeaterInitialization)
            .on('repeater:row:added', repeaterRowAdded)
            .on('repeater:row:removed', repeaterRowRemoved)
            .on('repeater:field:focus', repeaterFieldFocus)
            .on('repeater:field:change', repeaterFieldChange)
            .Repeater({ min: 1, max: 50 });

        MaestroLayout.disabledFields = true;
        MaestroLayout.errorTooltips = true;

        var columns = [            
            { data: 'idSaldoCtaCtePrv', visible: false },
            { data: 'fechaDesde', width: '15%', class: 'text-center', render: DataTableEx.renders.date },
            { data: 'fechaHasta', width: '15%', class: 'text-center', render: DataTableEx.renders.date },
            { data: 'descripcion', width: '45%' },
            {
                data: 'cantidad',
                width: '10%',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-success">${data}</span>`;
                }
            },
            {
                data: 'total',
                class: 'text-center',
                render: function (data, type, row, meta) {
                    return `<span class="badge badge-warning">${DataTableEx.renders.currency(data)}</span>`;
                }
            }
        ];

        var order = [[0, 'desc']];

        dtView = MaestroLayout.initializeDatatable('idSaldoCtaCtePrv', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.editDialogOpening = editDialogOpening;

        $('.modal', mainClass).on('blur', 'input[name$=".NetoGravado"]', setTotal);
        $('.modal', mainClass).on('change', 'select[name$=".IdAlicuota"]', setTotal);
        $('.modal', mainClass).on('blur', 'input[name$=".Percepciones"]', setTotal);

        btProcesarExcel.on('click', function () {
            $('#FileExcel').val('');
            $('#lblFileExcel').text('').hide();
            $('.btOkProcesarExcel').prop('disabled', true);
            modalProcesarExcel.modal({ backdrop: 'static', keyboard: false, modalOverflow: true });
        });

        $('#FileExcel').on('change', async function (e) {

            var fE = document.getElementById('FileExcel');
            console.log('Selected file: ' + fE.files.item(0).name);

            $('#lblFileExcel').text(fE.files.item(0).name).show();
            $('.btOkProcesarExcel').prop('disabled', false);
        });
    }

    function newDialogOpening(dialog, $form) {

        editMode = false;

        $('input[name^=FechaDesde]', $form).val(moment().format('DD/MM/YYYY'));
        $('input[name^=FechaHasta]', $form).val(moment().format('DD/MM/YYYY'));

        $form.find('.repeater-component').Repeater()
            .clear();
    }

    function editDialogOpening($form, entity) {

        editMode = true;

        $form.find('#IdSaldoCtaCtePrv_Edit').val(entity.idSaldoCtaCtePrv);
        $form.find('#FechaDesde_Edit').val(moment(entity.fechaDesde).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#FechaHasta_Edit').val(moment(entity.fechaHasta).format(enums.FormatoFecha.DefaultFormat));
        $form.find('#Descripcion_Edit').val(entity.descripcion);

        if (entity.items.length > 0)
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

        $('input[name$=".Sucursal"]', $newRow).alphanum({
            allowNumeric: true,
            allowSpace: false,
            allowUpper: false,
            allowLower: false
        })
        .on('blur', function (e) {

            var numero = $(this).val();

            if (numero == "")
                return;

            numero = `${String("00000" + numero).slice(-5)}`;

            $(this).val(numero);
        });

        $('input[name$=".Numero"]', $newRow).alphanum({
            allowNumeric: true,
            allowSpace: false,
            allowUpper: false,
            allowLower: false
        })
        .on('blur', function (e) {

            var numero = $(this).val();

            if (numero == "")
                return;

            numero = `${String("00000000" + numero).slice(-8)}`;

            $(this).val(numero);
        });
    }

    function repeaterRowRemoved(e, $removedRow, $rows) {

        rebuildItemNumbers($rows);
        setTotal({ target: $('tbody', e.target) });

    }

    function repeaterFieldFocus(e, data) {

        console.log(data);

    }

    function repeaterFieldChange(e, data) {

        console.log(data);

    }

    function setTotal(e) {

        var tr = $(e.target).closest('tr');
        var total = tr.find('input[name$=".Total"]');

        total.val(0);

        var subtotal = AutoNumeric.isManagedByAutoNumeric(tr.find('input[name$=".NetoGravado"]')[0]) ? AutoNumeric.getNumber(tr.find('input[name$=".NetoGravado"]')[0]) : tr.find('input[name$=".NetoGravado"]').val();
        var alicuota = tr.find('select[name$=".IdAlicuota"]').val();
        var percepciones = AutoNumeric.isManagedByAutoNumeric(tr.find('input[name$=".NetoGravado"]')[0]) ? AutoNumeric.getNumber(tr.find('input[name$=".Percepciones"]')[0]) : tr.find('input[name$=".Percepciones"]').val();

        if (!isNaN(parseFloat(subtotal)) && !isNaN(parseFloat(alicuota))) {
            var importe = parseFloat(subtotal) * (1 + (parseFloat(alicuota) / 100));
            var percepcion = percepciones | 0;

            importe = importe + percepcion;

            if (AutoNumeric.isManagedByAutoNumeric(total.get(0)))
                AutoNumeric.set(total.get(0), importe);
            else
                total.val(accounting.formatMoney(importe));
        }
    }

    this.ajaxProcesarExcelBegin = function () {
        Utils.modalLoader('Procesando...');
    }

    this.ajaxProcesarExcelSuccess = function (response) {
        Utils.modalClose();

        modalProcesarExcel.modal('hide');

        if (response.status == 'OK') {
            if (response.detail) {

                if (editMode) {
                    $('.modalEdit .repeater-component').Repeater()
                        .clear();

                    $('.modalEdit .repeater-component').Repeater()
                        .source(response.detail);
                } else {
                    $('.modalNew .repeater-component').Repeater()
                        .clear();

                    $('.modalNew .repeater-component').Repeater()
                        .source(response.detail);
                }
            }
            else {
                Utils.alertError('No se encontraron registros.');
            }
        } else {
            Utils.alertError(response.detail);
        }
    }

    this.ajaxProcesarExcelFailure = function (jqXHR, textStatus, errorThrown) {
        Utils.modalClose();

        Ajax.ShowError(jqXHR, textStatus, errorThrown);
    }
});

$(function () {
    ABMSdoCtaCtePrvView.init();
});
