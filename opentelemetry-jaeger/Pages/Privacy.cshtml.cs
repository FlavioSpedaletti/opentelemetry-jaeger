using Microsoft.AspNetCore.Mvc.RazorPages;
using opentelemetry_jaeger.Data;
using opentelemetry_jaeger.Models;

namespace opentelemetry_jaeger.Pages
{
    public class PrivacyModel(ILogger<PrivacyModel> logger, DatabaseContext databaseContext) : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger = logger;
        private readonly DatabaseContext _databaseContext = databaseContext;

        public List<Carro> Carros { get; set; }

        public void OnGet()
        {
            Carros = [.. _databaseContext.Carros.Where(c => c.Ano > 1990)];
        }
    }

}
