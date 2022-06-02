using System;
using System.Collections.Generic;

namespace ZegaFinancials.Business.Support.SpecificationsImport
{
    /// <summary>
    /// Class contains field for import data from any XML file
    /// </summary>
    public static class SpecificFieldsForImport
    {
        #region TDVeo Account fields

        private static readonly List<Tuple<string, List<string>>> _TDVeoAccountVariables = new List<Tuple<string, List<string>>>
                                                                                                {
                                                                                                    Tuple.Create("AccountNumber", new List<string>{ "accountNumber" }),
                                                                                                    Tuple.Create("ClientName", new List<string>{ "accountDescription" }),
                                                                                                    Tuple.Create("RepCode", new List<string>{ "repCode" }),
                                                                                                    Tuple.Create("AccountValue", new List<string>{ "accountValue" }),
                                                                                                    Tuple.Create("CashNetBal", new List<string>{ "netBalance" }),
                                                                                                    Tuple.Create("CashEq", new List<string>{ "cashEquivalent" }),
                                                                                                    Tuple.Create("SBuyingPower", new List<string>{ "buyingPower" }),
                                                                                                    Tuple.Create("OBuyingPower", new List<string>{ "optionBuyingPower" })
                                                                                                };

        /// <summary>
        /// Get Account fields mapping for "TD Veo" XML file
        /// </summary>
        /// <returns>List of Account fields</returns>
        public static List<Tuple<string, List<string>>> GetTDVeoAccount()
        {
            return _TDVeoAccountVariables;
        }

        #endregion
    }
}
