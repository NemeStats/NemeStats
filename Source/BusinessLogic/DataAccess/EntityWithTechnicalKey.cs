using System.Linq;

namespace BusinessLogic.DataAccess
{
    //TODO make generic
    public abstract class EntityWithTechnicalKey<T> : EntityWithTechnicalKey, ISingleColumnTechnicalKey<T>
    {
        public abstract T Id { get; set; }

        public virtual bool AlreadyInDatabase()
        {
            if(Id == null)
            {
                return false;
            }

            return !Id.Equals(default(T));
        }
    }

    public interface EntityWithTechnicalKey
    {
        bool AlreadyInDatabase();
    }
}
