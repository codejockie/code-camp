﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCodeCamp.Controllers
{
  [Route("api/[controller]")]
  public class CampsController : Controller
  {
    private ICampRepository _repo;
    private ILogger<CampsController> _logger;

    public CampsController(ICampRepository repo, ILogger<CampsController> logger)
    {
      _repo = repo;
      _logger = logger;
    }

    // GET: /<controller>/
    [HttpGet("")]
    public IActionResult Get()
    {
      var camps = _repo.GetAllCamps();
      return Ok(camps);
    }

    [HttpGet("{id}", Name = "CampGet")]
    public IActionResult Get(int id, bool includeSpeakers = false)
    {
      try
      {
        Camp camp = null;

        if (includeSpeakers)
        {
          camp = _repo.GetCampWithSpeakers(id);
        }
        else
        {
          camp = _repo.GetCamp(id);
        }

        if (camp == null) return NotFound($"Camp {id} was not found.");

        return Ok(camp);
      }
      catch
      {

      }

      return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]Camp model)
    {
      try
      {
        _logger.LogInformation("Creating a new Code Camp");

        _repo.Add(model);
        if (await _repo.SaveAllAsync())
        {
          var newUri = Url.Link("CampGet", new { id = model.Id });
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody]Camp model)
    {
      try
      {
        var oldCamp = _repo.GetCamp(id);

        if (oldCamp == null)
        {
          return NotFound($"Could not find a camp with an ID of {id}");
        }

        // Map model to oldCamp
        oldCamp.Name = model.Name ?? oldCamp.Name;
        oldCamp.Description = model.Description ?? oldCamp.Description;
        oldCamp.Location = model.Location ?? oldCamp.Location;
        oldCamp.Length = model.Length > 0 ? model.Length : oldCamp.Length;
        oldCamp.EventDate = model.EventDate != DateTime.MinValue
          ? model.EventDate : oldCamp.EventDate;

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        var oldCamp = _repo.GetCamp(id);

        if (oldCamp == null)
        {
          return NotFound($"Could not find a camp with an ID of {id}");
        }

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
