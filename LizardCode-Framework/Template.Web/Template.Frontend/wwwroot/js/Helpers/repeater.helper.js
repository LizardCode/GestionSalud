/// <reference path="..\..\lib\jquery\jquery-3.2.1.js" />
/// <reference path="..\..\js\Helpers\parseDynamicContent.helper.js" />

(function ($) {

    $.fn.Repeater = function (settings) {

        var initialized = false;
        var $control = this;
        var $data;
        var $form;
        var $table;
        var $tableHead;
        var $tableBody;
        var $tableFoot;
        var $rowTemplate;

        var defaultSettings = {
        };

        if (this.length > 1) {
            this.each(function (i, e) {
                $(e).Repeater(settings);
            });

            return;
        }

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

            $.extend(defaultSettings, settings);

            var $rows = $tableBody.find('> tr');

            $rowTemplate = $rows.eq(0)
                .remove()
                .attr('class', '');

            $table.on('click', 'i.action-add', addRow);
            $table.on('click', 'i.action-remove', removeRow);
            $table.on('click', 'tfoot > tr > td > div', addFirstRow);
            $table.on('focus', 'tbody > tr > td .repeater-control', fieldFocus);
            $table.on('keydown', 'tbody > tr > td:nth-last-child(2)', autoAddRow);
            $table.on('change', 'tbody > tr > td select.repeater-control', fieldChange);
            $table.on('keyup', 'tbody > tr > td input[type=text].repeater-control', fieldChange);

            $control.data('repeater-control', { rowTemplate: $rowTemplate });
            $control.trigger('repeater:init');
            initialized = true;
        }


        function rebuildIdName($rows) {

            for (var i = 0; i < $rows.length; i++) {

                var $row = $rows.eq(i);
                var $controls = $row.find('.repeater-control');

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

            var $rows = $tableBody.find('> tr');

            $('.tooltip.show').tooltip('hide');
            rebuildIdName($rows);
            $.validator.unobtrusive.parseDynamicContent($newRow);
            focusFirstField($newRow);
            $control.trigger('repeater:row:added', [$newRow, $rows]);

        }

        function addFirstRow(e) {

            var $newRow = $rowTemplate.clone();

            $tableFoot.addClass('hidden');
            $tableBody.append($newRow);

            var $rows = $tableBody.find('> tr');

            $('.tooltip.show').tooltip('hide');
            rebuildIdName($rows);
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
                    var $control = $newRow
                        .find('.repeater-control')
                        .filter(function () {
                            return $(this).attr('name').toLowerCase().indexOf('.' + key.toLowerCase()) > -1;
                        })
                        .first();

                    if ($control.length > 0)
                        $control.val(cellData);

                }

                $rows = $tableBody.find('> tr');
                $tableBody.append($newRow);
                $control.trigger('repeater:row:added', [$newRow, $rows]);

            }

            $rows = $tableBody.find('> tr');
            $('.tooltip.show').tooltip('hide');
            rebuildIdName($rows);
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
        }

        function removeRow(e) {

            var $row = $(e.target).closest('tr');

            $row.remove();

            var $rows = $tableBody.find('> tr');

            if ($rows.length == 0)
                $tableFoot.removeClass('hidden');

            rebuildIdName($rows);
            focusFirstField($rows.last());
            $control.trigger('repeater:row:removed', [$row, $rows]);

        }

        function focusFirstField($tr) {

            var $controls = $tr.find('.repeater-control:not([readonly]):visible');

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


        var api = {
            clear: function () {
                $tableBody.find('tr').remove();
                $tableFoot.removeClass('hidden');
                $control.trigger('repeater:empty');
            },

            source: function (rowsArray) {
                addRows(rowsArray);
            }
        };

        return api;
    };

}(jQuery));