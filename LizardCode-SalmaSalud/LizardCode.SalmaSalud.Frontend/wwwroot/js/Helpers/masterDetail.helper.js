/// <reference path="..\..\lib\jquery\jquery-3.2.1.js" />

(function ($) {

    var defaults = {
        readonly: false,
        disableEdit: false,
        disableRemove: false,
        disableAdd: false
    };

    var formats = {
        date: "date",
        datetime: "datetime",
        currency: "currency",
    };


    $.fn.MasterDetail = function (settings) {

        var initialized = false;
        var opts;
        var $component = this;
        var $data;
        var $form;
        var $table;
        var $tableHead;
        var $tableBody;
        var $tableFoot;
        var $rowTemplate;
        var $dialog;

        var items = [];

        if (this.length > 1) {
            this.each(function (i, e) {
                new $(e).MasterDetail(settings);
            });

            return;
        }

        opts = $.extend({}, defaults, settings);

        $data = $component.data('master-detail-component');
        $form = $component.parents('form');
        $table = $component.find('> table.master-detail-table');
        $tableHead = $table.find('> thead');
        $tableBody = $table.find('> tbody');
        $tableFoot = $table.find('> tfoot');

        if ($data === undefined || $data === null)
            initialize();
        else {
            opts = $data.settings;
            items = $data.items;
            $rowTemplate = $data.rowTemplate;
            $dialog = $($data.modalSelector);
        }

        function initialize() {

            if (opts.readonly === true)
                $component.addClass('readonly');

            if (opts.disableEdit === true)
                $table.addClass('disable-edit');

            if (opts.disableRemove === true)
                $table.addClass('disable-remove');

            if (opts.disableAdd === true)
                $table.addClass('disable-add');

            $dialog = $(opts.modalSelector);

            var $rows = $tableBody.find('> tr');

            $rowTemplate = $rows.eq(0)
                .remove()
                .attr('class', '');

            $table.on('mouseenter', 'i.action-edit', function (e) {
                $(this).closest('div').animate({ scrollLeft: '+=600' }, 1000, 'linear');
            });

            $table.on('click', 'i.action-edit', showDialog);
            $table.on('click', 'i.action-add', showDialog);
            $table.on('click', 'i.action-remove', removeItem);
            $table.on('click', 'tfoot > tr > td > div', showDialog);

            $component.data('master-detail-component', {
                items: items,
                rowTemplate: $rowTemplate,
                settings: opts
            });

            setupValidator();

            $component.trigger('master-detail:init');
            initialized = true;

        }


        function setupValidator() {

            var $form = $dialog.find('form[role=form]');
            var validator = $form.data('validator');

            if (validator === undefined)
                return;

            validator.settings.showErrors = function (errorMap, errorList) {
                console.log('MasterDetail Validator showErrors()', errorList);

                this.defaultShowErrors();
                //markTabToggle($form);

                //if (!self.errorTooltips)
                //    return;

                //for (var i in errorList) {
                //    var item = errorList[i];
                //    var tooltipValidation = $(item.element).data('tooltip-validation');

                //    if (tooltipValidation !== true)
                //        continue;

                //    var title = $(item.element).attr('title');

                //    if (title !== undefined && title != '' && title != item.message)
                //        $(item.element).tooltip('dispose');

                //    var method = 'show';

                //    if ($('div.tooltip.show').length > 0)
                //        method = undefined;

                //    $(item.element)
                //        .attr('title', item.message)
                //        .not('.show')
                //        .tooltip({
                //            placement: 'top',
                //            trigger: 'manual'
                //        })
                //        .tooltip(method);

                //    break;
                //}
            };

            var success = validator.settings.success;
            validator.settings.success = function (label, element) {
                console.log('MasterDetail Validator success');

                success.apply($form.get(0), [$(label)]);
            }

            validator.settings.submitHandler = function (f, e) {
                e.preventDefault();
                console.log('MasterDetail Submitting form');

                return false;
            };

            $form.on('invalid-form.validate', function () {
                console.log('MasterDetail Validator invalid');
            });

        }

        function validatorSubmitHandler(f, e, dialog, action) {

            e.preventDefault();
            console.log('MasterDetail Submitting form');

            $(f).find(':disabled').prop('disabled', false);

            action(f, e, dialog);
            dialog.modal('hide');

            return false;

        }

        function rebuildIdName($rows) {

            for (var i = 0; i < $rows.length; i++) {

                var $row = $rows.eq(i);
                var $cells = $row.find('.master-detail-cell, .master-detail-cell-value');

                for (var j = 0; j < $cells.length; j++) {

                    var $cell = $cells.eq(j);
                    var newId = $cell.attr('id').replace(/_\d+__/, '_' + i + '__');
                    var newName = $cell.attr('name').replace(/\[\d+\]/, '[' + i + ']');

                    $cell.attr('id', newId);
                    $cell.attr('name', newName);

                }

            }

        }

        function addItem(f, e, $dialog) {

            var item = {};
            var regex = new RegExp(/\w+$/);
            var $form = $dialog.find('form[role=form]');

            $data = $component.data('master-detail-component');
            items = $data.items;

            var inputTexts = $form.find('input[type=hidden], input[type=text]:not(.select2-offscreen), textarea');
            var selects = $(f).find('select.select2-field, div.select2-container');

            inputTexts.each(function () {
                var name = $(this).prop('name');

                if (name == '')
                    return;

                var match = regex.exec(name);

                if (match.length != 1)
                    throw 'MasterDetail propiedad "name" incorrecta';

                var realValue = $(this).val();
                if (AutoNumeric.isManagedByAutoNumeric(this))
                    realValue = AutoNumeric.getNumber(this);
                if ($(this).hasClass('flatpickr') && $(this).val())
                    realValue = moment($(this).val(), enums.FormatoFecha.DefaultFullFormat).format(enums.FormatoFecha.DatabaseFullFormat);

                item[match[0]] = {
                    value: realValue,
                    text: $(this).val()
                };
            });

            selects.each(function () {
                var obj = $(this);
                if ($(this).hasClass('select2-container'))
                    obj = $(this).next();

                var data = $(obj).select2('data');
                var name = $(obj).prop('name');

                if (name == '')
                    return;

                if (data == null)
                    return;

                var match = regex.exec(name);

                if (match.length != 1)
                    throw 'MasterDetail propiedad "name" incorrecta';

                item[match[0]] = {
                    value: data.id,
                    text: data.text
                };
            });

            var $newRow = $rowTemplate.clone();
            var text = "";

            for (var i in Object.keys(item)) {
                var key = Object.keys(item)[i];
                var prop = item[key];

                $newRow
                    .find('input.master-detail-cell-value[name="' + key + '"], input.master-detail-cell-value[name$=".' + key + '"]')
                    .val(prop.value)
                    //.HiddenValueHook(function (o, v) { itemObservable(o, key, v) });

                var $span = $newRow
                    .find('span.master-detail-cell[name="' + key + '"], span.master-detail-cell[name$=".' + key + '"]');

                if (prop.text) {
                    switch ($span.data('format')) {
                        case formats.date:
                            text = moment(prop.text, enums.FormatoFecha.DefaultFormat).format(enums.FormatoFecha.DatabaseFormat);
                            break;
                        case formats.datetime:
                            text = moment(prop.text, enums.FormatoFecha.DefaultFullFormat).format(enums.FormatoFecha.DatabaseFullFormat);
                            break;
                        case formats.currency:
                            text = accounting.formatMoney(prop.value)
                            break;
                        default:
                            text = prop.text;
                            break;
                    }
                }
                else
                    text = prop.text;

                if ($span.find('span.check-mark').length == 1) {
                    $span.find('span.check-mark').removeClass('checked');

                    if (prop.value == true)
                        $span.find('span.check-mark').addClass('checked');
                }
                else
                    $span.text(text);
            }

            $newRow.data('master-detail-item', item);

            var $rows = $tableBody.find('> tr');
            var onAdding = $.Event('master-detail:row:adding');

            $component.trigger(onAdding, [item, items, $newRow, $rows]);

            if (onAdding.isDefaultPrevented())
                return;

            items.push(item);

            $tableFoot.addClass('hidden');
            $tableBody.append($newRow);

            $rows = $tableBody.find('> tr');

            rebuildIdName($rows);
            $component.trigger('master-detail:row:added', [item, items, $newRow, $rows]);

        }

        function editItem(f, e, $dialog) {

            var $row = $(e.target).closest('tr');
            var item = $row.data('master-detail-item');
            var regex = new RegExp(/\w+$/);
            
            var inputTexts = $(f).find('input[type=text]:not(.select2-offscreen), textarea');
            var selects = $(f).find('select.select2-field, div.select2-container');

            inputTexts.each(function () {
                var name = $(this).prop('name');

                if (name == '')
                    return;

                var match = regex.exec(name);

                if (match.length != 1)
                    throw 'MasterDetail propiedad "name" incorrecta';

                var realValue = $(this).val();
                if (AutoNumeric.isManagedByAutoNumeric(this))
                    realValue = AutoNumeric.getNumber(this);
                if ($(this).hasClass('flatpickr') && $(this).val())
                    realValue = moment($(this).val(), enums.FormatoFecha.DefaultFullFormat).format(enums.FormatoFecha.DatabaseFullFormat);

                item[match[0]] = {
                    value: realValue,
                    text: $(this).val()
                };
            });

            selects.each(function () {
                var obj = $(this);
                if ($(this).hasClass('select2-container'))
                    obj = $(this).next();

                var data = $(obj).select2('data');
                var name = $(obj).prop('name');

                if (name == '')
                    return;

                if (data == null)
                    return;

                var match = regex.exec(name);

                if (match.length != 1)
                    throw 'MasterDetail propiedad "name" incorrecta';

                item[match[0]] = {
                    value: data.id,
                    text: data.text
                };
            });

            var text = "";
            for (var i in Object.keys(item)) {
                var key = Object.keys(item)[i];
                var prop = item[key];

                $row
                    .find('input.master-detail-cell-value[name="' + key + '"], input.master-detail-cell-value[name$=".' + key + '"]')
                    .val(prop.value);

                var $span = $row
                    .find('span.master-detail-cell[name="' + key + '"], span.master-detail-cell[name$=".' + key + '"]');

                if (prop.text) {
                    switch ($span.data('format')) {
                        case formats.date:
                            text = moment(prop.text, enums.FormatoFecha.DefaultFormat).format(enums.FormatoFecha.DatabaseFormat);
                            break;
                        case formats.datetime:
                            text = moment(prop.text, enums.FormatoFecha.DatabaseFullFormat).format(enums.FormatoFecha.DatabaseFullFormat);
                            break;
                        case formats.currency:
                            text = accounting.formatMoney(prop.value)
                            break;
                        default:
                            text = prop.text;
                            break;
                    }
                }
                else
                    text = prop.text;

                if ($span.find('span.check-mark').length == 1) {
                    $span.find('span.check-mark').removeClass('checked');

                    if (prop.value == true)
                        $span.find('span.check-mark').addClass('checked');
                }
                else
                    $span.text(text);

            }

            var $rows = $tableBody.find('> tr');
            $data = $component.data('master-detail-component');

            $component.trigger('master-detail:row:edited', [item, $data.items, $row, $rows]);

        }

        function loadItem(f, e) {

            var $row = $(e.target).closest('tr');
            var item = $row.data('master-detail-item');

            for (var i in Object.keys(item)) {
                var key = Object.keys(item)[i];
                var prop = item[key];

                var field = $(f).find('[name="' + key + '"], [name$=".' + key + '"]');
                var tag = field.prop('tagName');

                if (tag == 'INPUT') {
                    if (AutoNumeric.isManagedByAutoNumeric(field[0]))
                        AutoNumeric.set(field[0], prop.value);
                    else if (field.hasClass('flatpickr') && prop.value)
                        field.val(moment(prop.value, enums.FormatoFecha.DatabaseFormat).format(enums.FormatoFecha.DefaultFormat));
                    else
                        field.val(prop.value);
                }
                
                else if (tag == 'TEXTAREA')
                    field.text(prop.value);
                else if (tag == 'SELECT')
                    field.select2('val', prop.value);
            }

            return item;

        }

        function addItems(rowsArray) {

            items = [];
            $tableBody.find('tr').remove();
            $tableFoot.addClass('hidden');

            var $rows

            for (var i = 0; i < rowsArray.length; i++) {

                var dataItem = rowsArray[i];
                var properties = Object.keys(dataItem);
                var item = {};
                var $newRow = $rowTemplate.clone();

                $newRow
                    .find('input.master-detail-cell-value')
                    .each(function () {

                        var propertyValue = $(this).data('property');
                        var keyValue = properties
                            .filter(function (k) { return k.toLowerCase() == propertyValue.toLowerCase(); })
                            .pop();

                        if (keyValue === undefined)
                            return;

                        var $span = $(this).next('span.master-detail-cell');
                        var propertyText = $span.data('property');
                        var keyText = properties
                            .filter(function (k) { return k.toLowerCase() == propertyText.toLowerCase(); })
                            .pop();

                        item[propertyValue] = {
                            value: dataItem[keyValue],
                            text: dataItem[keyText]
                        };

                        if (item[propertyValue].text) {
                            switch ($span.data('format')) {
                                case formats.date:
                                    $text = moment(item[propertyValue].text, enums.FormatoFecha.DatabaseFormat).format(enums.FormatoFecha.DefaultFormat);
                                    break;
                                case formats.datetime:
                                    $text = moment(item[propertyValue].text, enums.FormatoFecha.DatabaseFullFormat).format(enums.FormatoFecha.DefaultFullFormat);
                                    break;
                                case formats.currency:
                                    $text = accounting.formatMoney(item[propertyValue].text)
                                    break;
                                default:
                                    $text = item[propertyValue].text;
                                    break;
                            }
                        }
                        else
                            $text = item[propertyValue].text;

                        $(this)
                            .val(item[propertyValue].value);
                            //.HiddenValueHook(function (o, v) { itemObservable(o, propertyValue, v) });

                        if ($span.find('span.check-mark').length == 1) {
                            $span.find('span.check-mark').removeClass('checked');

                            if (item[propertyValue].value == true)
                                $span.find('span.check-mark').addClass('checked');
                        }
                        else
                            $span.text($text);
                    });

                $rows = $tableBody.find('> tr');

                var onAdding = $.Event('master-detail:row:adding');

                $component.trigger(onAdding, [item, items, $newRow, $rows]);

                if (onAdding.isDefaultPrevented())
                    continue;

                items.push(item);

                $data = $component.data('master-detail-component');
                $data.items = items;
                $component.data('master-detail-component', $data);

                $newRow.data('master-detail-item', item);
                $tableBody.append($newRow);
                $rows = $tableBody.find('> tr');

                $component.trigger('master-detail:row:added', [item, items, $newRow, $rows]);

            }

            $rows = $tableBody.find('> tr');
            rebuildIdName($rows);
            $component.trigger('master-detail:source:loaded', [items, $rows]);

        }

        function removeItem(e) {

            var $row = $(e.target).closest('tr');
            var item = $row.data('master-detail-item');

            $data = $component.data('master-detail-component');
            $data.items.splice($data.items.indexOf(item), 1);
            $row.remove();

            var $rows = $tableBody.find('> tr');

            if ($rows.length == 0)
                $tableFoot.removeClass('hidden');

            rebuildIdName($rows);
            
            $component.data('master-detail-component', $data);
            $component.trigger('master-detail:row:removed', [item, $data.items, $row, $rows]);

        }


        function itemObservable(o, key, value) {

            var $row = $(o).closest('tr');
            var item = $row.data('master-detail-item');
            var keys = Object.keys(item);

            if (keys.indexOf(key) != -1)
                item[key].value = value;

        }

        function showDialog(e) {

            var $form = $dialog.find('form[role=form]');
            var validator = $form.data('validator');
            var action = addItem;
            var title = 'Agregar item...';
            var $item = undefined;


            if ($(e.target).hasClass('action-edit')) {
                action = editItem;
                title = 'Editar item...';
                $item = loadItem($form, e);
            }

            $dialog
                .find('.modal-title')
                .text(title);

            $dialog
                .one('hidden.bs.modal', function () {

                    Utils.resetValidator($form);

                    $dialog
                        .find('button[type=submit]')
                        .off();

                    if (validator === undefined)
                        return;

                    validator.settings.submitHandler = function (f, e) {
                        e.preventDefault();
                        console.log('MasterDetail Submitting form');

                        return false;
                    };

                })
                .one('show.bs.modal', function (e) {
                    $('[data-editingdisabled="true"]', $form).prop('disabled', true);
                    if (action == editItem)
                        $component.trigger('master-detail:dialog-edit-opening', [$dialog, $form, $item]);
                    else
                        $component.trigger('master-detail:dialog-add-opening', [$dialog, $form]);
                })
                .modal({ backdrop: 'static' });

            $dialog
                .find('button[type=submit]')
                .off()
                .on('click', function () {

                    if (validator === undefined)
                        return;

                    validator.settings.submitHandler = function (f, ee) {
                        return validatorSubmitHandler(f, e, $dialog, action);
                    };

                    $form.submit();

                });
        }


        var api = {
            clear: function () {
                items = [];
                $tableBody.find('tr').remove();
                $tableFoot.removeClass('hidden');
                $component.data('master-detail-component').items = [];
                $component.trigger('master-detail:empty');
            },

            source: function (rowsArray) {
                addItems(rowsArray);
            },

            getItems: function (includeAllColumns) {
                $data = $component.data('master-detail-component');

                if (includeAllColumns !== true)
                    return $data.items;

                var rows = $tableBody.find('tr');
                var items = [];

                rows.each(function (i, row) {

                    var item = { ...$(row).data('master-detail-item') };
                    var keys = Object.keys(item);
                    var fields = $(this).find('input[type=hidden].master-detail-cell-value');

                    fields.each(function (j, field) {
                        var property = $(field).data('property');
                        var text = $(field)
                            .next('span.master-detail-cell')
                            .text()
                            .trim();

                        if (keys.indexOf(property) == -1) {
                            item[property] = {
                                value: $(field).val(),
                                text: text == ''
                                    ? $(field).val()
                                    : text
                            };
                        }
                    });

                    items.push(item);

                });

                return items;
            },

            refresh: function () {
                var rows = $tableBody.find('tr');

                rows.each(function (i, row) {

                    var item = { ...$(row).data('master-detail-item') };
                    var fields = $(this).find('input[type=hidden].master-detail-cell-value');

                    fields.each(function (j, field) {
                        var property = $(field).data('property');
                        $(field).val(item[property].value)
                            .next('span.master-detail-cell')
                            .text(item[property].text);
                    });
                });
            }
        };

        return api;
    };

    $.fn.HiddenValueHook = function (callback) {

        const property = 'value';

        function findPropertyDescriptor(obj, prop) {
            if (obj != null) {
                return Object.hasOwnProperty.call(obj, prop)
                    ? Object.getOwnPropertyDescriptor(obj, prop)
                    : findPropertyDescriptor(Object.getPrototypeOf(obj), prop);
            }
        }

        this.each(function (idx, obj) {
            var descriptor = findPropertyDescriptor(obj, property);

            Object.defineProperty(obj, property, {
                configurable: true,
                enumerable: true,

                get() {
                    return descriptor.get.call(this);
                },

                set(v) {
                    descriptor.set.call(this, v);
                    callback(obj, v)
                }
            });
        });

    };

}(jQuery));