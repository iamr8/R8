﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using R8.AspNetCore.Routing;
using R8.AspNetCore.Sitemap;
using R8.FileHandlers.AspNetCore;

namespace R8.AspNetCore.WebTest.Pages
{
    [SitemapSettings]
    public class IndexModel : Services.Routing.PageModel
    {
        public IndexModel()
        {
            PageTitle = "Index";
        }

        [BindProperty]
        public IFormFile Test { get; set; }

        public void OnGet()
        {
            var testCulture = this.HttpContext.GetLocalization();
            var testLocalizer = this.Localizer;
            var testCulturizedUrl = this.Url;
            if (testCulturizedUrl == null)
            {
            }
        }

        public async Task<IActionResult> OnPost()
        {
            var upload = await Test.UploadAsync();
            return this.RedirectToPageLocalized();
        }

        public async Task<IActionResult> OnGetUpdateLocalizerAsync()
        {
            await Localizer.RefreshAsync(true);
            return Page();
        }
    }
}