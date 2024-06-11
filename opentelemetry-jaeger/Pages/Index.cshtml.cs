using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using opentelemetry_jaeger.Data;
using opentelemetry_jaeger.Models;

namespace opentelemetry_jaeger.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, DatabaseContext databaseContext) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly DatabaseContext _databaseContext = databaseContext;

        public async Task OnGet()
        {
            if (!_databaseContext.Carros.Any())
            {
                using var activity = System.Diagnostics.ActivitySourceProvider.Source.StartActivity();

                await _databaseContext.Carros.AddRangeAsync(new List<Carro>
                {
                    new("Fusca", "VW", 1985),
                    new("Uno", "Fiat", 2000)
                });

                await _databaseContext.SaveChangesAsync();
            }
        }
    }
}
