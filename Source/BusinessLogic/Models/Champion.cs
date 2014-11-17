using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class Champion : EntityWithTechnicalKey<int>
    {
        public Champion()
        {
            DateCreated = DateTime.UtcNow;
        }

        public override int Id { get; set; }
        public int GameDefinitionId { get; set; }
        public int PlayerId { get; set; }
        public DateTime DateCreated { get; set; }
        public float WinPercentage { get; set; }

        [ForeignKey("GameDefinitionId")]
        public virtual GameDefinition GameDefinition { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        public bool SameChampion(Champion otherChampion)
        {
            return otherChampion != null 
                && otherChampion.PlayerId == this.PlayerId;
        }

        public override bool Equals(object obj)
        {
            Champion championToCompare = obj as Champion;
            if (championToCompare == null)
            {
                return false;
            }
            return this.GameDefinitionId == championToCompare.GameDefinitionId
                   && this.PlayerId == championToCompare.PlayerId
                   && this.WinPercentage == championToCompare.WinPercentage;
        }
    }
}
