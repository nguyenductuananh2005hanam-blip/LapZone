using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lapzone_test.Models;

[Table("orders")]
public partial class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("total_amount")]
    public double TotalAmount { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("order_date", TypeName = "datetime")]
    public DateTime? OrderDate { get; set; }

    [Column("customer_name")]
    [StringLength(100)]
    public string? CustomerName { get; set; }

    [Column("customer_phone")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CustomerPhone { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }
}
