using MyShop.Core.Models;
using MyShop.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.ViewModels;

namespace MyShop.WebUI.Controllers
{
  public class HomeController : Controller
  {
    IRepository<Product> context;
    IRepository<ProductCategory> productCategories;

    public HomeController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoryContext)
    {
      context = productContext;
      productCategories = productCategoryContext;
    }

    public ActionResult Index(string Category=null)//if category is not supplied, we display all products of all categories; It's capitalized Category because that's what shows in URL
    {
      List<Product> products;
      List<ProductCategory> categories = productCategories.Collection().ToList();

      if (Category == null)
        products = context.Collection().ToList();
      else
        products = context.Collection().Where(p => p.Category == Category).ToList();

      ProductListViewModel model = new ProductListViewModel();
      model.Products = products;
      model.ProductCategories = categories;

      return View(model);
    }

    public ActionResult Details(string Id)
    {
      Product product = context.Find(Id);
      if (product == null)
        return HttpNotFound();

      return View(product);
    }

    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }
  }
}