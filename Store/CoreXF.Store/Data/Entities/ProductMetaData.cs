using System;

namespace CoreXF.Store.Data.Entities
{
    public class ProductMetaData
    {
        public Guid Id { get; set; }

        public Product Product { get; set; }

        public decimal Price { get; set; }
    }
}