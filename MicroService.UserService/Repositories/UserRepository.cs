using MicroService.UserService.Context;
using MicroService.UserService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.UserService.Repositories
{
    public class UserRepository:IUserRepository
    {

        private UserContext userContext;

        public UserRepository(UserContext userContext) 
        {
            this.userContext = userContext;
        }


        public IEnumerable<User> GetUsers()
        {
            return userContext.Users.ToList();
        }


        public IEnumerable<User> GetUsers(int roleId)
        {
            return userContext.Users.Where(memeber => memeber.FRoleId == roleId);
        }


        public User GetUserById(int id)
        {
            return userContext.Users.Find(id);
        }

        public void Create(User User)
        {
            userContext.Users.Add(User);
            userContext.SaveChanges();
        }

        public void Update(User User)
        {
            userContext.Users.Update(User);
            userContext.SaveChanges();
        }

        public void Delete(User User)
        {
            userContext.Users.Remove(User);
            userContext.SaveChanges();
        }

        public bool UserExists(int id)
        {
            return userContext.Users.Any(e => e.FId == id);
        }
    }
}
