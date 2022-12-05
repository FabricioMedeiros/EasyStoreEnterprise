using ESE.WebApp.MVC.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Services
{
    public interface ICatalogService
    {
       Task<IEnumerable<ProductViewModel>> Get();
       Task<ProductViewModel> GetById(Guid id);
    }
}
