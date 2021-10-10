using MicroService.UserService.Models;
using MicroService.UserService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.UserService.Services
{
    public class UserServiceImpl:IUserService
    {
        private readonly IUserRepository userRepository;

        public UserServiceImpl(IUserRepository userRepository) 
        {
            this.userRepository = userRepository;
        }

        public IEnumerable<User> GetUsers()
        {
            return userRepository.GetUsers();
        }

        public IEnumerable<User> GetUsers(int teamId)
        {
            return userRepository.GetUsers(teamId);
        }

        public User GetUserById(int id)
        {
            return userRepository.GetUserById(id);
        }

        public void Create(User user)
        {
            userRepository.Create(user);
        }

        public void Update(User user)
        {
            userRepository.Update(user);
        }

        public void Delete(User user)
        {
            userRepository.Delete(user);
        }

        public bool UserExists(int id)
        {
            return userRepository.UserExists(id);
        }
    }
}
