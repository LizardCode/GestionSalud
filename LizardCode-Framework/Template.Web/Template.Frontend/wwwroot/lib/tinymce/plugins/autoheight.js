﻿tinymce.PluginManager.add('autoheight', function (editor) {

    var editor_container;
    var bars_height = 0;

    function resizeEditor(e) {
        if (typeof (editor_container) === 'undefined') return;

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

    editor.on('ResizeWindow', resizeEditor);
    editor.on("init", function () {
        try {
            editor_container = editor.getContainer().parentNode;
        } catch (e) { }
        setTimeout(function () {
            resizeEditor();
        }, 10);
    });
});
