using System.Collections.Generic;

namespace CuBuy.Infraestructure
{
    public static class SelectOptions
    {
        public static string[] ProductCategories = new string[] {
            "Computers",
            "Vehicles",
            "Electronics",
            "Home",
            "Beauty",
            "Furniture",
            "Clothes",
            "Other"
        };

        public static List<(int, string)> Months = new List<(int, string)>{

            (1,"January"),
            (2,"February"),
            (3,"March"),
            (4,"April"),
            (5,"May"),
            (6,"June"),
            (7,"July"),
            (8,"August"),
            (9,"September"),
            (10,"October"),
            (11,"November"),
            (12,"December"),
        };
    }
}