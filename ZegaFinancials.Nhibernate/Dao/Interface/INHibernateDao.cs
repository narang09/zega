using System.Collections.Generic;

namespace ZegaFinancials.Nhibernate.Dao.Interface
{
    public interface INHibernateDao<EntityType>
    {       
        void Persist(EntityType entity);
        void Persist(IEnumerable<EntityType> entities);
        void Delete(EntityType entity);
        void Delete(int[] entityIds);
        void Delete(int entityId);
        EntityType Get(int id);
        ICollection<EntityType> GetAll();
        EntityType Create();
        bool IsExist(int id);
 
    }
}
