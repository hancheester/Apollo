using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_testimonials_new : BasePage
    {
        public IUtilityService UtilityService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            var newTestimonial = new Testimonial();
            newTestimonial.Comment = txtComment.Text.Trim();
            newTestimonial.Name = txtName.Text.Trim();
            newTestimonial.Priority = Convert.ToInt32(txtPriority.Text.Trim());

            int id = UtilityService.InsertTestimonial(newTestimonial);

            Response.Redirect("/marketing/cms_testimonials_info.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.TestimonialCreated + "&" + QueryKey.ID + "=" + id.ToString());
        }
    }
}