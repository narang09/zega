using NHibernate.Dialect;
using NHibernate.Dialect.Function;
namespace ZegaFinancials.Nhibernate
{
    public class MsSql2012DialectExtension : MsSql2012Dialect
    {
        public MsSql2012DialectExtension()
        {
            RegisterFunction("STRING_AGG", new StandardSQLFunction("STRING_AGG"));
        }
    }
}
