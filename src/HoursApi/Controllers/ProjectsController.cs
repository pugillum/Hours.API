using AutoMapper;
using HoursApi.Entities;
using HoursApi.Models;
using HoursApi.Services;
using Microsoft.AspNetCore.JsonPatch;
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
        public IActionResult GetProject(int id, bool includeWorkItems = false)
        {
            var project = _hoursApiRepository.GetProject(id,includeWorkItems);

            if (project == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<ProjectDto>(project);
            return Ok(result);
        }

        [HttpPost()]
        public IActionResult CreateProject([FromBody] ProjectForCreationDto projectForCreationDto)
        {
            if (projectForCreationDto == null)
            {
                return BadRequest();
            }

            //todo: Better validation checks
            if (projectForCreationDto.Description == projectForCreationDto.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = Mapper.Map<Project>(projectForCreationDto);

            //todo: Improve this
            project.WorkItems = new List<WorkItem>();

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

            var projectEntity = _hoursApiRepository.GetProject(id,false);
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

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateProject(int id,
            [FromBody] JsonPatchDocument<ProjectForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var projectEntity = _hoursApiRepository.GetProject(id,false);
            if (projectEntity == null)
            {
                return NotFound();
            }

            var projectToPatch = Mapper.Map<ProjectForUpdateDto>(projectEntity);

            patchDoc.ApplyTo(projectToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (projectToPatch.Description == projectToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            TryValidateModel(projectToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(projectToPatch, projectEntity);

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

            var projectEntity = _hoursApiRepository.GetProject(id,false);
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
