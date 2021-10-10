using MicroService.UserService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.UserService.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();

        IEnumerable<User> GetUsers(int teamId);

        User GetUserById(int id);

        void Create(User user);

        void Update(User user);

        void Delete(User user);

        bool UserExists(int id);
    }
}
