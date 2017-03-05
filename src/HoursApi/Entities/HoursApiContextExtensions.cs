using System.Collections.Generic;
using System.Linq;

namespace HoursApi.Entities
{
    public static class HoursApiContextExtensions
    {
        public static void EnsureSeedDataForContext(this HoursApiContext context)
        {
            if (context.Projects.Any())
            {
                return;
            }

            // init seed data
            var projects = new List<Project>()
            {
                new Project()
                {
                    Id = 1,
                    Name = "Acme Inc",
                    Description = "Just another project",
                },
                new Project()
                {
                    Id = 2,
                    Name = "Classic websites",
                    Description = "Classic sites of the web",
                },
                new Project()
                {
                    Id= 3,
                    Name = "Movie site",
                    Description = "A movie site like no other",
                }
            };

            context.Projects.AddRange(projects);
            context.SaveChanges();
        }
    }
}
