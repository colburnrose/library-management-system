using System;
using System.Collections.Generic;
using System.Text;
using Library.Data.Models;
using Library.Data.Repositories;

namespace Library.Data.UnitOfWork
{
    public class DataRepositories
    {
        private readonly Patron _loggedInUser;
        private readonly LibraryContext _context;

        public DataRepositories(Patron loggedInUser, LibraryContext context)
        {
            if (loggedInUser == null || loggedInUser.Id < 1)
            {
                throw new ArgumentNullException(nameof(loggedInUser), "Logged in Patron is Invalid.");
            }

            _loggedInUser = loggedInUser;
            _context = context;
        }

        #region Properties

        private GenericRepository<Book> _bookRepository;
        private GenericRepository<BranchHours> _branchHoursRepository;
        private GenericRepository<Checkout> _checkoutRepository;
        private GenericRepository<Hold> _holdRepository;
        private GenericRepository<LibraryAsset> _libraryAssetRepository;
        private GenericRepository<LibraryBranch> _libraryBranchRepository;
        private GenericRepository<LibraryCard> _libraryCardRepository;
        private GenericRepository<Patron> _patronRepository;
        private GenericRepository<Status> _statusRepository;
        private GenericRepository<Video> _videoRepository;

        #endregion
    }
}
