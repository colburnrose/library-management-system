using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Web.Models.Catalog
{
    public class AssetIndexModel
    {
        public IEnumerable<AssetListingViewModel> Assets { get; set; }
    }
}
