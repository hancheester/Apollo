﻿using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductCategoryMap : EntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryMap()
        {
            this.ToTable("ProductCategory");
            this.HasKey(pc => pc.Id);
        }
    }
}
