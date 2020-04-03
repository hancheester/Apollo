using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class BrandCategoryTreeControl : BaseUserControl
    {
        public IBrandService BrandService { get; set; }

        private const int ROOT_INDEX = -1;

        public delegate void TreeEventHandler(string brandCategoryName, int brandCategoryId);

        public event TreeEventHandler TreeChanged;

        public event TreeEventHandler TreeNodeSelected;

        protected int BrandId
        {
            get { return ((BasePage)this.Page).QueryBrandId; }
        }

        public string CssClass
        {
            set { tvBrandCategory.CssClass = value; }
        }

        protected void tvBrandCategory_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            Populate(e.Node);
        }

        protected void tvBrandCategory_SelectedNodeChanged(object sender, EventArgs e)
        {
            string name = tvBrandCategory.SelectedNode.Text;
            int id = Convert.ToInt32(tvBrandCategory.SelectedNode.Value);

            tvBrandCategory.SelectedNode.Selected = false;

            InvokeSelected(name, id);
            InvokeChanged(name, id);
        }

        protected void tvBrandCategory_TreeNodeCollapsed(object source, TreeNodeEventArgs e)
        {
            InvokeChanged(e.Node.Text, Convert.ToInt32(e.Node.Value));
        }

        protected void tvBrandCategory_TreeNodeExpanded(object source, TreeNodeEventArgs e)
        {
            InvokeChanged(e.Node.Text, Convert.ToInt32(e.Node.Value));
        }

        private void InvokeChanged(string brandCategoryName, int brandCategoryId)
        {
            TreeEventHandler handler = TreeChanged;
            if (handler != null)
                handler(brandCategoryName, brandCategoryId);
        }

        private void InvokeSelected(string brandCategoryName, int brandCategoryId)
        {
            TreeEventHandler handler = TreeNodeSelected;
            if (handler != null)
                handler(brandCategoryName, brandCategoryId);
        }

        private void Populate(TreeNode node)
        {
            if (BrandId != 0)
            {
                IList<BrandCategory> brandCategories = new List<BrandCategory>();

                if (node.Value == ROOT_INDEX.ToString())
                {
                    node.Text = BrandService.GetBrandById(BrandId).Name;                 
                    brandCategories = BrandService.GetBrandCategoriesByBrandParent(BrandId, ROOT_INDEX);
                }
                else
                    brandCategories = BrandService.GetBrandCategoriesByParentId(Convert.ToInt32(node.Value));
                
                if (brandCategories.Count > 0)
                    PopulateCategories(node, brandCategories);
                else
                    node.SelectAction = TreeNodeSelectAction.Select;
            }
        }

        private void PopulateCategories(TreeNode node, IList<BrandCategory> brandCategories)
        {
            for (int i = 0; i < brandCategories.Count; i++)
            {
                TreeNode newNode = new TreeNode(brandCategories[i].Name, brandCategories[i].Id.ToString());

                if (brandCategories[i].Visible == false)
                {
                    newNode.Text = newNode.Text + " <i class='fa fa-eye-slash' title='hidden' aria-hidden='true'></i>";
                }

                newNode.SelectAction = TreeNodeSelectAction.Select;
                newNode.Expanded = false;
                newNode.PopulateOnDemand = true;
                newNode.ImageUrl = "~/img/folder.png";
                node.ChildNodes.Add(newNode);
            }
        }

        public void FindSelectedNode(int categoryId, IList<int> treeList)
        {
            tvBrandCategory.Nodes[0].ChildNodes.Clear();
            FindSelectedNode(tvBrandCategory.Nodes[0], treeList);
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

        public void Repopulate()
        {
            tvBrandCategory.Nodes[0].ChildNodes.Clear();
            Populate(tvBrandCategory.Nodes[0]);
        }

        public void Clear()
        {
            tvBrandCategory.Nodes[0].ChildNodes.Clear();
        }
    }
}