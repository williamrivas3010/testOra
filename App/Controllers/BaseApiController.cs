using BussinesAccess.Repositories;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Context;

namespace App.Controllers
{
    public class BaseApiController<TRepository, TEntity> : ApiController //new()
       
       where TEntity : BaseModel
       where TRepository : BaseRepository<ApplicationContext, TEntity>, new()
    {




        protected ApplicationContext Context
        {
            get
            {
                // Create default for unit Testing
                return Request!=null? Request.GetOwinContext().Get<ApplicationContext>() :ApplicationContext.Create() ;
            }
        }

        protected TRepository Repository
        {
            get
            {
                return new TRepository()
                {
                    Context = Context
                };
            }
        }

        public Guid? LoggedUser
        {
            get
            {
                if (!User.Identity.IsAuthenticated) return null;

                var id = User.Identity.GetUserId();

                return new Guid(id);
            }
        }

        protected IHttpActionResult ResultAction<Tmodel>(Tmodel model) where Tmodel : BaseModel
        {
            if (model == null) return NotFound();
            return Ok(model);
        }
        protected IHttpActionResult ResultAction<Tmodel>( List<Tmodel> model) where Tmodel : BaseModel
        {
            if (model == null) return NotFound();
            return Ok(model);
        }


    }
}
