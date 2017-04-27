using BussinesAccess.Repositories;
using Context;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace App.Controllers
{
    [RequireHttps]
    public class BaseController<TRepository, TContext, TEntity> : Controller //new()
       where TContext : DbContext
       where TEntity : BaseModel
       where TRepository : BaseRepository<TContext, TEntity>, new()
    {
        private ApplicationUserManager _userManager;

        protected TContext Context
        {
            get
            {

                return Request.GetOwinContext().Get<TContext>();
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


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        

        //public Guid? LoggedUser
        //{
        //    get
        //    {
        //        if (!User.Identity.IsAuthenticated) return null;

        //        var id = User.Identity.GetUserId();

        //        return new Guid(id);
        //    }
        //}
    }
}