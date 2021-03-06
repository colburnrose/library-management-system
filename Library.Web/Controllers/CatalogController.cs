﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Data.Interface;
using Library.Web.Models;
using Library.Web.Models.Catalog;
using Library.Web.Models.Checkout;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Extensions;

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
                    HoldPlaced = _checkout.GetCurrentHoldPlaced(s.Id),
                    CustomerName = _checkout.GetCurrentHoldPatronName(s.Id),
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
                CurrentLocation = _asset.GetCurrentLocation(id)?.Name,
                DeweyCallNumber = _asset.GetDeweyIndex(id),
                CheckoutHistory = _checkout.GetCheckoutHistory(id),
                CurrentHolds = currentHolds,
                CustomerName = _checkout.GetCurrentCheckoutPatron(id)
            };
            return View(model);
        }

        public IActionResult CheckIn(int id)
        {
            _checkout.CheckInItem(id);
            return RedirectToAction("Detail", new {id = id});
        }

        public IActionResult Checkout(int id)
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

        public IActionResult Hold(int id)
        {
            var asset = _asset.GetById(id);

            var model = new CheckoutModel()
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkout.IsCheckedOut(id),
                HoldCount = _checkout.GetCurrentHolds(id).Count(),
            };

            return View(model);
        }

        public IActionResult MarkLost(int assetId)
        {
            _checkout.MarkLost(assetId);
            return RedirectToAction("Detail", new {id = assetId});
        }

        public IActionResult MarkFound(int assetId)
        {
            _checkout.MarkFound(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            _checkout.CheckOutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new {id = assetId});
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            _checkout.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}