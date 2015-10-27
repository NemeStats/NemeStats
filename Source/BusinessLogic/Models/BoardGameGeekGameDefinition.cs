using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class BoardGameGeekGameDefinition : EntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
    }
}
