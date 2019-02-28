using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Library.Data;
using Library.Data.Interface;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public class CheckoutService : ICheckout
    {
        private readonly LibraryContext _context;

        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return GetAll().FirstOrDefault(b => b.Id == checkoutId);
        }

        public Checkout GetLatestCheckout(int assetId)
        {
            return _context.Checkouts.Where(w => w.LibraryAsset.Id == assetId)
                .OrderByDescending(c => c.Since).FirstOrDefault();
        }

        public void Add(Checkout newCheckOut)
        {
            _context.Add(newCheckOut);
            _context.SaveChanges();
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == id);
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);
            if (checkout == null) return "Not checked out.";

            var card = checkout.LibraryCard.Id;
            var patron = _context.Patrons.Include(p => p.LibraryCard).First(c => c.LibraryCard.Id == card);

            return patron?.FirstName + " " + patron?.LastName;
        }

        public bool IsCheckedOut(int id)
        {
            var isCheckedOut = _context.Checkouts.Any(c => c.LibraryAsset.Id == id);
            return isCheckedOut;
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return _context.Checkouts
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(c => c.LibraryAsset.Id == assetId);
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var today = DateTime.Today;
            var asset = _context.LibraryAssets.Include(a => a.Status).FirstOrDefault(s => s.Id == assetId);
            if (asset == null)
            {
                throw new DataException("Asset is invalid.");
            }
            if (asset.Status.Name == "Available")
            {
                UpdateMarkFound(assetId, "On Hold");
            }
            var card = _context.LibraryCards.FirstOrDefault(s => s.Id == libraryCardId);

            var hold = new Hold()
            {
                HoldPlaced = today,
                LibraryAsset = asset,
                LibraryCard = card,
            };

            _context.Add(hold);
            _context.SaveChanges();
        }

        public string GetCurrentHoldPatronName(int holdid)
        {
            var currentHold = _context.Holds
                .Include(s => s.LibraryAsset)
                .Include(s => s.LibraryCard)
                .FirstOrDefault(s => s.Id == holdid);

            if (currentHold == null) return "Not checked out.";

            var card = currentHold.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.Id == card);

                return patron?.FirstName + " " + patron?.LastName;
        }

        public string GetCurrentHoldPlaced(int holdid)
        {
            var hold = _context.Holds
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(v => v.Id == holdid);

            return hold.Select(a => a.HoldPlaced)
                .FirstOrDefault().ToString(CultureInfo.InvariantCulture);
        }

        private DateTime GetPlacedHold(int holdid)
        {
            var placeHold = _context.Holds.Include(h => h.LibraryAsset).Include(h => h.LibraryCard)
                .FirstOrDefault(w => w.Id == holdid);
            if (placeHold == null)
            {
                throw new DataException("No holds have been placed on your account.");
            }
            return placeHold.HoldPlaced;
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Where(w => w.LibraryAsset.Id == id);
        }

        public void MarkLost(int assetId)
        {
            var item = _context.LibraryAssets.FirstOrDefault(b => b.Id == assetId);
            if (item != null)
            {
                _context.Update(item);
                item.Status = _context.Statuses.FirstOrDefault(s => s.Name == "Lost");
            }
            _context.SaveChanges();
        }

        public void MarkFound(int assetId)
        {
            var now = DateTime.Now;

            UpdateMarkFound(assetId, "Available");
            RemoveExistingChecktous(assetId);
            ClosingCheckoutHistory(assetId, now);
            
            _context.SaveChanges();
        }

        private void UpdateMarkFound(int assetId, string status)
        {
            var item = _context.LibraryAssets.FirstOrDefault(b => b.Id == assetId);
            if (item != null)
            {
                _context.Update(item);
                item.Status = _context.Statuses.FirstOrDefault(s => s.Name == status);
            }
        }

        private void ClosingCheckoutHistory(int assetId, DateTime now)
        {
            // close any existing checkout history
            var history = _context.CheckoutHistories.FirstOrDefault(s => s.LibraryAsset.Id == assetId && s.CheckedIn == null);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }         
        }

        private void RemoveExistingChecktous(int assetId)
        {
            // remove any existing checkouts on the item.
            var checkout = _context.Checkouts.FirstOrDefault(c => c.LibraryAsset.Id == assetId);
            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            if (IsCheckeduOut(assetId))
            {
                return;
                // add logic here to handle feedback to the user;
            }

            var item = _context.LibraryAssets.FirstOrDefault(b => b.Id == assetId);
            UpdateMarkFound(assetId, "Checked Out");

            var card = _context.LibraryCards.Include(c => c.Checkouts).FirstOrDefault(f => f.Id == libraryCardId);

            var now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = card,
                Since = now,
                Until = GetDefaultCheckoutTime(now),
            };

            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = card
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        private bool IsCheckeduOut(int assetId)
        {
            return _context.Checkouts.Any(c => c.LibraryAsset.Id == assetId);
        }

        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets.FirstOrDefault(s => s.Id == assetId);
            if (item != null)
            {
                _context.Update(item);
            }
            // remove any existing checkout on the item.
            RemoveExistingChecktous(assetId);
            // close any existing checkout history
            ClosingCheckoutHistory(assetId, now);

            // look for existing holds on the item. if there are holds, checkout the item to the 
            // librarycard with the earliest holds.
            // otherwise, update the item status to available.
            var currentHold = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == assetId);

            if (currentHold.Any())
            {
                CheckoutEarliestHold(assetId, currentHold);
                //return;
            }

            UpdateMarkFound(assetId, "Available");
            _context.SaveChanges();
        }

        private void CheckoutEarliestHold(int assetId, IEnumerable<Hold> currentHold)
        {
            var recentHold = currentHold.OrderBy(h => h.HoldPlaced).FirstOrDefault();

            if (recentHold != null)
            {
                var card = recentHold.LibraryCard;

                _context.Remove(recentHold);
                _context.SaveChanges();

                CheckOutItem(assetId, card.Id);
            }
        }
    }
}
