using System.Linq;

namespace BusinessLogic.DataAccess
{
    public interface ISingleColumnTechnicalKey<T>
    {
        T Id { get; set; }
    }
}
