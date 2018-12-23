using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Data.Interface;
using Library.Web.Models;
using Library.Web.Models.Catalog;
using Library.Web.Models.Checkout;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAsset _asset;
        private readonly ICheckout _checkout;

        public CatalogController(ILibraryAsset assets, ICheckout checkouts)
        {
            _asset = assets;
            _checkout = checkouts;
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
            var asset = _asset.GetById(id);

            var currentHolds = _checkout.GetCurrentHolds(id)
                .Select(s => new AssetHoldModel()
                {
                    HoldPlaced = _checkout.GetCurrentHoldPlaced(s.Id).ToString("d"),
                    PatronName = _checkout.GetCurrentHoldPatronName(s.Id),
                });

            var model = new AssetDetailViewModel()
            {
                AssetId = id,
                Title = asset.Title,
                Type = _asset.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _asset.GetAuthorOrDirector(id),
                CurrentLocation = _asset.GetCurrentLocation(id).Name,
                DeweyCallNumber = _asset.GetDeweyIndex(id),
                CheckoutHistory = _checkout.GetCheckoutHistory(id),
                CurrentHolds = currentHolds
            };
            return View(model);
        }

        public IActionResult GetCheckout(int id)
        {
            var asset = _asset.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkout.IsCheckedOut(id)
            }; 

            return View(model);
        }
    }
}