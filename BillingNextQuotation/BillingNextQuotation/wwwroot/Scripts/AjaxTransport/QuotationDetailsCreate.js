'use strict';

window.onload = function () {

    document.getElementById("deloperation").style.display = "none";

    $('.spanChange').on('DOMSubtreeModified', function () {
        Inch6Rounder($('#' + this.id).text(), "measureCentimeter" + this.id.substring(9));
    });

    function alignModal() {
        var modalDialog = $(this).find(".loader");

        // Applying the top margin on modal dialog to align it vertically center
        modalDialog.css("margin-top", Math.max(0, ($(window).height() - modalDialog.height()) / 2));
    }
    // Align modal when it is displayed
    $(".modal").on("shown.bs.modal", alignModal);

    // Align modal when user resize the window
    $(window).on("resize", function () {
        $(".modal:visible").each(alignModal);
    });   

    $(document).ajaxStart(function () {
        $("#wait").modal({ show: true, backdrop: 'static' });
        $("#wait").show();
    });
    $(document).ajaxComplete(function () {
        $('#wait').on('shown.bs.modal', function (e) {
            $("#wait").modal('hide');
            $("#wait").hide();
        })

    });
    $('.selectProduct').select2({ placeholder: "Select a Product" });
};

var responsesProducts = [];
var counter = -1;
var quotationid;
var localCounter = -1;

function addInput() {
    counter++;
    var rowid = 'row' + counter;
    var newRow = $("<tr id='row" + counter + "'>");
    var cols = "";
    cols += '<td><select id="products' + counter + '" onchange="productchanged(this.id,' + counter + ')" class="form-control select2 selectProduct" style="width:220px;"></select></td>';
    cols += '<td class="text-center"><select id="sheetSizing' + counter + '"  onchange="sheetSizeChange(this.id,' + counter + ')" class="form-control"></select></td>';
    cols += '<td class="text-center"><select id="sheetMeasurement' + counter + '" onchange="sheetMeasurementChange(this.id,' + counter + ')" class="form-control" > </select></td>';
    cols += '<td class="text-center"> <input id="dimensionX' + counter + '" min="0" oninput="convertToInchesX($(this).val(),' + counter + ');" onpaste="convertToInchesX($(this).val(),' + counter + ');" onkeyup="convertToInchesX($(this).val(),' + counter + ');"  type="number" class="form-control" style="width:120px;"><div id="dimensionXFI' + counter + '"  style="display:none; padding:5px;"> <div class="row" ng-controller="testApp as ta"> <div>Feet and Inches</div><div class="input-group"> <inches-to-feet-and-inches measure="ta.someMeasure"></inches-to-feet-and-inches> <div class="input-group-append"> Total Inches <span class="input-group-text spanChange" id="totalinchX' + counter + '" ng-bind="ta.someMeasure | json"></span> </div></div></div></div> <div id="dimensionXC' + counter + '"> Total Inches(Rounded to 6) <span id="measureCentimeterX' + counter +'" class="input-group-text InchMeasureX" ></span></div></td>';
    cols += '<td class="text-center" ><input id="dimensionY' + counter + '" min="0" type="number" oninput="convertToInchesY($(this).val(),' + counter + ');" onpaste="convertToInchesY($(this).val(),' + counter + ');" onkeyup="convertToInchesY($(this).val(),' + counter + ');"  class="form-control" style="width:120px;"> <div id="dimensionYFI' + counter + '" style="display:none; padding:5px;"> <div class="row" ng-controller="testApp as ta"> <div>Feet and Inches</div><div class="input-group"> <inches-to-feet-and-inches measure="ta.someMeasure"></inches-to-feet-and-inches> <div class="input-group-append"> Total Inches <span class="input-group-text spanChange" id="totalinchY' + counter + '" ng-bind="ta.someMeasure | json"></span> </div></div></div></div> <div id="dimensionYC' + counter + '" > Total Inches(Rounded to 6)<span id="measureCentimeterY' + counter +'" class="input-group-text InchMeasureY"></span> </div></td>';
    cols += '<td class="text-right"><input class="form-control pull-right count-me" style="text-align:right; width:120px; " min="0" oninput="calcQuantity();" type="number" id="quantity' + counter + '" required/></td>';

    document.getElementById("deloperation").style.display = "inline";
    newRow.append(cols);
    $('#billdet > tbody > tr').eq(counter).after(newRow);
    $('#products-1 option').clone().appendTo('#products' + counter);
    $('.select2').select2(); 
    $('#sheetSizing-1 option').clone().appendTo('#sheetSizing' + counter);
    $('#sheetMeasurement-1 option').clone().appendTo('#sheetMeasurement' + counter);
    angular.bootstrap(document.getElementById("dimensionXFI" + counter), ['feetAndInch']);
    angular.bootstrap(document.getElementById("dimensionYFI" + counter), ['feetAndInch']);
    $('.spanChange').on('DOMSubtreeModified', function () {
        Inch6Rounder($('#' + this.id).text(), "measureCentimeter" + this.id.substring(9));
    });
}
function removeRow() {
    if (counter == 0) {
        document.getElementById("deloperation").style.display = "none";
        $('#row' + counter).remove();
        counter -= 1;
        calcQuantity();
    }
    else if (counter == -1) {
        document.getElementById("deloperation").style.display = "none";
        calcQuantity();
    }
    else {
        $('#row' + counter).remove();
        counter -= 1;
        calcQuantity();
    }
}

