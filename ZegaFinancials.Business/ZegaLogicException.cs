using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZegaFinancials.Business
{
  public class ZegaLogicException :Exception
    {
		public bool IsHandled { get; set; }
		public ZegaLogicException()
			: base()
		{
		}
		public ZegaLogicException(string message)
			: base(message)
		{
		}
		public ZegaLogicException(string message, params object[] args)
			: base(string.Format(message, args))
		{
		}
		public ZegaLogicException(string message, Exception inner)
			: base(message, inner)
		{
		}
		public ZegaLogicException(Exception inner)
			: base(inner.Message, inner)
		{
		}
	}
}
