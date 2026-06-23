using System.ComponentModel.DataAnnotations;

namespace CustomerWebApi.Dtos.Request
{
    public class PagedRequest
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber debe ser mayor a 0")]
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0 ? value : 1;
        }

        [Range(1, 100, ErrorMessage = "PageSize debe estar entre 1 y 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 0 && value <= 100 ? value : 10;
        }

        [StringLength(100, ErrorMessage = "Search no puede exceder 100 caracteres")]
        public string Search { get; set; }

        [StringLength(100, ErrorMessage = "SearchFields no puede exceder 100 caracteres")]
        public string SearchFields { get; set; }

        [StringLength(50, ErrorMessage = "SortBy no puede exceder 50 caracteres")]
        public string SortBy { get; set; } = "Id";

        [RegularExpression("^(asc|desc)$", ErrorMessage = "SortOrder debe ser 'asc' o 'desc'")]
        public string SortOrder { get; set; } = "asc";
    }
}
