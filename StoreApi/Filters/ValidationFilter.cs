using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StoreApi.Filters
{
    public class ValidationFilter<T> : IAsyncActionFilter where T : class
    {
        private readonly IValidator<T>? _validator;

        public ValidationFilter(IValidator<T>? validator = null)
        {
            _validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(_validator is null)
            {
                await next();
                return;
            }

            var orderDto = context.ActionArguments.Values.FirstOrDefault(v => v is T);

            if (orderDto is T dto)
            {
                var validationResult = await _validator.ValidateAsync(dto);

                if (!validationResult.IsValid)
                {
                    context.Result = new BadRequestObjectResult(validationResult.Errors);
                    return;
                }
            }

            await next();
        }
    }
}
