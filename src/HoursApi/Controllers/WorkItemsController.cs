using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HoursApi.Services;
using Microsoft.Extensions.Logging;
using HoursApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HoursApi.Controllers
{
    [Route("api/projects")]
    public class WorkItemsController : Controller
    {
        private ILogger<WorkItemsController> _logger;
        private IHoursApiRepository _HoursApiRepository;


        public WorkItemsController(ILogger<WorkItemsController> logger,
            IHoursApiRepository HoursApiRepository)
        {
            _logger = logger;
            _HoursApiRepository = HoursApiRepository;
        }

        [HttpGet("{ProjectId}/WorkItems")]
        public IActionResult GetWorkItems(int ProjectId)
        {
            try
            {
                if (!_HoursApiRepository.ProjectExists(ProjectId))
                {
                    _logger.LogInformation($"Project with id {ProjectId} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                var WorkItemsForProject = _HoursApiRepository.GetWorkItemsForProject(ProjectId);
                var WorkItemsForProjectResults =
                                   Mapper.Map<IEnumerable<WorkItemDto>>(WorkItemsForProject);

                return Ok(WorkItemsForProjectResults);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for Project with id {ProjectId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{ProjectId}/WorkItems/{id}", Name = "GetWorkItem")]
        public IActionResult GetWorkItem(int ProjectId, int id)
        {
            if (!_HoursApiRepository.ProjectExists(ProjectId))
            {
                return NotFound();
            }

            var WorkItem = _HoursApiRepository.GetWorkItemForProject(ProjectId, id);

            if (WorkItem == null)
            {
                return NotFound();
            }

            var WorkItemResult = Mapper.Map<WorkItemDto>(WorkItem);
            return Ok(WorkItemResult);
        }

        [HttpPost("{ProjectId}/WorkItems")]
        public IActionResult CreateWorkItem(int ProjectId,
            [FromBody] WorkItemForCreationDto WorkItem)
        {
            if (WorkItem == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_HoursApiRepository.ProjectExists(ProjectId))
            {
                return NotFound();
            }

            var finalWorkItem = Mapper.Map<Entities.WorkItem>(WorkItem);

            _HoursApiRepository.AddWorkItemForProject(ProjectId, finalWorkItem);

            if (!_HoursApiRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            var createdWorkItemToReturn = Mapper.Map<Models.WorkItemDto>(finalWorkItem);

            return CreatedAtRoute("GetWorkItem", new
            { ProjectId = ProjectId, id = createdWorkItemToReturn.Id }, createdWorkItemToReturn);
        }

        [HttpPut("{ProjectId}/WorkItems/{id}")]
        public IActionResult UpdateWorkItem(int ProjectId, int id,
            [FromBody] WorkItemForUpdateDto WorkItem)
        {
            if (WorkItem == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_HoursApiRepository.ProjectExists(ProjectId))
            {
                return NotFound();
            }

            var WorkItemEntity = _HoursApiRepository.GetWorkItemForProject(ProjectId, id);
            if (WorkItemEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(WorkItem, WorkItemEntity);

            if (!_HoursApiRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }


        [HttpPatch("{ProjectId}/WorkItems/{id}")]
        public IActionResult PartiallyUpdateWorkItem(int ProjectId, int id,
            [FromBody] JsonPatchDocument<WorkItemForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_HoursApiRepository.ProjectExists(ProjectId))
            {
                return NotFound();
            }

            var WorkItemEntity = _HoursApiRepository.GetWorkItemForProject(ProjectId, id);
            if (WorkItemEntity == null)
            {
                return NotFound();
            }

            var WorkItemToPatch = Mapper.Map<WorkItemForUpdateDto>(WorkItemEntity);

            patchDoc.ApplyTo(WorkItemToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TryValidateModel(WorkItemToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(WorkItemToPatch, WorkItemEntity);

            if (!_HoursApiRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{ProjectId}/WorkItems/{id}")]
        public IActionResult DeleteWorkItem(int ProjectId, int id)
        {
            if (!_HoursApiRepository.ProjectExists(ProjectId))
            {
                return NotFound();
            }

            var WorkItemEntity = _HoursApiRepository.GetWorkItemForProject(ProjectId, id);
            if (WorkItemEntity == null)
            {
                return NotFound();
            }

            _HoursApiRepository.DeleteWorkItem(WorkItemEntity);

            if (!_HoursApiRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }
    }
}