function getSheetFullSize(productid, productname) {
    $(document).ajaxComplete(function () {
            $("#wait").modal('hide');
            $("#wait").hide();
    });
    var options = {};
    options.url = "/Quotation/Create?id=" + productid + "&&handler=ProductSizes";
    options.type = "GET";
    options.dataType = "json";
    options.success = function (data) {
        responsesProducts.push(data);
    };
    options.error = function () {
        toastr["error"]("Error occured fetching default sheet sizes for " + productname);
    };
    return $.ajax(options);
}

function sheetSizeChange(id, counter) {

    var selectedid = $("#" + id).val();
    if (selectedid == 1) {
        var productid = $("#products" + counter).val();
        Promise.all([getSheetFullSize(productid, $("#products" + counter+" option:selected").text())]).then(values => {
            sheetSizeChangedOperations(values, counter);
        }).catch(() => {
            toastr["error"]("One or more errors occured while fetching product details.");
        })
    }
    else {
        $("#dimensionX" + counter).attr('readonly', false);
        $("#dimensionY" + counter).attr('readonly', false);
        $('#dimensionX' + counter).removeClass('input-disabled');
        $('#dimensionY' + counter).removeClass('input-disabled');
        $("#sheetMeasurement" + counter).prop('disabled', false);
        $('#sheetMeasurement' + counter).removeClass('input-disabled');
    }

}


function sheetSizeChangedOperations(productinfo, counter) {
    if (productinfo != null) {
        $('#sheetMeasurement' + counter).val(0).change();
        document.getElementById('dimensionX' + counter).value = productinfo[0][0].productDimensionX;
        document.getElementById('dimensionY' + counter).value = productinfo[0][0].productDimensionY;
        $("#dimensionX" + counter).attr('readonly', true);
        $("#dimensionY" + counter).attr('readonly', true);
        $('#dimensionX' + counter).addClass('input-disabled');
        $('#dimensionY' + counter).addClass('input-disabled');
        $("#sheetMeasurement" + counter).prop('disabled', true);
        $('#sheetMeasurement' + counter).addClass('input-disabled');
        $("#measureCentimeterX" + counter).text($("#dimensionX" + counter).val());
        $("#measureCentimeterY" + counter).text($("#dimensionY" + counter).val());
    }
    else {
        location.reload(true);
    }
}

function productchanged(id, counter) {
    sheetSizeChange("sheetSizing" + counter, counter);
}

function sheetMeasurementChange(id, counter) {
    var selectedid = $("#" + id).val();
    $("#measureCentimeterX" + counter).text("0");
    $("#measureCentimeterY" + counter).text("0");
    if (selectedid == 1) {
        $("#dimensionX" + counter).css("display", "none");
        $("#dimensionY" + counter).css("display", "none");
        $("#dimensionXFI" + counter).css("display", "block");
        $("#dimensionYFI" + counter).css("display", "block");
        $('#dimensionX' + counter).val(0);
        $('#dimensionY' + counter).val(0);
    }
    else if (selectedid == 2)
    {
        $("#dimensionX" + counter).css("display", "block");
        $("#dimensionY" + counter).css("display", "block");
        $("#dimensionXFI" + counter).css("display", "none");
        $("#dimensionYFI" + counter).css("display", "none");
        $("#dimensionXC" + counter).css("display", "block");
        $("#dimensionYC" + counter).css("display", "block");
        $('#dimensionX' + counter).val(0);
        $('#dimensionY' + counter).val(0);
    }
    else {
        $("#dimensionX" + counter).css("display", "block");
        $("#dimensionY" + counter).css("display", "block");
        $("#dimensionXFI" + counter).css("display", "none");
        $("#dimensionYFI" + counter).css("display", "none");
        $('#dimensionX' + counter).val(0);
        $('#dimensionY' + counter).val(0);
    }
}



