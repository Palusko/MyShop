using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
  public class BasketService : IBasketService
  {
    IRepository<Product> productContext;
    IRepository<Basket> basketContext;

    public const string BasketSessionName = "eCommerceBasket";

    public BasketService(IRepository<Product> productContext, IRepository<Basket> basketContext)
    {
      this.productContext = productContext;
      this.basketContext = basketContext;
    }

    private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
    {
      HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);
      var basket = new Basket();

      if (cookie != null)
      {
        string basketId = cookie.Value;
        if (!string.IsNullOrEmpty(basketId))
          basket = basketContext.Find(basketId);
        else
        {
          if (createIfNull)
            basket = CreateNewBasket(httpContext);
        }
      }
      else
      {
        if (createIfNull)
        {
          basket = CreateNewBasket(httpContext);
        }
      }
      return basket;
    }

    private Basket CreateNewBasket(HttpContextBase httpContext)
    {
      var basket = new Basket();
      basketContext.Insert(basket);
      basketContext.Commit();

      var cookie = new HttpCookie(BasketSessionName);
      cookie.Value = basket.Id;
      cookie.Expires = DateTime.Now.AddDays(1);
      httpContext.Response.Cookies.Add(cookie);

      return basket;
    }

    public void AddBasket(HttpContextBase httpContext, string productId)
    {
      var basket = GetBasket(httpContext, true);
      var item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

      if (item == null)
      {
        item = new BasketItem()
                {
                  BasketId = basket.Id,
                  ProductId = productId,
                  Quantity = 1
                };
        basket.BasketItems.Add(item);
      }
      else
      {
        item.Quantity += 1;
      }

      basketContext.Commit();
    }

    public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
    {
      var basket = GetBasket(httpContext, true);
      var item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

      if (item != null)
      {        
        basket.BasketItems.Remove(item);
        basketContext.Commit();
      }
    }

    public List<BasketItemViewModel> GetBasketItems(HttpContextBase httpContext)
    {
      var basket = GetBasket(httpContext, false);

      if (basket != null)
      {
        var results = (from b in basket.BasketItems
                       join p in productContext.Collection() on b.Id equals p.Id
                       select new BasketItemViewModel()
                       {
                         Id = b.Id,
                         Quantity = b.Quantity,
                         ProductName = p.Name,
                         Price = p.Price,
                         Image = p.Image
                       }).ToList();

        return results;
      }
      else
        return new List<BasketItemViewModel>();
    }

    public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext)
    {
      Basket basket = GetBasket(httpContext, false);
      var model = new BasketSummaryViewModel(0, 0);

      if (basket != null)
      {
        int? basketCount = (from item in basket.BasketItems
                            select item.Quantity).Sum();

        decimal? basketTotal = (from item in basket.BasketItems
                                join p in productContext.Collection() on item.ProductId equals p.Id
                                select item.Quantity * p.Price).Sum();

        model.BasketCount = basketCount ?? 0; //if there is a basket count, return basket count, otherwise (if it's null), default to zero
        model.BasketTotal = basketTotal ?? decimal.Zero;

        return model;
      }
      else
        return model;
    }
  }
}
