using System;
using System.Threading.Tasks;
using ProcessService.Context;
using ProcessService.Interfaces;
using ProcessService.Repositories;

namespace ProcessService.UnitOfWork
{
    public class UoW : IUoW
    {
        private readonly ApplicationDbContext _context;

        private IUserRepository _userRepository;
        private IProfileRepository _profileRepository;

        public UoW(ApplicationDbContext context)
        {
            _context = context;
        }
                
        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new UserRepository(_context);

                return _userRepository;
            }
        }


        public IProfileRepository ProfileRepository
        {
            get
            {
                if (_profileRepository == null)
                    _profileRepository = new ProfileRepository(_context);

                return _profileRepository;
            }
        }




        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }
    }
}
