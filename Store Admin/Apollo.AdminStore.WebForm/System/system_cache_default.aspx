<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_cache_default.aspx.cs" Inherits="Apollo.AdminStore.WebForm.System.system_cache_default" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Settings</h2>
            <h3>Cache</h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <table class="table table-striped">
                    <tr>
                        <th>Store front refresh cache link</th>
                        <td><asp:TextBox ID="txtStoreFrontRefreshCacheLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Store front get performance data link</th>
                        <td><asp:TextBox ID="txtStoreFrontGetPerfDataLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Store front get cache keys link</th>
                        <td><asp:TextBox ID="txtStoreFrontGetCacheKeysLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Store front cache token</th>
                        <td><asp:TextBox ID="txtStoreFrontToken" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                 </table>
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12 row">
                    <asp:LinkButton ID="lbPublish" runat="server" Text="Publish" OnClick="lbPublish_Click" CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('This action will refresh all setting related data on store front and performance could be affected.\nAre you sure to publish?');"></asp:LinkButton>
                    <asp:LinkButton ID="lbUpdate" runat="server" Text="Update" OnClick="lbUpdate_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">            
            <h2>Report</h2>
            <h3>Performance</h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <div class="tabs-container">
                    <div class="tabs-left">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#store-memory" data-toggle="tab">Store Front</a></li>
                            <li><a href="#admin-memory" data-toggle="tab">Store Admin</a></li>
                            <li><a href="#service-memory" data-toggle="tab">Web Service</a></li>
                        </ul>
                        <div class="tab-content">
                            <div id="store-memory" class="tab-pane active">
                                <div class="panel-body">
                                    <div class="col-lg-3">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Memory Cache
                                            </div>
                                            <p></p>
                                            <div class="col-lg-12">
                                                <button type="button" class="btn btn-sm btn-primary" onclick="loadStore();">Retrieve</button>
                                            </div>
                                            <div class="clearfix"></div>
                                            <p></p>
                                            <table class="table" id="store-memory-table"></table>
                                        </div>
                                    </div>
                                    <div class="col-lg-9">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Memory Cache Keys
                                            </div>
                                            <p></p>
                                            <div class="col-lg-12">
                                                <button type="button" class="btn btn-sm btn-primary" onclick="loadStoreKeys();">Retrieve</button>
                                            </div>
                                            <div class="clearfix"></div>
                                            <p></p>
                                            <table class="table" id="store-memory-keys-table"></table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="admin-memory" class="tab-pane">
                                <div class="panel-body">                                    
                                    <div class="col-lg-3">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Memory Cache
                                            </div>
                                            <p></p>
                                            <div class="col-lg-12">
                                                <button type="button" class="btn btn-sm btn-primary" onclick="loadAdmin();">Retrieve</button>
                                            </div>
                                            <div class="clearfix"></div>
                                            <p></p>
                                            <table class="table" id="admin-memory-table"></table>
                                        </div>
                                    </div>

                                    <div class="col-lg-9">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Memory Cache Keys
                                            </div>
                                            <p></p>
                                            <div class="col-lg-12">
                                                <button type="button" class="btn btn-sm btn-primary" onclick="loadAdminKeys();">Retrieve</button>
                                            </div>
                                            <div class="clearfix"></div>
                                            <p></p>
                                            <table class="table" id="admin-memory-keys-table"></table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="service-memory" class="tab-pane">
                                <div class="panel-body">
                                    <div class="col-lg-3">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Memory Cache
                                            </div>
                                            <p></p>
                                            <div class="col-lg-12">
                                                <button type="button" class="btn btn-sm btn-primary" onclick="loadService();">Retrieve</button>
                                            </div>
                                            <div class="clearfix"></div>
                                            <p></p>
                                            <table class="table" id="service-memory-table"></table>
                                        </div>
                                    </div>

                                    <div class="col-lg-9">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Memory Cache Keys
                                            </div>
                                            <p></p>
                                            <div class="col-lg-12">
                                                <button type="button" class="btn btn-sm btn-primary" onclick="loadServiceKeys();">Retrieve</button>
                                            </div>
                                            <div class="clearfix"></div>
                                            <p></p>
                                            <table class="table" id="service-memory-keys-table"></table>
                                        </div>
                                    </div>

                                    <div class="clearfix"></div>

                                    <div class="col-lg-4">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Dache Cache
                                            </div>
                                            <p></p>
                                            <div class="col-lg-12">
                                                <button type="button" class="btn btn-sm btn-primary" onclick="loadServiceDache();">Retrieve</button>
                                            </div>
                                            <div class="clearfix"></div>
                                            <p></p>
                                            <table class="table" id="service-dache-table"></table>
                                        </div>
                                    </div>

                                    <div class="col-lg-8">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Dache Cache Keys
                                            </div>
                                            <p></p>
                                            <div class="col-lg-12">
                                                <button type="button" class="btn btn-sm btn-primary" onclick="loadServiceDacheKeys();">Retrieve</button>
                                            </div>
                                            <div class="clearfix"></div>
                                            <p></p>
                                            <table class="table" id="service-dache-keys-table"></table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
        </div>
    </div>

    <script type="text/javascript">
        function loadAdmin() {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "'admin_memory_cache'", "loadingData", "'admin-memory-table'") %>;
        }

        function loadAdminKeys() {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "'admin_memory_keys'", "loadingData", "'admin-memory-keys-table'") %>;
        }

        function loadStore() {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "'store_memory_cache'", "loadingData", "'store-memory-table'") %>;
        }

        function loadStoreKeys() {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "'store_memory_keys'", "loadingData", "'store-memory-keys-table'") %>;
        }

        function loadService() {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "'service_memory_cache'", "loadingData", "'service-memory-table'") %>;
        }

        function loadServiceDache() {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "'service_dache_cache'", "loadingData", "'service-dache-table'") %>;
        }

        function loadServiceDacheKeys() {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "'service_dache_keys'", "loadingData", "'service-dache-keys-table'") %>;
        }

        function loadServiceKeys() {
            <%= this.Page.ClientScript.GetCallbackEventReference(this, "'service_memory_keys'", "loadingData", "'service-memory-keys-table'") %>;
        }

        function loadingData(msg, context) {
            $('#' + context).html(msg);
        }

    </script>
</asp:Content>