var propcount = 0;
var imagearray = [];

$(document).ready(function () {

    //$(".picker").imagepicker({show_label: true});
    $('#selectImage').on('click', function () {
        $("#file-input").trigger('click');
    });

    $("#file-input").on("change", async function (event) {
        let files = [...event.target.files];
        let images = await Promise.all(files.map(f => { return readAsDataURL(f, files.indexOf(f) + imagearray.length) })).then(data => {
        });
        //for (var i = 0; i < files.length; i++) {
        //    var tmppath = URL.createObjectURL(files[i]);
        //    $(".image-picker").append('<option id="img_' + i + ' " data-img-src="' + tmppath + '" data-img-alt="page ' + i + '" value="' + i + '" >  page ' + i + ' </option>');
        //}
    });

});



function readAsDataURL(file,index) {
    return new Promise((resolve, reject) => {
        let fileReader = new FileReader();
        fileReader.onload = function (e) {
            var img = document.createElement("img");
            img.src = e.target.result;
            img.onload = function () {
                addImage(index,file,img)
            }
            return resolve({ data: fileReader.result, name: file.name, size: file.size, type: file.type });
        }
        fileReader.readAsDataURL(file);
    })
}

function addImage(index, file,img) {
    var canvas = document.createElement("canvas");
    var width = 236;
    var height = 236;
    canvas.width = width;
    canvas.height = height;
    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0, width, height);
    imagearray.push(file);
    canvas.toBlob(function (blob) {
        $(".picker").append('<option id="img_select_' + index + '" data-img-src="' + URL.createObjectURL(blob) + '" data-img-alt="page ' + index + '" value="' + index + '" > ' + file.name + ' </option>');
        if (index == 0)
            $(".image_picker_selector").append('<li id="img_li_' + index + '" class="img-list"><input type="button" id="img_remove_' + index + '" class="close" onclick="removeImg(' + index + ',\'' + file.name + '\')" value="X" /><div class="thumbnail selected" id="thumbnail_' + index + '" onclick="onImageSelect(' + index + ')"><label class="label truncate">' + file.name + '</label><img class="image_picker_image" src="' + URL.createObjectURL(blob) + '" alt="' + file.name + '"></div></li>');
        else
            $(".image_picker_selector").append('<li id="img_li_' + index + '" class="img-list"><input type="button" id="img_remove_' + index + '" class="close" onclick="removeImg(' + index + ',\'' + file.name + '\')" value="X" /><div class="thumbnail" id="thumbnail_' + index + '" onclick="onImageSelect(' + index + ')"><label class="label truncate">' + file.name + '</label><img class="image_picker_image" src="' + URL.createObjectURL(blob) + '" alt="' + file.name + '"></div></li>');

    });
}

function onImageSelect(index) {
    $(".thumbnail").each(function (valindex) {
        $(this).removeClass('selected');
    });
    $('#thumbnail_' + index).addClass('selected');
    $(".picker").val(index);
}

function clearInputFile() {
    document.getElementById("file-input").value = "";
}

function removeImg(id, file) {
    //id ile değil list elemanının bir sonrakini alıp yap
    var after = $('#img_li_' + id).next();
    var child = $(after).children('.thumbnail');
    var selectedval = $("#picker option:selected").val();
    if (selectedval == id) {
        $(child).addClass('selected');
    }
    $("#img_remove_" + id).remove();
    $("#img_select_" + id).remove();
    $("#img_li_" + id).remove();
    var elem = imagearray.find(f => f.name == file);
    imagearray.splice(imagearray.indexOf(elem), 1);
    clearInputFile();
}


$(document).ready(function () {
    var subcategorys = [];
    $.ajax({
        url: '/Category/GetCategories',
        type: 'get',
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            response.forEach(function (item) {
                if (item.isMainCategory == true) {
                    $('#category_select').append($('<option>', {
                        value: item.id,
                        text: item.properties[0].value[0]
                    }));
                }
            });
            response.forEach(function (item,index) {
                if (item.isMainCategory == true) {
                    response[index].subCategorys.forEach(function (item) {
                        subcategorys.push(item);
                    });
                    return false;
                }
            });
            getSubs(subcategorys);
        }
    });
});

$("#category_select").change(function () {
    getCategory($(this).val());
});

function getCategory(id) {
    var subcategorys = [];
    $.ajax({
        url: '/Category/GetCategory?id=' + id,
        type: 'get',
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            response.subCategorys.forEach(function (item) {
                subcategorys.push(item);
            });
            getSubs(subcategorys);
        }
    });
}

function getSubs(ids) {
    $('#sub_category_select').empty();
    $.ajax({
        url: '/Category/GetSubCategorys?ids=' + JSON.stringify(ids),
        type: 'get',
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            response.forEach(function (item) {
                $('#sub_category_select').append($('<option>', {
                    value: item.id,
                    text: item.properties[0].value[0]
                }));
            });
        }
    });
}


$('#Create').on("click", function () {
    createProduct();
});

$("#addProp").on("click", function () {
    propcount++;
    $(".props").append(
        '<div class="form-group">' +
        '<div class="tv" id="tvprop_' + propcount + '">' +
        '<input type="button" id="prop_remove_' + propcount + '" class="close" onclick="removeProp(' + propcount + ')" value="X" />' +
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
    $("#prop_labelname_" + id).remove();
    $("#prop_labelvalue_" + id).remove();
    $("#tvprop_" + id).remove();
    $("#prop_name_" + id).remove();
    $("#prop_val_" + id).remove();
    $("#prop_span_" + id).remove();
    $("#prop_remove_" + id).remove();
    propcount--;
}


function createJsonObject() {
    jsonObj = [];
    var mainImage = {}
    var mainImageValue = [];
    mainImage["Name"] = "MainImage";
    mainImageValue.push(imagearray[$("#picker option:selected").val()].name);
    mainImage["Value"] = mainImageValue;
    jsonObj.push(mainImage);
    $(".propertiesname").each(function (index) {
        var name = $(this).val();
        var value = [];
        $(".propertiesvalue").each(function (valindex) {
            if (index == valindex) {
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
    });
    return jsonObj;
}

function createProduct() {
    var files = imagearray;
    if (imagearray.length == 0) {
        alert("Add at Least One Image!");
    }
    else {
        var formData = new FormData();
        for (var i = 0; i != files.length; i++) {
            formData.append("files", files[i]);
        }
        var data = createJsonObject();
        var categoryId = $('#category_select').find(":selected").val();
        var subCategoryId = $('#sub_category_select').find(":selected").val();
        formData.append('properties', JSON.stringify(data));
        formData.append('categoryId', categoryId);
        formData.append('subCategoryId', subCategoryId);
        $.ajax(
            {
                url: "/Product/addProduct",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    alert("Files Uploaded!");
                    location.reload(true);
                },
                error: function (error) {
                    alert("Picture's are Not Uploaded Please Edit This Product");
                }
            }
        );
    }
    
}


