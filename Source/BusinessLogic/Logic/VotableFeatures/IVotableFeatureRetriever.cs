using BusinessLogic.Models;

namespace BusinessLogic.Logic.VotableFeatures
{
    public interface IVotableFeatureRetriever
    {
        VotableFeature RetrieveVotableFeature(int id);
    }
}
