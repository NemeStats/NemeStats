namespace UI.Models.GameDefinitionModels
{
	public class NewGameDefinitionViewModel
	{
		public NewGameDefinitionViewModel(string returnUrl)
		{
			this.ReturnUrl = returnUrl;
		}
		public string Name { get; set; }
		public string Description { get; set; }
		public string ReturnUrl { get; set; }
		public int BoardGameGeekObjectId { get; set; }
	}
}