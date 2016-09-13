using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models.Games
{
    public class BoardGameGeekGameCategory : EntityWithTechnicalKey<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public int BoardGameGeekGameCategoryId { get; set; }

        public string CategoryName { get; set; }

        public virtual IList<BoardGameGeekGameDefinition> Games { get; set; }
    }
}
