<%@ Control Language="C#" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_CategoryTreeControl" Codebehind="CategoryTreeControl.ascx.cs" %>
<asp:TreeView ID="tvCategory" runat="server" ExpandDepth="1" PathSeparator="|" CssClass="tree"
    OnTreeNodePopulate="tvCategory_TreeNodePopulate"
    OnSelectedNodeChanged="tvCategory_SelectedNodeChanged" 
    OnTreeNodeCollapsed="tvCategory_TreeNodeCollapsed"
    OnTreeNodeExpanded="tvCategory_TreeNodeExpanded">
    <Nodes>
        <asp:TreeNode Text="Root Category" Value="0" PopulateOnDemand="true" />
    </Nodes>
</asp:TreeView>