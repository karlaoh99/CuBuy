using CuBuy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CuBuy.ViewModels
{
    public class ListModel
    {
        [DataType(DataType.Currency, ErrorMessage = "Error")]
        public float? MinPrice { get; set; }

        [DataType(DataType.Currency)]
        public float? MaxPrice { get; set; }

        public string Category { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class ListAuctionsModel : ListModel
    {
        public IEnumerable<Auction> Auctions { get; set; }
    }

    public class MakeABidModel
    {
        public Auction Auction { get; set; }

        [Required(ErrorMessage = "Please enter your bid")]
        [DataType(DataType.Currency)]
        public float Price { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class AuctionInsertModel
    {
        [Required(ErrorMessage = "Enter the product of the auction")]
        public string Product { get; set; }

        [Required(ErrorMessage = "Enter the start price of the auction")]
        [Range(0.01, 10e8)]
        public float Price { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Choose the category of the product")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Enter the final date of the auction")]
        public DateTime FinalDate { get; set; }
    }

    //Payment of the product in the auction
    public class AuctionJoinViewModel
    {
        public Auction AuctionProduct { get; set; }

        public string ReturnUrl { get; set; }

        [RegularExpression("[0-9]+", ErrorMessage = "Please enter only numbers")]
        public string CreditCard { get; set; }
    }
}
