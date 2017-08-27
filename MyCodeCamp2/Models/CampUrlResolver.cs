using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp2.Entities;
using AutoMapper;
using MyCodeCamp2.Controllers;
//using MyCodeCamp.Controllers;

//LD STEP888
namespace MyCodeCamp.Models
{
    public class CampUrlResolver : IValueResolver<Camp, CampModel, string>
    {
        private IHttpContextAccessor _httpContextAccessor;

        //LD "IHttpContextAccessor" is useful to access to the context
        public CampUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public string Resolve(Camp source, CampModel destination, string destMember, ResolutionContext context)
        {
            var url = (IUrlHelper)_httpContextAccessor.HttpContext.Items[BaseController.URLHELPER];
            //return url.Link("CampGetSpecific", new { id = source.Id });
            return url.Link("CampGetMoniker", new { moniker = source.Moniker });

        }
    }
} 
