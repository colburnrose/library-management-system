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

        public GenericRepository<Book> BookRepository => _bookRepository ?? (_bookRepository = new GenericRepository<Book>(_loggedInUser,_context));
        public GenericRepository<BranchHours> BranchRepository => _branchHoursRepository ?? (_branchHoursRepository = new GenericRepository<BranchHours>(_loggedInUser, _context));
        public GenericRepository<Checkout> CheckoutRepository => _checkoutRepository ?? (_checkoutRepository = new GenericRepository<Checkout>(_loggedInUser, _context));
        public GenericRepository<Hold> HoldRepository => _holdRepository ?? (_holdRepository = new GenericRepository<Hold>(_loggedInUser, _context));
        public GenericRepository<LibraryAsset> LibraryAssetRepository => _libraryAssetRepository ?? (_libraryAssetRepository = new GenericRepository<LibraryAsset>(_loggedInUser, _context));
        public GenericRepository<LibraryBranch> LibraryBranchRepository => _libraryBranchRepository ?? (_libraryBranchRepository = new GenericRepository<LibraryBranch>(_loggedInUser, _context));
        public GenericRepository<LibraryCard> LibraryCardRepository => _libraryCardRepository ?? (_libraryCardRepository = new GenericRepository<LibraryCard>(_loggedInUser, _context));
        public GenericRepository<Patron> PatronRepository => _patronRepository ?? (_patronRepository = new GenericRepository<Patron>(_loggedInUser, _context));
        public GenericRepository<Status> StatusRepository => _statusRepository ?? (_statusRepository = new GenericRepository<Status>(_loggedInUser, _context));
        public GenericRepository<Video> VideoRepository => _videoRepository ?? (_videoRepository = new GenericRepository<Video>(_loggedInUser, _context));
    }
}
