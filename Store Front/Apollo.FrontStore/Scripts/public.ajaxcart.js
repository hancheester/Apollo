
var AjaxCart = {
	loadWaiting: false,
	usePopupNotifications: false,
	topCartQuantitySelector: '',
    topCartSubtotalSelector: '',
    flyoutCartSelector: '',
    miniBasketNotifySelector: '',
    quickModalSelector: '',

    init: function (usePopupNotifications, topCartQuantitySelector, topCartSubtotalSelector, flyoutCartSelector, miniBasketNotifySelector, quickModalSelector) {
		this.loadWaiting = false;
		this.usePopupNotifications = usePopupNotifications;
		this.topCartQuantitySelector = topCartQuantitySelector;
		this.topCartSubtotalSelector = topCartSubtotalSelector;
		this.flyoutCartSelector = flyoutCartSelector;
        this.miniBasketNotifySelector = miniBasketNotifySelector;
        this.quickModalSelector = quickModalSelector;
	},

	setLoadWaiting: function (display) {
		displayAjaxLoading(display); // the function is from public.common.js
		this.loadWaiting = display;
	},

	resetLoadWaiting: function () {
		AjaxCart.setLoadWaiting(false);
	},

	ajaxFailure: function () {
		alert('Failed to add the product. Please refresh the page and try one more time.');
	},

	// add a product to the cart from catalog pages (e.g. category, brands or search pages)
	addProductToCart_Catalog: function (urlAdd) {
		if (this.loadWaiting != false) {
			return;
		}
		this.setLoadWaiting(true);

		$.ajax({
			cache: false,
			url: urlAdd,
			type: 'post',
			success: this.success_process,
			complete: this.resetLoadWaiting,
			error: this.ajaxFailure
		});
	},

	// add a product to the cart from product details page
	addProductToCart_Details: function (urlAdd, formSelector) {
		if (this.loadWaiting != false) {
			return;
		}
		this.setLoadWaiting(true);

		$.ajax({
			cache: false,
			url: urlAdd,
			data: $(formSelector).serialize(),
			type: 'post',
			success: this.success_process,
			complete: this.resetLoadWaiting,
			error: this.ajaxFailure
		});
	},

    success_process: function (response) {

        if (response.hideQuickAddToCartSection == true) {
            $(AjaxCart.quickModalSelector).modal('hide');
        }

        if (response.updateQuickAddToCartSectionHtml) {
            $(AjaxCart.quickModalSelector + ' > .modal-dialog').html(response.updateQuickAddToCartSectionHtml);
            $(AjaxCart.quickModalSelector).modal('show');
            return false;
        }

		if (response.updateTopCartQuantitySectionHtml) {
		    $(AjaxCart.topCartQuantitySelector).html(response.updateTopCartQuantitySectionHtml);
		}

		if (response.updateTopCartSubtotalSectionHtml) {
		    $(AjaxCart.topCartSubtotalSelector).html(response.updateTopCartSubtotalSectionHtml);
		}

		if (response.updateFlyoutCartSectionHtml) {
			$(AjaxCart.flyoutCartSelector).replaceWith(response.updateFlyoutCartSectionHtml);
		}

		if (response.updateMiniBasketNotifySectionHtml) {
		    $(AjaxCart.miniBasketNotifySelector).html(response.updateMiniBasketNotifySectionHtml);
		}

		if (response.message) {
			if (response.success == true) {
                if (AjaxCart.usePopupNotifications == true ) {
                    displayPopupNotification(response.message, 'success');
                } else {
                    if (response.offerMessage) {
                        displayBarNotification(response.message, response.offerMessage, 'success', 0);
                    } else {
                        displayBarNotification(response.message, '', 'success', 3500);
                    }
				}
			} else {
				if (AjaxCart.usePopupNotifications == true) {
					displayPopupNotification(response.message, '', 'error');
				} else {
					displayBarNotification(response.message, '', 'error', 0);
				}
			}

			return false;
		}

		if (response.redirect) {
			location.href = response.redirect;
			return true;
		}

		return false;
	}
}