using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZegaFinancials.Services
{
    public class ZegaServiceException :Exception
    {
		public ZegaServiceException()
		{
		}
		public ZegaServiceException(string message) : base(message)
		{
		}
		public ZegaServiceException(string message, params object[] args) : base(string.Format(message, args))
		{
		}
		public ZegaServiceException(string message, Exception inner) : base(message, inner)
		{
		}
		public ZegaServiceException(Exception inner)
			: base(inner.Message, inner)
		{
		}
	}
}
