using System;
using System.Threading.Tasks;

namespace ProcessService.Interfaces
{
    public interface IUoW
    {        
        IUserRepository UserRepository { get; }
        IProfileRepository ProfileRepository { get; }

        Task Commit();
    }
}
