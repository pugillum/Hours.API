using HoursApi.Entities;
using System.Collections.Generic;

namespace HoursApi.Services
{
    public interface IHoursApiRepository
    {
        bool ProjectExists(int projectId);
        IEnumerable<Project> GetProjects();
        Project GetProject(int projectId);
        void AddProject(Project project);
        void DeleteProject(Project project);
        bool Save();
    }
}
