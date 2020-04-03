(function () {
    $('#offer-types').on('hidden.bs.collapse', function (e) {
        hiddenCollapse('offer-types', e);
    });
    $('#offer-types').on('shown.bs.collapse', function (e) {
        shownCollapse('offer-types', e);
    });
    $('#category-tree').on('hidden.bs.collapse', function (e) {        
        hiddenCollapse('category-tree', e);
    });
    $('#category-tree').on('shown.bs.collapse', function (e) {        
        shownCollapse('category-tree', e);
    });
    $('#category-tree-mobile').on('hidden.bs.collapse', function (e) {
        hiddenCollapse('category-tree-mobile', e);
    });
    $('#category-tree-mobile').on('shown.bs.collapse', function (e) {
        shownCollapse('category-tree-mobile', e);
    });
    $('#brand-tree').on('hidden.bs.collapse', function (e) {
        hiddenCollapse('brand-tree', e);
    });
    $('#brand-tree').on('shown.bs.collapse', function (e) {
        shownCollapse('brand-tree', e);
    });
    $('#brand-tree-mobile').on('hidden.bs.collapse', function (e) {
        hiddenCollapse('brand-tree-mobile', e);
    });
    $('#brand-tree-mobile').on('shown.bs.collapse', function (e) {
        shownCollapse('brand-tree-mobile', e);
    });
    $('#offer-tree').on('hidden.bs.collapse', function (e) {
        hiddenCollapse('offer-tree', e);
    });
    $('#offer-tree').on('shown.bs.collapse', function (e) {
        shownCollapse('offer-tree', e);
    });
    $('#mobile-filters').on('hidden.bs.collapse', function (e) {
        hiddenCollapse('mobile-filters', e);
    });
    $('#mobile-filters').on('shown.bs.collapse', function (e) {
        shownCollapse('mobile-filters', e);
    });
    $('#refine-panel').on('shown.bs.collapse', function () {
        $('.filter-refine').text('hide');
    });
    $('#refine-panel').on('hidden.bs.collapse', function () {
        $('.filter-refine').text('refine');
    });
    $('#quick-modal').on('show.bs.modal', alignModal('#quick-modal .modal-dialog', 500));

    // Align modal when user resize the window
    $(window).on('resize', function () {
        $('#quick-modal:visible').each(alignModal('#quick-modal .modal-dialog', 500));
    });  

    var offset = 220;
    var duration = 500;
    $(window).scroll(function () {
        if ($(this).scrollTop() > offset) {
            $('.back-to-top').fadeIn(duration);
        } else {
            $('.back-to-top').fadeOut(duration);
        }
    });

    $('.back-to-top').click(function (event) {
        event.preventDefault();
        $('html, body').animate({ scrollTop: 0 }, duration);
        return false;
    });

    $('.mob-customer-reviews').click(function (event) {        
        event.preventDefault();
        $('html, body').animate({ scrollTop: ($('#customer-reviews').offset().top - 60) }, duration);
        return false;
    });

    $('.customer-reviews').click(function (event) {
        event.preventDefault();
        $('html, body').animate({ scrollTop: $('#reviews').offset().top }, duration);
        return false;
    });

    function alignModal(selector, height) {
        var modalDialog = $(selector);
        /* Applying the top margin on modal dialog to align it vertically center */
        modalDialog.css('margin-top', Math.max(0, ($(window).height() - height) / 2));
    }
})();

function setVendorStyle(element, prop, value) {
    var vendor_prefix = ['-moz-', '-webkit-', '-ms-'];
    element.style[prop] = value;
    if (element.style[prop] === value) return value;
    for (var i = 0; i < vendor_prefix.length; i++) {
        var vendor_value = vendor_prefix[i] + value;
        element.style[prop] = vendor_value;
        if (element.style[prop] == vendor_value) return vendor_value;
    }
    return false;
}

function hiddenCollapse(id, e) {    
    $('#' + id + ' > .panel > .panel-heading > .panel-title a.collapsed span').removeClass('glyphicon-minus').addClass('glyphicon-plus');
    e.stopPropagation();
}

function shownCollapse(id, e) {    
    $('#' + id + ' > .panel > .panel-heading > .panel-title a:not(.collapsed) span').removeClass('glyphicon-plus').addClass('glyphicon-minus');
    e.stopPropagation();
}

