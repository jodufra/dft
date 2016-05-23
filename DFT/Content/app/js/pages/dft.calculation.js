function ClearTags() {
    $('body [data-plugin="tags-selectize"]')[0].selectize.clear();
}

FusionCharts.ready(function () {
    var inputSignal = new FusionCharts({
        type: 'line',
        renderAt: 'inputsignal-chart-container',
        width: '1140',
        height: '500',
        dataFormat: 'json',
        dataSource: {
            "chart": {
                "caption": "Input Signal",
                "subCaption": "",
                "xAxisName": "Index",
                "yAxisName": "Input",
                "lineThickness": "2",
                "paletteColors": "#0075c2",
                "baseFontColor": "#333333",
                "baseFont": "Helvetica Neue,Arial",
                "captionFontSize": "14",
                "subcaptionFontSize": "14",
                "subcaptionFontBold": "0",
                "showBorder": "0",
                "bgColor": "#ffffff",
                "showShadow": "0",
                "canvasBgColor": "#ffffff",
                "canvasBorderAlpha": "0",
                "divlineAlpha": "100",
                "divlineColor": "#999999",
                "divlineThickness": "1",
                "divLineDashed": "1",
                "divLineDashLen": "1",
                "divLineGapLen": "1",
                "showXAxisLine": "1",
                "xAxisLineThickness": "1",
                "xAxisLineColor": "#999999",
                "showAlternateHGridColor": "0"
            },
            "data": Charts.InputSignal.data
        }
    }).render();
    var magnitude = new FusionCharts({
        type: 'line',
        renderAt: 'magnitude-chart-container',
        width: '1140',
        height: '500',
        dataFormat: 'json',
        dataSource: {
            "chart": {
                "caption": "Magnitude",
                "subCaption": "",
                "xAxisName": "Index",
                "yAxisName": "Magnitude",
                "lineThickness": "2",
                "paletteColors": "#0075c2",
                "baseFontColor": "#333333",
                "baseFont": "Helvetica Neue,Arial",
                "captionFontSize": "14",
                "subcaptionFontSize": "14",
                "subcaptionFontBold": "0",
                "showBorder": "0",
                "bgColor": "#ffffff",
                "showShadow": "0",
                "canvasBgColor": "#ffffff",
                "canvasBorderAlpha": "0",
                "divlineAlpha": "100",
                "divlineColor": "#999999",
                "divlineThickness": "1",
                "divLineDashed": "1",
                "divLineDashLen": "1",
                "divLineGapLen": "1",
                "showXAxisLine": "1",
                "xAxisLineThickness": "1",
                "xAxisLineColor": "#999999",
                "showAlternateHGridColor": "0"
            },
            "data": Charts.Magnitude.data
        }
    }).render();
    var phase = new FusionCharts({
        type: 'line',
        renderAt: 'phase-chart-container',
        width: '1140',
        height: '500',
        dataFormat: 'json',
        dataSource: {
            "chart": {
                "caption": "Phase",
                "subCaption": "",
                "xAxisName": "Index",
                "yAxisName": "Phase",
                "lineThickness": "2",
                "paletteColors": "#0075c2",
                "baseFontColor": "#333333",
                "baseFont": "Helvetica Neue,Arial",
                "captionFontSize": "14",
                "subcaptionFontSize": "14",
                "subcaptionFontBold": "0",
                "showBorder": "0",
                "bgColor": "#ffffff",
                "showShadow": "0",
                "canvasBgColor": "#ffffff",
                "canvasBorderAlpha": "0",
                "divlineAlpha": "100",
                "divlineColor": "#999999",
                "divlineThickness": "1",
                "divLineDashed": "1",
                "divLineDashLen": "1",
                "divLineGapLen": "1",
                "showXAxisLine": "1",
                "xAxisLineThickness": "1",
                "xAxisLineColor": "#999999",
                "showAlternateHGridColor": "0"
            },
            "data": Charts.Phase.data
        }
    }).render();
});