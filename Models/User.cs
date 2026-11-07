using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lapzone_test.Models;

[Table("users")]
[Index("Email", Name = "UQ__users__AB6E6164FD8B2E53", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    [Display(Name = "Mã người dùng")]
    public int Id { get; set; }

    [Column("full_name")]
    [StringLength(100)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    [Unicode(false)]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Column("phone")]
    [StringLength(20)]
    [Unicode(false)]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [Column("address")]
    [DisplayName("Địa chỉ")]
    public string? Address { get; set; }

    [Column("role")]
    [StringLength(20)]
    [Unicode(false)]
    [DisplayName("Vai trò")]
    public string Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("User")]
    
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
