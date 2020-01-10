using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;

namespace openld.Services {
    public class UserService : IUserService {
        private readonly OpenLDContext _context;
        public UserService(OpenLDContext context) {
            _context = context;
        }

        public async Task<User> GetUserDetailsAsync(string id) {
            User user;
            try {
                user = await _context.Users.FirstAsync(u => u.Id == id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("User ID not found");
            }

            return new User { Id = user.Id, UserName = user.UserName, Email = user.Email };
        }
    }

    public interface IUserService {
        Task<User> GetUserDetailsAsync(string id);
    }
}