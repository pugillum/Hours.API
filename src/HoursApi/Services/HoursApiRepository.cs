using System.Collections.Generic;
using System.Linq;
using HoursApi.Entities;

namespace HoursApi.Services
{
    public class HoursApiRepository : IHoursApiRepository
    {
        private HoursApiContext _context;
        public HoursApiRepository(HoursApiContext context)
        {
            _context = context;
        }

        public void AddProject(Project project)
        {
            _context.Projects.Add(project);
        }

        public void DeleteProject(Project project)
        {
            _context.Projects.Remove(project);
        }

        public Project GetProject(int projectId)
        {
            return _context.Projects.Where(p => p.Id == projectId).FirstOrDefault();
        }

        public IEnumerable<Project> GetProjects()
        {
            return _context.Projects.OrderBy(p => p.Name).ToList();
        }

        public bool ProjectExists(int projectId)
        {
            return _context.Projects.Any(p => p.Id == projectId);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
