using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_CategoryTreeControl : BaseUserControl
    {
        private const int ROOT_INDEX = 0;

        public delegate void TreeEventHandler(string categoryName, int categoryId);

        public event TreeEventHandler TreeChanged;
        public event TreeEventHandler TreeNodeSelected;

        public ICategoryService CategoryService { get; set; }
        public IProductService ProductService { get; set; }

        public string CssClass
        {
            set { tvCategory.CssClass = value; }
        }

        protected void tvCategory_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            Populate(e.Node);
        }

        protected void tvCategory_SelectedNodeChanged(object sender, EventArgs e)
        {
            string name = tvCategory.SelectedNode.Text;
            int id = Convert.ToInt32(tvCategory.SelectedNode.Value);

            tvCategory.SelectedNode.Selected = false;

            InvokeSelected(name, id);
            InvokeChanged(name, id);
        }

        protected void tvCategory_TreeNodeCollapsed(object source, TreeNodeEventArgs e)
        {
            InvokeChanged(e.Node.Text, Convert.ToInt32(e.Node.Value));
        }

        protected void tvCategory_TreeNodeExpanded(object source, TreeNodeEventArgs e)
        {
            if (e.Node.Depth <= 3)
                InvokeChanged(e.Node.Text, Convert.ToInt32(e.Node.Value));
        }

        public void FindSelectedNode(int categoryId, IList<int> treeList)
        {
            tvCategory.Nodes[ROOT_INDEX].ChildNodes.Clear();
            FindSelectedNode(tvCategory.Nodes[ROOT_INDEX], treeList);
        }

        public void Clear()
        {
            tvCategory.Nodes[ROOT_INDEX].ChildNodes.Clear();
        }

        public void Repopulate()
        {
            tvCategory.Nodes[ROOT_INDEX].ChildNodes.Clear();
            Populate(tvCategory.Nodes[ROOT_INDEX]);
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

        private void PopulateCategories(TreeNode node, IList<Category> categories)
        {
            for (int i = 0; i < categories.Count; i++)
            {
                TreeNode newNode = new TreeNode(categories[i].CategoryName, categories[i].Id.ToString());
                
                if (categories[i].Visible == false)
                {
                    newNode.Text = newNode.Text + " <i class='fa fa-eye-slash' title='hidden' aria-hidden='true'></i>";
                }

                int count = ProductService.GetProductCountByCategory(categories[i].Id);
                if (count > 0) newNode.Text = newNode.Text + " (" + count + ")";
                
                newNode.SelectAction = TreeNodeSelectAction.Select;
                newNode.Expanded = false;
                newNode.PopulateOnDemand = true;
                
                newNode.ImageUrl = "~/img/folder.png";
                node.ChildNodes.Add(newNode);
            }
        }
        
        private void Populate(TreeNode node)
        {
            if (node.Depth <= 2)
            {
                int parentId = Convert.ToInt32(node.Value);
                var categories = CategoryService.GetCategoryByParent(parentId);
                if (categories.Count > 0)
                    PopulateCategories(node, categories);
                else
                    node.SelectAction = TreeNodeSelectAction.Select;
            }
        }

        private void InvokeChanged(string categoryName, int categoryId)
        {
            TreeEventHandler handler = TreeChanged;
            if (handler != null)
                handler(categoryName.Replace(" <i class='fa fa-eye-slash' title='hidden' aria-hidden='true'></i>", string.Empty), categoryId);
        }

        private void InvokeSelected(string categoryName, int categoryId)
        {
            TreeEventHandler handler = TreeNodeSelected;
            if (handler != null)
                handler(categoryName.Replace(" <i class='fa fa-eye-slash' title='hidden' aria-hidden='true'></i>", string.Empty), categoryId);
        }
    }
}