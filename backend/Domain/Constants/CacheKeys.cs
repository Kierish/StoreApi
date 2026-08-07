using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Constants;
public static class CacheKeys
{
    public static string GetProductKey(Guid id) => $"product:{id}";
    public static string GetProductListKey(int page, int size, int version) =>
        $"product:list:v{version}:page:{page}:size:{size}";
    public static string GetProductListVersionKey() => "product:list:version";

    public static string GetCommentListKey(Guid productId, int version) =>
        $"comment:list:v{version}:product:{productId}";
    public static string GetCommentListVersionKey(Guid productId) => $"comment:list:version:{productId}";

    public static string GetTagsListKey() => "tags:list";

    public static string GetCategoryListKey() => "category:list";
}
