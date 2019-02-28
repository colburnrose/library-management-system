using System;
using System.Collections.Generic;
using System.Text;
using Library.Data.Models;

namespace Library.Data.Interface
{
    public interface ICheckout
    {
        IEnumerable<Checkout> GetAll();
        Checkout GetById(int checkoutId);
        Checkout GetLatestCheckout(int assetId);
        void Add(Checkout newCheckOut);
        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId, int libraryCardId);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
        string GetCurrentCheckoutPatron(int assetId);
        bool IsCheckedOut(int id);

        void PlaceHold(int assestId, int libraryCardId);
        string GetCurrentHoldPatronName(int id);
        string GetCurrentHoldPlaced(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);

        void MarkLost(int assetId);
        void MarkFound(int assetId);
    }
}
