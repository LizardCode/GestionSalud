/// <reference path="../../shared/maestrolayout.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />

var ABMPlantillasView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');
    //var viewer;
    //var viewerPage;
    
    var btEdit;
    //var Base64Prefix = "data:application/pdf;base64,";
    //var fabricCanvas;

    this.init = function () {

        modalNew = $('.modal.modalNew', mainClass);
        modalEdit = $('.modal.modalEdit', mainClass);
        btEdit = $('.toolbar-actions button.btEdit', mainClass);

        MaestroLayout.init();

        buildControls();
        bindControlEvents();

    }


    function buildControls() {

        $('select.select2-field', mainClass).Select2Ex();

        MaestroLayout.errorTooltips = false;

        var columns = [
            { data: 'idPlantilla' },
            { data: 'tipoPlantilla' }, 
            { data: 'descripcion' }
        ];

        var order = [[0, 'asc']];

        dtView = MaestroLayout.initializeDatatable('idPlantilla', columns, order);

        $('[data-toggle="tooltip"]').tooltip();

    }

    function bindControlEvents() {

        MaestroLayout.editDialogOpening = editDialogOpening;
        MaestroLayout.newDialogOpening = newDialogOpening;
        //MaestroLayout.tabChanged = tabChanged;

        MaestroLayout.mainTableRowDoubleClicked = mainTableRowDoubleClicked;

        //$('.modal select[name="IdTipoPlantilla"]', mainClass)
        //    .on('change', function () {

        //        var dialog = $(this).closest('.modal[role=dialog]');
        //        var list = $('.tab-01 div.frame-etiquetas > ul', dialog);
        //        var template = list.find('> li.template');

        //        var params = {
        //            idTipoPlantilla: $(this).select2('val')
        //        };

        //        Ajax.GetJson('/Plantillas/GetPlantillaEtiquetas/', params)
        //            .done(function (items) {

        //                list.find('> li:not(.template)').remove();

        //                for (var key in items) {

        //                    var newItem = template
        //                        .clone()
        //                        .removeClass('template')
        //                        .attr('data-example', items[key].ejemplo)
        //                        .attr('data-value', items[key].idPlantillaEtiqueta);

        //                    newItem.find('> i').attr('title', items[key].descripcion);
        //                    newItem.find('> span').text(items[key].etiqueta);

        //                    list.append(newItem);
        //                }

        //                $('[data-toggle="tooltip"]').tooltip();
        //            })
        //            .fail(Ajax.ShowError);

        //    });

        //$('.tab-01 div.frame-etiquetas', mainClass).on('dblclick', 'ul > li', function (e) {

        //    if (fabricCanvas == undefined)
        //        return;

        //    var label = $(this).data('example').trim();
        //    var idTag = $(this).data('value');

        //    var textbox = new fabric.Textbox(label, {
        //        id: idTag,
        //        left: 50,
        //        top: 50,
        //        fontSize: 15,
        //        fontFamily: 'Quicksand',
        //        strokeWidth: 5,
        //        linethrough: false,
        //        lockRotation: true,
        //        lockScalingX: true,
        //        lockScalingY: true,
        //        lockSkewingX: true,
        //        lockSkewingY: true,
        //    });

        //    fabricCanvas.add(textbox)
        //        .setActiveObject(textbox);
        //});

        //$('.tab-01 button.btTrash', mainClass).on('click', function (e) {
        //    fabricCanvas.remove(fabricCanvas.getActiveObject());
        //});

        //$('.tab-01 button.btZoomIn', mainClass).on('click', function (e) {
        //    var zoom = fabricCanvas.getZoom();
        //    zoom *= 1.10;
        //    fabricCanvas.setZoom(zoom);

        //    e.preventDefault();
        //    e.stopPropagation();
        //});

        //$('.tab-01 button.btZoomOut', mainClass).on('click', function (e) {
        //    var zoom = fabricCanvas.getZoom();
        //    zoom /= 1.10;
        //    fabricCanvas.setZoom(zoom);

        //    e.preventDefault();
        //    e.stopPropagation();
        //});

        //$('.tab-01 button.btZoomReset', mainClass).on('click', function (e) {
        //    fabricCanvas.setZoom(1);
        //});

        //$('.btnCargaDocumento', mainClass).on('click', function (e) {
        //    $('#FilePDF').trigger('click');
        //});

        //$('#FilePDF').on('change', async function (e) {

        //    var viewer = { viewerWidth: viewerPage.width() - 20, viewerHeight: viewerPage.height() };

        //    fabricCanvas.setDimensions({ width: viewer.viewerWidth, height: viewer.viewerHeight });
        //    await pdfToImage(e.target.files[0], fabricCanvas);

        //});
    }

    //function updateFabricJSON(e) {
    //    $('#FabricObjects', mainClass).val(JSON.stringify(fabricCanvas.toJSON(['id'])));
    //}

    //async function pdfToImage(pdfData, canvas) {
    //    const scale = 1 / window.devicePixelRatio;
    //    return (await printPDF(pdfData))
    //        .map(async c => {
    //            var img = new fabric.Image(await c, {
    //                scaleX: scale,
    //                scaleY: scale,
    //            });

    //            canvas.setBackgroundImage(img, canvas.renderAll.bind(canvas), {
    //                scaleX: 1,
    //                scaleY: 1
    //            });
    //        });
    //}

    function mainTableRowDoubleClicked() {

        if (!btEdit.prop('disabled'))
            btEdit.click();

    }

    //function tabChanged(dialog, $form, index, name) {

    //    if (index == 1) {
    //        dialog.addClass('modal-80');
    //    }
    //    else {
    //        dialog.removeClass('modal-80');
    //    }

    //}

    function newDialogOpening(dialog, $form) {

        //viewer = $('.pdf-viewer', modalNew);
        //viewerPage = $('.pdf-viewer-page', viewer);

        //fabricCanvas = new fabric.Canvas('pdfCanvas');

        //fabricCanvas.on('mouse:wheel', function (opt) {
        //    var delta = opt.e.deltaY;
        //    var zoom = fabricCanvas.getZoom();

        //    zoom *= 0.999 ** delta;
        //    if (zoom > 20)
        //        zoom = 20;
        //    if (zoom < 0.01)
        //        zoom = 0.01;

        //    fabricCanvas.setZoom(zoom);

        //    opt.e.preventDefault();
        //    opt.e.stopPropagation();
        //});

        //fabricCanvas.on('object:added', updateFabricJSON);
        //fabricCanvas.on('object:moving', updateFabricJSON);
        //fabricCanvas.on('object:modified', updateFabricJSON);
        //fabricCanvas.on('object:removed', updateFabricJSON);

    }

    function editDialogOpening($form, entity) {

        //viewer = $('.pdf-viewer', modalEdit);
        //viewerPage = $('.pdf-viewer-page', viewer);

        //fabricCanvas = new fabric.Canvas('pdfCanvas_Edit');

        //fabricCanvas.off('object:added');
        //fabricCanvas.off('object:moving');
        //fabricCanvas.off('object:modified');
        //fabricCanvas.off('object:removed');

        $form.find('#IdPlantilla_Edit').val(entity.idPlantilla);
        $form.find('#IdTipoPlantilla_Edit').select2('val', entity.idTipoPlantilla, true);
        $form.find('#Nombre_Edit').val(entity.nombre);
        $form.find('#Descripcion_Edit').val(entity.descripcion);
        $form.find('#Texto_Edit').val(entity.texto);
        $form.find('#FabricObjects_Edit').val(entity.fabricObjects);

        //fabricCanvas.loadFromJSON(entity.fabricObjects, function (e) {

        //    fabricCanvas.setDimensions({ width: fabricCanvas.backgroundImage.width, height: fabricCanvas.backgroundImage.height });
        //    fabricCanvas.renderAll();

        //    fabricCanvas.on('object:added', updateFabricJSON);
        //    fabricCanvas.on('object:moving', updateFabricJSON);
        //    fabricCanvas.on('object:modified', updateFabricJSON);
        //    fabricCanvas.on('object:removed', updateFabricJSON);

        //});

        //fabricCanvas.on('mouse:wheel', function (opt) {
        //    var delta = opt.e.deltaY;
        //    var zoom = fabricCanvas.getZoom();

        //    zoom *= 0.999 ** delta;
        //    if (zoom > 20)
        //        zoom = 20;
        //    if (zoom < 0.01)
        //        zoom = 0.01;

        //    fabricCanvas.setZoom(zoom);

        //    opt.e.preventDefault();
        //    opt.e.stopPropagation();
        //});
    }

    //function getPdfHandler() {
    //    return window['pdfjs-dist/build/pdf'];
    //}

    //function readBlob(blob) {
    //    return new Promise((resolve, reject) => {
    //        const reader = new FileReader();
    //        reader.addEventListener('load', () => resolve(reader.result));
    //        reader.addEventListener('error', reject)
    //        reader.readAsDataURL(blob);
    //    })
    //}

    //async function printPDF(pdfData) {

    //    const pdfjsLib = await getPdfHandler();
    //    pdfData = pdfData instanceof Blob ? await readBlob(pdfData) : pdfData;
    //    const data = atob(pdfData.startsWith(Base64Prefix) ? pdfData.substring(Base64Prefix.length) : pdfData);
        
    //    const loadingTask = pdfjsLib.getDocument({ data });
    //    return loadingTask.promise
    //        .then((pdf) => {
    //            const numPages = pdf.numPages;
    //            return new Array(numPages).fill(0)
    //                .map((__, i) => {
    //                    const pageNumber = i + 1;

    //                    return pdf.getPage(pageNumber)
    //                        .then((page) => {

    //                            var viewport = page.getViewport({ scale: 1 });
    //                            const scale = fabricCanvas.width / viewport.width;

    //                            viewport = page.getViewport({ scale: scale });

    //                            const canvas = document.createElement('canvas');
    //                            const context = canvas.getContext('2d');

    //                            canvas.height = viewport.height
    //                            canvas.width = viewport.width;

    //                            fabricCanvas.setDimensions({ width: viewport.width, height: viewport.height });

    //                            const renderContext = {
    //                                canvasContext: context,
    //                                viewport: viewport
    //                            };
    //                            const renderTask = page.render(renderContext);
    //                            return renderTask.promise.then(() => canvas);
    //                        });
    //                });
    //        });
    //}
});

$(function () {
    ABMPlantillasView.init();
});
