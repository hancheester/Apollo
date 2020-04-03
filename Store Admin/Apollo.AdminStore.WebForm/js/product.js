$(document).ready(function () {
    //$('#slider-range').slider({
    //    range: 'max',
    //    min: 0,
    //    max: 100,
    //    slide: function (event, ui) {
    //        $('#ApolloRating span').html(ui.value);
    //        $('#ApolloRating input[type=text]').val(ui.value);
    //    }
    //});

    //var ApolloRating = $('#ApolloRating input[type=text]').val();
    //$('#slider-range').slider('value', ApolloRating);
    //$('#ApolloRating span').html(ApolloRating);

    //$('#customerRating span').html($('#customerRating input[type=text]').val());
    //$('#popularity span').html($('#popularity input[type=text]').val());

});

function set_primary_image(current) {
    $('.image_box input[type=radio]').attr('checked', false);
    $(current).attr('checked', true);
}

function choose_image(value, context) {
    $('.image_box_' + context + ' label').html(value);
}

function check_actions(trigger) {
    $('.actions').hide();
    $('.discontinued').hide();
    var action = $(trigger).val();
    switch (action) {
        case 'changestatus':
            $('.actions').show();
            break;
        case 'changediscontinued':
            $('.discontinued').show();
            break;
        default:
            break;
    }
}

function load_stock_level(msg, context) {
    $('#' + context).html(msg);
}

function show_processing(id) {
    $('#' + id).html('<i class="fa fa-spinner fa-spin"></i>');
    $('#tagContent').css('visibility', 'hidden');
}

function minmax(value, min, max) {
    if (parseInt(value) < min || isNaN(value))
        return 0;
    else if (parseInt(value) > max)
        return 100;
    else return value;
}