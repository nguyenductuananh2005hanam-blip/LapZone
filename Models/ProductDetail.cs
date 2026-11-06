using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lapzone_test.Models;

[Table("product_details")]
[Index("ProductId", Name = "UQ__product___47027DF414BBA412", IsUnique = true)]
public partial class ProductDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("cpu")]
    [StringLength(255)]
    public string? Cpu { get; set; }

    [Column("ram")]
    [StringLength(255)]
    public string? Ram { get; set; }

    [Column("storage")]
    [StringLength(255)]
    public string? Storage { get; set; }

    [Column("gpu")]
    [StringLength(255)]
    public string? Gpu { get; set; }

    [Column("screen")]
    [StringLength(255)]
    public string? Screen { get; set; }

    [Column("battery")]
    [StringLength(255)]
    public string? Battery { get; set; }

    [Column("audio")]
    [StringLength(255)]
    public string? Audio { get; set; }

    [Column("ports")]
    [StringLength(255)]
    public string? Ports { get; set; }

    [Column("wireless")]
    [StringLength(255)]
    public string? Wireless { get; set; }

    [Column("weight")]
    [StringLength(100)]
    public string? Weight { get; set; }

    [Column("color")]
    [StringLength(100)]
    public string? Color { get; set; }

    [Column("os")]
    [StringLength(255)]
    public string? Os { get; set; }

    [Column("condition_status")]
    [StringLength(100)]
    public string? ConditionStatus { get; set; }

    [Column("warranty")]
    [StringLength(100)]
    public string? Warranty { get; set; }

    [Column("more_info")]
    public string? MoreInfo { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductDetail")]
    public virtual Product Product { get; set; } = null!;
}
