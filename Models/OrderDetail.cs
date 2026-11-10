using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lapzone_test.Models;

[Table("order_details")]
public partial class OrderDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("quantity")]
    [DisplayName("Số lượng")]
    public int Quantity { get; set; }

    [Column("price")]
    [DisplayName("Giá")]
    public double Price { get; set; }

    [Column("order_id")]
    [DisplayName("ID đơn hàng")]
    public int OrderId { get; set; }

    [Column("product_id")]
    [DisplayName("ID sản phẩm")]
    public int ProductId { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderDetails")]
    [DisplayName("Thông tin đơn hàng")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("OrderDetails")]
    [DisplayName("Thông tin sản phẩm")]
    public virtual Product Product { get; set; } = null!;
}
