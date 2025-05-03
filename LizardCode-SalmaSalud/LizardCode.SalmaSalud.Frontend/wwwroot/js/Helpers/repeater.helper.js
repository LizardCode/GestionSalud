/// <reference path="..\..\lib\jquery\jquery-3.2.1.js" />
/// <reference path="..\..\js\Helpers\parseDynamicContent.helper.js" />

(function ($) {

    $.fn.Repeater = function (settings) {

        var initialized = false;
        var opts;
        var $control = this;
        var $data;
        var $form;
        var $table;
        var $tableHead;
        var $tableBody;
        var $tableFoot;
        var $rowTemplate;

        var defaultSettings = {
            readonly: false,
            disableRemove: false,
            disableAdd: false,
            autoAddRow: true,
            min: 0,
            max: 0,
            dropdownAutoWidth: true,
            select2Width: ''
        };

        if (this.length > 1) {
            this.each(function (i, e) {
                $(e).Repeater(settings);
            });

            return;
        }

        opts = $.extend(defaultSettings, settings);

        $data = $control.data('repeater-control');
        $form = $control.closest('form');
        $table = $control.find('> table.repeater-table');
        $tableHead = $table.find('> thead');
        $tableBody = $table.find('> tbody');
        $tableFoot = $table.find('> tfoot');

        if ($data === undefined)
            initialize();
        else
            $rowTemplate = $data.rowTemplate;


        function initialize() {

            if (opts.readonly === true)
                $control.addClass('readonly');

            if (opts.disableRemove === true)
                $table.addClass('disable-remove');

            if (opts.disableAdd === true) {
                $table.addClass('disable-add');
                setReadonlyMode(true);
            }

            if (opts.min > opts.max && opts.max > 0)
                throw 'Error de configuración. El parámetro "Min" no puede ser mayor al parámetro "Max"';

            if (opts.min > 0)
                addMinItemsValidator();

            if (opts.max > 0)
                addMaxItemsValidator();

            var $rows = $tableBody.find('> tr');

            $rowTemplate = $rows.eq(0)
                .remove()
                .attr('class', '');

            $table.on('mouseenter', 'i.action-add', function (e) {
                $(this).closest('div').animate({ scrollLeft: '+=600' }, 1000, 'linear');
            });

            $table.on('click', 'i.action-add', addRow);
            $table.on('click', 'i.action-remove', removeRow);
            $table.on('click', 'tfoot > tr > td > div', addFirstRow);
            $table.on('focus', 'tbody > tr > td .repeater-control', fieldFocus);
            if (opts.autoAddRow === true)
                $table.on('keydown', 'tbody > tr > td:nth-last-child(2)', autoAddRow);
            $table.on('change', 'tbody > tr > td select.repeater-control', fieldChange);
            $table.on('keyup', 'tbody > tr > td input[type=text].repeater-control', fieldChange);
            $table.on('blur', 'tbody > tr > td input[type=text].repeater-control', fieldBlur);
            $table.on('change', 'tbody > tr > td input[type=checkbox].repeater-control', fieldCheckboxChange);

            $control.data('repeater-control', { rowTemplate: $rowTemplate });
            $control.trigger('repeater:init');
            initialized = true;
        }


        function rebuildIdName($control, $rows) {

            $control.find('.hidMinValidator').val($rows.length).valid();
            $control.find('.hidMaxValidator').val($rows.length).valid();

            for (var i = 0; i < $rows.length; i++) {

                var $row = $rows.eq(i);
                var $controls = $row.find('.repeater-control:not(div)');

                for (var j = 0; j < $controls.length; j++) {

                    var $control = $controls.eq(j);
                    var newId = $control.attr('id').replace(/_\d+__/, '_' + i + '__');
                    var newName = $control.attr('name').replace(/\[\d+\]/, '[' + i + ']');

                    $control.attr('id', newId);
                    $control.attr('name', newName);

                }
            }

        }

        function addRow(e) {

            var $row = $(e.target).closest('tr');
            var $newRow = $rowTemplate.clone();

            $row.after($newRow);

            buildSelect2($newRow.find('select.repeater-select2'));

            $newRow.find(`input[type=text][data-constraint='autonumeric']`).each(function (i, obj) {
                var obj = $(obj)[0];
                var type = $(obj).data('constraint-type');

                var currencySymbol = $(obj).data('constraint-currencysymbol');
                var decimalCharacter = $(obj).data('constraint-decimalcharacter');
                var digitGroupSeparator = $(obj).data('constraint-digitgroupseparator');
                var decimalPlaces = $(obj).data('constraint-decimalplaces');

                switch (type) {
                    case enums.AutoNumericConstraintType.Numeric:
                        new AutoNumeric(obj, { unformatOnSubmit: true, decimalCharacter: decimalCharacter, emptyInputBehavior: "press", digitGroupSeparator: digitGroupSeparator, decimalPlaces: decimalPlaces, decimalCharacterAlternative: "." });
                        break;

                    case enums.AutoNumericConstraintType.Currency:
                        new AutoNumeric(obj, { unformatOnSubmit: true, currencySymbol: currencySymbol, emptyInputBehavior: "press", decimalCharacter: decimalCharacter, digitGroupSeparator: digitGroupSeparator, decimalPlaces: decimalPlaces, decimalCharacterAlternative: "." });
                        break;

                    case enums.AutoNumericConstraintType.Percentage:
                        new AutoNumeric(obj, { unformatOnSubmit: true, decimalCharacter: decimalCharacter, emptyInputBehavior: "press", decimalPlaces: decimalPlaces, decimalCharacterAlternative: "." });
                        break;
                }
            });

            $newRow.find(`input[type=text][data-constraint='mask']`).each(function (i, obj) {
                var obj = $(obj)[0];
                var type = $(obj).data('constraint-type');
                var blocks = $(obj).data('constraint-blocks');
                var datePattern = $(obj).data('constraint-datepattern');
                var delimiters = $(obj).data('constraint-delimiters');
                var numericOnly = $(obj).data('constraint-numericonly');
                var uppercase = $(obj).data('constraint-uppercase');

                switch (type) {
                    case enums.MaskConstraintType.Custom:
                        new Cleave(obj, {
                            delimiter: delimiters || "",
                            blocks: blocks || [],
                            uppercase: uppercase || false,
                            numericOnly: numericOnly || false
                        });
                        break;

                    case enums.MaskConstraintType.String:
                        new Cleave(obj, {
                            blocks: blocks || [],
                            uppercase: uppercase
                        });
                        break;

                    case enums.MaskConstraintType.Number:
                        new Cleave(obj, {
                            numeral: true
                        });
                        break;

                    case enums.MaskConstraintType.Date:
                        new Cleave(obj, {
                            date: true,
                            delimiter: "/",
                            datePattern: eval(datePattern)
                        });
                        break;
                }
            });

            var $rows = $tableBody.find('> tr');

            $('.tooltip.show').tooltip('hide');
            rebuildIdName($control, $rows);
            $.validator.unobtrusive.parseDynamicContent($newRow);
            focusFirstField($newRow);
            $control.trigger('repeater:row:added', [$newRow, $rows]);

        }

        function addFirstRow(e) {

            if ($table.hasClass('disable-add'))
                return;

            if ($control.hasClass('readonly'))
                return;

            var $newRow = $rowTemplate.clone();

            $tableFoot.addClass('hidden');
            $tableBody.append($newRow);

            buildSelect2($newRow.find('select.repeater-select2'))

            $newRow.find(`input[type=text][data-constraint='autonumeric']`).each(function (i, obj) {
                var obj = $(obj)[0];
                var type = $(obj).data('constraint-type');

                var currencySymbol = $(obj).data('constraint-currencysymbol');
                var decimalCharacter = $(obj).data('constraint-decimalcharacter');
                var digitGroupSeparator = $(obj).data('constraint-digitgroupseparator');
                var decimalPlaces = $(obj).data('constraint-decimalplaces');

                switch (type) {
                    case enums.AutoNumericConstraintType.Numeric:
                        new AutoNumeric(obj, { unformatOnSubmit: true, decimalCharacter: decimalCharacter, emptyInputBehavior: "press", digitGroupSeparator: digitGroupSeparator, decimalPlaces: decimalPlaces, decimalCharacterAlternative: "." });
                        break;

                    case enums.AutoNumericConstraintType.Currency:
                        new AutoNumeric(obj, { unformatOnSubmit: true, currencySymbol: currencySymbol, emptyInputBehavior: "press", decimalCharacter: decimalCharacter, digitGroupSeparator: digitGroupSeparator, decimalPlaces: decimalPlaces, decimalCharacterAlternative: "." });
                        break;

                    case enums.AutoNumericConstraintType.Percentage:
                        new AutoNumeric(obj, { unformatOnSubmit: true, decimalCharacter: decimalCharacter, emptyInputBehavior: "press", decimalPlaces: decimalPlaces, decimalCharacterAlternative: "." });
                        break;
                }
            });

            $newRow.find(`input[type=text][data-constraint='mask']`).each(function (i, obj) {
                var obj = $(obj)[0];
                var type = $(obj).data('constraint-type');
                var blocks = $(obj).data('constraint-blocks');
                var datePattern = $(obj).data('constraint-datepattern');
                var delimiters = $(obj).data('constraint-delimiters');
                var numericOnly = $(obj).data('constraint-numericonly');
                var uppercase = $(obj).data('constraint-uppercase');

                switch (type) {
                    case enums.MaskConstraintType.Custom:
                        new Cleave(obj, {
                            delimiter: delimiters || "",
                            blocks: blocks || [],
                            uppercase: uppercase || false,
                            numericOnly: numericOnly || false
                        });
                        break;

                    case enums.MaskConstraintType.String:
                        new Cleave(obj, {
                            blocks: blocks || [],
                            uppercase: uppercase
                        });
                        break;

                    case enums.MaskConstraintType.Number:
                        new Cleave(obj, {
                            numeral: true
                        });
                        break;

                    case enums.MaskConstraintType.Date:
                        new Cleave(obj, {
                            date: true,
                            delimiter: "/",
                            datePattern: eval(datePattern)
                        });
                        break;
                }
            });

            var $rows = $tableBody.find('> tr');

            $('.tooltip.show').tooltip('hide');
            rebuildIdName($control, $rows);
            $.validator.unobtrusive.parseDynamicContent($newRow);
            focusFirstField($newRow);
            $control.trigger('repeater:row:added', [$newRow, $rows]);

        }

        function addRows(rowsArray) {

            $tableBody.find('tr').remove();
            $tableFoot.addClass('hidden');

            var $rows

            for (var i = 0; i < rowsArray.length; i++) {

                var rowData = rowsArray[i];
                var $newRow = $rowTemplate.clone();
                var keys = Object.keys(rowData);

                for (var j in keys) {

                    var key = keys[j];
                    var cellData = rowData[key];
                    var $field = $newRow
                        .find('.repeater-control')
                        .filter(function () {
                            return $(this).attr('name').toLowerCase().indexOf('.' + key.toLowerCase()) > -1;
                        })
                        .first();

                    if ($field.length > 0)
                        $field.val(cellData).data('value', cellData);

                    if ($control.hasClass('readonly'))
                        $field.prop('disabled', true);

                    if ($field.is(":checkbox")) {
                        if (cellData) {
                            $field.prop('checked', true);
                            $field.val('checked', true);
                            $field.closest('td').find('input[type="hidden"]').val(true);
                        }
                    }

                    var constraint = $field.data('constraint');
                    if (constraint == "autonumeric") {
                        var type = $field.data('constraint-type');
                        var currencySymbol = $field.data('constraint-currencysymbol');
                        var decimalCharacter = $field.data('constraint-decimalcharacter');
                        var digitGroupSeparator = $field.data('constraint-digitgroupseparator');
                        var decimalPlaces = $field.data('constraint-decimalplaces');

                        switch (type) {
                            case enums.AutoNumericConstraintType.Numeric:
                                new AutoNumeric($field[0], { unformatOnSubmit: true, decimalCharacter: decimalCharacter, emptyInputBehavior: "press", digitGroupSeparator: digitGroupSeparator, decimalPlaces: decimalPlaces, decimalCharacterAlternative: "." });
                                break;

                            case enums.AutoNumericConstraintType.Currency:
                                new AutoNumeric($field[0], { unformatOnSubmit: true, currencySymbol: currencySymbol, emptyInputBehavior: "press", decimalCharacter: decimalCharacter, digitGroupSeparator: digitGroupSeparator, decimalPlaces: decimalPlaces, decimalCharacterAlternative: "." });
                                break;

                            case enums.AutoNumericConstraintType.Percentage:
                                new AutoNumeric($field[0], { unformatOnSubmit: true, decimalCharacter: decimalCharacter, emptyInputBehavior: "press", decimalPlaces: decimalPlaces, decimalCharacterAlternative: "." });
                                break;
                        }

                        AutoNumeric.set($field[0], cellData);
                    }

                    if (constraint == "mask") {
                        var type = $field.data('constraint-type');
                        var blocks = $field.data('constraint-blocks');
                        var datePattern = $field.data('constraint-datepattern');
                        var delimiters = $field.data('constraint-delimiters');
                        var numericOnly = $field.data('constraint-numericonly');
                        var uppercase = $field.data('constraint-uppercase');

                        switch (type) {
                            case enums.MaskConstraintType.Custom:
                                new Cleave($field, {
                                    delimiter: delimiters || "",
                                    blocks: blocks || [],
                                    uppercase: uppercase || false,
                                    numericOnly: numericOnly || false
                                });
                                break;

                            case enums.MaskConstraintType.String:
                                new Cleave($field, {
                                    blocks: blocks || [],
                                    uppercase: uppercase
                                });
                                break;

                            case enums.MaskConstraintType.Number:
                                new Cleave($field, {
                                    numeral: true
                                });
                                break;

                            case enums.MaskConstraintType.Date:
                                new Cleave($field, {
                                    date: true,
                                    delimiter: "/",
                                    datePattern: eval(datePattern)
                                });

                                if (cellData != "")
                                    $field.val(moment(cellData, enums.FormatoFecha.DatabaseFormat).format(enums.FormatoFecha.DefaultFormat));

                                break;
                        }
                    }

                    if ($field.hasClass('currency'))
                        $field.val(accounting.formatMoney(cellData));
                }

                $rows = $tableBody.find('> tr');
                $tableBody.append($newRow);
                buildSelect2($newRow.find('select.repeater-select2'));
                $control.trigger('repeater:row:added', [$newRow, $rows]);

            }

            $rows = $tableBody.find('> tr');
            $('.tooltip.show').tooltip('hide');
            rebuildIdName($control, $rows);
            $.validator.unobtrusive.parseDynamicContent($rows);
            $control.trigger('repeater:source:loaded');

        }

        function autoAddRow(e) {

            if (e.which != 9)
                return;

            var $td = $(e.target);
            var $tr = $td.closest('tr');

            if ($tr.next().length == 0)
                addRow({ target: $td });

            e.preventDefault();
        }

        function removeRow(e) {

            var $row = $(e.target).closest('tr');

            $row.remove();

            var $rows = $tableBody.find('> tr');

            if ($rows.length == 0)
                $tableFoot.removeClass('hidden');

            rebuildIdName($control, $rows);
            focusFirstField($rows.last());
            $control.trigger('repeater:row:removed', [$row, $rows]);

        }

        function focusFirstField($tr) {

            var $controls = $tr.find('.repeater-control:not([readonly]):not(.select2-container):visible, .repeater-control .select2-focusser:visible');

            $controls.first().focus();
        }

        function fieldFocus(e) {

            var $tr = $(e.target).closest('tr');
            var $td = $(e.target).closest('td');

            $control.trigger('repeater:field:focus', [{
                tr: $tr,
                td: $td,
                field: $(e.target),
                name: $(e.target).prop('name'),
                value: $(e.target).val()
            }]);

        }

        function fieldBlur(e) {

            var $tr = $(e.target).closest('tr');
            var $td = $(e.target).closest('td');

            $control.trigger('repeater:field:blur', [{
                tr: $tr,
                td: $td,
                field: $(e.target),
                name: $(e.target).prop('name'),
                value: $(e.target).val()
            }]);

        }

        function fieldChange(e) {

            var $tr = $(e.target).closest('tr');
            var $td = $(e.target).closest('td');

            $control.trigger('repeater:field:change', [{
                tr: $tr,
                td: $td,
                field: $(e.target),
                name: $(e.target).prop('name'),
                value: $(e.target).val()
            }]);

        }

        function fieldCheckboxChange(e) {

            var checked = $(e.target).is(':checked');
            $(e.target).closest('td').find('input[type="hidden"]').val(checked);
            $(e.target).val(checked);
        }

        function buildSelect2(element) {

            $(element)
                .on('change', function () {
                    $(this).closest('form').validate();

                    if ($(this).valid())
                        $(this).prev('div.repeater-control.select2-container').tooltip('dispose');
                })
                .Select2Ex({
                    dropdownCssClass: 'repeater-component-dropdown',
                    dropdownAutoWidth: opts.dropdownAutoWidth,
                    width: opts.select2Width
                });

        }


        function addMinItemsValidator() {

            $control
                .find('.hidMinValidator')
                .rules('add', {
                    min: opts.min,
                    messages: {
                        min: jQuery.validator.format('Agregue al menos "{0}" item(s)')
                    }
                });

        }

        function addMaxItemsValidator() {

            $control
                .find('.hidMaxValidator')
                .rules('add', {
                    max: opts.max,
                    messages: {
                        max: jQuery.validator.format('Solo se permiten "{0}" item(s)')
                    }
                });

        }

        function setReadonlyMode(flag) {

            var footerSpan = $tableFoot
                .find('> tr > td > div > span')
                .first();

            footerSpan.text('Lista vacía, haga click aquí para agregar un item nuevo');

            if (flag === true)
                footerSpan.text('Lista vacía');

        }


        var api = {
            clear: function () {
                $tableBody.find('tr').remove();
                $tableFoot.removeClass('hidden');
                $control.trigger('repeater:empty');

                return api;
            },

            source: function (rowsArray) {
                addRows(rowsArray);

                return api;
            },

            comboSource: function (property, optionsArray, addBlankOption) {

                if (property == undefined || property == null || property.length == 0) {
                    console.error('No se especificó valor para property');
                    return;
                }

                if (!$.isArray(optionsArray)) {
                    console.error('optionsArray no es un array');
                    return;
                }

                if (optionsArray == undefined || optionsArray == null) {
                    console.error('No se especificó valor para optionsArray');
                    return;
                }

                var combo = $rowTemplate.find('select[name$=".' + property + '"]');

                if (combo.length == 0) {
                    console.error('No se encontró ningún select con name terminado en: ' + property);
                    return;
                }

                combo.find('option').remove();

                if (optionsArray.length == 0)
                    return;

                var props = Object.keys(optionsArray[0]);

                if (!props.includes('value') || !props.includes('text')) {
                    console.error('optionsArray tiene un formato incorrecto');
                    return;
                }

                if (addBlankOption) {
                    combo.append(
                        $('<option>')
                    );
                }

                for (var i in optionsArray) {
                    var option = optionsArray[i];

                    combo.append(
                        $('<option>')
                            .attr('value', option.value)
                            .text(option.text)
                    );
                }
            },

            readonly: function (flag) {

                $control.removeClass('readonly');
                $control
                    .find('tbody > tr > td > .repeater-control')
                    .prop('disabled', false);

                if (flag !== true)
                    return;

                $control.addClass('readonly');
                $control
                    .find('tbody > tr > td > .repeater-control')
                    .prop('disabled', true);

                setReadonlyMode(flag);

                return api;

            },

            isReadonly: function () {
                return $control.hasClass('readonly');
            }
        };

        return api;
    };

    function setupValidator() {

        $.validator.unobtrusive.adapters.add("repeaterremote", ['additionalfields', 'url'], function (options) {
            options.rules['repeaterremote'] = options.params;
            options.messages['repeaterremote'] = options.message;
        });

        $.validator.addMethod("repeaterremote", function (value, element, params) {

            var row = $(element).closest('tr');
            var field = element.name.substr(element.name.indexOf('.') + 1);
            var action = params.url;
            var parameters = {};
            var result = false;

            parameters[field] = value;

            if (params.additionalfields != null && params.additionalfields.length > 0) {
                var keys = params.additionalfields.split(',');

                for (var i in keys) {
                    var key = keys[i];
                    var additionalField = row.find('[name$=' + key + ']');
                    var value = additionalField.val();

                    if (additionalField.data('select2'))
                        value = additionalField.select2('val');
                    else if (AutoNumeric.isManagedByAutoNumeric(additionalField.get(0)))
                        value = AutoNumeric.getNumber(additionalField.get(0));

                    parameters[key] = value;
                }
            }

            $.ajax({
                url: action,
                type: "POST",
                async: false,
                cache: false,
                data: parameters,

                error: Ajax.ShowError,

                success: function (response) {
                    result = response;
                }
            });

            return result;
        });

    }

    setupValidator();

}(jQuery));