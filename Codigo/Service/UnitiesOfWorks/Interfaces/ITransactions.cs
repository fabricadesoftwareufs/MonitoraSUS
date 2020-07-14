namespace Service.UnitiesOfWorks.Interfaces
{
    public interface ITransactions
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
