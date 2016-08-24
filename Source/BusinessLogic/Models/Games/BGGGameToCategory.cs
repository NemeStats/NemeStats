using BusinessLogic.DataAccess;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessLogic.Models
{
    public class BGGGameToCategory : EntityWithTechnicalKey<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public int BoardGameGeekGameDefinitionId { get; set; }
        public int BoardGameGeekGameCategoryId { get; set; }
    }
}
