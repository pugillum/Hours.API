﻿using AutoMapper;
using HoursApi.Entities;
using HoursApi.Models;
using HoursApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoursApi.Controllers
{
    [Route("api/projects")]
    public class ProjectsController: Controller
    {
        private IHoursApiRepository _hoursApiRepository;
        private ILogger<ProjectsController> _logger;

        public ProjectsController(ILogger<ProjectsController> logger,
            IHoursApiRepository hoursApiRepository)
        {
            _hoursApiRepository = hoursApiRepository;
            _logger = logger;
        }

        [HttpGet()]
        public IActionResult GetProjects()
        {
            var projectEntities = _hoursApiRepository.GetProjects();
            var results = Mapper.Map<IEnumerable<ProjectDto>>(projectEntities);

            return Ok(results);
        }

        [HttpGet("{id}", Name = "GetProject")]
        public IActionResult GetProject(int id)
        {
            var project = _hoursApiRepository.GetProject(id);

            if (project == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<ProjectDto>(project);
            return Ok(result);
        }

        [HttpPost()]
        public IActionResult CreateProject([FromBody] ProjectForCreationDto projectForCreation)
        {
            if (projectForCreation == null)
            {
                return BadRequest();
            }

            //todo: Better validation checks
            if (projectForCreation.Description == projectForCreation.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = Mapper.Map<Project>(projectForCreation);

            _hoursApiRepository.AddProject(project);

            if (!_hoursApiRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            var createdProjectToReturn = Mapper.Map<Models.ProjectDto>(project);

            _logger.LogInformation("$Project created with id {createdProjectToReturn.Id}.");

            return CreatedAtRoute("GetProject", 
                new { id = createdProjectToReturn.Id }, createdProjectToReturn);

        }

        [HttpPut("{id}")]
        public IActionResult UpdateProject(int id,
            [FromBody] ProjectForUpdateDto projectForUpdate)
        {
            if (projectForUpdate == null)
            {
                return BadRequest();
            }

            //TODO: Test for duplicate name
            if (projectForUpdate.Description == projectForUpdate.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectEntity = _hoursApiRepository.GetProject(id);
            if (projectEntity == null)
            {
                return NotFound();
            }
            
            Mapper.Map(projectForUpdate, projectEntity);

            if (!_hoursApiRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProject(int id)
        {
            if (!_hoursApiRepository.ProjectExists(id))
            {
                return NotFound();
            }

            var projectEntity = _hoursApiRepository.GetProject(id);
            if (projectEntity == null)
            {
                return NotFound();
            }

            _hoursApiRepository.DeleteProject(projectEntity);

            if (!_hoursApiRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }


    }
}
