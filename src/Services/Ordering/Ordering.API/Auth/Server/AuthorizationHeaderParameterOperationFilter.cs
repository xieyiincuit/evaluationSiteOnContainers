namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.Auth;

public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
        var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter)
            .Any(filter => filter is AuthorizeFilter);
        var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter)
            .Any(filter => filter is IAllowAnonymousFilter);

        //需要授权的API
        if (isAuthorized && !allowAnonymous)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "access token",
                Required = true
            });
        }
    }
}