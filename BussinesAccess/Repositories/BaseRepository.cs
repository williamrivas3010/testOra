using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BussinesAccess.Repositories
{
    public abstract class BaseRepository<TContext, TEntity>
        where TContext : DbContext
        where TEntity : BaseModel
        //where TEntity : class 
    {

        //[Dependency]
        public TContext Context;

        public BaseRepository(TContext db)
        {
            this.Context = db;

        }

        protected BaseRepository()
        {

        }

        static string[] PotentialIdClaim = { System.Security.Claims.ClaimTypes.NameIdentifier, "sub" };


        protected Guid UserId
        {
            get
            {
                var userIdClaim = ClaimsPrincipal.Current.FindFirst(c => PotentialIdClaim.Contains(c.Type));

                if (userIdClaim == null) throw new Exception("Authentication Failed, verify identity   Settings");

                return new Guid(userIdClaim.Value);
            }
        }

        protected string UserRole
        {
            get
            {
                var claim = ClaimsPrincipal.Current.FindFirst(c => c.Type == ClaimTypes.Role);

                return claim != null ? claim.Value : string.Empty;
            }
        }

        #region IRepository<T> Members



        public virtual IQueryable<TEntity> AsQueryable
        {
            get { return Context.Set<TEntity>(); }
        }

        public virtual IQueryable<TEntity> AsIQueryable(params string[] children)
        {

            if (children == null || children.Length == 0)
            {
                return Context.Set<TEntity>();
            }

            IQueryable<TEntity> query = children.Aggregate<string, IQueryable<TEntity>>(Context.Set<TEntity>(),
                (current, child) => current.Include(child));
            return query;
        }



        public virtual Task<TEntity> FindByIdAsync<T>(T id) // where TEntity : BaseModel
        {
            return Context.Set<TEntity>().FindAsync(id);
        }

        public virtual Task<TEntity> FindByIdAsync(int id, params string[] children)
        {
            if (children == null || children.Length == 0)
            {
                return Context.Set<TEntity>().FindAsync(id);
            }

            IQueryable<TEntity> query = children.Aggregate<string, IQueryable<TEntity>>(Context.Set<TEntity>(),
                (current, child) => current.Include(child));
            return query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
            await Context.SaveChangesAsync();
        }


        public virtual void InsertOrUpdate(TEntity entity) //where TEntity : BaseModel
        {
            if (entity.Id != 0) Context.Entry(entity).State = EntityState.Modified;

            else Context.Set<TEntity>().Add(entity);
        }

        

        public virtual void InsertOrUpdate(IEnumerable<TEntity> entities)
        {
            entities.ToList().ForEach(InsertOrUpdate);
        }



        public virtual void Delete(TEntity entity) //where TEntity : BaseModel
        {
            Context.Set<TEntity>().Remove(entity);
        }


        public virtual Task DeleteAsync<TEntity>(TEntity model) where TEntity : class
        {
            Context.Set<TEntity>().Remove(model);
            return SaveAsync();
        }

        public virtual Task DeleteAsync
            <TEntity>(IEnumerable<TEntity> items, Guid? user) where TEntity : class
        {
            items.ToList().ForEach(i => Context.Set<TEntity>().Remove(i));
            return SaveAsync();
        }

        public virtual async Task DeleteByIdAsync(int id)
        {
            var model = await FindByIdAsync(id);

            Delete(model);

            await SaveAsync();
        }


        public async Task SaveAsync(Guid? currentUserId = null)
        {
            if (currentUserId == null)
                currentUserId = ClaimsPrincipal.Current.FindFirst(c => PotentialIdClaim.Contains(c.Type)) != null ? (Guid?)UserId : null;

            if (currentUserId == null)
            {
                await Context.SaveChangesAsync();
                return;
            }


            var entities =
                Context.ChangeTracker.Entries()
                    .Where(
                        x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));



            foreach (var entity in entities)
            {

                //((BaseModel)entity.Entity).ModifiedBy = currentUserId.Value;
                //((BaseModel)entity.Entity).LastUpdate = DateTime.Now;
                if(entity.Entity.GetType() == typeof( ApplicationUser))
                {

                }

            }

            await Context.SaveChangesAsync();
        }

        public Task AddOrUpdate<Tmodel>(Tmodel model) where Tmodel : BaseModel
        {
            var state = Context.Entry(model).State;

            if (state == EntityState.Detached) Context.Set<Tmodel>().Attach(model);

            state = Context.Entry(model).State = model.Id != 0 ? EntityState.Modified : EntityState.Added;

            return SaveAsync();
        }

        #endregion
    }
}
