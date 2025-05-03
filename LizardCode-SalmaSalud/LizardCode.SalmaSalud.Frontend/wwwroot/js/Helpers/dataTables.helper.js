
(function ($) {

    $.fn.DataTableEx = function (settings) {

        var isInit = false;
        var $selector = this;
        var dtTable;

        var defaultSettings = {
            procesing: true,
            searching: true,
            searchDelay: 500,
            paging: true,
            select: false,
            ordering: true,
            order: [[0, 'asc']],
            autoWidth: false,
            lengthMenu: [5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150],
            pageLength: 30,
            language: {
                url: RootPath + '/lib/datatables/DataTables-1.10.20/js/dataTables.es.json'
            },
            dom: "<'dt--top-section'<l><f>>" +
                 "<'table-responsive'tr>" +
                 "<'dt--bottom-section d-flex flex-row-reverse'<'dt--pages-count  mb-sm-0 mb-2'i><'dt--pagination'p>>",
        };

        if (settings === undefined) {
            dtTable = $selector.DataTable();

            if (dtTable === undefined)
                throw 'DataTable no inicializada.';
        }
        else if (typeof settings === 'object') {
            isInit = true;
            $.extend(defaultSettings, settings);
            init();
        }


        function init() {

            $.fn.dataTable.moment('DD/MM/YYYY');

            if (settings.ajaxUrl !== undefined) {
                defaultSettings.serverSide = true;
                defaultSettings.ajax = function (data, callback, dtSettings) {
                    $.post(settings.ajaxUrl, data, 'json')
                        .done(function (response) {

                            if (response == null || typeof response !== 'object' ||$.isEmptyObject(response) || response.constructor !== {}.constructor)
                                callback([]);

                            if (response.hasOwnProperty('status') && response.hasOwnProperty('detail')) {
                                if (response.status === enums.AjaxStatus.OK) {
                                    callback({
                                        draw: data.draw,
                                        recordsTotal: response.detail.length,
                                        recordsFiltered: response.detail.length,
                                        data: response.detail
                                    });
                                }
                                else {
                                    if ($.isFunction(settings.ajaxFail))
                                        settings.ajaxFail(response.detail);
                                    else
                                        callback([]);
                                }
                            }
                            else
                                callback([]);

                        })
                        .fail(function (xhr, ajaxOptions, thrownError) {
                            if (xhr.status == 401) {
                                location.href = RootPath + '/Login';
                            }
                        });
                }
            }
            else if (settings.data !== undefined)
                defaultSettings.data = settings.data;

            if (settings.procesing !== undefined)
                defaultSettings.procesing = settings.procesing;

            if (settings.searching !== undefined)
                defaultSettings.searching = settings.searching;

            if (settings.searchDelay !== undefined)
                defaultSettings.searchDelay = settings.searchDelay;

            if (settings.paging !== undefined)
                defaultSettings.paging = settings.paging;

            if (settings.select !== undefined)
                defaultSettings.select = settings.select;

            if (settings.ordering !== undefined)
                defaultSettings.ordering = settings.ordering;

            if (settings.order !== undefined)
                defaultSettings.order = settings.order;

            if (settings.autoWidth !== undefined)
                defaultSettings.autoWidth = settings.autoWidth;

            if (settings.lengthMenu !== undefined)
                defaultSettings.lengthMenu = settings.lengthMenu;

            if (settings.pageLength !== undefined)
                defaultSettings.pageLength = settings.pageLength;

            if (settings.language !== undefined)
                defaultSettings.language = settings.language;

            if (settings.dom !== undefined)
                defaultSettings.dom = settings.dom;

            if (settings.buttons !== undefined)
                defaultSettings.buttons = settings.buttons;


            if ($selector.hasClass('doNotRender')) {
                if ($.isFunction(defaultSettings.onInit))
                    defaultSettings.onInit($selector, defaultSettings);
            }
            else {
                dtTable = $selector
                    .on('draw.dt', function (e, settings) {
                        if ($.isFunction(defaultSettings.onDraw))
                            defaultSettings.onDraw(e, settings, api);
                        feather.replace();
                    })
                    .on('init.dt', function (e, settings, json) {
                        if ($.isFunction(defaultSettings.onInit))
                            defaultSettings.onInit(e, settings, json);
                    })
                    .on('error.dt', function (e, settings, techNote, message) {
                        if ($.isFunction(defaultSettings.onError))
                            defaultSettings.onError(e, settings, techNote, message);
                    })
                    .on('select.dt', function (e, dt, type, indexes) {
                        if (defaultSettings.select === false)
                            return;

                        if (type === 'row') {
                            var data = dt.rows({ selected: true }).data();
                            var arr = $.makeArray(data);
                            if ($.isFunction(defaultSettings.onSelected))
                                defaultSettings.onSelected(arr, api);
                        }
                    })
                    .on('user-select.dt', function (e, dt, type, cell, originalEvent) {
                        var selected = dt.row({ selected: true }).index();

                        if (selected === cell[0][0].row)
                            e.preventDefault();
                    })
                    .on('order.dt', function (e, settings) {
                        if ($.isFunction(defaultSettings.onOrdered))
                            defaultSettings.onOrdered(settings.order);
                    })
                    .on('dblclick.dt', function (e) {
                        if ($.isFunction(defaultSettings.onDoubleClick))
                            defaultSettings.onDoubleClick();
                    })
                    .DataTable(defaultSettings);
            }
        }


        var api = {
            api: function () {
                return dtTable;
            },

            data: function ($tr) {
                return dtTable.rows($tr).data()[0];
            },

            selected: function () {
                var data = dtTable.rows({ selected: true }).data();
                var dataArr = $.makeArray(data);

                return (dataArr === undefined ? null : dataArr);
            },

            reload: function (callback, resetPaging) {
                dtTable.ajax.reload(callback, resetPaging);
            },

            source: function (rowsArray, page) {

                if (page === undefined)
                    page = true;

                dtTable
                    .clear()
                    .rows.add(rowsArray)
                    .draw(page);

            },

            empty: function () {
                dtTable
                    .clear()
                    .draw();
            },

            clearSearch: function () {
                dtTable
                    .search('')
                    .draw();
            },

            column: function (index) {

                if (isNaN(parseInt(index)))
                    throw 'El índice de columan no es un número entero';

                return dtTable.column(index);
            },

            columnHeader: function (index, text) {

                if (isNaN(parseInt(index)))
                    throw 'El índice de columan no es un número entero';

                var $header = $(
                    dtTable
                        .column(index)
                        .header()
                );

                if (text === undefined || text === null || typeof (text) !== 'string')
                    return $header.text();
                else {
                    $header.text(text);
                    return text;
                }
            },

            columnsCount: function () {

                return dtTable.columns().indexes().length;

            },

            addRow: function (data) {
                dtTable
                    .row.add(data)
                    .draw();
            },

            expandRow: function (e, row, fnRender, icons) {

                var isShow = false;
                var $icon = $(e);
                var callback = function () {
                    return fnRender(row);
                };

                if (icons == undefined)
                    icons = { open: 'fa-plus', close: 'fa-minus' };

                dtTable.rows().eq(0).each(function (i) {
                    var row = dtTable.row(i);
                    if (row.child.isShown()) {
                        isShow = true;
                        return false;
                    }
                });

                if (row.child.isShown()) {
                    row.child.hide();
                    $icon.removeClass(icons.close);
                    $icon.addClass(icons.open);
                }
                else {
                    if (isShow)
                        return false;

                    row.child(callback).show();
                    $icon.removeClass(icons.open);
                    $icon.addClass(icons.close);
                }

            },

            removeRow: function ($tr) {
                dtTable
                    .row($tr)
                    .remove()
                    .draw()
            },

            table: function() {
                return $selector;
            },

            container: function () {
                return dtTable.table().container();
            },

            filter: function (columnIndex, search) {

                if (columnIndex === undefined && search === undefined) {

                    var n = dtTable.columns().indexes().length;

                    for (var i = 0; i < n; i++) {
                        dtTable.column(i).search('');
                    }

                    dtTable.draw();

                }

                dtTable
                    .columns(columnIndex)
                    .search(search)
                    .draw();

            }
        };


        return api;
    };

}(jQuery));

