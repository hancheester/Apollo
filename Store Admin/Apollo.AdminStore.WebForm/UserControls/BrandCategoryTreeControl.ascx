<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BrandCategoryTreeControl.ascx.cs" Inherits="Apollo.AdminStore.WebForm.UserControls.BrandCategoryTreeControl" %>
<asp:TreeView ID="tvBrandCategory" runat="server" ExpandDepth="1" PathSeparator="|"
    OnTreeNodePopulate="tvBrandCategory_TreeNodePopulate"
    OnSelectedNodeChanged="tvBrandCategory_SelectedNodeChanged" 
    OnTreeNodeCollapsed="tvBrandCategory_TreeNodeCollapsed"
    OnTreeNodeExpanded="tvBrandCategory_TreeNodeExpanded">
    <Nodes>
        <asp:TreeNode Text="Root" Value="-1" PopulateOnDemand="true" />
    </Nodes>
</asp:TreeView>