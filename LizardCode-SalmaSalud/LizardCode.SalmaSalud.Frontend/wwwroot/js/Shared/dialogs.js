
var Dialogs = new function () {

    var self = this;
    let avoidCloseCallback = false;

    this.AvoidCloseCallback = function (value) {
        if (value) {
            self.avoidCloseCallback = value;
        } else {
            return self.avoidCloseCallback;
        }
    };

    /*
    * Diálogos Popup
    * @param {element} $dialogSelector.
    * @param {element} $bodySelector.
    * @param {number} width del Popup.
    * @param {string} action para la carga del Popup.
    * @param {function} callback función para cargar lo necesario del Popup.
    * */
    this.loadDialog = function ($dialogSelector, $bodySelector, widthClass, action, callback, closeCallBack) {

        if (typeof $dialogSelector === 'string')
            $dialogSelector = $($dialogSelector);

        if ($.isPlainObject($dialogSelector))
            $dialogSelector = $($dialogSelector);

        if (typeof $bodySelector === 'string')
            $bodySelector = $($bodySelector);

        if ($.isPlainObject($bodySelector))
            $bodySelector = $($bodySelector);

        if ($dialogSelector instanceof jQuery === false) {
            alert('Selector de diálogo no vákido');
            return false;
        }

        if ($bodySelector instanceof jQuery === false) {
            alert('SelectorBody de diálogo no vákido');
            return false;
        }

        //if (!$.isNumeric(width)) {
        //    alert('width dene tener un valor numérico');
        //    return false;
        //}

        if (action === undefined || action == '') {
            alert('Action no válida');
            return false;
        }

        Utils.modalLoader();

        $bodySelector.load(action, function (response, status, xhr) {

            if (xhr.status == 401) {
                Utils.modalClose();
                Utils.doLoginRedirect();
                return false;
            }

            if (status == 'error') {
                Utils.modalClose();
                Utils.modalError('Se ha producido un error', xhr.status + ' ' + xhr.statusText);
            }
            else {
                //JSON RESPONSE PATCH
                var jsonResponse = {};
                let isJSONResponse = false;
                try {
                    jsonResponse = JSON.parse(response);
                    if (typeof jsonResponse === 'object')
                        isJSONResponse = true;
                }
                catch (e) {
                    isJSONResponse = false;
                }
                if (isJSONResponse) {
                    if (!jsonResponse.success) {
                        Utils.modalClose();
                        Utils.modalError('', jsonResponse.message);
                    }
                }
                //JSON RESPONSE PATCH

                var wClass = widthClass || 'modal-50';
                $dialogSelector.find('.modal-dialog').addClass(wClass);

                if (!isJSONResponse) {
                    $dialogSelector.modal({
                        modalOverflow: true,
                        show: true,
                        backdrop: 'static',
                        keyboard: false
                    }).unbind('shown.bs.modal').on('shown.bs.modal', function (evt) {
                        //Eventos / Config
                        if ($.isFunction(callback))
                            callback.apply(this);

                        Utils.modalClose(); //Cerrar Loader
                    }).unbind('hidden.bs.modal').on('hidden.bs.modal', function (evt) {
                        if ($.isFunction(closeCallBack) && !Dialogs.avoidCloseCallback)
                            closeCallBack.apply(this);

                        $(this).data('modal', null);
                        $(this).modal('dispose');  //Limpiar Diálogo

                        $bodySelector.empty(); //Limpiar Contenido
                    });
                }
            }
        });
    };
};