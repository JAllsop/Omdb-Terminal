using Microsoft.AspNetCore.OData.Query;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OmdbTerminal.ApiService.Swagger;

public class ODataOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the endpoint has the [EnableQuery] attribute
        var hasEnableQuery = context.MethodInfo.GetCustomAttributes(true)
            .OfType<EnableQueryAttribute>().Any();

        if (!hasEnableQuery) return;

        operation.Parameters ??= new List<IOpenApiParameter>();

        // Add the standard OData parameters to the Swagger UI
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "$filter",
            In = ParameterLocation.Query,
            Description = "Filter the results e.g. contains(Title, 'Batman')",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "$orderby",
            In = ParameterLocation.Query,
            Description = "Order the results e.g. Year desc",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "$top",
            In = ParameterLocation.Query,
            Description = "Limit the number of results returned",
            Schema = new OpenApiSchema { Type = JsonSchemaType.Integer }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "$skip",
            In = ParameterLocation.Query,
            Description = "Skip a number of results for pagination",
            Schema = new OpenApiSchema { Type = JsonSchemaType.Integer }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "$select",
            In = ParameterLocation.Query,
            Description = "Select specific properties e.g. Title, Year",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "$expand",
            In = ParameterLocation.Query,
            Description = "Expand related entities e.g. Movie Ratings",
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Default = System.Text.Json.Nodes.JsonValue.Create("Ratings")
            }
        });
    }
}