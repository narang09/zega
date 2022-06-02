using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZegaFinancials.Nhibernate
{
    public class ZegaDaoException :Exception
    {
		public ZegaDaoException()
			: base()
		{
		}
		public ZegaDaoException(string message)
			: base(message)
		{
		}
		public ZegaDaoException(string message, params object[] args)
			: base(string.Format(message, args))
		{
		}
		public ZegaDaoException(string message, Exception inner)
			: base(message, inner)
		{
		}
		public ZegaDaoException(Exception inner)
			: base(inner.Message, inner)
		{
		}
	}
}
