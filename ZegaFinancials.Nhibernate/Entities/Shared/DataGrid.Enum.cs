namespace ZegaFinancials.Nhibernate.Entities.Shared
{
    public enum DataRequestSource 
    {
        AccountListing = 1,
        ModelListing = 2,
        AdvisorListing = 3,
        SleeveListing = 4,
        StrategyListing = 5,
        AuditLogListing = 6,
        RepCodeListing = 7,
        UserContactNumbers = 8,
        UserEmails = 9,
        AdvisorRepCodes = 10,
        ModelListingSidebar = 11,
        DashboardAdmin = 12,
        DashboardAdvisor = 13,
        ImportHistory = 14,
        ModelListingSubGrid = 15,
    }
    public enum GridColumnTypes
    {
        None = 0,
        String = 1,
        Number = 2,
        Enum = 3,
        DateTime = 4,
        List = 5,
        ListWithNames = 6,
        Bool = 7,
    }

    public enum GridTemplateTypes
    {
        None = 0,
        String = 1,
        Number = 2,
        Decimal = 3,
        Percentage = 4,
        Currency = 5,
        Date = 6,
        DateTime = 7,
        Checkbox = 8,
        IpAddress = 9,
        UserActive = 10,
        Favourite = 11
    }
}
