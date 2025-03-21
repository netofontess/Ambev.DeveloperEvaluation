using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Application.DTOs
{
    public class SaleListResponseDTO
    {
        public IEnumerable<SaleDTO> Items { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalPages { get; set; }
    }
}