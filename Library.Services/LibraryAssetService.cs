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
    public class LibraryAssetService : ILibraryAsset
    {
        private LibraryContext _context;

        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location);
        }

        public LibraryAsset GetById(int id)
        {
            return GetAll().FirstOrDefault(assets => assets.Id == id);
        }

        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }

        public string GetAuthorOrDirector(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>().Any(a => a.Id == id);

            return isBook
                ? _context.Books.FirstOrDefault(b => b.Id == id)?.Author : _context.Videos.FirstOrDefault(video => video.Id == id)?.Director ?? "Unknown";
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id)?.DeweyIndex;
            }
             return "";
        }

        public string GetType(int id)
        {
            var book = _context.LibraryAssets.OfType<Book>().Where(b => b.Id == id);

            return book.Any() ? "Book" : "Video";
        }

        public string GetTitle(int id)
        {
            return _context.LibraryAssets.FirstOrDefault(a => a.Id == id)?.Title;
        }

        public string GetIsbn(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(b => b.Id == id)?.ISBN;
            }

            return "";
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return GetById(id)?.Location;
            //return _context.LibraryAssets.FirstOrDefault(a => a.Id == id)?.Location;
        }
    }
}
