using UserService.Models;

namespace UserService.Services
{
   
    public interface IUserService
    {
        Task<int> Register(UserEntity user);
        Task<UserModel> Login(string userName, string password);
        Task<List<UserModel>> GetUserList();
        Task<UserModel> GetUserById(int id);
    }

    public class UserService : IUserService
    {
        public static List<UserEntity> _Users = new List<UserEntity>();
        //{
        //    new UserEntity { Id = 1, UserName = "john.doe@example.com", Password = "123", Role="Admin" },
        //    new UserEntity { Id = 2, UserName = "jane.smith@example.com", Password = "1234", Role = "User" },
        //    new UserEntity { Id = 3, UserName = "mike.johnson@example.com", Password = "12345", Role = "SuperAdmin" }
        //};

        public async Task<int> Register(UserEntity user)
        {
            int id = _Users.LastOrDefault()?.Id ?? 1;
            bool isExist = _Users.Any(x => x.UserName == user.UserName);

            if (!isExist)
            {
                _Users.Add(new UserEntity()
                {
                    Id = id,
                    UserName = user.UserName,
                    Password = user.Password,
                    Role = user.Role,
                    BalanceLeave = user.BalanceLeave,
                });
            }
            else
            {
                return 0;
            }

            return id;
        }

        public async Task<UserModel> Login(string userName, string password)
        {
            UserModel? user = new UserModel();
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                user = _Users.Where(x => x.UserName == userName && x.Password == password).Select(x=> new UserModel()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Role = x.Role
                }).FirstOrDefault();
            }
            return user;
        }

        public async Task<List<UserModel>> GetUserList()
        {
            List<UserModel>? users = new List<UserModel>();
            
            users = _Users.Select(x => new UserModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                Role = x.Role
            }).ToList();
            
            return users;
        }

        public async Task<UserModel> GetUserById(int id)
        {
            UserModel? user = new UserModel();
            
            user = _Users.Where(x => x.Id == id).Select(x => new UserModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                Role = x.Role
            }).FirstOrDefault();
            
            return user;
        }
    }
}
