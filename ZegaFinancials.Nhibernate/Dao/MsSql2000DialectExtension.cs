
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace ZegaFinancials.Nhibernate.Dao
{
   public  class MsSql2000DialectExtension : MsSql2000Dialect
    {
        public MsSql2000DialectExtension()
        {
            RegisterFunction("STRING_AGG", new StandardSQLFunction("STRING_AGG"));
        }
    }
}
