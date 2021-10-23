using BillingNextQuotation.Data;
using Microsoft.AspNetCore.Identity;
using NonFactors.Mvc.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Lookups
{
    public class ProductsLookup : ALookup<Models.Products>
    {
        private readonly ApplicationDbContext _context;

        public ProductsLookup(ApplicationDbContext context)
        {
            _context = context;
            GetId = (model) => model.ProductId;
            GetLabel = (model) => model.ProductName;
        }
        public ProductsLookup()
        {
            Url = "/LookupHandlers/AllLookupsApi?handler=AllProducts";
            Title = "Products";
            Filter.Order = LookupSortOrder.Desc;
        }

        public override IQueryable<Models.Products> GetModels()
        {
            return _context.Products.AsQueryable();
        }
    }
}
