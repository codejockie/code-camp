﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCodeCamp.Controllers
{
  [Route("api/[controller]")]
  public class CampsController : Controller
  {
    private ICampRepository _repo;

    public CampsController(ICampRepository repo)
    {
      _repo = repo;
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
        _repo.Add(model);
        if (await _repo.SaveAllAsync())
        {
          var newUri = Url.Link("CampGet", new { id = model.Id });
          return Created(newUri, model);
        }
      }
      catch (Exception)
      {

      }

      return BadRequest();
    }
  }
}
