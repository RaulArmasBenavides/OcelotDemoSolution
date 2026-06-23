using MongoDB.Driver;
using OrderWebApi.Dtos.Response;
using OrderWebApi.Models;
using OrderWebApi.Dtos.Request;

namespace OrderWebApi.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PaginatedResponse<T>> ToPaginatedAsync<T>(
            this IMongoCollection<T> collection, FilterDefinition<T> filter, int pageNumber, int pageSize)
        {
            var totalRecords = (int)await collection.CountDocumentsAsync(filter);
            var data = await collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PaginatedResponse<T>(data, totalRecords, pageNumber, pageSize);
        }

        public static FilterDefinition<Order> ApplyFilters(
            this FilterDefinition<Order> filter, OrderFilterRequest request)
        {
            var filters = new List<FilterDefinition<Order>> { filter };

            if (!string.IsNullOrWhiteSpace(request.CustomerId) && int.TryParse(request.CustomerId, out var customerId))
                filters.Add(Builders<Order>.Filter.Eq(o => o.CustomerId, customerId));

            if (request.DateFrom.HasValue)
                filters.Add(Builders<Order>.Filter.Gte(o => o.OrderedOn, request.DateFrom.Value));

            if (request.DateTo.HasValue)
                filters.Add(Builders<Order>.Filter.Lte(o => o.OrderedOn, request.DateTo.Value));

            return Builders<Order>.Filter.And(filters);
        }

        public static async Task<PaginatedResponse<Order>> ToPaginatedWithFiltersAsync(
            this IMongoCollection<Order> collection, OrderFilterRequest request)
        {
            var filter = Builders<Order>.Filter.Empty.ApplyFilters(request);

            var totalRecords = (int)await collection.CountDocumentsAsync(filter);

            var findFluent = collection.Find(filter);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                var sort = request.SortOrder?.ToLower() == "desc"
                    ? Builders<Order>.Sort.Descending(request.SortBy)
                    : Builders<Order>.Sort.Ascending(request.SortBy);
                findFluent = findFluent.Sort(sort);
            }

            var data = await findFluent
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Limit(request.PageSize)
                .ToListAsync();

            return new PaginatedResponse<Order>(data, totalRecords, request.PageNumber, request.PageSize);
        }
    }
}