function convertToInchesX(valCentimeter, counter) {
    if ($('#sheetMeasurement' + counter).val() == 2) {
        var x = (valCentimeter * 0.39370).toFixed();
        var rounded = x % 6 == 0 ? x : Math.ceil(x / 6) * 6;
        $('#measureCentimeterX' + counter).text(rounded);
    }
    else if ($('#sheetMeasurement' + counter).val() == 0) {
        var rounded = valCentimeter % 6 == 0 ? valCentimeter : Math.ceil(valCentimeter / 6) * 6;
        $('#measureCentimeterX' + counter).text(rounded);
    }
}


function convertToInchesY(valCentimeter, counter) {
    if ($('#sheetMeasurement' + counter).val() == 2) {
        var x = (valCentimeter * 0.39370).toFixed();
        var rounded = x % 6 == 0 ? x : Math.ceil(x / 6) * 6;
        $('#measureCentimeterY' + counter).text(rounded);
    }
    else if ($('#sheetMeasurement' + counter).val() == 0) {
        var rounded = valCentimeter % 6 == 0 ? valCentimeter : Math.ceil(valCentimeter / 6) * 6;
        $('#measureCentimeterY' + counter).text(rounded);
    }
}


function Inch6Rounder(Inchvalue, idToUpdate) {
    var rounded = Inchvalue % 6 == 0 ? Inchvalue : Math.ceil(Inchvalue / 6) * 6;
    $('#'+idToUpdate).text(rounded);
}

