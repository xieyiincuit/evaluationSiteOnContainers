using Swashbuckle.AspNetCore.SwaggerGen;

namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Auth;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        //configured the authentication for controllers and action methods
        // Check for authorize attribute
        var hasAuthorize =
            context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
            context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
        var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter)
            .Any(filter => filter is IAllowAnonymousFilter);

        if (!hasAuthorize || allowAnonymous) return;

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

        var oAuthScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
        };

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [oAuthScheme] = new[] {"identity api"}
            }
        };
    }
}