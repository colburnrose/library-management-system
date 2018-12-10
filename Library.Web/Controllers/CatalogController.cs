using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Data.Interface;
using Library.Web.Models;
using Library.Web.Models.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _asset;

        public CatalogController(ILibraryAsset assets)
        {
            _asset = assets;
        }

        public IActionResult Index()
        {
            var assets = _asset.GetAll();

            var coll = assets.Select(s => new AssetListingViewModel
            {
                Id = s.Id,
                ImageUrl = s.ImageUrl,
                Title = s.Title,
                AuthorOrDirector = _asset.GetAuthorOrDirector(s.Id),
                Type = _asset.GetType(s.Id),
                DeweyCallNumber = _asset.GetDeweyIndex(s.Id),
            });

            var model = new AssetIndexModel()
            {
                Assets = coll
            }; 

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var details = _asset.GetById(id);

            var coll = new AssetDetailViewModel()
            {
                AssetId = id,
                Title = details.Title,
                AuthorOrDirector = _asset.GetAuthorOrDirector(id),
                Type = _asset.GetType(id),
                Year = details.Year,
                ISBN = _asset.GetIsbn(id),
                DeweyCallNumber = _asset.GetDeweyIndex(id),
                Status = details.Status.Name,
                Cost = details.Cost,
                CurrentLocation = _asset.GetCurrentLocation(id).Name,

            };

            return View(coll);
        }
    }
}