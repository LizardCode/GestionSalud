var Ajax = new (function () {

    var self = this;


    this.Execute = function (action, params, dataType, method) {

        if (action === undefined || action == null || action === '')
            throw 'Ajax.Execute: El parámetro "Action" no puede ser nulo o vacío.';

        if (params === undefined || params == null || params === '')
            params = {};

        if (dataType === undefined || dataType == null || dataType === '')
            dataType = 'json';

        var finalUrl = RootPath + '/' + (action[0] == '/' ? action.substr(1) : action);
        var finalParams = params;//JSON.stringify(params);

        method = method || 'POST';

        var promise = $.ajax({
            type: method,
            url: finalUrl,
            data: finalParams,
            dataType: dataType
        });

        promise
            .done(function () {
                console.log('Ajax.Execute: ' + action + ' (Ok)')
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.error('Ajax.Execute: ' + action + ' (Error)', [jqXHR.status, textStatus, errorThrown, jqXHR.responseText, jqXHR.responseJSON]);
                Ajax.ShowError(jqXHR, textStatus, errorThrown);
            });

        return promise;
    }

    this.ToDiv = function (action, params, selector) {
        return this
            .Execute(action, params, 'html')
            .done(function (response) {
                if (selector instanceof jQuery)
                    selector.empty().html(response);
                else
                    $(selector).empty().html(response);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.error('Ajax.Execute: ' + action + ' (Error)', [jqXHR.status, textStatus, errorThrown, jqXHR.responseText, jqXHR.responseJSON]);
                Ajax.ShowError(jqXHR, textStatus, errorThrown);
            });
    }

    this.ToSelect = function (settings) {

        var defaultSettings = {
            action: null,
            params: null,
            selector: null,
            defaultText: null,
            valueProperty: 'Value',
            textProperty: 'Text',
            includeValueInText: false
        };

        $.extend(defaultSettings, settings);

        if (defaultSettings.action === undefined || defaultSettings.action === null || defaultSettings.action === '' ||
            defaultSettings.selector === undefined || defaultSettings.selector === null || defaultSettings.selector === '') {
            throw 'Missing parameters.';
            return;
        }

        return this.Execute(defaultSettings.action, defaultSettings.params)
            .done(function (response) {
                var $select;

                if (defaultSettings.selector instanceof jQuery)
                    $select = defaultSettings.selector;
                else
                    $select = $(defaultSettings.selector);

                if ($select.length != 1) {
                    throw 'There is not target element.';
                    return;
                }

                $select.empty();

                if (defaultSettings.defaultText !== undefined && defaultSettings.defaultText != null)
                    $select.append("<option value=''>" + defaultSettings.defaultText + "</option>");

                Ajax.ParseResponse(
                    response,

                    function (results) {
                        $.each(results, function (i, item) {
                            var text = item[defaultSettings.textProperty];

                            if (defaultSettings.includeValueInText)
                                text = item[defaultSettings.valueProperty] + '.' + text;

                            $('<option />')
                                .val(item[defaultSettings.valueProperty])
                                .text(text)
                                .appendTo($select);
                        });

                        $select.prop('disabled', false);
                    },

                    function (errorMessage) {
                        console.error(errorMessage);
                    }
                );
            });
    }

    this.GetJson = function (action, params) {

        if (action === undefined || action == null || action === '')
            throw 'Ajax.GetJson: El parámetro "Action" no puede ser nulo o vacío.';

        var finalUrl = action;
        if (action.lastIndexOf(RootPath) == -1)
            finalUrl = RootPath + '/' + (action[0] == '/' ? action.substr(1) : action);

        var promise = $.getJSON(finalUrl, params)
            .then(function (response) {

                var parsedResponse = undefined;

                self.ParseResponse(response,
                    function (detail) {
                        parsedResponse = detail;
                    },
                    function (fail) {
                        console.error('Ajax.GetJson: ' + action + ' (Error)' + '(' + fail + ')');
                        Utils.modalError('Error', 'Se produjo un error al intentar consultar los datos.');
                    }
                );

                return parsedResponse;

            });

        promise
            .done(function () {
                console.log('Ajax.GetJson: ' + action + ' (Ok)')
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.error('Ajax.GetJson: ' + action + ' (Error)', [jqXHR.status, textStatus, errorThrown, jqXHR.responseText, jqXHR.responseJSON]);
                Utils.modalError('Error', 'Se produjo un error al intentar consultar los datos.');
            });

        return promise;
    }

    this.ShowError = function (jqXHR, textStatus, errorThrown) {

        console.error('Status: ' + textStatus + ' - Error: ' + errorThrown);
        console.error(jqXHR.responseText);

        try {
            if (jqXHR.status == 500) {
                throw 'error';
            }
            else if (jqXHR.status == 401) {
                BaseLayoutView.expiredSession();
            }
            else {
                if (jqXHR.responseJSON)
                    Utils.modalError("Error", jqXHR.responseJSON.detail);
                else
                    Utils.modalError("Error", textStatus);
            }
        }
        catch (ex) {
            Utils.modalError("Error", "No fue posible realizar correctamente la acción solicitada, por favor intente en unos minutos o contacte al administrador del sistema.");
        }
    }

    this.ParseResponse = function (response, doneCallback, failCallback) {

        if (response == null || typeof response !== 'object' || $.isEmptyObject(response) || response.constructor !== {}.constructor)
            if ($.isFunction(doneCallback))
                doneCallback(null);

        if (response.hasOwnProperty('status') && response.hasOwnProperty('detail')) {
            if (response.status === enums.AjaxStatus.OK) {
                if ($.isFunction(doneCallback))
                    doneCallback(response.detail);
            }
            else {
                if ($.isFunction(failCallback))
                    failCallback(response.detail);
            }
        }
        else
            failCallback('JSON format incorrect. Missing \'status\' and \'detail\' properties.');

    }

    this.Download = function (action, data, method) {

        if (action === undefined || action === null || action === '')
            throw 'Ajax.Download: No se especificó URL';

        var isValidData = (data instanceof Array) || $.isPlainObject(data);

        if (!isValidData)
            throw 'Ajax.Download: El tipo de dato del parámetro "data" es inválido. (Array) o (JSON)';

        if (method === undefined || method === null || method === '')
            method = 'GET';

        if (method.toLowerCase() !== 'get' && method.toLowerCase() !== 'post')
            method = 'GET';

        var isPost = (method.toLowerCase() === 'post');
        var url = RootPath + '/' + (action[0] == '/' ? action.substr(1) : action);

        if (!isPost) {
            var q = (action.indexOf('?') ? '&' : '?');

            if ($.isPlainObject(data))
                url += q + $.param(data);
        }

        var xhr = new XMLHttpRequest();
        xhr.open(method, url);
        xhr.responseType = "arraybuffer"; // <- Super importante!

        xhr.onload = function (e) {
            if (this.status == 200) {

                var fileName = 'TradeApp.xlsx';
                var contentType = this.getResponseHeader('content-type');
                var disposition = /filename=(.*)/.exec(this.getResponseHeader('content-disposition'));

                if ($.isArray(disposition))
                    fileName = disposition[1];

                var blob = this.response;

                if (blob.size == 0)
                    throw 'Ajax.Download: Body size = 0';

                //TODO fallback needed for IE8 & IE9
                if (navigator.appVersion.toString().indexOf('.NET') > 0) {
                    //IE 10+
                    window.navigator.msSaveBlob(blob, fileName);
                } else {
                    //Firefox, Chrome
                    var a = document.createElement("a");
                    var blobUrl = window.URL.createObjectURL(new Blob([blob], { type: (blob.type === undefined ? contentType : blob.type) }));
                    document.body.appendChild(a);
                    a.style = "display: none";
                    a.href = blobUrl;
                    a.download = fileName;
                    a.click();
                }
            }
            else
                throw 'Ajax.Download: Error ' + this.status;
        };

        if (!isPost)
            xhr.send();
        else {
            if (isValidData) {
                xhr.setRequestHeader('Content-Type', 'application/json');
                xhr.send(JSON.stringify(data));
            }
            else
                xhr.send();
        }
    }

    this.ToFormData = function (json, formField, ignoreList) {
        var formData = new FormData();

        function appendFormData(data, root) {
            if (!ignore(root)) {
                root = root || '';
                if (data instanceof File) {
                    formData.append(root, data);
                } else if (Array.isArray(data)) {
                    for (var i = 0; i < data.length; i++) {
                        appendFormData(data[i], root + '[' + i + ']');
                    }
                } else if (typeof data === 'object' && data) {
                    for (var key in data) {
                        if (data.hasOwnProperty(key)) {
                            if (root === '') {
                                appendFormData(data[key], key);
                            } else {
                                appendFormData(data[key], root + '.' + key);
                            }
                        }
                    }
                } else {
                    if (data !== null && typeof data !== 'undefined') {
                        formData.append(root, data);
                    }
                }
            }
        }

        function ignore(root) {
            return Array.isArray(ignoreList)
                && ignoreList.some(function (x) { return x === root; });
        }

        appendFormData(json, formField);

        return formData;
    }
});

