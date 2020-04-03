namespace Apollo.DataAccess.EF
{
    public interface IDataContextFactory
    {
        IDbContext GetScopedDataContext();
        IDbContext GetDataContext();
    }
}
