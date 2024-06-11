using opentelemetry_jaeger_api.Data;
using opentelemetry_jaeger_api.Models;

namespace opentelemetry_jaeger_api.Configuration
{
    public static class RoutesConfigurationExtension
    {
        public static RouteGroupBuilder MapEndpoints(this RouteGroupBuilder group)
        {
            group.MapGet("/getAll", async (CancellationToken cancellationToken, DatabaseContext databaseContext) =>
            {
                return TypedResults.Ok(databaseContext.Carros.Where(c => c.Ano > 1990).ToList());
            });

            group.MapPost("/SetupPayerAuth/", async (CancellationToken cancellationToken, DatabaseContext databaseContext) =>
            {
                if (!databaseContext.Carros.Any())
                {
                    await databaseContext.Carros.AddRangeAsync(new List<Carro>
                    {
                        new("Fusca", "VW", 1985),
                        new("Uno", "Fiat", 2000)
                    });

                    await databaseContext.SaveChangesAsync();

                    return Results.Ok("Carros inseridos com sucesso");
                }

                return Results.Ok();
            });

            return group;
        }
    }
}
