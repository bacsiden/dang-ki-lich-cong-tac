using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.Objects.DataClasses;
using System.Data.Objects;

namespace Alabama
{
    public class DB
    {
        public static VPSS1Entities Entities
        {
            get
            {
                return new VPSS1Entities();
            }
        }

        public class BaseClass<T> where T : EntityObject
        {
            VPSS1Entities _db;

            public BaseClass()
            {
                _db = new VPSS1Entities();
            }

            public T GetByID(int id)
            {
                System.Data.EntityKey key = new System.Data.EntityKey(_db.DefaultContainerName + "." + typeof(T).Name, "ID", id);
                object obj = null;
                _db.TryGetObjectByKey(key, out  obj);
                if (obj == null)
                    return null;
                _db.Detach(obj);
                return (T)obj;
            }

            public T GetByID(int id, bool alowDetach)
            {
                System.Data.EntityKey key = new System.Data.EntityKey(_db.DefaultContainerName + "." + typeof(T).Name, "ID", id);
                object obj = null;
                _db.TryGetObjectByKey(key, out  obj);
                if (obj == null)
                    return null;
                if (alowDetach)
                    _db.Detach(obj);
                return (T)obj;
            }

            public T Insert(T entity)
            {
                _db.AddObject(_db.DefaultContainerName + "." + typeof(T).Name, entity);
                _db.SaveChanges();
                return entity;
            }

            public T Update(T entity)
            {
                _db.AttachTo(_db.DefaultContainerName + "." + typeof(T).Name, entity);
                _db.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                _db.SaveChanges();
                return entity;
            }

            public void Delete(T entity)
            {
                _db.Attach(entity);
                _db.DeleteObject(entity);
                _db.SaveChanges();
            }

            public void Delete(int id)
            {
                T entity = GetByID(id);
                Delete(entity);
            }
        }
    }

    public class BaseEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public BaseEntity()
        {
        }

        public BaseEntity(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }
}