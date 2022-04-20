var propcount = 0;
var subcount = 0;
var subcategorys = [];

$(document).ready(function () {
    $.ajax({
        url: '/Category/GetCategories',
        type: 'get',
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            response.forEach(function (item) {
                subcategorys.push(item);
            });
            console.log(subcategorys);
        }
    });
});

$('#Create').on("click", function () {
    createCategory();
});

$("#addProp").on("click", function () {
    propcount++;
    $(".props").append(
        '<div class="form-group">' +
        '<input type="button" id="prop_remove_' + propcount + '" class="close" onclick="removeProp(' + propcount +')" value="X" />' +
        '<div class="tv" id="tvprop_' + propcount +'">'+
        '    <label class="control-label" id="prop_labelname_' + propcount + '">Property Name</label>' +
        '    <input id="prop_name_' + propcount + '" class="form-control propertiesname" />' +
        '    <label class="control-label" id="prop_labelvalue_' + propcount + '">Property Value</label>' +
        '    <input id="prop_val_' + propcount + '" class="form-control propertiesvalue" placeholder"Use Comma To Seperate Multiple Values"/>' +
        '    <span class="text-danger" id="prop_span_' + propcount + '"></span>' +
        '</div>'+
        '</div>'
    );
});

function removeProp(id) {
    console.log(id);
    $("#prop_labelname_" + id).remove();
    $("#tvprop_" + id).remove();
    $("#prop_labelvalue_" + id).remove();
    $("#prop_name_" + id).remove();
    $("#prop_val_" + id).remove();
    $("#prop_span_" + id).remove();
    $("#prop_remove_" + id).remove();
    propcount--;
}

$("#addSub").on("click", function () {
    subcount++;
    $(".subs").append(
        '<div class="form-group">' +
        '<input type="button" id="sub_remove_' + subcount + '" class="close" onclick="removeSub(' + subcount + ')" value="X" />' +
        '<div class="tv" id="tvsub_' + subcount+'">' +
        '    <label class="control-label" id="sub_labelvalue_' + subcount + '">Sub Category</label>' +
        '    <select class="form-control subSelect" id="sub_val_' + subcount + '">' +
        '    </select>'+
        '    <span class="text-danger" id="sub_span_' + subcount + '"></span>' +
        '</div>' +
        '</div>'
    );
    subcategorys.forEach(function (item) {
        if (item.isMainCategory == false) {
            $('#sub_val_' + subcount).append($('<option>', {
                value: item.id,
                text: item.properties[0].value[0]
            }));
        }
    });
    
});

function removeSub(id) {
    console.log(id);
    $("#sub_labelvalue_" + id).remove();
    $("#tvsub_" + id).remove();
    $("#sub_val_" + id).remove();
    $("#sub_span_" + id).remove();
    $("#sub_remove_" + id).remove();
    subcount--;
}


function createJsonObject() {
    jsonObj = [];
    $(".propertiesname").each(function (index) {
        var name = $(this).val();
        var value = [];
        $(".propertiesvalue").each(function (valindex) {
            if (index == valindex) {
                console.log("Equal " + index);
                var values = $(this).val().split(",");
                values.forEach(function (item) {
                    if(item != "")
                        value.push(item);
                });
            }
        });
        item = {}
        item["Name"] = name;
        item["Value"] = value;
        jsonObj.push(item);
        console.log(item);
    });
    return jsonObj;
}

function createSubJsonObject() {
    var subs = [];
    $(".subSelect").each(function (index) {
        subs.push($(this).val());
    });
    return subs;
}

function controlSubJsonObject() {
    var subs = [];
    var same = false;
    $(".subSelect").each(function (index) {
        if (jQuery.inArray($(this).val(), subs))
            subs.push($(this).val());
        else {
            alert("You can't Choose same Sub Category");
            same = true;
        }
        if (same == true) return false;
    });
    return same;
}

function createCategory() {
    if (!controlSubJsonObject()) {
        var data = createJsonObject();
        var subdata = createSubJsonObject();
        var isMain = $('#isMain_val').is(':checked') == true ? true : false;
        $.ajax({
            url: '/Category/addCategory?properties=' + JSON.stringify(data) + '&isMainCategory=' + isMain + '&subCategorys=' + JSON.stringify(subdata),
            type: 'post',
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
            }
        });
        location.reload(true);
    }
    
}


