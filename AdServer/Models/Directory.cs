using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdServer.Data;

namespace AdServer.Models
{
    public class Directory : IEntityWithId, IRowVersion
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Advertisement> Advertisements { get; set; }

        [NotMapped]
        public int ItemsCount { get; set; }

        public byte[] VersionStamp { get; set; }
    }
}