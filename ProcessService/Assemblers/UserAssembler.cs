using DTO;
using Microsoft.AspNetCore.Identity;
using ProcessService.Entities;
using ProcessService.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProcessService.Assemblers
{
    public class UserAssembler
    {
        private readonly IUoW _uow;
        private readonly UserManager<ApplicationUser> _userManager;        

        public UserAssembler(IUoW uow, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;            
            _uow = uow;
        }

        public async Task<IEnumerable<UserAccount>> GetUserAccounts()
        {
            var userData = _uow.UserRepository.FindAll();
            var profileData = _uow.ProfileRepository.FindAll();

            var restaurantOwners = await _userManager.GetUsersInRoleAsync("RestaurantOwner");
            var consumers = await _userManager.GetUsersInRoleAsync("Consumer");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            var users = new List<ApplicationUser>();
            users.AddRange(restaurantOwners);
            users.AddRange(consumers);
            users.AddRange(admins);

            if (users == null)
                return null;

            var userAccounts = new List<UserAccount>();

            foreach(var user in users)
            {
                var profiles = await (from p in profileData
                                      join u in userData on p.Id equals u.ProfileId
                                      select p).ToListAsync();

                var profile = profiles.FirstOrDefault(p => p.Id == user.ProfileId);

                userAccounts.Add(
                    new UserAccount
                    {
                        Id = user.Id,
                        Name = profile.Name,
                        Confirmed = user.EmailConfirmed,
                        Email = user.Email,
                        LockedOut = user.Profile.LockedOut,
                        ProfilePicture = profile.ProfilePicsture, 
                        RestaurantFeatured = profile.RestaurantFeatured,
                    }
                );
            }            

            userAccounts = userAccounts.OrderBy(l => l.LockedOut)
                .ThenBy(p => !p.Confirmed)
                .ThenBy(p => p.RestaurantFeatured)
                .ThenBy(p => p.Name)
                .ToList();

            return userAccounts;
        }

        public async Task<UserAccount> GetUserAccount(string id)
        {
            var user = await _uow.UserRepository.FindByCondition(u => u.Id == id).FirstOrDefaultAsync();
            var profile = await _uow.ProfileRepository.FindByCondition(p => p.Id == user.ProfileId).FirstOrDefaultAsync();

            if (user == null)
                return null;

            var userAccount = new UserAccount
            {
                Id = user.Id,
                Name = profile.Name,
                Confirmed = user.EmailConfirmed,
                Email = user.Email,
                LockedOut = user.Profile.LockedOut,
                ProfilePicture = profile.ProfilePicsture,
                RestaurantFeatured = profile.RestaurantFeatured,
            };

            return userAccount;
        }

        public async Task<UserAccount> GetOwnProfile(string id)
        {
            var user = await _uow.UserRepository.FindByCondition(u => u.Id == id).FirstOrDefaultAsync();        
            var profile = await _uow.ProfileRepository.FindByCondition(p => p.Id == user.ProfileId).FirstOrDefaultAsync();                     

            var userAccount = new UserAccount
            {
                Id = user.Id,
                Name = profile.Name,
                Confirmed = user.EmailConfirmed,
                Email = user.Email,
                LockedOut = user.Profile.LockedOut,
                ProfilePicture = profile.ProfilePicsture,
                RestaurantFeatured = profile.RestaurantFeatured,
            };

            return userAccount;
        }

        public async Task<UserAccount> UpdateUserProfile(string id, UserAccount userAccount)
        {
            var user = await _uow.UserRepository.FindByCondition(u => u.Id == id).FirstOrDefaultAsync();            

            if (user == null)
                return null;

            var updatedProfile = new Profile
            {
                LockedOut = userAccount.LockedOut,
                Name = userAccount.Name,
                ProfilePicsture = userAccount.ProfilePicture,
                RestaurantFeatured = userAccount.RestaurantFeatured
            };

            _uow.ProfileRepository.Update(updatedProfile);
            await _uow.Commit();

            return userAccount;
        }

        public async Task<UserAccount> UpdateProfilePicture(string id, string imageUrl)
        {
            var user = await _uow.UserRepository.FindByCondition(u => u.Id == id).FirstOrDefaultAsync();
            var profile = await _uow.ProfileRepository.FindByCondition(p => p.Id == user.ProfileId).FirstOrDefaultAsync();

            if (user == null)
                return null;

            var updatedProfile = new Profile
            {
                LockedOut = profile.LockedOut,
                Name = profile.Name,
                ProfilePicsture = imageUrl,
                RestaurantFeatured = profile.RestaurantFeatured
            };

            _uow.ProfileRepository.Update(updatedProfile);
            user.Profile = updatedProfile;
            _uow.UserRepository.Update(user);
            await _uow.Commit();

            return new UserAccount
            {
                Confirmed = user.EmailConfirmed,
                Email = user.Email,
                LockedOut = updatedProfile.LockedOut,
                Name = updatedProfile.Name,
                ProfilePicture = updatedProfile.ProfilePicsture,
                RestaurantFeatured = updatedProfile.RestaurantFeatured
            };
        }

        public async Task<UserAccount> UpdateRestaurantFeatured(string id, bool featured)
        {
            var user = await _uow.UserRepository.FindByCondition(u => u.Id == id).FirstOrDefaultAsync();
            var profile = await _uow.ProfileRepository.FindByCondition(p => p.Id == user.ProfileId).FirstOrDefaultAsync();

            var userRole = await _userManager.IsInRoleAsync(user, "RestaurantOwner");

            if (user == null || userRole == false)
                return null;

            var updatedProfile = new Profile
            {
                LockedOut = profile.LockedOut,
                Name = profile.Name,
                ProfilePicsture = profile.ProfilePicsture,
                RestaurantFeatured = featured
            };

            user.Profile = updatedProfile;
            _uow.UserRepository.Update(user);
            _uow.ProfileRepository.Update(updatedProfile);
            await _uow.Commit();

            return new UserAccount
            {
                Confirmed = user.EmailConfirmed,
                Email = user.Email,
                LockedOut = updatedProfile.LockedOut,
                Name = updatedProfile.Name,
                ProfilePicture = updatedProfile.ProfilePicsture,
                RestaurantFeatured = updatedProfile.RestaurantFeatured
            };
        }        
    }
}
