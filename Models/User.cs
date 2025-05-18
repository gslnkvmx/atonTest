using System;
using System.ComponentModel.DataAnnotations;

namespace atonTest.Models;

public class User
{
  public Guid Id { get; set; }

  [Required(ErrorMessage = "Логин обязателен")]
  [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только латинские буквы и цифры")]
  public string Login { get; set; }

  [Required(ErrorMessage = "Пароль обязателен")]
  [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Пароль может содержать только латинские буквы и цифры")]
  public string Password { get; set; }

  [Required(ErrorMessage = "Имя обязательно")]
  [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Имя может содержать только латинские и русские буквы")]
  public string Name { get; set; }

  [Required(ErrorMessage = "Пол обязателен")]
  [Range(0, 2, ErrorMessage = "Пол должен быть 0 (женщина), 1 (мужчина) или 2 (неизвестно)")]
  public int Gender { get; set; }
  
  public DateTime? Birthday { get; set; }

  public bool Admin { get; set; }

  [Required]
  public DateTime CreatedOn { get; set; }

  [Required]
  public string CreatedBy { get; set; }

  [Required]
  public DateTime ModifiedOn { get; set; }

  [Required]
  public string ModifiedBy { get; set; }

  public DateTime? RevokedOn { get; set; }

  public string RevokedBy { get; set; }
}