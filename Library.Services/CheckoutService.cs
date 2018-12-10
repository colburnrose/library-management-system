using System;
using System.Collections.Generic;
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
        private LibraryContext _context;

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

        public void PlaceHold(int assestId, int libraryCardId)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentHoldPatronName(int id)
        {
            throw new NotImplementedException();
        }

        public DateTime GetCurrentHoldPlaced(int id)
        {
            throw new NotImplementedException();
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
            var item = _context.LibraryAssets.FirstOrDefault(b => b.Id == assetId);
            if (item != null)
            {
                _context.Update(item);
                item.Status = _context.Statuses.FirstOrDefault(s => s.Name == "Available");
            }
            // remove any existing checkouts on the item.
            var checkout = _context.Checkouts.FirstOrDefault(c => c.LibraryAsset.Id == assetId);
            if (checkout != null)
            {
                _context.Remove(checkout);
            }

            // close any existing checkout history
            var history = _context.CheckoutHistories.FirstOrDefault(s => s.LibraryAsset.Id == assetId && s.CheckedIn == null);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }

            _context.SaveChanges();
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }

        public void CheckInItem(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }
    }
}
