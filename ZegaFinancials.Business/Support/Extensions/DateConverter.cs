using System;

namespace ZegaFinancials.Business.Support.Extensions
{
    public static class DateConverter
    {
		public static string ConvertToStringTime(int hour, int minute)
		{
			return string.Format("{0:00}:{1:00}", hour, minute);
		}

		public static int[] ConvertToIntTime(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			var time = str.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var datetime = new int[2];

			if (time.Length == 3 && time[2] == "PM")
			{
				datetime[0] = Convert.ToInt32(time[0]) + 12;
				datetime[0] = datetime[0] == 24 ? 0 : datetime[0];
			}
			else
				datetime[0] = Convert.ToInt32(time[0]);

			datetime[1] = Convert.ToInt32(time[1]);
			return datetime;
		}
	}
}
