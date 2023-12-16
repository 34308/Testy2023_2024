namespace JJ_API.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public interface IDbConnection : IDisposable
    {
        void Open();
        void QueryFirstOrDefault();
        IDbTransaction BeginTransaction();
        // Other necessary members
    }

    public interface IDbTransaction : IDisposable
    {
        void Commit();
        void Rollback();
        // Other necessary members
    }
}
