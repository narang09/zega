using System;
using System.Collections.Generic;
using System.Linq;

namespace ZegaFinancials.Business.Support.SpecificationsImport
{
    public static class MappingFieldsForImport
    {
        /// <summary>
        /// Wrap mapping fields in IDictionary interface for import
        /// </summary>
        /// <param name="mappingVariable"></param>
        /// <returns>Return IDictionary collection of fields for import data</returns>
        public static IDictionary<string, Mappings> GetMapping(List<Tuple<string, List<string>>> mappingVariable)
        {
            var returnMappings = mappingVariable.Select(mappingVar => new Mappings { Field = mappingVar.Item1, XmlFields = mappingVar.Item2}).ToList();

            return returnMappings.ToDictionary(key => key.Field);
        }
    }
}
