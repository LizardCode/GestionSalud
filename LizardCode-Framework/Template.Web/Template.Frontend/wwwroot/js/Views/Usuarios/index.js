/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMUsersView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    var btBlank;
    var dtView;
    var closeSession;

    var modalBlankPassword;


    this.init = function () {

        btBlank = $('.toolbar-actions button.btBlank', mainClass);
        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        modalBlankPassword = $('.modal.modalBlankPassword', mainClass);
        closeSession = false;

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }


    function buildControls() {

        $('select.select2-field', mainClass).Select2Ex();

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'idUsuario' },
            { data: 'login' },
            { data: 'nombre' },
            { data: 'email' },
            { data: null, class: 'text-center', orderData: 5, render: adminRender },
            { data: 'admin', visible: false }
        ];

        var order = [[1, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idUsuario', columns, order);

    }

    function bindControlEvents() {

        MaestroLayout.onAjaxBegin = ajaxBegin;

        MaestroLayout.newDialogOpening = newDialogOpening;
        MaestroLayout.newDialogClosed = modalClosed;

        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.editDialogClosed = modalClosed;

        MaestroLayout.removeDialogOpening = removeDialogOpening;

        MaestroLayout.dialogSuccess = dialogSuccess;

        MaestroLayout.mainTableRowSelected = mainTableRowSelected;
        MaestroLayout.mainTableDraw = mainTableDraw;

        MaestroLayout.tabChanged = tabChanged;


        btBlank.on('click', blankPassword);
    }


    function ajaxBegin(dialog, $form, action, arguments) {

    }

    function newDialogOpening(dialog, $form) {

    }

    function editDialogOpening($form, entity) {

        $form.find('#IdUsuario_Edit').val(entity.idUsuario);
        $form.find('#IdTipoUsuario_Edit').select2('val', entity.idTipoUsuario);
        $form.find('#Login_Edit').val(entity.login);
        $form.find('#Nombre_Edit').val(entity.nombre);
        $form.find('#Email_Edit').val(entity.email);

    }

    function removeDialogOpening(dialog, $form) {

        var idLoggedUser = $('#hidLoggedUser').val();
        var data = dtView.selected();
        var p = dialog.find('.modal-body p');

        if (data.idUser == idLoggedUser) {
            closeSession = true;
            p.html(
                'Está intentando eliminar el usuario en uso actualmente. ' + '<br>' +
                'Si continua, se cerrará la sesión inmediatamente' + '<br>' +
                'La eliminación no puede revertirse' + '<br>' +
                '¿Desea continuar?'
            );
        }
        else {
            closeSession = false;
            p.html(
                'La eliminación no puede revertirse' + '<br>' +
                '¿Desea continuar?'
            );
        }

    }


    function dialogSuccess(dialog) {

        if (dialog === 'new') {
            swal(
                'Alta exitosa',
                'La clave temporal es "1234" y el sistema pedirá establecer una nueva durante el inicio de sesión.',
                'success'
            );
        }
        else if (dialog === 'remove') {
            if (closeSession)
                location.href = RootPath + '/Login';
        }

    }

    function modalClosed($form) {

        //var modal = $form.parents('.modal');

    }

    function mainTableRowSelected(data) {

        btBlank.prop('disabled', data == null);

        if (data.admin) {
            $('.toolbar-actions button.btEdit').prop('disabled', true);
            $('.toolbar-actions button.btRemove').prop('disabled', true);
        }

    }

    function mainTableDraw() {

        var selected = dtView.api.selected;

        btBlank.prop('disabled', selected == null);

    }


    function tabChanged(dialog, $form, index, name) {

        if (index == 1)
            dialog.addClass('modal-60');
        else
            dialog.removeClass('modal-60');

    }

    function blankPassword(e) {

        var data = dtView.selected();
        var action = RootPath + '/Usuarios/Blanqueo';

        modalBlankPassword
            .find('.modal-footer button.btOk')
            .off()
            .one('click', function () {

                $.post(action, { id: data.idUsuario })
                    .done(function (response) {

                        dtView.reload();
                        modalBlankPassword.modal('hide');

                        if (response == null || typeof response !== 'object' || $.isEmptyObject(response) || response.constructor !== {}.constructor)
                            console.error('Usuarios-Blanqueo: ', response);

                        if (response.hasOwnProperty('status') && response.hasOwnProperty('detail')) {
                            if (response.status === enums.AjaxStatus.OK) {

                                swal(
                                    'Blanking successful',
                                    'The temporary password is "1234" and the system will ask to set a new one during the first login.',
                                    'success'
                                );

                            }
                            else
                                console.error('Usuarios-Blanqueo: ' + response.detail);
                        }
                        else
                            console.error('Usuarios-Blanqueo: ', response);

                    })
                    .fail(function (xhr, ajaxOptions, thrownError) {

                        modalBlankPassword.modal('hide');

                        if (xhr.status == 401)
                            location.href = RootPath + '/Login';
                        else {
                            console.error('Usuarios-Blanqueo: ' + thrownError);
                            Utils.alertError('Critical error');
                        }
                    });

            });

        modalBlankPassword.modal({ backdrop: 'static' });
    }


    function checkAll(e) {

        var table = $(this).parents('.table.dataTable');
        var api = table.DataTable();
        var col = $(this).data('col');
        var colSrc = api.column(col).dataSrc();
        var value = $(this).is(':checked');

        var data = api.data();

        for (var i in data) {
            var item = data[i];

            if (colSrc == 'acceso') {
                if (item['acceso'] !== null)
                    item['acceso'] = value;

                if (!value) {
                    if (item['alta'] !== null)
                        item['alta'] = false;

                    if (item['baja'] !== null)
                        item['baja'] = false;

                    if (item['modificacion'] !== null)
                        item['modificacion'] = false;
                }
            }
            else {
                if (item[colSrc] !== null)
                    item[colSrc] = value;

                if (value) {
                    if (item['acceso'] !== null && item[colSrc] !== null)
                        item['acceso'] = true;
                }
            }
        }

        api.data().rows().invalidate();

        if (colSrc == 'acceso') {
            if (!value)
                table.find('thead tr th input[type=checkbox][data-col!=1]').prop('checked', false);
        }
        else {
            if (value) {
                var columnsData = Array.from(data);
                var allAccChecked = columnsData.every(function (f) { return (f.Acc === true || f.Acc === null); });
                table.find('thead tr th input[type=checkbox][data-col=1]').prop('checked', allAccChecked);
            }
        }
    }

    function checkOne(e) {

        var table = $(this).parents('.table.dataTable');
        var api = table.DataTable();
        var td = $(this).closest('td');
        var tr = td.closest('tr');
        var col = $(this).data('col');
        var colSrc = api.column(col).dataSrc();
        var value = $(this).is(':checked');

        table.DataTable().cell(td).data(value);

        if (colSrc === 'acceso') {
            if (!value) {
                tr.find('input[type=checkbox][data-col!=1]').each(function (e) {
                    var td = $(this).closest('td');
                    var data = table.DataTable().cell(td).data();

                    if (data != null)
                        table.DataTable().cell(td).data(false);
                });
            }
        }
        else {
            if (value) {
                var td = tr.find('input[type=checkbox][data-col=1]').closest('td');
                var data = table.DataTable().cell(td).data();

                if (data != null)
                    table.DataTable().cell(td).data(true);
            }
        }

        var columnsData = Array.from(table.DataTable().column(col).data());
        var allChecked = columnsData.every(function (e, i, a) { return (e === true || e === null); });

        table.find('thead tr th input[type=checkbox][data-col=' + col + ']').prop('checked', allChecked);

        api.data().rows().invalidate();
    }


    function checkboxRender(data, type, row, meta) {

        if (data === null)
            return '';

        var template =
            '<label class="custom-control custom-checkbox">' +
                '<input type="checkbox" class="custom-control-input" data-col="@col" @checked>' +
                '<span class="custom-control-label"></span>' +
            '</label>';

        var checked = (
            data == 1
                ? 'checked'
                : ''
        );

        return template
            .replace('@col', meta.col)
            .replace('@checked', checked);

    }

    function adminRender(data, type, row, meta) {

        if (data.admin)
            return '<span class="badge badge-primary"><i class="fas fa-check"></i></span>';
        else
            return '';

    }
});

$(function () {
    ABMUsersView.init();
});
