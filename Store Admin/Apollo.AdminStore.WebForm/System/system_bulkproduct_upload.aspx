<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.System.system_bulkproduct_upload" Codebehind="system_bulkproduct_upload.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
     <script type="text/javascript">
         $(document).ready(function () {
             var panel = $('#<%= hfCurrentPanel.ClientID %>').val();
             if (panel) {
                 $('.nav-tabs a[href=#' + panel + ']').tab('show');
             }
         });
     </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <asp:HiddenField ID="hfCurrentPanel" runat="server" />
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Bulk Product Upload</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />                
                <div class="tabs-container">
                    <div class="tabs-left">
                        <ul class="nav nav-tabs">
                            <li class="active"><a data-toggle="tab" href="#products">Import Products</a></li>
                            <li><a data-toggle="tab" href="#images">Import Images</a></li>
                        </ul>
                        <div class="tab-content">
                            <div id="products" class="tab-pane active">
                                <div class="panel-body">
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Please select a file <small class="text-navy clearfix">(<a href="/product-entry-template.xlsx">click here</a> to download product upload template)</small></th>
                                            <td><asp:FileUpload ID="productFileUpload" runat="server" CssClass="form-control" /></td>
                                        </tr>
                                    </table>
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12 row">
                                        <asp:LinkButton ID="lbFileUpload" runat="server" Text="Upload" OnClick="lbFileUpload_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                                    </div>
                                    <div class="col-lg-12"><br /></div>
                                    <Apollo:CustomGrid ID="gvBulkProducts" runat="server" CssClass="table table-striped table-bordered table-hover dataTable" 
                                        AutoGenerateColumns="False" CustomPageIndex="0" RecordCount="0" AllowSorting="true" >
                                        <Columns>
                                            <asp:TemplateField HeaderText="Product ID">
                                                <ItemTemplate>
                                                    <%# Eval("Item1") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Name">
                                                <ItemTemplate>
                                                    <%# Eval("Item2") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:HyperLink runat="server" 
                                                        NavigateUrl='<%# string.Format("/catalog/product_info.aspx?productid={0}", Eval("Item1")) %>'    
                                                        Text="Edit"></asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </Apollo:CustomGrid>
                                </div>
                            </div>
        
                            <div id="images" class="tab-pane">
                                <div class="panel-body">                                    
                                    <table id="uploadList" class="table table-striped">
                                        <tr>
                                            <td>Please select an image</td>
                                            <td> 
                                                <asp:FileUpload ID="imageFileupload" runat="server" CssClass="fileUpload form-control" />
                                                <asp:RequiredFieldValidator
                                                        runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Please choose a file.</span>"
                                                        ControlToValidate="imageFileupload"  
                                                        ErrorMessage="Please choose a file."  ValidationGroup ="imgvalidation">  
                                                </asp:RequiredFieldValidator>
                                                <%-- <asp:RegularExpressionValidator runat="server" ID="valUp" ControlToValidate="imageFileupload" 
                                                    ErrorMessage="File name must have only numbers, letters and hyphen, no space" 
                                                    ValidationExpression="((?=^.{1,}$)(?!.*\s)[0-9a-zA-Z!@#$%*()_+^&]).*(.jpg|.png|.gif)$"  ValidationGroup="imgvalidation"/>--%>
                                            </td>
                                        </tr>                                            
                                    </table>
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12">
                                        <a class="btn btn-sm btn-danger" onclick="AddMoreImages();">Add image</a>
                                        <asp:LinkButton ID="lbUploadImages" runat="server" Text="Upload" OnClick="lbUploadImages_Click" ValidationGroup="imgvalidation" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
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
        function AddMoreImages() {
            if (!document.getElementById && !document.createElement)
                return false;
            
            // Create a row
            var table = document.getElementById("uploadList");
            var row = table.insertRow(0);
            var cell1 = row.insertCell(0);
            var cell2 = row.insertCell(1);

            // Row title
            cell1.innerHTML = "Please select an image";

            var newFile = document.createElement("input");
            newFile.type = "file";
            newFile.setAttribute("class", "fileUpload form-control");
            if (!AddMoreImages.lastAssignedId)
                AddMoreImages.lastAssignedId = 100;
            newFile.setAttribute("id", "fileUploadarea" + AddMoreImages.lastAssignedId);
            newFile.setAttribute("name", "fileUploadarea" + AddMoreImages.lastAssignedId);

            // Add file input
            cell2.appendChild(newFile);

            AddMoreImages.lastAssignedId++;
        }
    </script>
</asp:Content>