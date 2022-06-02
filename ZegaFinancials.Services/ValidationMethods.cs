using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZegaFinancials.Services
{
    public static class ValidationMethods
    {   
        public static bool DoPenetrationCheck(string inputValue)
        {
            return string.IsNullOrEmpty(inputValue) || !Regex.IsMatch(inputValue, @"^(\s*)[-+@=!#$%^&* ]");
        }
    }
}
