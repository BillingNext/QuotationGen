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

};

function defaultCalculationChanged() {
    if ($('#defaultCalcCHK').is(':checked') === false) {
        $('#spamount').hide();
        $("#QuotationSpecialCharges_SpecialChargeAmount").val(null);
    }
    else {
        $('#spamount').show();
        $(document).ajaxComplete(function () {
            $("#wait").modal('hide');
            $("#wait").hide();
        });
        var value = $("#QuotationSpecialCharges_SpecialChargesId").val();
        var options = {};
        options.url = "/Quotation/Admin/Create?id=" + value + "&&handler=SpecialChargeAmount";
        options.type = "GET";
        options.dataType = "json";
        options.success = function (data) {
            $("#QuotationSpecialCharges_SpecialChargeAmount").val(data);
        };
        options.error = function () {
            toastr["error"]("Error occured fetching amount for Special Charge.");
        };
        $.ajax(options);
    }
}