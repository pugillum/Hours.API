using HoursApi.Entities;
using System.Collections.Generic;

namespace HoursApi.Services
{
    public interface IHoursApiRepository
    {
        bool ProjectExists(int projectId);
        IEnumerable<Project> GetProjects();
        Project GetProject(int projectId, bool includeWorkItems);
        void AddProject(Project project);
        void DeleteProject(Project project);
        IEnumerable<WorkItem> GetWorkItemsForProject(int projectId);
        WorkItem GetWorkItemForProject(int projectId, int workItemId);
        void AddWorkItemForProject(int projectId, WorkItem workItem);
        void DeleteWorkItem(WorkItem workItem);
        bool Save();
    }
}
