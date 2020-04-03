using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_GoogleTaxonomyControl : BaseUserControl
    {
        private const int ROOT_INDEX = 0;

        public ICategoryService CategoryService { get; set; }

        public delegate void TreeEventHandler(string taxonomyName, int taxonomyId);

        public event TreeEventHandler TreeChanged;

        public event TreeEventHandler TreeNodeSelected;

        public string CssClass
        {
            set { tvTaxonomy.CssClass = value; }
        }

        private void FindSelectedNode(TreeNode node, IList<int> treeList)
        {
            Populate(node);
            node.Expand();
            node.SelectAction = TreeNodeSelectAction.Select;

            if (treeList.Count > 0)
            {
                int childId;

                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (treeList.Contains(Convert.ToInt32(node.ChildNodes[i].Value)))
                    {
                        childId = Convert.ToInt32(node.ChildNodes[i].Value);
                        treeList.Remove(childId);
                        FindSelectedNode(node.ChildNodes[i], treeList);
                        break;
                    }
                }
            }
        }

        private void PopulateTaxonomy(TreeNode node, IList<GoogleTaxonomy> taxonomies)
        {
            for (int i = 0; i < taxonomies.Count; i++)
            {
                TreeNode newNode = new TreeNode(taxonomies[i].Name, taxonomies[i].Id.ToString());
                newNode.SelectAction = TreeNodeSelectAction.Select;
                newNode.Expanded = false;
                newNode.PopulateOnDemand = true;
                newNode.ImageUrl = "~/img/folder.png";
                node.ChildNodes.Add(newNode);
            }
        }

        private void Populate(TreeNode node)
        {
            int parentId = Convert.ToInt32(node.Value);

            var taxonomies = CategoryService.GetGoogleTaxonomyTreeByParentId(parentId);

            if (taxonomies.Count > 0)
                PopulateTaxonomy(node, taxonomies);
            else
                node.SelectAction = TreeNodeSelectAction.Select;
        }

        private void InvokeChanged(string taxonomyName, int taxonomyId)
        {
            TreeEventHandler handler = TreeChanged;
            if (handler != null)
                handler(taxonomyName, taxonomyId);
        }

        private void InvokeSelected(string taxonomyName, int taxonomyId)
        {
            TreeEventHandler handler = TreeNodeSelected;
            if (handler != null)
                handler(taxonomyName, taxonomyId);
        }

        protected void tvTaxonomy_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            Populate(e.Node);
        }

        protected void tvTaxonomy_SelectedNodeChanged(object sender, EventArgs e)
        {
            string name = tvTaxonomy.SelectedNode.Text;
            int id = Convert.ToInt32(tvTaxonomy.SelectedNode.Value);

            tvTaxonomy.SelectedNode.Selected = false;

            InvokeSelected(name, id);
            InvokeChanged(name, id);
        }

        protected void tvTaxonomy_TreeNodeCollapsed(object source, TreeNodeEventArgs e)
        {
            InvokeChanged(e.Node.Text, Convert.ToInt32(e.Node.Value));
        }

        protected void tvTaxonomy_TreeNodeExpanded(object source, TreeNodeEventArgs e)
        {
            InvokeChanged(e.Node.Text, Convert.ToInt32(e.Node.Value));
        }

        public void FindSelectedNode(int taxonomyId, IList<int> treeList)
        {
            tvTaxonomy.Nodes[ROOT_INDEX].ChildNodes.Clear();
            FindSelectedNode(tvTaxonomy.Nodes[ROOT_INDEX], treeList);
        }

        public void Repopulate()
        {
            tvTaxonomy.Nodes[ROOT_INDEX].ChildNodes.Clear();
            Populate(tvTaxonomy.Nodes[ROOT_INDEX]);
        }

        public void Clear()
        {
            tvTaxonomy.Nodes[ROOT_INDEX].ChildNodes.Clear();
        }
    }
}