<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuControl.ascx.cs" Inherits="Apollo.AdminStore.WebForm.UserControls.MenuControl" %>
<nav class="navbar-default navbar-static-side hidden-print" role="navigation">
    <div class="sidebar-collapse">
        <ul class="nav metismenu" id="side-menu">
            <li class="nav-header">
                <div class="dropdown profile-element"> 
                    <span>
                        <img alt="image" class="img-circle" src="/img/profile_small.jpg" />
                    </span>
                    <a data-toggle="dropdown" class="dropdown-toggle" href="#">
                        <span class="clear"> 
                            <span class="block m-t-xs"><strong class="font-bold"><asp:Literal ID="ltProfileName" runat="server"></asp:Literal></strong><b class="caret"></b></span> 
                            <%--<span class="text-muted text-xs block"></span>--%>
                        </span>
                    </a>
                    <ul class="dropdown-menu animated fadeInRight m-t-xs">
                        <li><a href="#">Profile</a></li>
                        <li><a href="#">Contacts</a></li>
                        <li><a href="#">Mailbox</a></li>
                        <li class="divider"></li>
                        <li><a href="/logout.aspx">Logout</a></li>
                    </ul>
                </div>
                <div class="logo-element">EC</div>
            </li>            
            <li>                
                <%= GenerateMenuItem("<a href=\"/dashboard.aspx\"><i class=\"fa fa-th-large\"></i> <span class=\"nav-label\">Dashboards</span></a>", "Administrator,Despatch,Customer Service,Inventory,Report") %>
            </li>
            <li>
                <%= GenerateMenuItem("<a href=\"#\"><i class=\"fa fa-gift\"></i> <span class=\"nav-label\">Sales</span><span class=\"fa arrow\"></span></a>", "Administrator,Despatch,Customer Service") %>
                <%--<a href="#"><i class="fa fa-gift"></i> <span class="nav-label">Sales</span></a>--%>
                <ul class="nav nav-second-level collapse">
                    <li><a href="/sales/order_default.aspx">Orders</a></li>
                    <li>
                        <a href="#">Fulfillment <span class="fa arrow"></span></a>
                        <ul class="nav nav-third-level collapse">
                            <li><a href="/fulfillment/order_fulfillment.aspx">Ship Orders</a></li>
                            <li><a href="/fulfillment/order_line_items.aspx">Ordered Line Items</a></li>
                            <li><a href="/fulfillment/order_line_item_distribution_async.aspx">Line Item Dist.</a></li>
                            <li><a href="/fulfillment/order_line_item_pick_in_progress.aspx">Pick In Progress</a></li>
                            <li><a href="/fulfillment/order_line_item_distribution_management_async.aspx">Manage Line Dist.</a></li>
                            <li><a href="/fulfillment/order_line_item_stock_admission.aspx">Stock Admission</a></li>
                            <li><a href="/fulfillment/order_line_item_status.aspx">Line Item Status</a></li>
                            <li>
                                <a href="#">Branches <span class="fa arrow"></span></a>
                                <ul class="nav nav-fourth-level collapse">
                                    <li><a href="/order_line_item_branch_pending_default.aspx?id=4">Warehouse</a></li>
                                    <li><a href="/order_line_item_branch_pending_default.aspx?id=1">Petter</a></li>
                                    <li><a href="/order_line_item_branch_pending_default.aspx?id=3">Pharmacia</a></li>
                                    <li><a href="/order_line_item_branch_pending_default.aspx?id=5">Acorn</a></li>
                                </ul>
                            </li>
                        </ul>
                    </li>
                    <%= GenerateMenuItem("<li><a href=\"/sales/cancellation_default.aspx\">Cancellation</a></li><li><a href=\"/sales/refund_default.aspx\">Refunds</a></li>", "Administrator,Customer Service") %>
                </ul>
            </li>
            <li>
                <%= GenerateMenuItem("<a href=\"#\"><i class=\"fa fa-rocket\"></i> <span class=\"nav-label\">Catalog</span><span class=\"fa arrow\"></span></a>", "Administrator,Inventory,Customer Service") %>
                <%--<a href="#"><i class="fa fa-rocket"></i> <span class="nav-label">Catalog</span><span class="fa arrow"></span></a>--%>
                <ul class="nav nav-second-level collapse">
                    <li><a href="/catalog/product_default.aspx">Products</a></li>
                    <li><a href="/catalog/product_price_default.aspx">Product Price</a></li>
                    <li><a href="/catalog/category_default.aspx">Categories</a></li>
                    <li><a href="/catalog/brand_default.aspx">Brands</a></li>
                    <li><a href="#">Search <span class="fa arrow"></span></a>
                        <ul class="nav nav-third-level collapse">
                            <li><a href="/catalog/search_term_default.aspx">Search Terms</a></li>
                            <li><a href="/catalog/custom_dict_default.aspx">Custom Dictionary</a></li>
                            <li><a href="/catalog/test_search_result.aspx">Test Search Result</a></li>
                        </ul>
                    </li>                            
                    <li><a href="#">Product Reviews <span class="fa arrow"></span></a>
                        <ul class="nav nav-third-level collapse">
                            <li><a href="/catalog/reviews_pending.aspx">Pending Reviews</a></li>
                            <li><a href="/catalog/reviews_all.aspx">All Reviews</a></li>
                        </ul>
                    </li>
                </ul>
            </li>
            <li>
                <%= GenerateMenuItem("<a href=\"/customer/customer_default.aspx\"><i class=\"fa fa-users\"></i> <span class=\"nav-label\">Accounts </span></a>", "Administrator,Customer Service") %>                    
            </li>
            <li>
                <%= GenerateMenuItem("<a href=\"#\"><i class=\"fa fa-lightbulb-o\"></i> <span class=\"nav-label\">Promotions</span><span class=\"fa arrow\"></span></a>", "Administrator,Marketing") %>
                <%--<a href="#"><i class="fa fa-lightbulb-o"></i> <span class="nav-label">Promotions</span><span class="fa arrow"></span></a>--%>
                <ul class="nav nav-second-level collapse">
                    <li><a href="/marketing/promo_catalog_offer_default.aspx">Catalog Offer Rules</a></li>
                    <li><a href="/marketing/promo_cart_offer_default.aspx">Cart Offer Rules</a></li>
                    <li><a href="/marketing/promo_google_shopping_custom_labels_default.aspx">Google Shopping Custom Labels</a></li>
                </ul>
            </li>
            <li>
                <%= GenerateMenuItem("<a href=\"#\"><i class=\"fa fa-file-image-o\"></i> <span class=\"nav-label\">Content Management</span><span class=\"fa arrow\"></span></a>", "Administrator,Marketing") %>
                <%--<a href="#"><i class="fa fa-file-image-o"></i> <span class="nav-label">CMS</span><span class="fa arrow"></span></a>--%>
                <ul class="nav nav-second-level collapse">
                    <li>
                        <a href="#">Banners <span class="fa arrow"></span></a>
                        <ul class="nav nav-third-level collapse">
                            <li><a href="/marketing/cms_largebanner_default.aspx">Large Banners</a></li>
                            <li><a href="/marketing/cms_offerbanner_default.aspx">Offer Banners</a></li>
                        </ul>
                    </li>
                    <li><a href="/marketing/cms_featureditem_default.aspx">Featured Items</a></li>   
                    <li><a href="/marketing/cms_testimonials_default.aspx">Testimonials</a></li>    
                    <li>
                        <a href="#">Blog <span class="fa arrow"></span></a>
                        <ul class="nav nav-third-level collapse">
                            <li><a href="/marketing/cms_blog_post_default.aspx">Blog Posts</a></li>
                            <li><a href="/marketing/cms_blog_comment_default.aspx">Blog Comments</a></li>
                        </ul>
                    </li>
                </ul>
            </li>
            <li>
                <%= GenerateMenuItem("<a href=\"#\"><i class=\"fa fa-newspaper-o\"></i> <span class=\"nav-label\">Newsletter</span><span class=\"fa arrow\"></span></a>", "Administrator,Marketing") %>
                <%--<a href="#"><i class="fa fa-newspaper-o"></i> <span class="nav-label">Newsletter</span><span class="fa arrow"></span></a>--%>
                <ul class="nav nav-second-level collapse">
                    <li><a href="/marketing/newsletter_subscribers.aspx">Subscribers</a></li>
                </ul>
            </li>
            <li>
                <%= GenerateMenuItem("<a href=\"#\"><i class=\"fa fa-bar-chart\"></i> <span class=\"nav-label\">Reports</span><span class=\"fa arrow\"></span></a>", "Administrator,Report,Inventory,Marketing") %>                
                <ul class="nav nav-second-level collapse">
                    <%= GenerateMenuItem("<li><a href=\"/report/report_orders.aspx\">Orders</a></li>", "Administrator,Report") %>
                    <%= GenerateMenuItem("<li><a href=\"/report/report_transaction_worldwide.aspx\">Transaction Worldwide</a></li>", "Administrator,Report") %>                    
                    <%= GenerateMenuItem("<li><a href=\"/report/report_pending_line_inventory.aspx\">Pending Line Inventory</a></li>", "Administrator,Report,Inventory") %>
                    <%= GenerateMenuItem("<li><a href=\"/report/report_product_analysis.aspx\">Product Analysis</a></li>", "Administrator,Report,Marketing") %>
                    <%= GenerateMenuItem("<li><a href=\"#\">Registrations</a></li>", "Administrator,Report,Marketing") %>
                    <%= GenerateMenuItem("<li><a href=\"#\">Product Affinity</a></li>", "Administrator,Report,Marketing") %>
                    <%= GenerateMenuItem("<li><a href=\"#\">New vs Returning Orders</a></li>", "Administrator,Report") %>
                    <%= GenerateMenuItem("<li><a href=\"#\">Quarterly Sales</a></li>", "Administrator,Report") %>                    
                </ul>
            </li>
            <li>
                <%= GenerateMenuItem("<a href=\"#\"><i class=\"fa fa-cogs\"></i> <span class=\"nav-label\">Configuration</span><span class=\"fa arrow\"></span></a>", "Administrator") %>
                <%--<a href="#"><i class="fa fa-cogs"></i> <span class="nav-label">Configuration</span><span class="fa arrow"></span></a>--%>
                <ul class="nav nav-second-level collapse">
                    <li>
                        <a href="#">Settings <span class="fa arrow"></span></a>
                        <ul class="nav nav-third-level collapse">
                            <li><a href="/system/system_settings_general.aspx">General</a></li>
                            <li><a href="/system/system_settings_store.aspx">Store</a></li>
                            <li><a href="/system/system_settings_customer.aspx">Customer</a></li>
                            <li><a href="/system/system_settings_shipping.aspx">Shipping</a></li>
                            <li><a href="/system/system_settings_tax.aspx">Tax</a></li>
                            <li><a href="/system/system_settings_catalog.aspx">Catalog</a></li>
                            <li><a href="/system/system_settings_shoppingcart.aspx">Shopping Cart</a></li>
                            <li><a href="/system/system_settings_media.aspx">Media</a></li>
                            <li><a href="/system/system_settings_currency.aspx">Currency</a></li>
                            <li><a href="/system/system_settings_email.aspx">Email</a></li>
                            <li><a href="/system/system_settings_seo.aspx">SEO</a></li>
                            <li><a href="/system/system_settings_blog.aspx">Blog</a></li>
                        </ul>
                    </li>
                    <li>
                        <a href="#">Widgets <span class="fa arrow"></span></a>
                        <ul class="nav nav-third-level collapse">
                            <li><a href="/system/system_widgets_google_analytics_default.aspx">Google Analytics</a></li>
                            <li><a href="#">Google Adwords</a></li>
                            <li><a href="#">Affiliate Windows</a></li>
                        </ul>
                    </li>
                    <li>
                        <a href="#">Plugins <span class="fa arrow"></span></a>
                        <ul class="nav nav-third-level collapse">
                            <li><a href="/system/system_plugins_sagepay_default.aspx">SagePay</a></li>
                        </ul>
                    </li>
                    <li><a href="/system/system_cache_default.aspx">Cache</a></li>
                    <li><a href="/system/system_security_default.aspx">Security</a></li>
                    <li><a href="/system/system_currency_default.aspx">Currency</a></li>
                    <li><a href="/system/system_country_default.aspx">Country</a></li>
                    <li><a href="/system/system_shipping_default.aspx">Shipping Options</a></li>
                    <li><a href="/system/system_bulkproduct_upload.aspx">Bulk Product Upload</a></li>
                    <li><a href="/system/system_sitemap_default.aspx">Sitemap</a></li>
                    <li><a href="/system/system_google_taxonomy_upload.aspx">Google Taxonomy Upload</a></li>
                    <li><a href="/system/system_nhs_default.aspx">NHS</a></li>
                    <li><a href="/system/system_vector_default.aspx">Vector</a></li> 
                    <li><a href="/system/system_nhunspell_default.aspx">NHunspell</a></li>
                    <li><a href="/system/system_awin_default.aspx">Awin</a></li>
                </ul>
            </li>
        </ul>
    </div>
</nav>