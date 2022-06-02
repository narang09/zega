namespace ZegaFinancials.Nhibernate.Entities.Shared
{
    public class SortDescription
    {
        public SortDescription() : this(string.Empty, int.MaxValue)
        { }
        public SortDescription(string field, int priority)
        {
            Field = field;
            FieldDirection = "asc";
            Priority = priority;
        }

        public string FieldDirection { get; set; }

        public ListSortDirection Direction { get { return string.IsNullOrEmpty(FieldDirection) ? ListSortDirection.Ascending : FieldDirection.Equals("asc") ? ListSortDirection.Ascending : ListSortDirection.Descending; } }
        public string Field { get; set; }
        public int Priority { get; set; }
    }

    public enum ListSortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}
