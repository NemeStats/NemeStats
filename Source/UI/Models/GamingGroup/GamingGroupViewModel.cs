using BusinessLogic.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BusinessLogic.Models.Games;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Models.GamingGroup
{
    public class GamingGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OwningUserId { get; set; }
        [DisplayName("Owning User Name")]
        public string OwningUserName { get; set; }
        [DataType(DataType.EmailAddress)]
        [DisplayName("Invitee Email")]
        [Required(ErrorMessage = "Please enter an e-mail!", AllowEmptyStrings = false)]
        public string InviteeEmail { get; set; }
        public IList<GameDefinitionSummary> GameDefinitionSummaries { get; set; }
        public IList<PlayerWithNemesisViewModel> Players { get; set; }
        public IList<InvitationViewModel> Invitations { get; set; }
        public IList<PlayedGameDetailsViewModel> RecentGames { get; set; }
    }
}