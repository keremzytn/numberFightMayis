using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace numberFightMayis.Pages.Game
{
    public class StartModel : PageModel
    {
        private readonly ILogger<StartModel> _logger;

        public StartModel(ILogger<StartModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
} 