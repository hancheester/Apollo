using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Blogs;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_blog : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<BlogSettings>();
                cbEnabled.Checked = setting.Enabled;
                cbAllowNotRegisteredUsersToLeaveComments.Checked = setting.AllowNotRegisteredUsersToLeaveComments;
                cbNotifyAboutNewBlogComments.Checked = setting.NotifyAboutNewBlogComments;
                txtPostsPageSize.Text = setting.PostsPageSize.ToString();
                txtNumberOfTags.Text = setting.NumberOfTags.ToString();
                cbShowHeaderRssUrl.Checked = setting.ShowHeaderRssUrl;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<BlogSettings>();
            setting.Enabled = cbEnabled.Checked;
            setting.AllowNotRegisteredUsersToLeaveComments = cbAllowNotRegisteredUsersToLeaveComments.Checked;
            setting.NotifyAboutNewBlogComments = cbNotifyAboutNewBlogComments.Checked;
            setting.PostsPageSize = Convert.ToInt32(txtPostsPageSize.Text.Trim());
            setting.NumberOfTags = Convert.ToInt32(txtNumberOfTags.Text.Trim());
            setting.ShowHeaderRssUrl = cbShowHeaderRssUrl.Checked;

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Blog settings were updated successfully.";
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Setting);

            if (result)
                enbNotice.Message = "All settings related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }
    }
}