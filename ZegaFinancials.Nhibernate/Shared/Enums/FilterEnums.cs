namespace ZegaFinancials.Nhibernate.Shared.Enums
{
    public enum DateTimeConditionTypes
    {
        WithInLast = 1,
        MoreThan = 2,
        Between = 3,
        InRange = 4
    }
    public enum StringConditionTypes
    {
        Contains = 1,
        EndsWith = 2,
        Equals = 3,
        StartsWith = 4,
        DoesNotContain = 5
    }

    public enum NumericConditionTypes
    {
        Equals = 1,
        GreaterThanOrEqual = 2,
        LessThanOrEqual = 3,
        RangeBetween = 4,
        DoesNotEqual = 5
    }

}
