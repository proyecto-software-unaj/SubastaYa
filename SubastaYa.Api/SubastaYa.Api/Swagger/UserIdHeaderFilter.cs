using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SubastaYa.Api.Swagger
{
    public class UserIdHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            
            if (operation.Parameters is null)
            {
                operation.Parameters = new List<IOpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "UserId",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Id del usuario que realiza la acción (sin auth).",
                Schema = new OpenApiSchema { Type = JsonSchemaType.Integer }

            });
        }
    }
}
