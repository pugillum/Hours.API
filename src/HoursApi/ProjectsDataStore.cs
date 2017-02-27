using HoursApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoursApi
{
    public class ProjectsDataStore
    {
        public static ProjectsDataStore Current { get; } = new ProjectsDataStore();
        public List<ProjectDto> Projects { get; set; }

        public ProjectsDataStore()
        {
            // init dummy data
            Projects = new List<ProjectDto>()
            {
                new ProjectDto()
                {
                     Id = 1,
                     Name = "Acme Inc",
                     Description = "Just another project",
                },
                new ProjectDto()
                {
                    Id = 2,
                    Name = "Classic websites",
                    Description = "Classic sites of the web",
                },
                new ProjectDto()
                {
                    Id= 3,
                    Name = "Movie site",
                    Description = "A movie site like no other",
                }
            };

        }
    }
}
