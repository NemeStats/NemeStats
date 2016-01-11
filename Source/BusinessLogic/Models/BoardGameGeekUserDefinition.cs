using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;

namespace BusinessLogic.Models
{
    public class BoardGameGeekUserDefinition : EntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }

        public string Avatar { get; set; }


        [Index("IX_USERID_AND_NAME", 1, IsUnique = true)]
        [StringLength(128)]
        [Key, ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Index("IX_USERID_AND_NAME", 2, IsUnique = true)]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
    }
}