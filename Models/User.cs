using System;
using System.ComponentModel.DataAnnotations;

namespace atonTest.Models;

/// <summary>
/// Модель пользователя системы
/// </summary>
public class User
{
  /// <summary>
  /// Уникальный идентификатор пользователя
  /// </summary>
  public Guid Id { get; set; }

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

  /// <summary>
  /// Дата создания записи
  /// </summary>
  [Required]
  public DateTime CreatedOn { get; set; }

  /// <summary>
  /// Логин пользователя, создавшего запись
  /// </summary>
  [Required]
  public required string CreatedBy { get; set; }

  /// <summary>
  /// Дата последнего изменения записи
  /// </summary>
  [Required]
  public DateTime ModifiedOn { get; set; }

  /// <summary>
  /// Логин пользователя, изменившего запись
  /// </summary>
  [Required]
  public required string ModifiedBy { get; set; }

  /// <summary>
  /// Дата отзыва доступа
  /// </summary>
  public DateTime? RevokedOn { get; set; }

  /// <summary>
  /// Логин пользователя, отозвавшего доступ
  /// </summary>
  public required string RevokedBy { get; set; }
}