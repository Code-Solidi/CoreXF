using System;

namespace CoreXF.Store.Data.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ProductMetaData MetaData { get; set; }
    }
}