window.addEventListener('load', function () {
    var topMenuItems = document.querySelectorAll('#nav-header .menu-top > li > a');
    
    if (topMenuItems.length > 0) {
        for(var i = 0; i < topMenuItems.length; i++) {
            topMenuItems[i].addEventListener('touchstart', function (e) {
                if (hasClass(this, 'hover')) {
                    if (this.tagName === 'A') {
                        window.location = this.href;
                    }
                } else {
                    var items = document.querySelectorAll('#nav-header .menu-top > li > a');

                    for (var i = 0; i < items.length; i++) {
                        if (hasClass(items[i], 'hover')) {
                            items[i].classList.remove('hover');
                        }
                    }

                    this.classList.add('hover');
                }

                e.preventDefault();
            }, false);
        }
    }

    var screenWidth = screen.width;

    var productBoxes = document.querySelectorAll('.product-box');

    // Except for small devices - Phones (<768px)
    if (productBoxes.length > 0 && screenWidth >= 768) {
        for(var i = 0; i < productBoxes.length; i++) {
            productBoxes[i].addEventListener('touchstart', function (e) {                
                if (hasClass(this, 'hover')) {
                    var productLinks = $(this).find('a[href*="/product/"]');
                    if (productLinks.length > 0) {                        
                        window.location = productLinks[0].href;
                    }
                } else {
                    this.classList.add('hover');
                }
                
                e.preventDefault();
            }, false);
        }
    }

    var productRows = document.querySelectorAll('.product-row');

    // Except for small devices - Phones (<768px)
    if (productRows.length > 0 && screenWidth >= 768) {
        for (var i = 0; i < productRows.length; i++) {
            productRows[i].addEventListener('touchstart', function (e) {
                if (hasClass(this, 'hover')) {
                    var productLinks = $(this).find('a[href*="/product/"]');
                    if (productLinks.length > 0) {
                        window.location = productLinks[0].href;
                    }
                } else {
                    this.classList.add('hover');
                }

                e.preventDefault();
            }, false);
        }
    }

    var menuMiddleItems = document.querySelectorAll('#nav-header .menu-top .menu-middle > li > a');

    if (menuMiddleItems.length > 0) {
        for (var i = 0; i < menuMiddleItems.length; i++) {
            menuMiddleItems[i].addEventListener('touchstart', function (e) {
                if (hasClass(this, 'hover')) {
                    if (this.tagName === 'A') {
                        window.location = this.href;
                    }
                } else {
                    var items = document.querySelectorAll('#nav-header .menu-top .menu-middle > li > a');

                    for (var i = 0; i < items.length; i++) {
                        if (hasClass(items[i], 'hover')) {
                            items[i].classList.remove('hover');
                        }
                    }

                    this.classList.add('hover');
                }

                e.preventDefault();
            }, false);
        }
    }

});

var buyNowStuck = false;

function relocateStickyBuyButton() {
    var windowWidth = $(window).width();
    var windowTop = $(window).scrollTop();
    var addToCartAnchor = $('.add-to-cart-anchor');
    
    if (addToCartAnchor.length) {
        var elementTop = addToCartAnchor.offset().top;

        if (windowWidth < 768) {
            if (windowTop > elementTop + 60) {
                if (buyNowStuck == false) {
                    $('.buy-now').fadeOut(10, function () {
                        $(this).addClass('sticky').fadeIn(700);
                        $('#copyright').addClass('extended');
                        buyNowStuck = true;
                    })
                }
            } else {
                if (buyNowStuck == true) {
                    $('.buy-now').hide().removeClass('sticky').fadeIn(700);
                    $('#copyright').removeClass('extended');
                    buyNowStuck = false;
                }
            }
        }
    }
}

function displayAjaxLoading(display) {
    if (display) {
        $('.ajax-loading-block-window').show();
    } else {
        $('.ajax-loading-block-window').hide('slow');
    }
}

function reset() {
    window.location = getPathFromUrl(window.location.href);
}

function hasClass(element, cssClass) {
    return (' ' + element.className + ' ').indexOf(' ' + cssClass + ' ') > -1;
}

