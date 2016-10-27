using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class UserDeviceAuthToken :  EntityWithTechnicalKey<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override Guid Id { get; set; }

        [Required]
        [Index("IX_ApplicationUserId_DeviceId", 1, IsUnique = true)]
        public string ApplicationUserId { get; set; }

        [Required]
        [Index("IX_ApplicationUserId_DeviceId", 2, IsUnique = true)]
        public string DeviceId { get; set; }

        public DateTime? AuthenticationTokenExpirationDate { get; set; }
    }
}
