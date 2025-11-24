using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private HttpClient _httpClient;
        private Options _options;

        public IndexModel(HttpClient httpClient, Options options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        [BindProperty]
        public List<string> ImageList { get; private set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var imagesUrl = _options.ApiUrl;

            string imagesJson = await _httpClient.GetStringAsync(imagesUrl);

            IEnumerable<string> imagesList = JsonConvert.DeserializeObject<IEnumerable<string>>(imagesJson);

            this.ImageList = imagesList.ToList<string>();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            const long maxSizeBytes = 10 * 1024 * 1024; // 10 MB

            if (Upload != null && Upload.Length > 0)
            {
                if (Upload.Length > maxSizeBytes)
                {
                    ErrorMessage = "ขนาดไฟ,ืเกิน 10 mb";
                    await OnGetAsync();
                    return Page();
                }

                var imagesUrl = _options.ApiUrl;

                using (var image = new StreamContent(Upload.OpenReadStream()))
                {
                    image.Headers.ContentType = new MediaTypeHeaderValue(Upload.ContentType);
                    var response = await _httpClient.PostAsync(imagesUrl, image);
                }
            }

            return RedirectToPage("/Index");
        }
    }
}