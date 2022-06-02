using System;
using System.Globalization;

namespace ZegaFinancials.Nhibernate.Shared.Models
{
    public class DateConfigModel
    {
        public DateConfigModel()
        {
            singleDatePicker = false;
            startDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            endDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public bool singleDatePicker { get; set; }
    }
}
