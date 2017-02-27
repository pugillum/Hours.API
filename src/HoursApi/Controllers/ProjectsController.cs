using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoursApi.Controllers
{
    [Route("api/projects")]
    public class ProjectsController: Controller
    {
        [HttpGet()]
        public IActionResult GetProjects()
        {
            return Ok(ProjectsDataStore.Current.Projects);
        }

        [HttpGet("{id}")]
        public IActionResult GetProject(int id)
        {
            // find city
            var projectToReturn = ProjectsDataStore.Current.Projects.FirstOrDefault(c => c.Id == id);
            if (projectToReturn == null)
            {
                return NotFound();
            }

            return Ok(projectToReturn);
        }
    }
}
