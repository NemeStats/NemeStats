namespace BusinessLogic.Models.Games
{
    public class CreateGameDefinitionRequest
    {
        public CreateGameDefinitionRequest()
        {
            Active = true;
        }

        public bool Active { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}
