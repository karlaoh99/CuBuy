using System;

namespace CuBuy.Models
{
    public struct PagingInfo
    {
        public int TotalItems {get; set;}
        public int ItemsPerPage {get; set;}
        public int CurrentPage {get; set;}

        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage );
    }
}