using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_pharm_form_edit : BasePage
    {
        public IOrderService OrderService { get; set; }
        public IAccountService AccountService { get; set; }
        public IProductService ProductService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected override void OnInit(EventArgs e)
        {
            if (QueryOrderId <= 0)
                Response.Redirect("/sales/order_default.aspx");
            else
            {
                ltlTitle.Text = string.Format("<h3>Order # {0}</h3>", QueryOrderId.ToString());
                LoadPharmForm();
            }

            base.OnInit(e);
        }

        private void LoadPharmForm()
        {
            var pharm = OrderService.GetPharmOrderByOrderId(QueryOrderId);

            ltlNotFound.Visible = false;
            phPharmOrder.Visible = false;

            if (pharm == null)
            {
                ltlNotFound.Visible = true;
                return;
            }

            phPharmOrder.Visible = true;

            cbTakenOwner.Checked = pharm.TakenByOwner;
            txtAllergy.Text = pharm.Allergy;
            txtOwnderAge.Text = pharm.OwnerAge;
            cbHasOtherMed.Checked = pharm.HasOtherCondMed;
            txtOwnerOtherCond.Text = pharm.OtherCondMed;

            rptPharmItem.DataSource = pharm.Items;
            rptPharmItem.DataBind();

            var profileId = OrderService.GetProfileIdByOrderId(pharm.OrderId);

            var account = AccountService.GetAccountOverviewModelByProfileId(profileId);

            if (account != null)
            {
                ltName.Text = account.Name;
                ltEmail.Text = account.Email;
                ltContact.Text = string.IsNullOrEmpty(account.ContactNumber) ? string.Empty : account.ContactNumber;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var pharm = OrderService.GetPharmOrderByOrderId(QueryOrderId);

            if (pharm == null)
            {
                ltlNotFound.Visible = true;
                return;
            }

            pharm.TakenByOwner = cbTakenOwner.Checked;
            pharm.Allergy = txtAllergy.Text.Trim();
            pharm.OwnerAge = txtOwnderAge.Text.Trim();
            pharm.HasOtherCondMed = cbHasOtherMed.Checked;
            pharm.OtherCondMed = txtOwnerOtherCond.Text.Trim();

            for(int i = 0; i < rptPharmItem.Items.Count; i++)
            {
                var hfPharmItemId = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "hfPharmItemId") as HiddenField;
                var id = Convert.ToInt32(hfPharmItemId.Value);

                var item = pharm.Items.Where(x => x.Id == id).FirstOrDefault();

                if (item != null)
                {
                    var txtSymptoms = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "txtSymptoms") as TextBox;
                    var txtMedForSymptom = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "txtMedForSymptom") as TextBox;
                    var txtAge = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "txtAge") as TextBox;
                    var cbHasOtherCondMed = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "cbHasOtherCondMed") as CheckBox;
                    var txtOtherCondMed = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "txtOtherCondMed") as TextBox;
                    var txtPersistedInDays = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "txtPersistedInDays") as TextBox;
                    var cbHasTaken = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "cbHasTaken") as CheckBox;
                    var txtTakenQuantity = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "txtTakenQuantity") as TextBox;
                    var txtLastTimeTaken = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "txtLastTimeTaken") as TextBox;
                    var txtActionTaken = AdminStoreUtility.FindControlRecursive(rptPharmItem.Items[i], "txtActionTaken") as TextBox;
                    
                    item.Symptoms = string.IsNullOrEmpty(txtSymptoms.Text.Trim()) ? null : txtSymptoms.Text.Trim();
                    item.MedForSymptom = string.IsNullOrEmpty(txtMedForSymptom.Text.Trim()) ? null : txtMedForSymptom.Text.Trim();
                    item.Age = string.IsNullOrEmpty(txtAge.Text.Trim()) ? null : txtAge.Text.Trim();
                    item.HasOtherCondMed = cbHasOtherCondMed.Checked;
                    item.OtherCondMed = string.IsNullOrEmpty(txtOtherCondMed.Text.Trim()) ? null : txtOtherCondMed.Text.Trim();
                    item.PersistedInDays = string.IsNullOrEmpty(txtPersistedInDays.Text.Trim()) ? null : txtPersistedInDays.Text.Trim();
                    item.ActionTaken = string.IsNullOrEmpty(txtActionTaken.Text.Trim()) ? null : txtActionTaken.Text.Trim();
                    item.HasTaken = cbHasTaken.Checked.ToString();
                    item.TakenQuantity = string.IsNullOrEmpty(txtTakenQuantity.Text.Trim()) ? null : txtTakenQuantity.Text.Trim();
                    item.LastTimeTaken = string.IsNullOrEmpty(txtLastTimeTaken.Text.Trim()) ? null : txtLastTimeTaken.Text.Trim();
                }
            }

            OrderService.UpdateOrderPharmOrderAndItems(pharm);

            Response.Redirect("/sales/order_pharm_form.aspx?orderid=" + QueryOrderId + "&" + QueryKey.MSG_TYPE + "=" + (int)MessageType.PharmOrderUpdated);
        }
    }
}