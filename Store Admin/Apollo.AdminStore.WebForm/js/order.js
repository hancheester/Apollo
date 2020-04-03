$(function () {
    $('#pharm .otherCond input[type=checkbox]').click(function () {
        if ($('#pharm .otherCond input[type=checkbox]').is(':checked')) {
            $('#qOwnerCondition').show();
        }
        else {
            $('#qOwnerCondition').hide();
        }
    });

    $('#pharm .hasOtherCondition input[type=checkbox]').each(function () {        
        $('#pharm .hasOtherCondition input[type=checkbox]:not(:checked)').parent().parent().siblings('.otherCondition').hide();
        var validatorId2 = $('#pharm .hasOtherCondition input[type=checkbox]:not(:checked)').parent().parent().siblings('.otherCondition').find('.rfvOtherCond').attr('id');
        if (window[validatorId2]) window[validatorId2].validationGroup = '';
    });

    $('#pharm .hasOtherCondition input[type=checkbox]').click(function () {
        $('#pharm .hasOtherCondition input[type=checkbox]:checked').parent().parent().siblings('.otherCondition').show();

        var validatorId1 = $('#pharm .hasOtherCondition input[type=checkbox]:checked').parent().parent().siblings('.otherCondition').find('.rfvOtherCond').attr('id');
        if (window[validatorId1]) window[validatorId1].validationGroup = 'vgOrderDetails';

        $('#pharm .hasOtherCondition input[type=checkbox]:not(:checked)').parent().parent().siblings('.otherCondition').hide();
        var validatorId2 = $('#pharm .hasOtherCondition input[type=checkbox]:not(:checked)').parent().parent().siblings('.otherCondition').find('.rfvOtherCond').attr('id');
        if (window[validatorId2]) window[validatorId2].validationGroup = '';
    });

    $('#pharm .hasTaken input[type=checkbox]').each(function () {
        $('#pharm .hasTaken input[type=checkbox]:checked').parent().parent().siblings('.forHasTaken').show();
        $('#pharm .hasTaken input[type=checkbox]:not(:checked)').parent().parent().siblings('.forHasTaken').hide();
    });

    $('#pharm .hasTaken input[type=checkbox]').click(function () {
        $('#pharm .hasTaken input[type=checkbox]:checked').parent().parent().siblings('.forHasTaken').show();
        $('#pharm .hasTaken input[type=checkbox]:not(:checked)').parent().parent().siblings('.forHasTaken').hide();
    });

    $('#pharm .takenOwner input[type=checkbox]').click(function () {
        if ($('#pharm .takenOwner input[type=checkbox]').is(':checked')) {
            $('#pharm .age').hide();
            $('#pharm .hasOtherCondition').hide();
            $('#pharm .otherCondition').hide();

            var validatorId1 = $('#pharm .age .rfvAge').attr('id');
            if (window[validatorId1]) window[validatorId1].validationGroup = '';
        }
        else {
            $('#pharm .age').show();
            $('#pharm .hasOtherCondition').show();

            $('#pharm .hasOtherCondition input[type=checkbox]:checked').parent().parent().siblings('.otherCondition').show();
            $('#pharm .hasOtherCondition input[type=checkbox]:not(:checked)').parent().parent().siblings('.otherCondition').hide();

            var validatorId2 = $('#pharm .age .rfvAge').attr('id');
            if (window[validatorId2]) window[validatorId2].validationGroup = 'vgOrderDetails';
        }
    });

    $('.grandTotal').each(
	    function (index, domEle) {
	        showGrandTotal($(domEle).find('.orderId').val());
	    }
	);

    $('.activity-red').each(
        function (index, domEle) {
            $(domEle).parent().parent().addClass('traffic-red');
        }
    );

    $('.activity-orange').each(
        function (index, domEle) {
            $(domEle).parent().parent().addClass('traffic-orange');
        }
    );
});

function validate() {
    for (i = 0; i < arguments.length; i++) {
        console.log(arguments[i]);
        Page_ClientValidate(arguments[i]);

        if (!Page_IsValid) {
            $('.tabs').addClass('red');
            
            if (arguments[i] == 'vgPayment')
                $('#valSumPayment').show();
            if (arguments[i] == 'vgOrderDetails')
                $('#valSumOrder').show();

            return false;
        }
    }

    return true;
}

function validateSelection() {
    var isValid = $('.chkExportCSV input:checkbox:checked').length > 0;
    if (isValid == false)
        alert("Sorry, there is no selected order.");
    return isValid;
}