using System;

namespace ZegaFinancials.Business.Support.Parser
{
    public sealed class XmlParserException : Exception
    {
        public XmlParserException(string message) : base(message)
        { }
    }
}
