using System.Collections.Generic;

namespace HoursApi.Models
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int NumberOfWorkItems
        {
            get
            {
                return WorkItems.Count;
            }
        }

        public ICollection<WorkItemDto> WorkItems { get; set; }
        = new List<WorkItemDto>();
    }
}
