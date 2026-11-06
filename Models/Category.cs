using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lapzone_test.Models;

[Table("categories")]
[Index("Name", Name = "UQ__categori__72E12F1B040BB55E", IsUnique = true)]
public partial class Category
{
    [Key]
    [Column("id")]
    [DisplayName("Mã danh mục")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(100)]
    [DisplayName("Tên danh mục")]
    public string Name { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
