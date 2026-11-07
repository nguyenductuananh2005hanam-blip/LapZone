using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lapzone_test.Models;

[Table("products")]
public partial class Product
{
    [Key]
    [Column("id")]
    [DisplayName("Mã sản phẩm")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    [DisplayName("Tên sản phẩm")]
    public string Name { get; set; } = null!;

    [Column("price")]
    [DisplayName("Giá")]
    public double Price { get; set; }

    [Column("stock")]
    [DisplayName("Số lượng")]
    public int Stock { get; set; }

    [Column("image")]
    [StringLength(255)]
    [Unicode(false)]
    [DisplayName("Hình ảnh")]
    public string? Image { get; set; }

    [Column("category_id")]
    [DisplayName("Mã danh mục")]
    public int CategoryId { get; set; }

    [InverseProperty("Product")]
    [DisplayName("Giỏ hàng")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    [DisplayName("Danh mục")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    [DisplayName("Chi tiết đơn hàng")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [InverseProperty("Product")]
    [DisplayName("Cấu hình")]
    public virtual ProductDetail? ProductDetail { get; set; }
}
