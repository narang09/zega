using NHibernate;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface;

namespace ZegaFinancials.Nhibernate.Dao.Impl
{
    public class NHibernateDao<EntityType> : INHibernateDao<EntityType>
         where EntityType : class, new()
    {
        public readonly ISession _session;
        public const int MaxParametersCountPerOneQuery = 2000;

        public NHibernateDao(ISession session)
        {
            _session = session;
        }

        public virtual void Persist(EntityType entity)
        {
            _session.SaveOrUpdate(entity);
        }
        public virtual void Persist(IEnumerable<EntityType> entities)
        {
            foreach (var entity in entities)
                Persist(entity);
        }
        public virtual void Delete(EntityType entity)
        {
            _session.Delete(entity);
        }

        public virtual void Delete(int entityId)
        {
            _session.Delete("FROM " + typeof(EntityType).Name + " o WHERE o.Id = ?", entityId, global::NHibernate.NHibernateUtil.Int32);
        }
        public virtual EntityType Get(int id)
        {
            return !object.Equals(id, default(int)) ? _session.Get<EntityType>(id) : null;
        }
        public virtual ICollection<EntityType> GetAll()
        {
            return _session.CreateQuery("FROM " + typeof(EntityType).Name).List<EntityType>();
        }

        public virtual EntityType Create()
        {
            return new EntityType();
        }
        public void Delete(int[] entityIds)
        {
            _session.Delete(string.Format("FROM " + typeof(EntityType).Name + " o WHERE o.Id IN ({0})",string.Join(",", entityIds)));

        }
        public bool IsExist(int id)
        {
            return _session.CreateQuery("Select 1 From "+ typeof(EntityType).Name + " WHERE Id =:Id").SetParameter("Id",id).List<int>().Any();           
        }
    }
}
