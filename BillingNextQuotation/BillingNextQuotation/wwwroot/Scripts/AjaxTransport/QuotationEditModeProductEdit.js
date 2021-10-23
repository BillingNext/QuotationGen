'use strict';
window.onload = function () {
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

    var selectedid = $("#QuotationDetails_SheetSizingOptions").val();
    if (selectedid == 1) {
        $("#QuotationDetails_ProductDimensionX").prop('disabled', true);
        $("#QuotationDetails_ProductDimensionY").prop('disabled', true);
    }
    else {
      convertToInchesX($("#QuotationDetails_ProductDimensionX").val());
      convertToInchesY($("#QuotationDetails_ProductDimensionY").val());
    }
};

function sheetSizeChange() {

    var selectedid = $("#QuotationDetails_SheetSizingOptions").val();
    if (selectedid == 1) {
        var productid = $("#QuotationDetails_ProductId").val();
        Promise.all([getSheetFullSize(productid, $("#QuotationDetails_ProductId option:selected").text())]).then(values => {
            sheetSizeChangedOperations(values);
        }).catch(() => {
            toastr["error"]("One or more errors occured while fetching product details.");
        })
    }
    else {
        var productid = $("#QuotationDetails_ProductId").val();
        Promise.all([getSheetFullSize(productid, $("#QuotationDetails_ProductId option:selected").text())]).then(values => {
            document.getElementById('QuotationDetails_ProductRate').value = values[0][0].ratePeiceSheet;
        }).catch(() => {
            toastr["error"]("One or more errors occured while fetching product details.");
        })
        $("#QuotationDetails_ProductDimensionX").attr('readonly', false);
        $("#QuotationDetails_ProductDimensionY").attr('readonly', false);
        $('#QuotationDetails_ProductDimensionX').removeClass('input-disabled');
        $('#QuotationDetails_ProductDimensionY').removeClass('input-disabled');
        $("#QuotationDetails_ProductDimensionX").prop('disabled', false);
        $("#QuotationDetails_ProductDimensionY").prop('disabled', false);
    }

}

function getSheetFullSize(productid, productname) {
    $(document).ajaxComplete(function () {
        $("#wait").modal('hide');
        $("#wait").hide();
    });
    var options = {};
    options.url = "/Quotation/Admin/Create?id=" + productid + "&&handler=ProductSizes";
    options.type = "GET";
    options.dataType = "json";
    options.success = function (data) {
        //responsesProducts.push(data);
    };
    options.error = function () {
        toastr["error"]("Error occured fetching default sheet sizes for " + productname);
    };
    return $.ajax(options);
}


function sheetSizeChangedOperations(productinfo) {
    if (productinfo != null) {
        document.getElementById('QuotationDetails_ProductDimensionX').value = productinfo[0][0].productDimensionX;
        document.getElementById('QuotationDetails_ProductDimensionY').value = productinfo[0][0].productDimensionY;
        document.getElementById('QuotationDetails_ProductRate').value = productinfo[0][0].rateFullSheet;
        $("QuotationDetails_ProductDimensionX").attr('readonly', true);
        $("QuotationDetails_ProductDimensionY").attr('readonly', true);
        $('QuotationDetails_ProductDimensionX').addClass('input-disabled');
        $('QuotationDetails_ProductDimensionY').addClass('input-disabled');
        $("#QuotationDetails_ProductDimensionX").prop('disabled', true);
        $("#QuotationDetails_ProductDimensionY").prop('disabled', true);
        $("#measureInchX").val(productinfo[0][0].productDimensionX);
        $("#measureInchY").val(productinfo[0][0].productDimensionY);
        
    }
    else {
        location.reload(true);
    }
}

function Inch6Rounder(Inchvalue, idToUpdate) {
    var rounded = Inchvalue % 6 == 0 ? Inchvalue : Math.ceil(Inchvalue / 6) * 6;
    $('#' + idToUpdate).val(rounded);
}

function convertToInchesX(valCentimeter) {
    var rounded = valCentimeter % 6 == 0 ? valCentimeter : Math.ceil(valCentimeter / 6) * 6;
    $('#measureInchX').val(rounded);
}


function convertToInchesY(valCentimeter) {
    var rounded = valCentimeter % 6 == 0 ? valCentimeter : Math.ceil(valCentimeter / 6) * 6;
    $('#measureInchY').val(rounded);
}