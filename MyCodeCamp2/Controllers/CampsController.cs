using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp2.Data;
using MyCodeCamp2.Entities;
using MyCodeCamp.Models;

using AutoMapper; //LD STEP3
using System.Collections.Generic;
using MyCodeCamp2.Filters;

namespace MyCodeCamp2.Controllers
{
    //LD STEP1
    [Authorize] //LD STEP20
    [EnableCors("AnyGET")] //LD STEP16
    [ValidateModel]
    [Route("api/[controller]")]
    public class CampsController : BaseController
    {
        private ILogger<CampsController> _logger;
        private ICampRepository _repo;
        private IMapper _mapper; //LD STEP3

        //LD STEP22 constructor injection
        public CampsController(ICampRepository repo, ILogger<CampsController> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper; //LD STEP3
        }

        //LD STEP2
        [HttpGet]
        [Authorize(Policy = "Superusers")] //LD STEP37
        public IActionResult Get()
        {
            return Ok(new { stst = "w" });
        }

        //LD STEP55
        [HttpGet("getall")]
        public IActionResult GetAllCamps()
        {
            var camps = _repo.GetAllCamps();
            return Ok(camps); //return Ok(_mapper.Map<IEnumerable<CampModel>>(camps));
        }

        //LD STEP55
        [HttpGet("getspecific/{id}", Name = "CampGetSpecific")] //LD STEP100
        public IActionResult GetSpecific(int id, bool includeSpeakers = false)
        {
            try
            {
                Camp camps = null;

                if (includeSpeakers) camps = _repo.GetCampWithSpeakers(id);
                else camps = _repo.GetCamp(id);

                if (camps == null) return NotFound($"Camp {id} was not found");

                _logger.LogInformation("LD GET REQUEST DONE");
                return Ok(camps);
            }
            catch
            {

            }
            return BadRequest();//LD default return
        }


        [HttpGet("getspecificmoniker/{moniker}", Name = "CampGetMoniker")]
        public IActionResult GetMoniker(string moniker, bool includeSpeakers = false)
        {
            try
            {
                Camp camp = null;

                if (includeSpeakers) camp = _repo.GetCampByMonikerWithSpeakers(moniker);
                else camp = _repo.GetCampByMoniker(moniker);

                if (camp == null) return NotFound($"Camp {moniker} was not found");

                return Ok(_mapper.Map<CampModel>(camp)); //LD STEP3
            }
            catch
            {
            }

            return BadRequest();
        }


        //LD post by "Camp" model
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]Camp model)
        {
            try
            {
                _logger.LogInformation("Creating a new Code Camp");

                //LD STEP101
                _repo.Add(model);

                if (await _repo.SaveAllAsync())
                {
                    var newUri = Url.Link("CampGetSpecific", new { id = model.Id });
                    return Created(newUri, model);
                }
                else
                {
                    _logger.LogWarning("Could not save Camp to the database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while saving Camp: {ex}");
            }

            return BadRequest();

        }

         
        [EnableCors("Wildermuth")] //LD STEP17
        [HttpPost("postmapper")]//LD STEP999 - post by "CampModel" model
        public async Task<IActionResult> PostAsyncViewModel([FromBody]CampModel model)
        {
            try
            {
                //LD not used anymore because of step //LD STEP9
                //if (!ModelState.IsValid) return BadRequest(ModelState); //LD STEP9991 
                
                _logger.LogInformation("Creating a new Code Camp");

                var camp = Mapper.Map<Camp>(model);
                _repo.Add(camp);

                if (await _repo.SaveAllAsync())
                {
                    var newUri = Url.Link("CampGetMoniker", new { moniker = camp.Moniker });
                    return Created(newUri, _mapper.Map<CampModel>(camp));
                }
                else
                {
                    _logger.LogWarning("Could not save Camp to the database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while saving Camp: {ex}");
            }
            return BadRequest();

        }


        //LD STEP104 this is a my personal version, here I'm not using MAPPER
        [HttpPut("ldput/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CampModel model)
        {
            try
            {
                var oldCamp = _repo.GetCamp(id);
                if (oldCamp == null) return NotFound($"Could not find a camp with an moniker of {id}");

                oldCamp.Name = model.Name ?? oldCamp.Name; //LD set "oldCamp.Name" the value to "model.Name" if not null, otherwise set "oldCamp.Name"

                if (await _repo.SaveAllAsync())
                {
                    return Ok(oldCamp);
                }
            }
            catch (Exception)
            {

            }

            return BadRequest("Couldn't update Camp");
        }

        //LD STEP9992 - implementation of PUT as the trainer do
        [HttpPut("{moniker}")]
        public async Task<IActionResult> Put(string moniker, [FromBody] CampModel model)
        {
            try
            {
                //LD not used anymore because of step //LD STEP9
                //if (!ModelState.IsValid) return BadRequest(ModelState); //LD STEP9991

                var oldCamp = _repo.GetCampByMoniker(moniker);
                if (oldCamp == null) return NotFound($"Could not find a camp with an moniker of {moniker}");

                _mapper.Map(model, oldCamp); //LD here the source and the destination is the same

                if (await _repo.SaveAllAsync())
                {
                    return Ok(_mapper.Map<CampModel>(oldCamp));
                }
            }
            catch (Exception)
            {

            }

            return BadRequest("Couldn't update Camp");
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = _repo.GetCampByMoniker(moniker);
                if (oldCamp == null) return NotFound($"Could not find Camp with moniker of {moniker}");

                _repo.Delete(oldCamp);
                if (await _repo.SaveAllAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
            }

            return BadRequest("Could not delete Camp");
        }

    }
}
