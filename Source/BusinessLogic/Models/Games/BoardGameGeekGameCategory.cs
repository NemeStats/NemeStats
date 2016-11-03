using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models.Games
{
    public class BoardGameGeekGameCategory : EntityWithTechnicalKey<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Index("IX_BOARDGAMEGEEKCATEGORYID", 1, IsUnique = true)]
        public int BoardGameGeekGameCategoryId { get; set; }

        public string CategoryName { get; set; }

        public virtual IList<BoardGameGeekGameDefinition> Games { get; set; }
    }
}
