using System;
using System.Collections.Generic;
using System.Linq;
using HoursApi.Entities;
using Microsoft.EntityFrameworkCore;

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

        public void AddWorkItemForProject(int projectId, WorkItem workItem)
        {
            var project = GetProject(projectId, true);
            project.WorkItems.Add(workItem);
        }

        public void DeleteProject(Project project)
        {
            _context.Projects.Remove(project);
        }

        public void DeleteWorkItem(WorkItem workItem)
        {
            _context.WorkItems.Remove(workItem);
        }

        public Project GetProject(int projectId, bool includeWorkItems)
        {
            if (includeWorkItems)
            {
                return _context.Projects.Include(c => c.WorkItems)
                    .Where(c => c.Id == projectId).FirstOrDefault();
            } else
            {
                return _context.Projects.Where(p => p.Id == projectId).FirstOrDefault();
            }
        }

        public IEnumerable<Project> GetProjects()
        {
            return _context.Projects.OrderBy(p => p.Name).ToList();
        }

        public WorkItem GetWorkItemForProject(int projectId, int workItemId)
        {
            return _context.WorkItems
               .Where(w => w.ProjectId == projectId && w.Id == workItemId).FirstOrDefault();
        }

        public IEnumerable<WorkItem> GetWorkItemsForProject(int projectId)
        {
            return _context.WorkItems
                           .Where(w => w.ProjectId == projectId).ToList();
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
