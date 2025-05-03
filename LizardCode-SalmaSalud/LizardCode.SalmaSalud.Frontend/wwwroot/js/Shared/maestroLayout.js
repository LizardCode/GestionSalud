/// <reference path="utils.js" />
/// <reference path="../../lib/cleave/cleave.min.js" />
/// <reference path="../../lib/cleave/cleave-phone.ar.js" />
/// <reference path="../../lib/flatpickr/flatpickr.js" />

var MaestroLayout = new (function () {

    var self = this;
    var mainClass;
    var dtSelector;
    var btNewSelector;
    var btEditSelector;
    var btRemoveSelector;
    var btToggleFiltersSelector;
    var btApplyFiltersSelector;
    var btClearFiltersSelector;
    var filtersSelector;
    var modalNewSelector;
    var modalEditSelector;
    var modalRemoveSelector;
    var modalNewSaveSelector;
    var modalEditSaveSelector;
    var modalRemoveConfirmSelector;
    var currentModal;
    var filtersEnabled;
    var filtersApplied;

    var dtView = null;
    var dtKeyProperty = null;

    this.errorTooltips = true;
    this.disabledFields = false;

    // Eventos
    // https://stackoverflow.com/a/31146534/1812392
    //

    this.buildFiltersControls = function () {

        $('select.select2-filter-field', filtersSelector).Select2Ex({
            allowClear: true,
            allowInput: true,
            dropdownCssClass: 'advanced-filters select2-field'
        });

        $('input.flatpickr-filter-field', filtersSelector)
            .cleave({ date: true, datePattern: ['d', 'm', 'Y'], delimiter: '/' })
            .flatpickr({
                locale: "es",
                allowInput: true,
                dateFormat: "d/m/Y",
                onClose: function (selectedDates, dateStr, instance) {
                    if (dateStr == "")
                        instance.setDate(moment().format(enums.FormatoFecha.DefaultFormat));
                    else
                        instance.setDate(dateStr);
                }
            });

        var filtersRow = $(filtersSelector);
        filtersEnabled = (filtersRow.length == 1);
        filtersApplied = null;

        if (!filtersEnabled)
            return;

        $(btToggleFiltersSelector).on('click', function (e) {

            e.preventDefault();

            if (filtersRow.is(':visible')) {

                filtersRow.slideUp({
                    start: function () {
                        $(btToggleFiltersSelector)
                            .removeClass('btn-primary')
                            .addClass('btn-outline-primary');
                    }
                });

            }
            else {

                filtersRow.slideDown({
                    start: function () {
                        $(btToggleFiltersSelector)
                            .removeClass('btn-outline-primary')
                            .addClass('btn-primary');
                    }
                });

            }

        });

        $(btApplyFiltersSelector).on('click', applyFilters);

        $(btClearFiltersSelector).on('click', cleanFilters);

    }

    this.buildControls = function () {

        $('.modal select:not(.no-select2, .repeater-select2)', mainClass).select2();
        $('.modal select:not(.no-select2)', mainClass)
            .on('change', function (e) {
                $(this).valid();
            });
        $('.modal .flatpickr')
            .inputmask("99/99/9999")
            .flatpickr({
                locale: "es",
                allowInput: true,
                defaultDate: "today",
                dateFormat: "d/m/Y",
                onClose: function (selectedDates, dateStr, instance) {
                    if (dateStr == "")
                        instance.setDate(moment().format(enums.FormatoFecha.DefaultFormat));
                    else
                        instance.setDate(dateStr);
                }
            });
        $('.modal .input-group.dropdown-group-list').DropDownGroup();
        $('.modal.modalNew form [data-newvisible="false"]').addClass('no-validate');
        $('.modal.modalEdit form [data-editingdisabled="true"]').addClass('no-validate');

    }

    this.buildTooltips = function () {

        //$('form .form-control', modalNewSelector)
        //    .tooltip({
        //        placement: 'right',
        //        trigger: 'manual',
        //        container: 'body'
        //    });

    }

    this.bindControlEvents = function () {

        $('.modal').on('show.bs.modal', function () {
            $('.modal .modal-dialog')
                .addClass('bounceInRight animated');
        });

        $('.modal').on('hidden.bs.modal', function () {
            if ($('.modal.show').length > 0)
                $('body').addClass('modal-open');
        });

        $('.modal.modalNew').on('hide.bs.modal', function () {
            self.newDialogClosing($('form', this));
        });
        $('.modal.modalNew').on('hidden.bs.modal', function () {
            resetForm($(this));
            self.newDialogClosed($('form', this));
        });

        $('.modal.modalEdit').on('hide.bs.modal', function () {
            self.editDialogClosing($('form', this));
        });
        $('.modal.modalEdit').on('hidden.bs.modal', function () {
            resetForm($(this));
            self.editDialogClosed($('form', this));
        });

        $('.modal.modalRemove').on('hide.bs.modal', function () {
            self.removeDialogClosing($('form', this));
        });
        $('.modal.modalRemove').on('hidden.bs.modal', function () {
            self.removeDialogClosed($('form', this));
        });


        $(btNewSelector).on('click', function () {
            $('[data-newvisible="false"]').prop('disabled', true).closest('.field').hide();
            $('[data-editingdisabled]').prop('disabled', false);

            var form = $('form', modalNewSelector);
            var tabs = form.find('ul.nav.nav-tabs');

            if (tabs.length > 0) {
                tabs.find('li > a')
                    .removeClass('active')
                    .eq(0)
                    .addClass('active');

                form.find('div.tab-content div.tab-pane')
                    .removeClass('active')
                    .eq(0)
                    .addClass('active');
            }

            $(modalNewSelector)
                .one('show.bs.modal', function () {
                    currentModal = 'new';
                    self.tabChanged($(this).find('.modal-dialog'), form, 0, tabs.first().text().trim());
                    self.newDialogOpening(this, form);
                })
                .one('shown.bs.modal', function () {
                    self.newDialogShown(this, form);
                })
                .modal({ backdrop: 'static' });
        });

        $(btEditSelector).on('click', function () {
            editSelectedRecord(this);
        });

        $(btRemoveSelector).on('click', function () {
            removeSelectedRecord(this);
        });

        $(modalNewSaveSelector).on('click', function () {
            $('form', modalNewSelector).submit();
        });

        $(modalEditSaveSelector).on('click', function () {
            editSaveSelectedRecord(this);
        });


        bindTabToggles($('form', modalNewSelector));
        bindTabToggles($('form', modalEditSelector));

        setupValidator($('form', modalNewSelector));
        setupValidator($('form', modalEditSelector));

        fixModalBackdrops();

    }

    this.bindFiltersFieldsEvents = function () {

        if (!filtersEnabled)
            return;

        $(filtersSelector)
            .find('input, select, textarea')
            .on('change input', function (e) {

                var show = false;

                if (filtersApplied == null) {
                    show = (e.target.value.trim() != '');
                }
                else {
                    var fields = filtersGetValues();

                    for (var i in fields) {
                        var field = fields[i];
                        var applied = filtersApplied.find(function (f) { return f.id == field.id; });

                        if (applied != undefined && applied.value != field.value) {
                            show = true;
                            break;
                        }
                    }

                }

                if (show) {
                    if (!$(btApplyFiltersSelector).is(':visible'))
                        $(btApplyFiltersSelector).show();
                }
                else {
                    if ($(btApplyFiltersSelector).is(':visible'))
                        $(btApplyFiltersSelector).hide();
                }
            });

    }


    this.init = function () {
        mainClass = '.' + $('div[data-mainclass]').data('mainclass');
        dtSelector = mainClass + ' table.dt-view';
        btNewSelector = mainClass + ' .toolbar-actions button.btNew';
        btEditSelector = mainClass + ' .toolbar-actions button.btEdit';
        btRemoveSelector = mainClass + ' .toolbar-actions button.btRemove';
        btApplyFiltersSelector = mainClass + ' .toolbar-extended button.btApplyFilters';
        btClearFiltersSelector = mainClass + ' .toolbar-extended button.btClearFilters';
        btToggleFiltersSelector = mainClass + ' .toolbar-extended button.btFilters';
        filtersSelector = mainClass + ' .toolbar-advanced-filters';
        modalNewSelector = mainClass + ' .modal.modalNew';
        modalEditSelector = mainClass + ' .modal.modalEdit';
        modalRemoveSelector = mainClass + ' .modal.modalRemove';
        modalNewSaveSelector = mainClass + ' .modal.modalNew .modal-footer button.btSave';
        modalEditSaveSelector = mainClass + ' .modal.modalEdit .modal-footer button.btSave';
        modalRemoveConfirmSelector = mainClass + ' .modal.modalRemove .modal-footer button.btSave';

        self.buildFiltersControls();
        self.buildControls();
        self.buildTooltips();
        self.bindControlEvents();
        self.bindFiltersFieldsEvents();

        ConstraintsMaskEx.init();
    }


    this.initializeDatatable = function (keyProperty, columns, order, pageLength, multipleSelection, selection) {

        dtKeyProperty = keyProperty;

        if (order === undefined || !$.isArray(order))
            order = [[0, 'asc']];

        if (pageLength === undefined || !isFinite(pageLength))
            pageLength = 10;

        var ajaxGetAll = $(dtSelector).data('ajax-action');
        var defaultDom =
            "<'dt--top-section'<l><f>>" +
            "<'table-responsive'tr>" +
            "<'dt--bottom-section d-sm-flex justify-content-sm-between text-center'<'dt--pages-count  mb-sm-0 mb-3'i><'dt--pagination'p>>";

        var selectionStyle = 'single';
        if (selection)
            selectionStyle = selection;

        if (multipleSelection === true)
            selectionStyle = 'os';

        if (filtersEnabled)
            defaultDom = defaultDom.replace('f>', '>');

        dtView = $(dtSelector)
            .DataTableEx({

                ajax: {
                    url: ajaxGetAll,
                    type: 'POST',
                    data: buildFiltersData,
                    error: function (xhr, ajaxOptions, thrownError) {
                        Ajax.ShowError(xhr, xhr.statusText, thrownError);
                    },
                    callback: function (xhr) {
                        Ajax.ShowError(xhr, xhr.statusText, '');
                    }
                },
                processing: true,
                serverSide: true,
                pageLength: pageLength,
                lengthChange: false,
                dom: defaultDom,

                select: { style: selectionStyle, info: (selectionStyle === 'os') },
                columns: columns,
                order: order,

                onSelected: datatableSelectedRow,
                onDoubleClick: datatableDoubleClickedRow,
                onDraw: datatableDraw,
                onInit: datatableInit,
                "createdRow": onCreatedRow
            });

        return dtView;
    }


    this.ajaxFormBegin = function (context, arguments) {
        console.log('ajaxFormBegin', context);

        var dialog = $(this).parents('.modal');
        var $form = $(this);
        var action = 'new';

        if (dialog.hasClass('.modalEdit'))
            action = 'edit';

        var btSave = dialog.find('.modal-footer button.btSave')
        self.buttonLoader(btSave, true, 'Guardando');

        self.onAjaxBegin(dialog, $form, action, arguments);
    }

    this.ajaxFormSuccess = function () {
        console.log('ajaxFormSuccess');

        var dialog = $(this).parents('.modal');
        var isModalClose = (dialog.data('autoclose').toLowerCase() === 'true');

        if (isModalClose) {
            $(modalNewSelector).modal('hide');
            $(modalEditSelector).modal('hide');
        }

        dtView.reload();

        self.dialogSuccess(currentModal);

        Utils.alertSuccess('Guardado correctamente.');

        
        var $form = $(this);
        var action = 'new';

        if (dialog.hasClass('.modalEdit'))
            action = 'edit';

        var btSave = dialog.find('.modal-footer button.btSave')
        self.buttonLoader(btSave, false);

        self.onAjaxSuccess(dialog, $form, action);
    }

    this.ajaxFormFailure = function (context) {
        Ajax.ShowError(context, context.statusText, '');

        var dialog = $(this).parents('.modal');
        var $form = $(this);
        var action = 'new';

        if (dialog.hasClass('.modalEdit'))
            action = 'edit';

        var btSave = dialog.find('.modal-footer button.btSave')
        self.buttonLoader(btSave, false);

        self.onAjaxFailure(dialog, $form, action);
    }


    this.onAjaxBegin = function (dialog, $form, action, arguments) {
        // Implementar en cada ABM.
    }
    this.onAjaxSuccess = function (dialog, $form, action) {
        // Implementar en cada ABM.
    }
    this.onAjaxFailure = function (dialog, $form, action) {
        // Implementar en cada ABM.
    }

    this.newDialogOpening = function (dialog, $form) {
        // Implementar en cada ABM.
    }
    this.newDialogShown = function (dialog, $form) {
        // Implementar en cada ABM.
    }
    this.newDialogClosing = function ($form) {
        // Implementar en cada ABM.
    }
    this.newDialogClosed = function ($form) {
        // Implementar en cada ABM.
    }

    this.editDialogOpening = function ($form, entity) {
        // Implementar en cada ABM.
    }
    this.editDialogShown = function (dialog, $form) {
        // Implementar en cada ABM.
    }
    this.editDialogClosing = function ($form) {
        // Implementar en cada ABM.
    }
    this.editDialogClosed = function ($form) {
        // Implementar en cada ABM.
    }

    this.removeDialogOpening = function (dialog, $form) {
        // Implementar en cada ABM.
    }
    this.removeDialogShown = function (dialog, $form) {
        // Implementar en cada ABM.
    }
    this.removeDialogClosing = function (entity) {
        // Implementar en cada ABM.
    }
    this.removeDialogClosed = function ($form) {
        // Implementar en cada ABM.
    }

    this.dialogSuccess = function (dialog) {
        // Implementar en cada ABM.
    }

    this.mainTableRowSelected = function (dataArray, api) {
        // Implementar en cada ABM.
    }
    this.mainTableDraw = function () {
        // Implementar en cada ABM.
    }

    this.onCreatedRow = function (row, data, dataindex) {
        // Implementar en cada ABM.
    }

    this.tabChanged = function (dialog, $form, index, name) {
        // Implementar en cada ABM.
    }
    this.mainTableRowDoubleClicked = function () {
        // Implementar en cada ABM.
    }

    function datatableSelectedRow(dataArray, api) {
        enableCRUDButtons();
        self.mainTableRowSelected(dataArray, api);
    }

    function datatableDoubleClickedRow() {

        self.mainTableRowDoubleClicked();
    }

    function datatableDraw() {
        enableCRUDButtons();
        self.mainTableDraw();
        feather.replace();

        $('[data-toggle="tooltip"]').tooltip();
    }

    function datatableInit() {
        var toolbarExtended = $('.toolbar-extended');
        var searchInput = $('.dataTables_filter > label');

        toolbarExtended.append(searchInput);
    }

    function onCreatedRow(row, data, dataindex) {
        self.onCreatedRow(row, data, dataindex);
    }

    function enableCRUDButtons() {

        if (dtView.selected().length === 0) {
            $(btEditSelector).prop('disabled', true);
            $(btRemoveSelector).prop('disabled', true);
        }
        else {
            $(btEditSelector).prop('disabled', false);
            $(btRemoveSelector).prop('disabled', false);
        }

    }

    function resetForm($modal) {

        var $form = $modal.find('form');

        if ($form.length == 0)
            return;

        Utils.resetValidator($form);

        markTabToggle($form);

    }

    function setupValidator($form) {

        var validator = $form.data('validator');

        if (validator === undefined)
            return;

        validator.settings.showErrors = function (errorMap, errorList) {
            console.log('validator showErrors', errorList);

            this.defaultShowErrors();
            markTabToggle($form);

            if (!self.errorTooltips)
                return;

            for (var i in errorList) {
                var item = errorList[i];
                var tooltipValidation = $(item.element).data('tooltip-validation');

                if (tooltipValidation !== true)
                    continue;

                var tooltipContainer = item.element;
                var title = $(item.element).attr('title');

                if ($(item.element).hasClass('repeater-select2', 'validate', 'repeater-control'))
                    tooltipContainer = $(item.element).prev('div.repeater-control.select2-container');

                if (title !== undefined && title != '' && title != item.message)
                    $(tooltipContainer).tooltip('dispose');

                var method = 'show';

                if ($('div.tooltip.show').length > 0)
                    method = undefined;

                $(tooltipContainer)
                    .attr('title', item.message)
                    .not('.show')
                    .tooltip({
                        placement: 'top',
                        trigger: 'manual'
                    })
                    .tooltip(method);

                break;
            }
        };

        var success = validator.settings.success;
        validator.settings.success = function (label, element) {
            console.log('validator success');

            success.apply($form.get(0), [$(label)]);

            if (self.errorTooltips)
                $(element).tooltip('dispose');
        }

        validator.settings.submitHandler = function (f) {
            console.log('Submit Form');

            if (self.disabledFields == false)
                $(f).find(':disabled').prop('disabled', false);

            $form.find('input[type=text]').each(function () {

                if (AutoNumeric.isManagedByAutoNumeric(this))
                    AutoNumeric.getAutoNumericElement(this).unformat();

            });


            return true;
        };

        $form.on('invalid-form.validate', function () {
            console.log('validator invalid');
        });

    }

    function bindTabToggles($form) {

        var dialog = $form.parents('.modal-dialog');
        var nav = $form.find('ul.nav.nav-tabs');
        var content = $form.find('div.tab-content');
        var tabs = $form.find('ul.nav.nav-tabs');

        if (nav.length == 0 || content.length == 0)
            return;

        var toggles = nav.find('> li > a');

        if (toggles.length == 0)
            return;

        toggles.each(function (idx, e) {

            $(this).on('click', function () {
                self.tabChanged(dialog, $form, idx, tabs.text().trim());
            });
            
        });

    }

    function markTabToggle($form) {

        var nav = $form.find('ul.nav.nav-tabs');
        var content = $form.find('div.tab-content');

        if (nav.length == 0 || content.length == 0)
            return;

        var toggles = nav.find('> li > a');

        if (toggles.length == 0)
            return;

        for (var i = 0; i < toggles.length; i++) {
            var a = toggles.eq(i);
            var href = a.attr('href');

            var tab = content.find(href);

            if (tab.length == 0)
                continue;

            var li = a.parent();

            if (tab.find('.input-validation-error:not(.select2-container):not(.no-validate)').length == 0) {
                li.removeClass('error');
            }
            else {
                if (!li.hasClass('error'))
                    li.addClass('error');
            }
        }
    }

    function editSelectedRecord(button) {

        $('[data-newvisible]').closest('.field').show();
        $('[data-editingdisabled="true"]').prop('disabled', true);

        var form = $('form', modalEditSelector);
        var dialog = form.parents('.modal-dialog');
        var action = $(button).data('ajax-action');
        var data = dtView.selected()[0];
        var key = data[dtKeyProperty];

        var tabs = form.find('ul.nav.nav-tabs');

        if (tabs.length > 0) {
            tabs.find('li > a')
                .removeClass('active')
                .eq(0)
                .addClass('active');

            form.find('div.tab-content div.tab-pane')
                .removeClass('active')
                .eq(0)
                .addClass('active');
        }

        $.getJSON(action + '/' + key)
            .done(function (response) {

                if (response == null || typeof response !== 'object' || $.isEmptyObject(response) || response.constructor !== {}.constructor) {
                    console.error('Maestros-Edicion: ', response);
                    Utils.alertError();
                }

                if (response.hasOwnProperty('status') && response.hasOwnProperty('detail')) {
                    if (response.status === enums.AjaxStatus.OK) {
                        currentModal = 'edit';
                        self.tabChanged(dialog, form, 0, $(this).text().trim());
                        self.editDialogOpening(form, response.detail);
                    }
                    else {
                        console.error('Maestros-Edicion: ' + response.detail);
                        Utils.alertError();
                    }
                }
                else {
                    console.error('Maestros-Edicion: ', response);
                    Utils.alertError();
                }

                $(modalEditSelector)
                    .on('shown.bs.modal', function () {
                        self.editDialogShown(this, form);
                    })
                    .modal({ backdrop: 'static' });
            })
            .fail(function (xhr, ajaxOptions, thrownError) {
                console.error('Maestros-Edicion: ' + thrownError);
                Ajax.ShowError(xhr, xhr.statusText, thrownError);
            });
    }

    function editSaveSelectedRecord(button) {

        var form = $('form', modalEditSelector);

        form.submit();

    }

    function removeSelectedRecord(button) {

        var modal = $(modalRemoveSelector);
        var form = modal.find('form');
        var action = $(button).data('ajax-action');
        var data = dtView.selected()[0];
        var key = data[dtKeyProperty];

        $(modalRemoveConfirmSelector)
            .off()
            .one('click', function () {

                $.post(action + '/' + key)
                    .done(function (response) {

                        if (response == null || typeof response !== 'object' || $.isEmptyObject(response) || response.constructor !== {}.constructor) {
                            console.error('Maestros-Eliminacion: ', response);
                            Utils.alertError();
                        }

                        if (response.hasOwnProperty('status') && response.hasOwnProperty('detail')) {
                            if (response.status === enums.AjaxStatus.OK)
                                self.removeDialogClosing(data);
                            else {
                                console.error('Maestros-Eliminacion: ' + response.detail);
                                Utils.alertError();
                            }
                        }
                        else {
                            console.error('Maestros-Eliminacion: ', response);
                            Utils.alertError();
                        }

                        dtView.reload();

                        self.dialogSuccess(currentModal);

                        $(modalRemoveSelector).modal('hide');
                    })
                    .fail(function (xhr, ajaxOptions, thrownError) {

                        $(modalRemoveSelector).modal('hide');
                        console.error('Maestros-Eliminacion: ' + thrownError);
                        Ajax.ShowError(xhr, xhr.statusText, thrownError);

                    });

            });

        modal
            .one('show.bs.modal', function () {
                currentModal = 'remove';
                self.removeDialogOpening(modal, form);
            })
            .one('shown.bs.modal', function () {
                self.removeDialogShown(modal, form);
            })
            .modal({ backdrop: 'static' });
    }

    this.buttonLoader = function(button, flag, text) {

        var data = button.data('buttonLoader');

        if (flag && (data == undefined || data == null)) {
            data = {
                flag: flag,
                text: text || button.text(),
                html: button.html()
            };

            button.data('buttonLoader', data);
        }

        if (flag) {
            button
                .prop('disabled', true)
                .html('<i class="fas fa-cog fa-spin"></i> ' + data.text);
        }
        else if ($.isPlainObject(data)) {
            button.data('buttonLoader', null);
            button
                .prop('disabled', false)
                .html(data.html);
        }

    }


    function buildFiltersData(data) {

        if (!filtersEnabled)
            return data;

        var values = {};

        $(filtersSelector)
            .find('input:not(.select2-offscreen), select, textarea')
            .each(function () {
                var value = this.value;

                if (this.tagName == 'SELECT') {
                    var dataValue = $(this).data('value-property');
                    var data = $(this).select2('data');

                    if (data != null)
                        value = (dataValue === 'Value' ? data.id : data.text);
                }

                if (value.trim() != '')
                    values[this.id.replace('Filter_', '')] = value;
            });

        if (Object.keys(values).length == 0)
            return data;

        return $.extend({}, data, {
            filters: JSON.stringify(values)
        });
    }

    function filtersGetValues() {

        return $(filtersSelector)
            .find('input, select, textarea')
            .filter(function () {
                return this.id.indexOf('Filter') == 0
            })
            .map(function () {
                return { id: this.id, value: this.value };
            })
            .get();

    }

    function applyFilters(e) {

        filtersApplied = filtersGetValues();

        var values = filtersApplied.filter(function (f) {
            return f.value.trim() != ''
        });

        dtView.reload();

        $(btApplyFiltersSelector).hide();

        if (values.length > 0)
            $(btClearFiltersSelector).show();
        else
            $(btClearFiltersSelector).hide();

    }

    function cleanFilters(e) {

        if (!filtersEnabled)
            return;

        filtersApplied = null;

        $(filtersSelector)
            .find('input, select, textarea')
            .each(function () {
                $(this).val('');
                $(this).select2('val', null);
            });

        dtView.reload();
        $(btApplyFiltersSelector).hide();
        $(btClearFiltersSelector).hide();
    }

    function fixModalBackdrops() {

        //
        // https://stackoverflow.com/a/24914782/1812392
        //
        $(document)
            .on('show.bs.modal', '.modal', function () {
                var zindex = 1040 + 10 * $('.modal:visible').length;
                $(this).css('z-index', zindex);

                setTimeout(function () {
                    $('.modal-backdrop')
                        .not('.modal-stack')
                        .css('z-index', zindex - 1)
                        .addClass('modal-stack')
                });
        });

    }
});
