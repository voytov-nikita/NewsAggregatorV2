using System.Globalization;
using Common.Models;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Mvc;

public static class ControllerBaseExtensions
{
    /// <summary>
    /// Writes <c>X-Pagination-*</c> headers (Total / Offset / Returned) for the
    /// given collection and exposes them via <c>Access-Control-Expose-Headers</c>
    /// so a browser client can read them across origins.
    /// </summary>
    public static void AddPaginationHeaders<T>(this ControllerBase controller, OffsetCollection<T> collection)
    {
        IHeaderDictionary headers = controller.ControllerContext.HttpContext.Response.Headers;

        headers.Append("X-Pagination-Total",    collection.TotalCount.ToString(CultureInfo.InvariantCulture));
        headers.Append("X-Pagination-Offset",   collection.Offset.ToString(CultureInfo.InvariantCulture));
        headers.Append("X-Pagination-Returned", collection.Count.ToString(CultureInfo.InvariantCulture));
        headers.Append("Access-Control-Expose-Headers", "X-Pagination-Total, X-Pagination-Offset, X-Pagination-Returned");
    }
}
