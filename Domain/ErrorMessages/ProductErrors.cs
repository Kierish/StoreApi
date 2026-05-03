using Domain.Results;

namespace Domain.ErrorMessages
{
    public static class ProductErrors
    {
        public static readonly Error CreationFailed = Error.Failure(
            "Product.CreationFailed", "The product could not be created.");
        public static Error CategoryNotFound(string categoryName) => Error.NotFound(
            "Category.NotFound", $"Category '{categoryName}' doesn't exist.");
        public static Error ProductNotFound(Guid id) => Error.NotFound(
            "Product.NotFound", $"The product with ID {id} was not found.");
        public static Error TagsNotFound() => Error.NotFound(
            "Tags.NotFound", "One or more of the provided tags do not exist.");

    }
}
