using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_testimonials_info : BasePage
    {
        public IUtilityService UtilityService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadTestimonialInfo();
        }
        
        protected void lbDelete_Click(object sender, EventArgs e)
        {
            UtilityService.DeleteTestimonial(QueryId);            
            Response.Redirect("/marketing/cms_testimonials_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.TestimonialDeleted);
        }

        protected void lbSaveContinue_Click(object sender, EventArgs e)
        {
            var testimonial = UtilityService.GetTestimonial(QueryId);
            
            testimonial.Comment = txtComment.Text.Trim();
            testimonial.Name = txtName.Text.Trim();
            testimonial.Priority = Convert.ToInt32(txtPriority.Text.Trim());

            UtilityService.UpdateTestimonial(testimonial);

            enbNotice.Message = "Testimonial updated successfully.";
        }

        private void LoadTestimonialInfo()
        {
            var testimonial = UtilityService.GetTestimonial(QueryId);

            if (testimonial != null)
            {
                ltlTitle.Text = string.Format("{0} (ID: {1})", testimonial.Name, QueryId);
                txtComment.Text = testimonial.Comment;
                txtName.Text = testimonial.Name;
                txtPriority.Text = testimonial.Priority.ToString();
            }
        }
    }
}