﻿namespace CoffeeShop.Enumerations
{
    using System;

    public static class CoffeeSize
    {
        public static String SMALL = "Small";

        public static String MEDIUM = "Medium";

        public static String BIG = "Big";

        public static string[] getCoffeeSizes()
        {
            return new string[]
            {
            SMALL,
            MEDIUM,
            BIG
            };
        }
    
    }

}