function getPathFromUrl(url) {
    return url.split(/[?#]/)[0];
}

function updateQueryString(key, value, url) {
    if (!url) url = window.location.href;
    var re = new RegExp("([?&])" + key + "=.*?(&|#|$)(.*)", "gi"),
        hash;

    if (re.test(url)) {
        if (typeof value !== 'undefined' && value !== null)
            return url.replace(re, '$1' + key + "=" + value + '$2$3');
        else {
            hash = url.split('#');
            url = hash[0].replace(re, '$1$3').replace(/(&|\?)$/, '');
            if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                url += '#' + hash[1];
            return url;
        }
    }
    else {
        if (typeof value !== 'undefined' && value !== null) {
            var separator = url.indexOf('?') !== -1 ? '&' : '?';
            hash = url.split('#');
            url = hash[0] + separator + key + '=' + value;
            if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                url += '#' + hash[1];
            return url;
        }
        else
            return url;
    }
}

function displayPopupNotification(message, type) {
    // types: success, error, news
    var container;

    switch(type)
    {
        case 'error':
            container = $('#dialog-notification-error');
            break;
        case 'success':
        default:
            container = $('#dialog-notification-success');
            break;
    }

    var htmlCode = '';

    if ((typeof message) === 'string') {
        htmlCode = '<p>' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlCode = htmlCode + '<p>' + message[i] + '</p>';
        }
    }
    
    container.html(htmlCode);

    container.modal('show');
}

var barNotificationTimeout;
function displayBarNotification(message, offerMesage, type, timeout) {
    // types: success, error, news
    clearTimeout(barNotificationTimeout);

    var cssClass = 'success';
    switch (type) {
        case 'error':
            cssClass = 'error';
            break;
        case 'success':
        default:
            cssClass = 'success';
            break;
    }

    // remove previous CSS classes and notifications
    $('#bar-notification')
        .removeClass('success')
        .removeClass('error');
    $('#bar-notification .content').remove();

    // add new notifications
    var htmlCode = '';
    if ((typeof message) === 'string') {
        htmlCode = '<p class="content">' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlCode = htmlCode + '<p class="content">' + message[i] + '</p>';
        }
    }

    if ((typeof offerMesage) === 'string') {
        htmlCode = htmlCode + '<p class="content offer">' + offerMesage + '</p>';
    } else {
        for (var i = 0; i < offerMesage.length; i++) {
            htmlCode = htmlCode + '<p class="content offer">' + offerMesage[i] + '</p>';
        }
    }

    $('#bar-notification').append(htmlCode)
        .addClass(cssClass)
        .fadeIn('slow')
        .mouseenter(function () {
            clearTimeout(barNotificationTimeout);
        });

    $('#bar-notification .close').unbind('click').click(function () {
        $('#bar-notification').fadeOut('slow');
    });

    //timeout (if set)
    if (timeout > 0) {
        barNotificationTimeout = setTimeout(function () {
            $('#bar-notification').fadeOut('slow');
        }, timeout);
    }
}

function submitForm(path, params, method) {
    method = method || 'post'; // Set method to post by default if not specified.
    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement('form');
    form.setAttribute('method', method);
    form.setAttribute('action', path);
    form.setAttribute('id', 'unique');

    for (var key in params) {
        if (params.hasOwnProperty(key)) {
            var hiddenField = document.createElement('input');
            hiddenField.setAttribute('type', 'hidden');
            hiddenField.setAttribute('name', key);
            hiddenField.setAttribute('value', params[key]);

            form.appendChild(hiddenField);
        }
    }

    document.body.appendChild(form);
    
    form.submit();
}

function showQuickModal(url, method) {    
    displayAjaxLoading(true);
    method = method || 'post'; // Set method to post by default if not specified.

    $.ajax({
        cache: false,
        url: url,
        type: method,
        success: function (response) {

            if (response.success == true) {
                if (response.resultSectionHtml) {
                    $('#quick-modal > .modal-dialog').html(response.resultSectionHtml);
                    $('#quick-modal').modal('show');
                } else {
                    displayBarNotification(response.message, 'error', 0);
                }
            } else {
                displayBarNotification(response.message, 'error', 0);
            }
            
            return false;
        },
        complete: function () {
            displayAjaxLoading(false);
        },
        error: function () {
            alert('Failed to display information. Please refresh the page and try one more time.');
        }
    });
}

function removeVisitedItem(target, url, productId) {
    $.ajax({
        cache: false,
        url: url,
        data: {productId: productId},
        type: 'post',
        complete: target.remove(),
    });    
}