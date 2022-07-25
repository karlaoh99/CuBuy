using System;

namespace CuBuy.Infraestructure
{
    public class BankSystem
    {
        static Random r = new Random();
        public static bool ValidCreditCard(long number) =>  r.Next(1, 51) % 50 != 0;
        public static bool Pay(long number) => r.Next(1,31) % 30 != 0;
    }
}