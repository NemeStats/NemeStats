using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class UserDeviceAuthToken :  EntityWithTechnicalKey<int>
    {
        [Key]
        public override int Id { get; set; }

        [Required]
        [MaxLength(128)]
        [Index("IX_Authentication_Token", 1, IsUnique = true)]
        public string AuthenticationToken { get; set; }

        [Required]
        [MaxLength(128)]
        [Index("IX_ApplicationUserId_DeviceId", 1, IsUnique = true)]
        public string ApplicationUserId { get; set; }

        [MaxLength(128)]
        [Index("IX_ApplicationUserId_DeviceId", 2, IsUnique = true)]
        public string DeviceId { get; set; }

        public DateTime AuthenticationTokenExpirationDate { get; set; }
    }
}
