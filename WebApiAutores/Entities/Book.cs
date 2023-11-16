﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiAutores.Entities;

[Table("books", Schema = "transacctional")]
public class Book
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]// para especificar que sea una PK y que si se genere el GUID
    [Column("id")]
    public Guid Id { get; set; }
    [Column("isbn")]
    [StringLength(50)]
    [Required]
    public string ISBN { get; set; }
    [Column("title")]
    [StringLength(50)]
    [Required]
    public string Title { get; set; }
    [Column("publication_date")]
    [DataType(DataType.Date)]
    public DateTime PublicationDate { get; set; }
    [Column("autor_id")]
    public int AutorId { get; set; }
    [ForeignKey(nameof(AutorId))]
    public virtual Autor Autor { get; set; }
}