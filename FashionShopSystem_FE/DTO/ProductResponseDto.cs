﻿namespace FashionShopSystem_FE.DTO
{
    public class ProductResponseDto
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public int? Stock { get; set; }

        public int? CategoryId { get; set; }

        public string? Brand { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool? IsActive { get; set; }
        public CategoryResponseDto? Category { get; set; }
    }
}
