using System;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
    public class FcmToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string Token { get; set; }

        public int UserId { get; set; }
        [JsonIgnore] public virtual User User { get; set; }

        public DateTime DateCreated { get; set; }

        public FcmToken()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}