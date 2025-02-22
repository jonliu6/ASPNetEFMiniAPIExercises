
using AutoMapper;
using MinimalAPIsWithASPNetEF.Repositories;

namespace MinimalAPIsWithASPNetEF.Filters
{
    public class TestFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // the following code executed before the endpoint
            // the endpoint method with the filter has 3 parameters
            // var param1 = (int) context.Arguments[0]; // the following way is better without hard-coding the parameters in order
            var param1 = context.Arguments.OfType<int>().FirstOrDefault();
            var param2 = context.Arguments.OfType<IGenresRepository>().FirstOrDefault();
            var param3 = context.Arguments.OfType<IMapper>().FirstOrDefault();

            var result = await next(context);

            // the following code executed after the endpoint
            return result;

        }
    }
}
