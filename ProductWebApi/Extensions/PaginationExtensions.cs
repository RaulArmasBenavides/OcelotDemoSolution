using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProductWebApi.Dtos.Response;
using ProductWebApi.Models;
using ProductWebApi.Dtos.Request;

namespace ProductWebApi.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PaginatedResponse<T>> ToPaginatedAsync<T>(
            this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<T>(data, totalRecords, pageNumber, pageSize);
        }

        public static IQueryable<Product> ApplyFilters(
            this IQueryable<Product> query, ProductFilterRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(p => p.ProductName.Contains(request.Name));

            if (request.PriceMin.HasValue)
                query = query.Where(p => p.ProductPrice >= request.PriceMin.Value);

            if (request.PriceMax.HasValue)
                query = query.Where(p => p.ProductPrice <= request.PriceMax.Value);

            return query;
        }

        public static IQueryable<Product> ApplySearch(
            this IQueryable<Product> query, string search, string searchFields)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            var fields = searchFields?.Split(',')
                .Select(f => f.Trim().ToLower())
                .ToList() ?? new List<string>();

            if (fields.Count == 0)
                return query;

            var parameter = Expression.Parameter(typeof(Product), "p");
            Expression expression = null;

            foreach (var field in fields)
            {
                var property = typeof(Product).GetProperty(field,
                    System.Reflection.BindingFlags.IgnoreCase |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance);

                if (property != null && property.PropertyType == typeof(string))
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    var constant = Expression.Constant(search);
                    var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var methodCall = Expression.Call(propertyAccess, contains, constant);

                    expression = expression == null
                        ? methodCall
                        : Expression.OrElse(expression, methodCall);
                }
            }

            if (expression != null)
            {
                var lambda = Expression.Lambda<Func<Product, bool>>(expression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        public static IQueryable<Product> ApplySort(
            this IQueryable<Product> query, string sortBy, string sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var property = typeof(Product).GetProperty(sortBy,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            if (property == null)
                return query;

            var parameter = Expression.Parameter(typeof(Product), "p");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = sortOrder?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var genericMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(Product), property.PropertyType);

            return (IQueryable<Product>)genericMethod.Invoke(null, new object[] { query, lambda });
        }

        public static async Task<PaginatedResponse<Product>> ToPaginatedWithFiltersAsync(
            this IQueryable<Product> query, ProductFilterRequest request)
        {
            query = query
                .ApplyFilters(request)
                .ApplySearch(request.Search, request.SearchFields)
                .ApplySort(request.SortBy, request.SortOrder);

            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResponse<Product>(data, totalRecords, request.PageNumber, request.PageSize);
        }
    }
}
