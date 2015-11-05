namespace UI.Models.GameDefinitionModels
{
    public class CreateGameDefinitionViewModel
    {
        public CreateGameDefinitionViewModel()
        {
            Active = true;
        }

        public bool Active { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string ReturnUrl { get; set; }
    }
}