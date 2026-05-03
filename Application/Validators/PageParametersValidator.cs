using Application.Pagination;
using FluentValidation;

namespace Application.Validators
{
    public class PageParametersValidator : AbstractValidator<PageParameters>
    {
        public PageParametersValidator() 
        {
            RuleFor(pg => pg.Page).NotEmpty()
                .GreaterThan(0);

            RuleFor(pg => pg.PageSize).InclusiveBetween(1, 50);
        }
    }
}
