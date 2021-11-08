
function popup() {
    alert('inject');
} 
var swaggerJson;
 
$.ajax({

    url: "/swagger/v1/swagger.json",
    dataType: "json",

    success: function (jsonData) {
        swaggerJson = jsonData;

    }
});

//$(document).ready(function () {
//    $(document).on("click", function (e) {
//        somethingClicked(e);
//    });
//});

function  somethingClicked(e) {
    if ($(e.target).is('span.model-title__text')) {
        var div = $(e.target).closest('div.model-box');
        var model1 = $(e.target).closest('span.model');
        var model;
        if (model1.length > 0) {
            model = $(model1[0]).closest('span.model');
        }
        if (div.length > 0) {
            var text = $(e.target).closest('tr').children('td:first').text().replace("*", "");
            var spanmodel = div.find('span.model-title__text');
            
            if (spanmodel.length > 0) {
                //var f = spanmodel.find('span.model-title__text');
               // if (f.length > 0) {
                var mText = spanmodel[0].innerText;
                if (mText) {
                    if (swaggerJson.definitions[mText]) {
                            if (swaggerJson.definitions[mText].properties[text]) {
                                if (swaggerJson.definitions[mText].properties[text].description) {
                                    $(e.target)[0].innerText = swaggerJson.definitions[mText].properties[text].description;
                                }
                            }
                        } 
                    }
               // }
             
            }
        }

    }
}