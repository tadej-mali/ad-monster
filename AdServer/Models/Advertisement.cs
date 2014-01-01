using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdServer.Data;

namespace AdServer.Models
{
    public class Advertisement : IRowVersion
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string Url { get; set; }

        public int DirectoryId { get; set; }

        [Timestamp]
        public byte[] VersionStamp { get; set; }
    }
}