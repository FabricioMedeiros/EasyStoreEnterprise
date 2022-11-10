using System.Threading.Tasks;

namespace ESE.Core.Data
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}
