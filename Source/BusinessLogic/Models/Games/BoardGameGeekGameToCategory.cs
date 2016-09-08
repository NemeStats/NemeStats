using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models.Games
{
    public class BoardGameGeekGameToCategory : EntityWithTechnicalKey<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Index("IX_BGGGAMEDEFINITIONID_BGGGAMECATEGORYID", 1, IsUnique = true)]
        public int BoardGameGeekGameDefinitionId { get; set; }

        [Index("IX_BGGGAMEDEFINITIONID_BGGGAMECATEGORYID", 2, IsUnique = true)]
        public int BoardGameGeekGameCategoryId { get; set; }

        public virtual BoardGameGeekGameDefinition BoardGameGeekGameDefinition { get; set; }

        public virtual BoardGameGeekGameCategory BoardGameGeekGameCategory { get; set; }
    }
}
