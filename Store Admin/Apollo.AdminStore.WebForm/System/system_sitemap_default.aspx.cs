using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Common;
using Apollo.Core.Services.Interfaces;
using System;
using System.IO;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_sitemap_default : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public CommonSettings CommonSettings { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lbGenerate_Click(object sender, EventArgs e)
        {
            var bytes = UtilityService.GenerateSitemap();
            
            File.WriteAllBytes(CommonSettings.SitemapFileLocalPath + "sitemap.xml", bytes);

            enbNotice.Message = "Sitemap was successfully generated.";
        }
    }
}