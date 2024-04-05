using System.Threading.Tasks;

namespace CityLibrary.Shared.DbBase.SQL.UnitOfWorks
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        void Commit();
    }
}
