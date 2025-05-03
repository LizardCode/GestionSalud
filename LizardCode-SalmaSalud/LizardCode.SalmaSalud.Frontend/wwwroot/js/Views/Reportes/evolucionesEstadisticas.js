/// <reference path="../../shared/reportLayout.js" />
/// <reference path="../../shared/enums.js" />
/// <reference path="../../helpers/datatables.helper.js" />
/// <reference path="../../helpers/ajax-action.js" />
/// <reference path="../../helpers/formdata.helper.js" />
/// <reference path="../../helpers/dropdown-group-list.helper.js" />
/// <reference path="../../helpers/constraints.helper.js" />

var chartE;
var chartF;
var chartES;

var EvolucionesEstadisticasView = new (function () {

    var self = this;
    var mainClass = '.' + $('div[data-mainclass]').data('mainclass');

    this.init = function () {

        ReportLayout.init();

        $(".btApplyFilters").click(function () {

            renderCharts();
        });

        renderCharts();
    }
});

function makeChart(data) {

    $("#chartEvoluciones").html('');

    if (!data) {
        $("#chartEvoluciones").html('<span class="pl-5">No se encontraron datos</span>');
        return;
    }

    var options = {
        chart: {
            type: 'donut',
            width: 380
        },
        colors: ['#5c1ac3', '#e2a03f', '#e7515a', '#FFCCBC'],
        dataLabels: {
            enabled: false
        },
        legend: {
            position: 'bottom',
            horizontalAlign: 'center',
            fontSize: '14px',
            markers: {
                width: 10,
                height: 10,
            },
            itemMargin: {
                horizontal: 0,
                vertical: 8
            }
        },
        plotOptions: {
            pie: {
                donut: {
                    size: '65%',
                    background: 'transparent',
                    labels: {
                        show: true,
                        name: {
                            show: true,
                            fontSize: '29px',
                            fontFamily: 'Quicksand, sans-serif',
                            color: undefined,
                            offsetY: -10
                        },
                        value: {
                            show: true,
                            fontSize: '26px',
                            fontFamily: 'Quicksand, sans-serif',
                            color: '20',
                            offsetY: 16,
                            formatter: function (val) {
                                return val
                            }
                        },
                        total: {
                            show: true,
                            showAlways: true,
                            label: 'Total',
                            color: '#888ea8',
                            formatter: function (w) {
                                return w.globals.seriesTotals.reduce(function (a, b) {
                                    return a + b
                                }, 0)
                            }
                        }
                    }
                }
            }
        },
        stroke: {
            show: true,
            width: 25,
        },
        series: [data.turnos, data.demandaEspontanea, data.guardia, data.sobreTurnos],
        labels: ['Turnos', 'D. Esp.', 'Guardia', 'S. Turno'],
        responsive: [{
            breakpoint: 1599,
            options: {
                chart: {
                    width: '350px',
                    height: '400px'
                },
                legend: {
                    position: 'bottom'
                }
            },

            breakpoint: 1439,
            options: {
                chart: {
                    width: '250px',
                    height: '390px'
                },
                legend: {
                    position: 'bottom'
                },
                plotOptions: {
                    pie: {
                        donut: {
                            size: '65%',
                        }
                    }
                }
            },
        }]
    };

    chartE = new ApexCharts(
        document.querySelector("#chartEvoluciones"),
        options
    );

    chartE.render();

}

function makeChartFinanciadores(data) {

    $("#chartFinanciadores").html('');

    var dataSeries = [];
    var dataLabels = []

    if (!data || !data.length) {
        dataLabels.push('FINANCIADOR');
        dataSeries.push(0);
    }
    else
    { 
        for (let i = 0; i < data.length; i++) {
            dataLabels.push(data[i].financiador);
            dataSeries.push(data[i].cantidad);
        }
    }

    var options = {
        chart: {
            type: 'donut',
            width: 380
        },
        //colors: ['#5c1ac3', '#e2a03f', '#e7515a', '#e2a03f'],
        dataLabels: {
            enabled: false
        },
        legend: {
            position: 'bottom',
            horizontalAlign: 'center',
            fontSize: '14px',
            markers: {
                width: 10,
                height: 10,
            },
            itemMargin: {
                horizontal: 0,
                vertical: 8
            }
        },
        plotOptions: {
            pie: {
                donut: {
                    size: '65%',
                    background: 'transparent',
                    labels: {
                        show: true,
                        name: {
                            show: true,
                            fontSize: '29px',
                            fontFamily: 'Quicksand, sans-serif',
                            color: undefined,
                            offsetY: -10
                        },
                        value: {
                            show: true,
                            fontSize: '26px',
                            fontFamily: 'Quicksand, sans-serif',
                            color: '20',
                            offsetY: 16,
                            formatter: function (val) {
                                return val
                            }
                        },
                        total: {
                            show: true,
                            showAlways: true,
                            label: 'Total',
                            color: '#888ea8',
                            formatter: function (w) {
                                return w.globals.seriesTotals.reduce(function (a, b) {
                                    return a + b
                                }, 0)
                            }
                        }
                    }
                }
            }
        },
        stroke: {
            show: true,
            width: 25,
        },
        series: dataSeries,
        labels: dataLabels,
        responsive: [{
            breakpoint: 1599,
            options: {
                chart: {
                    width: '350px',
                    height: '400px'
                },
                legend: {
                    position: 'bottom'
                }
            },

            breakpoint: 1439,
            options: {
                chart: {
                    width: '250px',
                    height: '390px'
                },
                legend: {
                    position: 'bottom'
                },
                plotOptions: {
                    pie: {
                        donut: {
                            size: '65%',
                        }
                    }
                }
            },
        }]
    };

    chartF = new ApexCharts(
        document.querySelector("#chartFinanciadores"),
        options
    );

    chartF.render();

}

