using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Models;
using MyCodeCamp2.Data;
using MyCodeCamp2.Entities;

namespace MyCodeCamp2.Controllers
{
    //LD STEP41
    [Route("api/camps/{moniker}/speakers")]
    [ApiVersion("2.7")]
    public class Speakers2Controller : SpeakersController
    {
        public Speakers2Controller(ICampRepository repository,
          ILogger<SpeakersController> logger,
          IMapper mapper,
          UserManager<CampUser> userMgr)
          : base(repository, logger, mapper, userMgr) //we are passing the same parameters to the base class
        {
        }

        public override IActionResult GetWithCount(string moniker, bool includeTalks = false)
        {
            var speakers = includeTalks ? _repository.GetSpeakersByMonikerWithTalks(moniker) : _repository.GetSpeakersByMoniker(moniker);

            return Ok(new
            {
                currentTime = DateTime.UtcNow,
                count = speakers.Count(),
                results = _mapper.Map<IEnumerable<Speaker2Model>>(speakers) //LD STEP43
            });
        }
    }
}
