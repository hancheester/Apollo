var barcodeTimeout;

$(document).ready(function() {

    // disable enter key
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });

    $('body').delegate('span.incr', 'click', function () {        
        var newQty = parseInt($(this).siblings('.qty').text()) + 1;
        $(this).siblings('.qty').text(newQty);
        $(this).siblings('input').val(newQty);
    });

    $('body').delegate('span.decr', 'click', function () {    
        var newQty = parseInt($(this).siblings('.qty').text()) - 1;

        if (newQty == 0)
            $(this).parent().parent().remove();
        else {
            $(this).siblings('.qty').text(newQty);
            $(this).siblings('input').val(newQty);
        }
    });

    $('#barcodeInput').keyup(function() {
        barcodeTimeout = setTimeout(function() {
            var $displayArea = $('.displayArea');
            $displayArea.css('opacity', '0.1');

            var indicator = $('<span class="progress"></span>')
            indicator.offset({
                top: (($displayArea.height() / 2) + $displayArea.position().top),
                left: (($displayArea.width() / 2) + $displayArea.position().left)
            });
            indicator.insertBefore('.displayArea');

            var $barcodeInput = $('#barcodeInput');
            var barcode = jQuery.trim($barcodeInput.val());
            console.log('barcode: ' + barcode);
            var $messageBox = $('#messageBox');

            if (barcode != '') {
                // Perform request here
                $.ajax({
                    url: '/get_item_handler.aspx?barcode=' + barcode + '&branchid=' + $('.branchList').val(),
                    context: document.body,
                    dataType: 'json',
                    success: function (data) {
                        console.log('data.length: ' + data.length);
                        
                        if (data && data.length) {                            
                            var item = data[0];
                            if ($('#item' + item.id).length) {
                                var $qtyItemId = $('#qty' + item.id);
                                var $quantityItemId = $('#quantity' + item.id);
                                var currentQty = parseInt($qtyItemId.text());
                                var maxQty = parseInt(item.maxquantity);

                                if (currentQty >= maxQty) {
                                    $messageBox.html('Sorry, MAXIMUM required quantity has reached from the selected branch. It could mean the branch has sent extra item.');
                                    $messageBox.addClass('noticeBox');
                                    $messageBox.show();
                                }
                                else {
                                    var q = parseInt($qtyItemId.text()) + 1;
                                    $qtyItemId.text(q);
                                    $qtyItemId.val($qtyItemId.html());
                                    $quantityItemId.val(q);
                                }
                            }
                            else {
                                $('#items > tbody:last').append("<tr><td><input name=\"item" + item.id + "\" id=\"item" + item.id + "\" type=\"hidden\" value=\"" + item.id + "\" />" + item.name + " " + item.option + "</td><td><input name=\"quantity" + item.id + "\" id=\"quantity" + item.id + "\" type=\"hidden\" value=\"1\" /><span class=\"incr fa fa-plus-circle\"></span><span id=\"qty" + item.id + "\" class=\"qty\">1</span><span class=\"decr fa fa-minus-circle\"></span></td></tr></tr>");
                            }
                        }
                        else {
                            $messageBox.html('Sorry, item with barcode \'' + barcode + '\' is NOT found from the selected branch following possible reasons below.<br/>1. Branch sends wrong item.<br/>2. Product is not available anymore.');
                            $messageBox.addClass('noticeBox');
                            $messageBox.show();
                        }
                    },
                    error: function (result, status, error) {
                        $messageBox.html('Sorry, an error occurred while scanning. Please contact administrator. Error received is \'' + error + '\'.');
                        $messageBox.addClass('noticeBox');
                        $messageBox.show();
                    },
                    complete: function () {                        
                        $displayArea.css('opacity', '1');
                        $('.progress').remove();
                        clearTimeout(barcodeTimeout);
                    }
                });

                $barcodeInput.val('');
            }
        }, 1500);
    });

    $('#barcodeInput').keydown(function() {
        var $messageBox = $('#messageBox');
        $messageBox.html('');
        $messageBox.removeClass('noticeBox');
        $messageBox.hide();
        clearTimeout(barcodeTimeout);
    });
    
});

var chosenBranch;
function checkBranch(trigger) {
    $('.actions').css({ 'display': 'none', 'visibility': 'hidden' });
    $('.discontinued').css({ 'display': 'none', 'visibility': 'hidden' });

    for (var i = 0; i < trigger.options.length; i++) {
        if (trigger.options[i].selected == true && trigger.options[i].value == '') {            
            $('#barcodeInput').prop('disabled', true);            
        }
        else {                
            $('#barcodeInput').prop('disabled', false);

            if (trigger.options[i].selected == true && trigger.options[i].value != chosenBranch) {
                chosenBranch = trigger.options[i].value;

                // clear list
                $('.item-table #items tbody tr').remove();
            }
        }
    }
}