function updateQuotationAmount() {
     var options = {};
        options.url = "/Quotation/Create?handler=UpdateQuotationAmount&id=" + quotationid;
        options.type = "POST";
        options.contentType = "application/json; charset=utf-8";
        options.dataType = "json";
        options.beforeSend = function (xhr) {
            xhr.setRequestHeader("MY-XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        };
        options.success = function (msg) {
            if (msg == true) {
                toastr["success"]("Quotation amount saved successfully.");
                window.location.pathname = "/Quotation/Details/" + quotationid;
            }
            else {
                toastr["error"]("Error occured updating quotation amount.");
            }

        };
        options.error = function () {
            toastr["error"]("Error occured updating quotation amount.");
            return;
        };

        $.ajax(options);
}

function validateQuotation() {
    var i;
    var async_request = [];
    for (i = -1; i <= counter; i++) { 
        if (isNullOrWhitespace($("#products" + i).val())) {
            toastr["error"]("Product is not specified at row " + (i + 2) + ".");
            document.getElementById("btncreate").disabled = false;
            return false;
        }
        if ($("#quantity" + i).val() == 0) { 
            toastr["error"]("Quantity is not set in row " + (i + 2) + ".");
            document.getElementById("btncreate").disabled = false;
            return false;
        }
        if (isNullOrZero($("#measureCentimeterX" + i).text())) {
            toastr["error"]("Length is not set in row " + (i + 2) + ".");
            document.getElementById("btncreate").disabled = false;
            return false;
        }
        if (isNullOrZero($("#measureCentimeterY" + i).text())) {
            toastr["error"]("Width is not set in row " + (i + 2) + ".");
            document.getElementById("btncreate").disabled = false;
            return false;
        }
        async_request.push(getSheetFullSize($("#products" + i).val(), $("#products" + i + " option:selected").text()));
    }
    $.when.apply(null, async_request).done(function () {
        for (var i = -1; i <= counter; i++) {
            if (sheetSizeValidation(responsesProducts[i+1], $("#measureCentimeterX" + i).html(), $("#measureCentimeterY" + i).html())==false) {
                toastr["error"]("Sheet dimensions exceeds full sheet size of " + responsesProducts[i + 1][0].productDimensionX + " x " + responsesProducts[i + 1][0].productDimensionY + " at row " + (i + 2) + ".");
                responsesProducts.splice(0, responsesProducts.length);
                document.getElementById("btncreate").disabled = false;
                return false;
            }
        }
        
        actualSaveQuotation();
    });
}



function sheetSizeValidation(sizes, dimensionX, dimensionY) {
    if (sizes[0].productDimensionX < parseInt(dimensionX) || sizes[0].productDimensionY < parseInt(dimensionY))
    {
       return false;
    }
    return true;
}

function saveQuotation() {
    document.getElementById("btncreate").disabled = true;
    validateQuotation();
}

function actualSaveQuotation() {
    $(document).ajaxStart(function () {
       $('#wait').on('shown.bs.modal', function (e) {
        $("#wait").modal({ show: true, backdrop: 'static' });
        $("#wait").show();
         })
    });
    $(document).ajaxComplete(function () {
        $("#wait").modal({ show: true, backdrop: 'static' });
        $("#wait").show();
    });

    var options = {};
    options.url = "/Quotation/Create?handler=CreateQuotation";
    options.type = "POST";


    var obj = {};
    obj.QuotationTo = $('#quotationTo').val();
    obj.CompanyID = $('#companyId').val();

    options.data = JSON.stringify(obj);

    options.contentType = "application/json; charset=utf-8";
    options.dataType = "json";

    options.beforeSend = function (xhr) {
        xhr.setRequestHeader("MY-XSRF-TOKEN",
            $('input:hidden[name="__RequestVerificationToken"]').val());
    };
    options.success = function (msg) {
        var re = new RegExp('^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$', 'i');
        if (re.test(msg)) {
            quotationid = msg;
            toastr["success"]("Quotation created successfully.");
            saveQuotationDetails();
        }
        else {
            toastr["error"]("Error occured saving quotation :"+msg);
        }
        
    };
    options.error = function () {
        toastr["error"]("Error occured saving quotation.");
        window.setTimeout(function () { location.reload() }, 3000);
    };

    $.ajax(options);
}

function saveQuotationDetails() {
    $(document).ajaxStart(function () {
        $("#wait").modal({ show: true, backdrop: 'static' });
        $("#wait").show();
    });
    $(document).ajaxComplete(function () {
        // $('#wait').on('shown.bs.modal', function (e) {
        $("#wait").modal('hide');
        $("#wait").hide();
        //})

    });
    $('#billdet tr').each(function (row, tr) {

        var options = {};
        options.url = "/Quotation/Create?handler=CreateQuotationDetails";
        options.type = "POST";

        if (isNullOrWhitespace($(tr).find('td:eq(0)').find("select").val())) {
            return;
        }

        var obj = {};
        obj.SequenceNumber = parseInt($(tr).attr("id").substring(3)) + 2;
        obj.ProductName = $(tr).find('td:eq(0)').find("select option:selected").text();
        obj.SheetSizingOptions = parseInt($(tr).find('td:eq(1)').find("select").val());
        obj.SheetMeasurementOptions = parseInt($(tr).find('td:eq(2)').find("select").val());
        obj.ProductDimensionX = parseInt($(tr).find('td:eq(3)').find(".InchMeasureX").text());
        obj.ProductDimensionY = parseInt($(tr).find('td:eq(4)').find(".InchMeasureY").text());
        obj.ProductQuantity = parseInt($(tr).find('td:eq(5)').find("input").val());

        obj.QuotationId = quotationid;
        obj.ProductId = $(tr).find('td:eq(0)').find("select").val();
        obj.CompanyID = $('#companyId').val();

        options.data = JSON.stringify(obj);
        console.log(options.data);
        options.contentType = "application/json; charset=utf-8";
        options.dataType = "json";
        options.beforeSend = function (xhr) {
            xhr.setRequestHeader("MY-XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        };
        options.success = function (msg) {
            if (localCounter == counter) {
                updateQuotationAmount();
            }
            else {
                localCounter = localCounter+1;
            }
        };
        options.error = function () {
            toastr["error"]("Error occured saving quotation details.");
            return;
        };

        $.ajax(options);
    });
    return true;
}

function calcQuantity() {
    var grandTotal = 0;

    $('tr').each(function () {
        // var sum = 0;
        $(this).find('.count-me').each(function () {
            var combat = $(this).val();
            if (!isNaN(combat) && combat.length !== 0) {
                grandTotal += parseInt(combat);
            }
        });

    });

    document.getElementById('grandtotal').innerHTML =  grandTotal;
}

function isNullOrWhitespace(input) {
    return !input || !input.trim();
}

function isNullOrZero(input) {
    return !input || !parseInt(input) != 0; 
}

