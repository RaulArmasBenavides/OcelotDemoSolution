using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CustomerWebApi.Dtos.Response;
using CustomerWebApi.Models;
using CustomerWebApi.Dtos.Request;

namespace CustomerWebApi.Extensions
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

        public static IQueryable<Customer> ApplyFilters(
            this IQueryable<Customer> query, CustomerFilterRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(c => c.CustomerName.Contains(request.Name));

            if (!string.IsNullOrWhiteSpace(request.Email))
                query = query.Where(c => c.Email.Contains(request.Email));

            if (!string.IsNullOrWhiteSpace(request.Phone))
                query = query.Where(c => c.MobileNumber.Contains(request.Phone));

            return query;
        }

        public static IQueryable<Customer> ApplySearch(
            this IQueryable<Customer> query, string search, string searchFields)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            var fields = searchFields?.Split(',')
                .Select(f => f.Trim().ToLower())
                .ToList() ?? new List<string>();

            if (fields.Count == 0)
                return query;

            var parameter = Expression.Parameter(typeof(Customer), "c");
            Expression expression = null;

            foreach (var field in fields)
            {
                var property = typeof(Customer).GetProperty(field,
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
                var lambda = Expression.Lambda<Func<Customer, bool>>(expression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        public static IQueryable<Customer> ApplySort(
            this IQueryable<Customer> query, string sortBy, string sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var property = typeof(Customer).GetProperty(sortBy,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            if (property == null)
                return query;

            var parameter = Expression.Parameter(typeof(Customer), "c");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = sortOrder?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var genericMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(Customer), property.PropertyType);

            return (IQueryable<Customer>)genericMethod.Invoke(null, new object[] { query, lambda });
        }

        public static async Task<PaginatedResponse<Customer>> ToPaginatedWithFiltersAsync(
            this IQueryable<Customer> query, CustomerFilterRequest request)
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

            return new PaginatedResponse<Customer>(data, totalRecords, request.PageNumber, request.PageSize);
        }
    }
}
