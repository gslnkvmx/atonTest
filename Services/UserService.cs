using System;
using System.Collections.Generic;
using System.Linq;
using atonTest.Controllers;
using Microsoft.EntityFrameworkCore;
using atonTest.Data;
using atonTest.Models;
using System.Security.Cryptography;
using System.Text;

namespace atonTest.Services;

public class UserService
{
  private readonly ApplicationDbContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public UserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
  {
    _context = context;
    _httpContextAccessor = httpContextAccessor;
    EnsureAdminExists();
  }

  private string HashPassword(string password)
  {
    var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(hashedBytes);
  }

  private bool VerifyPassword(string password, string hashedPassword)
  {
    var hashedInput = HashPassword(password);
    return hashedInput == hashedPassword;
  }

  private void EnsureAdminExists()
  {
    if (_context.Users.Any(u => u.Login == "Admin")) return;
    var admin = new User
    {
      Id = Guid.NewGuid(),
      Login = "Admin",
      Password = HashPassword("Admin123"),
      Name = "Administrator",
      Gender = 1,
      Admin = true,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = "System",
      ModifiedOn = DateTime.UtcNow,
      ModifiedBy = "System",
      RevokedBy = "",
    };
    _context.Users.Add(admin);
    _context.SaveChanges();
  }

  public User CreateUser(UserCreateRequest request)
  {
    if (_context.Users.Any(u => u.Login == request.Login))
      throw new Exception("Пользователь с таким логином уже существует");

    var creator = _httpContextAccessor.HttpContext.User.FindFirst("login").Value;

    var user = new User
    {
      Id = Guid.NewGuid(),
      Login = request.Login,
      Password = HashPassword(request.Password),
      Name = request.Name,
      Gender = request.Gender,
      Birthday = request.Birthday,
      Admin = request.Admin,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = creator,
      ModifiedOn = DateTime.UtcNow,
      ModifiedBy = creator,
      RevokedBy = ""
    };

    _context.Users.Add(user);
    _context.SaveChanges();
    return user;
  }

  public User UpdateUser(string login, string name, int? gender, DateTime? birthday)
  {
    var user = _context.Users.FirstOrDefault(u => u.Login == login);
    if (user == null)
      throw new Exception("Пользователь не найден");

    var role = _httpContextAccessor.HttpContext.User.IsInRole("admin");
    if (!role)
    {
      VerifyUser(login);
    }

    var modifiedBy = _httpContextAccessor.HttpContext.User.FindFirst("login").Value;

    if (name != null) user.Name = name;
    if (gender.HasValue) user.Gender = gender.Value;
    if (birthday.HasValue) user.Birthday = birthday;

    user.ModifiedOn = DateTime.UtcNow;
    user.ModifiedBy = modifiedBy;

    _context.SaveChanges();
    return user;
  }

  public User UpdatePassword(string login, string newPassword)
  {
    var user = _context.Users.FirstOrDefault(u => u.Login == login);
    if (user == null)
      throw new Exception("Пользователь не найден");

    var role = _httpContextAccessor.HttpContext.User.IsInRole("admin");
    if (!role)
    {
      VerifyUser(login);
    }

    var modifiedBy = _httpContextAccessor.HttpContext.User.FindFirst("login").Value;

    user.Password = HashPassword(newPassword);
    user.ModifiedOn = DateTime.UtcNow;
    user.ModifiedBy = modifiedBy;

    _context.SaveChanges();
    return user;
  }

  public User UpdateLogin(string oldLogin, string newLogin)
  {
    var user = _context.Users.FirstOrDefault(u => u.Login == oldLogin);
    if (user == null)
      throw new Exception("Пользователь не найден");

    var role = _httpContextAccessor.HttpContext.User.IsInRole("admin");
    if (!role)
    {
      VerifyUser(oldLogin);
    }

    var modifiedBy = _httpContextAccessor.HttpContext.User.FindFirst("login").Value;

    if (_context.Users.Any(u => u.Login == newLogin))
      throw new Exception("Пользователь с таким логином уже существует");

    user.Login = newLogin;
    user.ModifiedOn = DateTime.UtcNow;
    user.ModifiedBy = modifiedBy;

    _context.SaveChanges();
    return user;
  }

  public IEnumerable<User> GetActiveUsers()
  {
    return _context.Users
        .Where(u => !u.RevokedOn.HasValue)
        .OrderBy(u => u.CreatedOn)
        .ToList();
  }

  public User GetUser(string login, string password)
  {
    var user = _context.Users.FirstOrDefault(u => u.Login == login);
    if (user == null || !VerifyPassword(password, user.Password))
      return null;

    if (user.RevokedOn.HasValue)
      throw new Exception("Пользователь неактивен");

    return user;
  }

  public User GetUserByLogin(string login)
  {
    return _context.Users.FirstOrDefault(u => u.Login == login);
  }

  public User GetUserByLoginAndPassword(string login, string password)
  {
    var user = _context.Users.FirstOrDefault(u => u.Login == login);
    if (user == null || !VerifyPassword(password, user.Password))
      return null;

    VerifyUser(login);

    return user;
  }

  public IEnumerable<User> GetUsersOlderThan(int age)
  {
    var cutoffDate = DateTime.UtcNow.AddYears(-age);
    return _context.Users
        .Where(u => u.Birthday.HasValue && u.Birthday.Value <= cutoffDate)
        .ToList();
  }

  public void DeleteUser(string login, bool softDelete)
  {
    var user = _context.Users.FirstOrDefault(u => u.Login == login);
    if (user == null)
      throw new Exception("Пользователь не найден");

    var revokedBy = _httpContextAccessor.HttpContext.User.FindFirst("login").Value;

    if (softDelete)
    {
      user.RevokedOn = DateTime.UtcNow;
      user.RevokedBy = revokedBy;
    }
    else
    {
      _context.Users.Remove(user);
    }

    _context.SaveChanges();
  }

  public User RestoreUser(string login)
  {
    var user = _context.Users.FirstOrDefault(u => u.Login == login);
    if (user == null)
      throw new Exception("Пользователь не найден");

    user.RevokedOn = null;
    user.RevokedBy = null;
    user.ModifiedOn = DateTime.UtcNow;

    _context.SaveChanges();
    return user;
  }

  private User VerifyUser(string login)
  {
    var user = _context.Users.FirstOrDefault(u => u.Login == login);
    if (user == null)
      throw new Exception("Пользователь не найден");

    if (user.RevokedOn.HasValue)
      throw new Exception("Пользователь неактивен");

    if (login != _httpContextAccessor.HttpContext.User.FindFirst("login").Value)
      throw new Exception("Вам недоступна данная операция");

    return user;
  }
}