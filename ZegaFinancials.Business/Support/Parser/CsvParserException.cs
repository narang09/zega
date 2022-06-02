using System;
namespace ZegaFinancials.Business.Support.Parser
{
    public sealed class CsvParserException : Exception
    {
        public CsvParserException(string message) : base(message)
        { }

    }
}
