using System;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions options) : base(options)
        {

        }

        //map to the entity objs properties and those columns listed in the db tables.
        public DbSet<Patron> Patrons { get; set; }
    }
}
