<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_GoogleTaxonomyControl" Codebehind="GoogleTaxonomyControl.ascx.cs" %>
<asp:TreeView ID="tvTaxonomy" runat="server" ExpandDepth="1" PathSeparator="|" CssClass="tree"
    OnTreeNodePopulate="tvTaxonomy_TreeNodePopulate"
    OnSelectedNodeChanged="tvTaxonomy_SelectedNodeChanged" 
    OnTreeNodeCollapsed="tvTaxonomy_TreeNodeCollapsed"
    OnTreeNodeExpanded="tvTaxonomy_TreeNodeExpanded">
    <Nodes>
        <asp:TreeNode Text="Root" Value="0" PopulateOnDemand="true" />
    </Nodes>
</asp:TreeView>