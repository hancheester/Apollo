function load_condition(nodeId, value, typeValue) {
    console.log('nodeId: ' + nodeId);
    console.log('value: ' + value);
    console.log('typeValue: ' + typeValue);

    var ruleId = $('.rule-root').data('ruleid');

    if (value != 'default') {
        var id = format_id(nodeId);
        console.log('id: ' + id);

        $(id + ' select').get(0).selectedIndex = 0;
        $(id + ' .more').hide();
        $(id + ' select').hide();
        $(id).append('<div class=\'loading\'><\div>');

        $.post('/marketing/promo_offer_handler.aspx',
               { elementId: nodeId, data: value, action: 'load_condition', type: typeValue, treeId: ruleId },
               function (data) {
                   $('.loading').remove();
                   console.log('data: ' + data);
                   if (data.match('^ok')) {
                       data = data.replace('ok|', '')
                       $(data).insertBefore(id);
                   }
                   else {
                       alert('Failed to load! ' + data);
                   }
                   $(id + ' .more').show();
                   $(id + ' select').hide();
               });
    }
}

function remove_condition(nodeId, typeValue) {
    var id = format_id(nodeId);
    var ruleId = $('.rule-root').data('ruleid');
    $.post("/marketing/promo_offer_handler.aspx",
           { elementId: nodeId, data: '', action: 'delete_condition', type: typeValue, treeId: ruleId },
           function (data) {
               console.log('data: ' + data);
               if (data != 'ok')
                   alert('Failed to remove! ' + data);
               else
                   $(id).remove();
           });
}

function update_operator(nodeId, value, typeValue) {
    var ruleId = $('.rule-root').data('ruleid');

    $.post('/marketing/promo_offer_handler.aspx',
           { elementId: nodeId, data: value, action: 'update_operator', type: typeValue, treeId: ruleId },
           function (data) {
               console.log('data: ' + data);
               if (data != 'ok')
                   alert('Failed to save! ' + data);
           });
}

function update_operand(nodeId, operandId, typeValue, caller) {
    var operandId = format_id(operandId);
    var ruleId = $('.rule-root').data('ruleid');
    $.post('/marketing/promo_offer_handler.aspx',
           { elementId: nodeId, data: $(operandId).val(), action: 'update_operand', type: typeValue, treeId: ruleId },
           function (data) {
               console.log('data: ' + data);
               if (data == 'ok') {
                   alert('Saved successfully!');
                   $(format_id(caller)).hide();
               }
               else
                   alert('Failed to save! ' + data);
           });
}

// update all/any
function update_cond1(nodeId, value, typeValue) {
    var ruleId = $('.rule-root').data('ruleid');
    $.post('/marketing/promo_offer_handler.aspx',
           { elementId: nodeId, data: value, action: 'update_cond1', type: typeValue, treeId: ruleId },
           function (data) {
               console.log('data: ' + data);
               if (data != 'ok')
                   alert('Failed to update! ' + data);
           });
}

// update matched
function update_cond2(nodeId, value, typeValue) {
    var ruleId = $('.rule-root').data('ruleid');
    $.post('/marketing/promo_offer_handler.aspx',
           { elementId: nodeId, data: value, action: 'update_cond2', type: typeValue, treeId: ruleId },
           function (data) {
               console.log('data: ' + data);
               if (data != 'ok')
                   alert('Failed to update! ' + data);
           });
}

// update item found
function update_cond3(nodeId, value) {
    var ruleId = $('.rule-root').data('ruleid');
    $.post('/marketing/promo_offer_handler.aspx',
           { elementId: nodeId, data: value, action: 'update_cond3', type: typeValue, treeId: ruleId },
           function (data) {
               console.log('data: ' + data);
               if (data != 'ok')
                   alert('Failed to update! ' + data);
           });
}

// update qty/amount
function update_cond4(nodeId, value, typeValue) {
    var ruleId = $('.rule-root').data('ruleid');
    $.post('/marketing/promo_offer_handler.aspx',
           { elementId: nodeId, data: value, action: 'update_cond4', type: typeValue, treeId: ruleId },
           function (data) {
               console.log('data: ' + data);
               if (data != 'ok')
                   alert('Failed to update! ' + data);
           });
}

function format_id(rawId) {
    return '#' + rawId.replace(/(:|\||\,)/g, '\\$1');
}

function toggle_selection(nodeId) {
    console.log('nodeId: ' + nodeId);
    var id = format_id(nodeId);
    console.log('id: ' + id);

    if ($(id + ' select').is(':visible')) {
        $(id + ' .more').show();
        $(id + ' select').hide();
    }
    else {
        $(id + ' select').show();
        $(id + ' .more').hide();
    }
}

function monitor_value(id, original, current) {
    console.log('original: ' + original);
    console.log('current: ' + current);
    var id = format_id(id);
    if (original != current) {
        if (!$(id).is(":visible"))
            $(id).show();
    }
    else {
        $(id).hide();
    }
}

function resetFreeItemOffer() {
    $('.free-itself input[type=radio]').attr('checked', false);
    $('.free-item input[type=radio]').attr('checked', false);
}