using UserTestMonnitorAPI.DBModels;
using UserTestMonnitorAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace UserTestMonnitorAPI.Services
{
    public class UserService
    {
        private IConfiguration _configuration;
        private MyDBContext _myDbContext;

        public UserService(MyDBContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _myDbContext = context;
        }

        public async Task<UserModel> Authorize(UserModel model)
        {
            try
            {
                var user = GetUser(model.username);

                if (user != null && user.Password.Equals(Encrypt(model.password)))
                {
                    IdentityOptions _options = new IdentityOptions();

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                        new Claim("UserName",user.Username.ToString()),
                        new Claim("UserId",user.UserGuid.ToString()),
                        }),
                        Expires = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JWT:Expire")),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Secret"))), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);

                    model.password = "";
                    model.expires = tokenDescriptor.Expires;
                    model.token = token;

                    await EditUser(user);

                    return model;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return null;

        }

        public User GetUser(string username)
        {
            return _myDbContext.Users.Where(c => (c.Username == username || c.Email == username)).FirstOrDefault();
        }

        public async Task<User> GetUserByGuid(Guid userGuid)
        {
            return await _myDbContext.Users.Where(c => (c.UserGuid == userGuid)).FirstOrDefaultAsync();
        }

        public async Task<Guid?> EditUser(User user)
        {
            try
            {
                user.EditedDate = DateTime.Now;
                await _myDbContext.SaveChangesAsync();
                return user.UserGuid;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<Guid?>> BlockUsers(List<Guid?> guids, bool action)
        {
            try
            {
                var users = await this.GetUsersByGuid(guids);
                foreach (var user in users)
                {
                    user.IsBlocked = action;
                    _myDbContext.Users.Update(user);
                }

                await _myDbContext.SaveChangesAsync();
                return guids;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Guid?> AddUser(OrgUserModel model)
        {
            try
            {
                User user = new User();

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Username = model.Username;
                user.Email = model.Email;
                user.Password = Encrypt(model.Password);
                user.IsBlocked = false;

                user.UserGuid = Guid.NewGuid();
                user.CreatedDate = DateTime.Now;
                user.EditedDate = DateTime.Now;

                var result = _myDbContext.AddAsync(user);
                await _myDbContext.SaveChangesAsync();
                return user.UserGuid;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<List<Guid?>> DeleteUsers(List<Guid?> guids)
        {
            try
            {
                var users = await this.GetUsersByGuid(guids);
                foreach (var user in users)
                {
                    _myDbContext.Users.Remove(user);
                }

                await _myDbContext.SaveChangesAsync();
                return guids;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<User>> GetUsersByGuid(List<Guid?> guids)
        {
            try
            {
                var result = _myDbContext.Users.Where(c => guids.Contains(c.UserGuid)).ToList();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<OrgUserModel>> GetUsers()
        {
            try
            {
                var result = _myDbContext.Users.ToList().OrderBy(c => c.CreatedDate);
                var listResult = new List<OrgUserModel>();

                foreach (var res in result)
                {
                    listResult.Add(new OrgUserModel
                    {
                        UserGuid = res.UserGuid,
                        FirstName = res.FirstName,
                        LastName = res.LastName,
                        Username = res.Username,
                        Email = res.Email,
                        IsBlocked = res.IsBlocked,
                        CreatedDate = res.CreatedDate,
                        EditedDate = res.EditedDate
                    });
                }

                return listResult;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string Encrypt(string message)
        {
            using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("EncryptSecret"))))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.ASCII.GetBytes(message));
                var builder = new StringBuilder();
                for (int i = 0; i < hashValue.Length; i++)
                {
                    builder.Append(hashValue[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}
