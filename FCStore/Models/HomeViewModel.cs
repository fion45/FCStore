using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCStore.Models
{
    public class HomeViewModel
    {
        private BrandDbContext mBrandDBContext = new BrandDbContext();
        private CategoryDbContext mCategoryDbContext = new CategoryDbContext();
        private ProductDbContext mProductDbContext = new ProductDbContext();

        public List<Brand> mBrandArr = new List<Brand>();
        public List<Category> mCategoryArr = new List<Category>();
        public List<Product> mHotArr = new List<Product>();
        public List<Product> mNewArr = new List<Product>();
        public List<Product> mDiscountArr = new List<Product>();

        public HomeViewModel()
        {
            mBrandArr = mBrandDBContext.Brands.ToList();
            mCategoryArr = mCategoryDbContext.Categorys.ToList();
            mHotArr = mProductDbContext.Products.ToList();
        }
    }
}