using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceApplication.Common
{

    //public class PaginatedList<T>
    //{
    //    //public List<T> Items { get; }
    //    //public int PageNumber { get; }
    //    //public int TotalPages { get; }
    //    //public int TotalCount { get; }
    //    //public int PageSize { get; }

    //    //public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    //    //{
    //    //    PageNumber = pageNumber;
    //    //    TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    //    //    TotalCount = count;
    //    //    PageSize = pageSize;
    //    //    Items = items;
    //    //}

    //    //public bool HasPreviousPage => PageNumber > 1;
    //    //public bool HasNextPage => PageNumber < TotalPages;

    //    //public static async Task<PaginatedList<T>> CreateAsync(
    //    //    IQueryable<T> source,
    //    //    int pageNumber,
    //    //    int pageSize,
    //    //    CancellationToken cancellationToken = default)
    //    //{
    //    //    var count = await source.CountAsync(cancellationToken);
    //    //    var items = await source
    //    //        .Skip((pageNumber - 1) * pageSize)
    //    //        .Take(pageSize)
    //    //        .ToListAsync(cancellationToken);

    //    //    return new PaginatedList<T>(items, count, pageNumber, pageSize);
    //    //}

    //    public List<T> Items { get; set; } = new();
    //    public int TotalCount { get; set; }
    //    public int PageNumber { get; set; }
    //    public int PageSize { get; set; }
    //    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    //    public bool HasPreviousPage => PageNumber > 1;
    //    public bool HasNextPage => PageNumber < TotalPages;

    //    //public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    //    //{
    //    //    PageNumber = pageNumber;
    //    //    TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    //    //    TotalCount = count;
    //    //    PageSize = pageSize;
    //    //    Items = items;
    //    //}

    //}

    public class PaginatedList<T>
    {
        public IReadOnlyList<T> Items { get; init; } = new List<T>();
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        // Parameterless constructor for deserialization
        [JsonConstructor]
        public PaginatedList() { }

        public PaginatedList(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
