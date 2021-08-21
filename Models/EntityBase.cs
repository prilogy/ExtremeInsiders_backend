using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Entities;

namespace ExtremeInsiders.Models
{
    public interface IWithId
    {
        public int Id { get; set; }
    }
    public class EntityBase : IWithId
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }
        [NotMapped] public string EntityType => GetType().Name.Replace("Proxy", "").ToLower();

        public EntityBase()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}