using BusinessLogic.Models;

namespace BusinessLogic.Logic.VotableFeatures
{
    public interface IVotableFeatureRetriever
    {
        VotableFeature RetrieveVotableFeature(string id);
    }
}
