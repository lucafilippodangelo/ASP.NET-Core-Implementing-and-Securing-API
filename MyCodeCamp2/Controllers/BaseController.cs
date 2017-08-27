using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

//LD STEP888
namespace MyCodeCamp2.Controllers
{
    public abstract class BaseController : Controller
    {
        public const string URLHELPER = "URLHELPER";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            //LD we are storing in the "HttpContext" in order to be able to reuse this url.
            context.HttpContext.Items[URLHELPER] = this.Url;
        }
    }
}