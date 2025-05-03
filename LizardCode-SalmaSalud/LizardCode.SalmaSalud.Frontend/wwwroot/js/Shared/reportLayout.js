/// <reference path="utils.js" />

var ReportLayout = new (function () {

    var self = this;
    var mainClass;
    var dtSelector;
    var btExcelSelector;
    var btPDFSelector;
    var btPrintSelector;
    var btToggleFiltersSelector;
    var btApplyFiltersSelector;
    var btClearFiltersSelector;
    var filtersSelector;
    var filtersEnabled;
    var filtersApplied;
    var datatablesInitComplete = false;

    var dtView = null;
    var dtColumnsFooter = null;
    var dtColumns = null;
    var dtKeyProperty = null;

    var dtDisplayFooter = false;

    this.errorTooltips = true;
    this.exportMessageTop = null;
    this.exportFilename = null;

    this.classColFooter = [];

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
                    if (dateStr != "")
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

    this.bindControlEvents = function () {

        $(btExcelSelector).on('click', function () {
            excelDatatables(this)
        });

        $(btPDFSelector).on('click', function () {
            pdfDatatables(this);
        });

        $(btPrintSelector).on('click', function () {
            printDatatables(this);
        });

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
        btExcelSelector = mainClass + ' .toolbar-actions button.btExcel';
        btPDFSelector = mainClass + ' .toolbar-actions button.btPDF';
        btPrintSelector = mainClass + ' .toolbar-actions button.btPrint';
        btApplyFiltersSelector = mainClass + ' .toolbar-extended button.btApplyFilters';
        btClearFiltersSelector = mainClass + ' .toolbar-extended button.btClearFilters';
        btToggleFiltersSelector = mainClass + ' .toolbar-extended button.btFilters';
        filtersSelector = mainClass + ' .toolbar-advanced-filters';

        self.buildFiltersControls();
        self.bindControlEvents();
        self.bindFiltersFieldsEvents();
    }


    this.initializeDatatable = function (keyProperty, columns, order, pageLength, multipleSelection, columnsFooterTotal, selectable) {

        dtKeyProperty = keyProperty;

        if (order === undefined || !$.isArray(order))
            order = [[0, 'asc']];

        if (pageLength === undefined || !isFinite(pageLength))
            pageLength = 10;

        var defaultDom =
            "<'dt--top-section'<l><f>>" +
            "<'table-responsive'tr>" +
            "<'dt--bottom-section d-sm-flex justify-content-sm-between text-center'<'dt--pages-count  mb-sm-0 mb-3'i><'dt--pagination'p>>";

        var selectionStyle = 'single';

        if (!selectable)
            selectionStyle = false;

        if (multipleSelection === true)
            selectionStyle = 'os';

        if (filtersEnabled)
            defaultDom = defaultDom.replace('f>', '>');

        if (columnsFooterTotal) {
            var foot = $("<tfoot>").appendTo(dtSelector);
            var rowFoot = "";
            for (var iLoop = 0; iLoop < columns.length; iLoop++)
                rowFoot += "<td></td>";

            foot.append(`<tr>${rowFoot}</tr>`);

            dtDisplayFooter = true;
            dtColumnsFooter = columnsFooterTotal;
        }

        var buttonsExport = [
            {
                extend: "excelHtml5",
                messageTop: function () { return self.exportMessageTop; },
                excelStyles: [
                    { template: 'blue_gray_medium' }
                ],
                footer: dtDisplayFooter,
                exportOptions: {
                    columns: ':visible',
                    format: {
                        body: function (data, row, column, node) {
                            data = data.replace(/<.*?>/ig, "");
                            if (!dtColumnsFooter)
                                return data;
                            return dtColumnsFooter.includes(column + 1) ?
                                `$${accounting.unformat(data, ",")}` :
                                data;
                        }
                    }
                }
            },
            {
                extend: "pdfHtml5",
                messageTop: function () { return self.exportMessageTop; },
                footer: dtDisplayFooter,
                orientation: 'landscape',
                exportOptions: {
                    columns: ':visible'
                },
                pageSize: 'A4'
            },
            {
                extend: "print",
                messageTop: function () { return self.exportMessageTop; },
                footer: dtDisplayFooter,
                exportOptions: {
                    columns: ':visible'
                }
            },
            'colvis'
        ];

        if (self.exportFilename) {
            buttonsExport[0].filename = function () { return self.exportFilename; };
            buttonsExport[1].filename = function () { return self.exportFilename; };
            buttonsExport[2].filename = function () { return self.exportFilename; };
        }

        dtView = $(dtSelector)
            .DataTableEx({
                data: [],
                buttons: buttonsExport,
                processing: true,
                serverSide: false,
                pageLength: pageLength,
                lengthChange: false,
                dom: defaultDom,

                select: selectionStyle,
                columns: columns,
                order: order,

                onInit: datatableInit,
                onSelected: datatableSelectedRow
            });

        dtColumns = columns;

        return dtView;
    }

    this.setClassColFooter = function (classColFooter) {
        this.classColFooter = classColFooter || [];
    }

    this.setExportMessageTop = function (title) {
        this.exportMessageTop = title.trim() == "" ? null : title.trim();
    }

    this.setExportFilename = function (filename) {
        this.exportFilename = filename.trim() == "" ? null : filename.trim();
    }

    function datatableInit() {
        if (!datatablesInitComplete) {
            var toolbarExtended = $('.toolbar-extended');
            var searchInput = $('.dataTables_filter > label');

            toolbarExtended.append(searchInput);

            getDatatablesRows();
        }
    }

    function datatableSelectedRow(dataArray, api) {
        self.mainTableRowSelected(dataArray, api);
    }

    function doTotalFooter() {

        if (!dtColumnsFooter)
            return false;

        var totals = [];
        for (let iLoop = 0; iLoop < dtColumns.length; iLoop++)
            totals.push("");

        for (let iLoop = 0; iLoop < dtColumnsFooter.length; iLoop++) {
            totals[dtColumnsFooter[iLoop]] = 0;
        }

        totals = $(dtSelector).DataTable().rows().data()
            .reduce(function (sum, record) {
                for (let iLoop = 0; iLoop < dtColumnsFooter.length; iLoop++) {
                    let iCol = dtColumnsFooter[iLoop];
                    sum[iCol] = sum[iCol] + numberFromString(record[dtColumns[iCol].data]);
                }
                return sum;
            }, totals);

        for (let iLoop = 0; iLoop < dtColumnsFooter.length; iLoop++) {
            let iCol = dtColumnsFooter[iLoop];
            var column = $(dtSelector).DataTable().column(iCol);
            $(column.footer()).html(formatNumber(totals[iCol], iLoop));
        }

        if (dtColumnsFooter[0] - 1 >= 0) {
            var totalLabel = $(dtSelector).DataTable().column(dtColumnsFooter[0] - 1);
            $(totalLabel.footer()).html(`<span class="badge badge-dark">TOTAL:</span>`);
            $(totalLabel.footer()).addClass("dt-body-right");
        }

    }

    function numberFromString(s) {
        return typeof s === 'string' ?
            s.replace(/[\$,]/g, '') * 1 :
            typeof s === 'number' ?
                s : 0;
    }

    function formatNumber(n, c) {
        return `<span class="${self.classColFooter[c]}">${DataTableEx.renders.currency(n)}</span>`;
    }

    function excelDatatables(button) {

        $(dtView.api().buttons()[0].node).trigger('click')
    }

    function pdfDatatables(button) {

        $(dtView.api().buttons()[1].node).trigger('click')
    }

    function printDatatables(button) {

        $(dtView.api().buttons()[2].node).trigger('click')
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

        getDatatablesRows();

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

        getDatatablesRows();

        $(btApplyFiltersSelector).hide();
        $(btClearFiltersSelector).hide();
    }

    function getDatatablesRows() {

        var ajaxGetAll = $(dtSelector).data('ajax-action');
        if (!ajaxGetAll)
            return;

        Ajax.Execute(ajaxGetAll, buildFiltersData())
            .done(function (response) {
                dtView.source(response);
                datatablesInitComplete = true;

                doTotalFooter();
            })
            .fail(Ajax.ShowError)
            .always(function (e) {
                feather.replace();
            });

    }

});
