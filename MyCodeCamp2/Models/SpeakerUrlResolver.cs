using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MyCodeCamp2.Entities;
using MyCodeCamp.Models;
using MyCodeCamp2.Controllers;
//using MyCodeCamp.Controllers;

namespace MyCodeCamp2.Models
{
    //LD STEP73
    public class SpeakerUrlResolver : IValueResolver<Speaker, SpeakerModel, string>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public SpeakerUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(Speaker source, SpeakerModel destination, string destMember, ResolutionContext context)
        {
            var url = (IUrlHelper)_httpContextAccessor.HttpContext.Items[BaseController.URLHELPER];
            //LD note that we arev providing the two variables to build the URL: "moniker" and "id" for the speaker
            return url.Link("SpeakerGet", new { moniker = source.Camp.Moniker, id = source.Id });

        }
    }
}