function makeChartEspecialidades(data) {

    $("#chartEspecialidades").html('');

    var dataSeries = [];
    var dataLabels = []

    if (!data || !data.length) {
        dataLabels.push('ESPECIALIDAD');
        dataSeries.push(0);
    }
    else
    { 
        for (let i = 0; i < data.length; i++) {
            dataLabels.push(data[i].especialidad);
            dataSeries.push(data[i].cantidad);
        }
    }

    var options = {
        chart: {
            type: 'donut',
            width: 380
        },
        //colors: ['#5c1ac3', '#e2a03f', '#e7515a', '#e2a03f'],
        dataLabels: {
            enabled: false
        },
        legend: {
            position: 'bottom',
            horizontalAlign: 'center',
            fontSize: '14px',
            markers: {
                width: 10,
                height: 10,
            },
            itemMargin: {
                horizontal: 0,
                vertical: 8
            }
        },
        plotOptions: {
            pie: {
                donut: {
                    size: '65%',
                    background: 'transparent',
                    labels: {
                        show: true,
                        name: {
                            show: true,
                            fontSize: '29px',
                            fontFamily: 'Quicksand, sans-serif',
                            color: undefined,
                            offsetY: -10
                        },
                        value: {
                            show: true,
                            fontSize: '26px',
                            fontFamily: 'Quicksand, sans-serif',
                            color: '20',
                            offsetY: 16,
                            formatter: function (val) {
                                return val
                            }
                        },
                        total: {
                            show: true,
                            showAlways: true,
                            label: 'Total',
                            color: '#888ea8',
                            formatter: function (w) {
                                return w.globals.seriesTotals.reduce(function (a, b) {
                                    return a + b
                                }, 0)
                            }
                        }
                    }
                }
            }
        },
        stroke: {
            show: true,
            width: 25,
        },
        series: dataSeries,
        labels: dataLabels,
        responsive: [{
            breakpoint: 1599,
            options: {
                chart: {
                    width: '350px',
                    height: '400px'
                },
                legend: {
                    position: 'bottom'
                }
            },

            breakpoint: 1439,
            options: {
                chart: {
                    width: '250px',
                    height: '390px'
                },
                legend: {
                    position: 'bottom'
                },
                plotOptions: {
                    pie: {
                        donut: {
                            size: '65%',
                        }
                    }
                }
            },
        }]
    };

    chartES = new ApexCharts(
        document.querySelector("#chartEspecialidades"),
        options
    );

    chartES.render();

}

function renderCharts() {
    
    Utils.modalLoader();
    var params = getParameters();

    var desde = $('#Filter_FechaDesde').val();
    var hasta = $('#Filter_FechaHasta').val();
    //var especialidad = 'TODAS';
    //if (params.IdEspecialidad > 0)
    //    especialidad = $('#Filter_IdEspecialidad').select2('data').text;

    var profesional = 'TODOS';
    if (params.IdProfesional > 0)
        profesional = $('#Filter_IdProfesional').select2('data').text;

    $('.bInfoBusqueda').text('Información para el período ' + desde + ' - ' + hasta + '. Profesional: ' + profesional + '.');

    $.getJSON('/Reportes/ObtenerEvolucionesEstadisticas', params)
        .done(function (data) {

            console.log(data);
            makeChart(data);

            Utils.modalClose();
        })
        .fail(function (data) {
            Utils.ShowError('Error generando gráfico.');
        });

    $.getJSON('/Reportes/ObtenerEvolucionesEstadisticasFinanciador', params)
        .done(function (data) {

            console.log(data);
            makeChartFinanciadores(data);

            Utils.modalClose();
        })
        .fail(function (data) {
            Utils.ShowError('Error generando gráfico F.');
        });

    $.getJSON('/Reportes/ObtenerEvolucionesEstadisticasEspecialidades', params)
        .done(function (data) {

            console.log(data);
            makeChartEspecialidades(data);

            Utils.modalClose();
        })
        .fail(function (data) {
            Utils.ShowError('Error generando gráfico E.');
        });
}

function getParameters() {

    return {
        FechaDesde: $('#Filter_FechaDesde').val(),
        FechaHasta: $('#Filter_FechaHasta').val(),
        IdEspecialidad: 0, //$('#Filter_IdEspecialidad').select2('val') ?? 0,
        IdProfesional: $('#Filter_IdProfesional').select2('val') ?? 0
    };
}

$(function () {
    EvolucionesEstadisticasView.init();
});
