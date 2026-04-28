using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreApi.Models.Store
{
    public class ProductSeo
    {
        public required string MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Slug {  get; set; }
        public string? OpenGraphImageUrl { get; set; }
    }
}
