using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Models;
using MyCodeCamp2.Data;
using MyCodeCamp2.Entities;
using MyCodeCamp2.Filters;

namespace MyCodeCamp2.Controllers
{
    //LD STEP7
    [Route("api/camps/{moniker}/speakers")]
    //LD STEP9
    [ValidateModel]
    [ApiVersion("1.0")] //LD STEP39
    [ApiVersion("1.1")]
    public class SpeakersController : BaseController
    {
        protected ILogger<SpeakersController> _logger;
        protected IMapper _mapper;
        protected ICampRepository _repository;
        protected UserManager<CampUser> _userMgr; //LD STEP31

        public SpeakersController(ICampRepository repository,
          ILogger<SpeakersController> logger,
          IMapper mapper,
          UserManager<CampUser> userMgr) //LD STEP31
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userMgr = userMgr; //LD STEP31
        }

        [HttpGet]
        [MapToApiVersion("1.0")] //LD STEP40
        //LD STEP10
        public IActionResult Get(string moniker, bool includeTalks = false) 
        {
            var speakers = includeTalks ? _repository.GetSpeakersByMonikerWithTalks(moniker) : _repository.GetSpeakersByMoniker(moniker);

            //return Ok(speakers);
            return Ok(_mapper.Map<IEnumerable<SpeakerModel>>(speakers));
        }

        [HttpGet]
        [MapToApiVersion("1.1")] //LD STEP40
        public virtual IActionResult GetWithCount(string moniker, bool includeTalks = false)
        {
            var speakers = includeTalks ? _repository.GetSpeakersByMonikerWithTalks(moniker) : _repository.GetSpeakersByMoniker(moniker);

            return Ok(new { count = speakers.Count(), results = _mapper.Map<IEnumerable<SpeakerModel>>(speakers) });
        }


        [HttpGet("{id}", Name = "SpeakerGet")]
        public IActionResult Get(string moniker, int id, bool includeTalks = false)
        {
            var speaker = includeTalks ? _repository.GetSpeakerWithTalks(id) : _repository.GetSpeaker(id);
            if (speaker == null) return NotFound();
            if (speaker.Camp.Moniker != moniker) return BadRequest("Speaker not in specified Camp");

            //return Ok(speaker);
            return Ok(_mapper.Map<SpeakerModel>(speaker));
        }

        //LD STEP74
        [HttpPost]
        [Authorize] //LD STEP30
        public async Task<IActionResult> Post(string moniker, [FromBody]SpeakerModel model)
        {
            try
            {
                //LD we get the specific camp record by searching by moniker
                var camp = _repository.GetCampByMoniker(moniker);
                if (camp == null) return BadRequest("Could not find camp");

                //LD after we map the "SpeakerModel" to "Speaker" entity
                var speaker = _mapper.Map<Speaker>(model);

                //LD then we assign the "Camp" entity to the "Speaker" entity
                speaker.Camp = camp;

                //LD STEP31
                var campUser = await _userMgr.FindByNameAsync(this.User.Identity.Name);
                if (campUser != null)
                {
                    speaker.User = campUser;

                    _repository.Add(speaker);

                    if (await _repository.SaveAllAsync())
                    {
                        var url = Url.Link("SpeakerGet", new { moniker = camp.Moniker, id = speaker.Id });
                        return Created(url, _mapper.Map<SpeakerModel>(speaker));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while adding speaker: {ex}");
            }
            return BadRequest("Could not add new speaker");
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(string moniker, int id, [FromBody] SpeakerModel model)
        {
            try
            {
                var speaker = _repository.GetSpeaker(id);
                if (speaker == null) return NotFound();
                if (speaker.Camp.Moniker != moniker) return BadRequest("Speaker and Camp do not match");

                if (speaker.User.UserName != this.User.Identity.Name) return Forbid();

                _mapper.Map(model, speaker);

                if (await _repository.SaveAllAsync())
                {
                    return Ok(_mapper.Map<SpeakerModel>(speaker));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while updating speaker: {ex}");
            }

            return BadRequest("Could not update speaker");
        }

        [HttpDelete("{id}")]
        [Authorize] //LD STEP30
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var speaker = _repository.GetSpeaker(id);
                if (speaker == null) return NotFound();
                if (speaker.Camp.Moniker != moniker) return BadRequest("Speaker and Camp do not match");

                if (speaker.User.UserName != this.User.Identity.Name) return Forbid();

                _repository.Delete(speaker);

                if (await _repository.SaveAllAsync())
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while deleting speaker: {ex}");
            }

            return BadRequest("Could not delete speaker");
        }

    }//LD close controller
}