var DataTableEx = (function () {

    var utils = {
        date: function (input) {
            if (input === null || input === undefined || input == '')
                return '';

            return moment(input).format('DD/MM/YYYY');
        },

        dateTime: function (input) {
            if (input === null || input === undefined || input == '')
                return '';

            return moment(input).format('DD/MM/YYYY HH:mm');
        },

        dateTimeSecs: function (input) {
            if (input === null || input === undefined || input == '')
                return '';

            return moment(input).format('DD/MM/YYYY HH:mm:ss');
        },

        timeShort: function (input) {
            if (input === null || input === undefined || input == '')
                return '';

            return moment(input).format('HH:mm');
        },

        timeLong: function (input) {
            if (input === null || input === undefined || input == '')
                return '';

            return moment(input).format('HH:mm:ss');
        },

        escapeHtml: function (input) {
            return input
                .replace(/&/g, "&amp;")
                .replace(/</g, "&lt;")
                .replace(/>/g, "&gt;")
                .replace(/"/g, "&quot;")
                .replace(/'/g, "&#039;");
        }
    }

    var renders = {
        currency: function (data, type, row, meta) {
            if (type === 'sort')
                return parseFloat(data);
            else
                return accounting.formatMoney(data);
        },

        percentage: function (data, type, row, meta) {
            if (type === 'sort')
                return parseFloat(data);
            else
                return accounting.formatMoney(data, { symbol: '%', format: '%v%s' });
        },

        numeric: function (data, type, row, meta) {
            if (type === 'sort')
                return parseFloat(data);
            else
                return accounting.formatNumber(data);
        },

        dateStringNullable: function (data, type, row, meta) {
            if (data == '01/01/9999')
                return '';
            return data;
        },

        date: function (data, type, row, meta) {
            return utils.date(data);
        },

        dateTime: function (data, type, row, meta) {
            return utils.dateTime(data);
        },

        dateTimeSecs: function (data, type, row, meta) {
            return utils.dateTimeSecs(data);
        },

        timeShort: function (data, type, row, meta) {
            return utils.timeShort(data);
        },

        timeLong: function (data, type, row, meta) {
            return utils.timeLong(data);
        },

        ellipsis: function (data, type, row, meta) {
            if (data == null)
                return "";

            var html =
                '<div style="position: relative; margin-bottom: 20px;" title="[@value]">' +
                        '<span style="position: absolute; left: 0; right: 0; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">' +
                        '[@value]'
                    '</span>' +
                '</div>';

            return html.replaceAll('[@value]', data);
        },

        escapeHtml: function (data, type, row, meta) {
            return utils.escapeHtml(data);
        }
    }


    return {
        utils: utils,
        renders: renders
    };
})();

//# sourceUrl=dataTables.helper.js