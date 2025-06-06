using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using atonTest.Services;
using Microsoft.AspNetCore.Authorization;

namespace atonTest.Controllers;

/// <summary>
/// Контроллер для управления пользователями
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
  private readonly UserService _userService;

  public UsersController(UserService userService)
  {
    _userService = userService;
  }

  /// <summary>
  /// Создает нового пользователя (только для администраторов)
  /// </summary>
  /// <param name="request">Данные для создания пользователя</param>
  /// <returns>Созданный пользователь</returns>
  [HttpPost]
  [Authorize(Roles = "admin")]
  public IActionResult CreateUser([FromBody] UserCreateRequest request)
  {


    try
    {
      var createdUser = _userService.CreateUser(request);
      return Ok(createdUser);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Обновляет информацию о пользователе
  /// </summary>
  /// <param name="login">Логин пользователя</param>
  /// <param name="request">Данные для обновления</param>
  /// <returns>Обновленный пользователь</returns>
  [HttpPut("{login}")]
  public IActionResult UpdateUser(string login, [FromBody] UserUpdateRequest request)
  {


    try
    {
      var user = _userService.UpdateUser(login, request.Name, request.Gender, request.Birthday);
      return Ok(user);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Обновляет пароль пользователя
  /// </summary>
  /// <param name="login">Логин пользователя</param>
  /// <param name="request">Новый пароль</param>
  /// <returns>Обновленный пользователь</returns>
  [HttpPut("{login}/password")]
  public IActionResult UpdatePassword(string login, [FromBody] PasswordUpdateRequest request)
  {


    try
    {
      var user = _userService.UpdatePassword(login, request.NewPassword);
      return Ok(user);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Обновляет логин пользователя
  /// </summary>
  /// <param name="login">Текущий логин пользователя</param>
  /// <param name="request">Новый логин</param>
  /// <returns>Обновленный пользователь</returns>
  [HttpPut("{login}/login")]
  public IActionResult UpdateLogin(string login, [FromBody] LoginUpdateRequest request)
  {


    try
    {
      var user = _userService.UpdateLogin(login, request.NewLogin);
      return Ok(user);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Получает список активных пользователей (только для администраторов)
  /// </summary>
  /// <returns>Список активных пользователей</returns>
  [HttpGet("active")]
  [Authorize(Roles = "admin")]
  public IActionResult GetActiveUsers()
  {
    try
    {
      var users = _userService.GetActiveUsers();
      return Ok(users);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Получает информацию о пользователе по логину (только для администраторов)
  /// </summary>
  /// <param name="login">Логин пользователя</param>
  /// <returns>Информация о пользователе</returns>
  [HttpGet("{login}")]
  [Authorize(Roles = "admin")]
  public IActionResult GetUserByLogin(string login)
  {
    try
    {
      var user = _userService.GetUserByLogin(login);
      if (user == null)
        return NotFound();
      return Ok(user);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Получает полную информацию о пользователе по логину и паролю
  /// </summary>
  /// <param name="login">Логин пользователя</param>
  /// <param name="request">Пароль пользователя</param>
  /// <returns>Полная информация о пользователе</returns>
  [HttpGet("{login}/fullInfo")]
  public IActionResult GetUserByLoginAndPassword(string login, [FromBody] GetUserByLoginAndPasswordRequest request)
  {
    try
    {
      var user = _userService.GetUserByLoginAndPassword(login, request.Password);
      if (user == null)
        return NotFound();
      return Ok(user);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Получает список пользователей старше указанного возраста (только для администраторов)
  /// </summary>
  /// <param name="age">Возраст</param>
  /// <returns>Список пользователей</returns>
  [HttpGet("older-than/{age}")]
  [Authorize(Roles = "admin")]
  public IActionResult GetUsersOlderThan(int age)
  {
    try
    {
      var users = _userService.GetUsersOlderThan(age);
      return Ok(users);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Удаляет пользователя (только для администраторов)
  /// </summary>
  /// <param name="login">Логин пользователя</param>
  /// <param name="softDelete">Флаг мягкого удаления</param>
  /// <returns>Результат операции</returns>
  [HttpDelete("{login}")]
  [Authorize(Roles = "admin")]
  public IActionResult DeleteUser(string login, [FromQuery] bool softDelete)
  {
    try
    {
      _userService.DeleteUser(login, softDelete);
      return Ok();
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Восстанавливает удаленного пользователя (только для администраторов)
  /// </summary>
  /// <param name="login">Логин пользователя</param>
  /// <returns>Восстановленный пользователь</returns>
  [HttpPut("{login}/restore")]
  [Authorize(Roles = "admin")]
  public IActionResult RestoreUser(string login)
  {
    try
    {
      var user = _userService.RestoreUser(login);
      return Ok(user);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }
}

/// <summary>
/// Модель запроса для создания пользователя
/// </summary>
public record UserCreateRequest
{
  /// <summary>
  /// Логин пользователя (только латинские буквы и цифры)
  /// </summary>
  [Required(ErrorMessage = "Логин обязателен")]
  [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только латинские буквы и цифры")]
  public required string Login { get; set; }

  /// <summary>
  /// Пароль пользователя (только латинские буквы и цифры)
  /// </summary>
  [Required(ErrorMessage = "Пароль обязателен")]
  [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Пароль может содержать только латинские буквы и цифры")]
  public required string Password { get; set; }

  /// <summary>
  /// Имя пользователя (латинские и русские буквы)
  /// </summary>
  [Required(ErrorMessage = "Имя обязательно")]
  [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Имя может содержать только латинские и русские буквы")]
  public required string Name { get; set; }

  /// <summary>
  /// Пол пользователя (0 - женщина, 1 - мужчина, 2 - неизвестно)
  /// </summary>
  [Required(ErrorMessage = "Пол обязателен")]
  [Range(0, 2, ErrorMessage = "Пол должен быть 0 (женщина), 1 (мужчина) или 2 (неизвестно)")]
  public int Gender { get; set; }

  /// <summary>
  /// Дата рождения пользователя
  /// </summary>
  public DateTime? Birthday { get; set; }

  /// <summary>
  /// Флаг администратора
  /// </summary>
  public bool Admin { get; set; }
}

/// <summary>
/// Модель запроса для обновления информации о пользователе
/// </summary>
public record UserUpdateRequest
{
  /// <summary>
  /// Новое имя пользователя (латинские и русские буквы)
  /// </summary>
  [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Имя может содержать только латинские и русские буквы")]
  public required string Name { get; set; }

  /// <summary>
  /// Новый пол пользователя (0 - женщина, 1 - мужчина, 2 - неизвестно)
  /// </summary>
  [Range(0, 2, ErrorMessage = "Пол должен быть 0 (женщина), 1 (мужчина) или 2 (неизвестно)")]
  public int? Gender { get; set; }

  /// <summary>
  /// Новая дата рождения пользователя
  /// </summary>
  public DateTime? Birthday { get; set; }
}

/// <summary>
/// Модель запроса для обновления пароля пользователя
/// </summary>
public record PasswordUpdateRequest
{
  /// <summary>
  /// Новый пароль пользователя (только латинские буквы и цифры)
  /// </summary>
  [Required(ErrorMessage = "Новый пароль обязателен")]
  [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Пароль может содержать только латинские буквы и цифры")]
  public required string NewPassword { get; set; }
}

/// <summary>
/// Модель запроса для обновления логина пользователя
/// </summary>
public record LoginUpdateRequest
{
  /// <summary>
  /// Новый логин пользователя (только латинские буквы и цифры)
  /// </summary>
  [Required(ErrorMessage = "Новый логин обязателен")]
  [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только латинские буквы и цифры")]
  public required string NewLogin { get; set; }
}

/// <summary>
/// Модель запроса для получения информации о пользователе по логину и паролю
/// </summary>
public record GetUserByLoginAndPasswordRequest
{
  /// <summary>
  /// Пароль пользователя
  /// </summary>
  public required string Password { get; set; }
}