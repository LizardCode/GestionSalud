/// <reference path="../../lib/tinymce/tinymce.min.js" />

var TinyMCEHelper = new (function () {

    this.build = function (selector, onChangeCallback) {

        var tinymceSettings = {
            selector: selector,
            language: 'es_MX',
            plugins: 'preview autolink visualchars fullscreen image link table charmap hr anchor insertdatetime advlist lists textcolor imagetools contextmenu colorpicker',
            menubar: false,
            toolbar1: 'newdocument | etiquetas | link image hr insertdatetime charmap table | numlist bullist outdent indent | visualchars | preview fullscreen',
            toolbar2: 'formatselect fontselect fontsizeselect | bold italic strikethrough forecolor backcolor | alignleft aligncenter alignright alignjustify | removeformat',
            image_advtab: true,
            resize: false,
            setup: function (editor) {

                //var menu = buildEditorMenu(editor);

                editor.on('change', function () {
                    tinymce.triggerSave();

                    if (onChangeCallback != undefined)
                        onChangeCallback(selector, $(this.targetElm));
                });

                //editor.addButton('etiquetas', {
                //    type: 'menubutton',
                //    text: 'Insertar Etiqueta',
                //    icon: false,
                //    menu: menu
                //});
            }
        };

        tinymce.init(tinymceSettings);

    }

    this.destroy = function (selector) {

        tinymce.get(selector).destroy();

    }

    this.fullHeight = function (id) {

        //
        // https://github.com/tinymce/tinymce/issues/3546#issuecomment-399127669
        // https://codepen.io/Misiu/pen/vZEGqm
        //

        var editor = tinyMCE.get(id);
        var editor_container = editor.getContainer().parentNode;
        var bars_height = 0;

        try {
            //var element_height = parseInt(window.getComputedStyle(editor_container).height);
            var element_height = editor_container.offsetHeight;
            /*calculate bar height only once*/
            if (bars_height === 0) {
                var toolbars = editor_container.querySelectorAll('.mce-toolbar, .mce-statusbar, .mce-menubar');
                /*IE11 FIX*/
                var toolbarsLength = toolbars.length;
                for (var i = 0; i < toolbarsLength; i++) {
                    var toolbar = toolbars[i];
                    /*skip sidebar*/
                    if (!toolbar.classList.contains('mce-sidebar-toolbar')) {
                        var bar_height = parseInt(window.getComputedStyle(toolbar).height);
                        bars_height += bar_height;
                    }
                }
            }
            /*the extra 8 is for margin added between the toolbars*/
            new_height = element_height - bars_height - 8;
            editor.theme.resizeTo('100%', new_height);
        } catch (err) {
            console.log(err);
        }
    }

    this.setContent = function (id, html) {

        var editor = tinyMCE.get(id);

        editor.setContent(html);
        editor.save();

    }

});