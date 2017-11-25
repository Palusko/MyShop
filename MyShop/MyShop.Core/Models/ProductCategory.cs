using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.Models
{
  public class ProductCategory : BaseEntity
  {
    //public string Id { get; set; }  we don't need it, because we inherit Id from BaseEntity
    public string Category { get; set; }


    /*   we don't need it, because we inherit Id from BaseEntity
    public ProductCategory()
    {
      this.Id = Guid.NewGuid().ToString();
    }
    */
  }
}
