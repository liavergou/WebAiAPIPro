using AutoMapper;
using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Helpers;
using CoordExtractorApp.Models;
using CoordExtractorApp.Repositories;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Linq.Expressions;

namespace CoordExtractorApp.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly ILogger<ProjectService> logger =
           new LoggerFactory().AddSerilog().CreateLogger<ProjectService>();

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        /// <summary>
        /// Retrieves a project by its unique identifier.
        /// </summary>
        /// <param name="id">The project ID.</param>
        /// <returns>A DTO containing project details.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the project does not exist.</exception>
        public async Task<ProjectDTO?> GetProjectByIdAsync(int id)
        {
            Project? project = null;
            try
            {
                project = await unitOfWork.ProjectRepository.GetAsync(id);

                if (project == null)

                {
                    throw new EntityNotFoundException("Project", "Project with id: " + id + " not found.");
                }
                logger.LogInformation("Project found with ID: {Id}", id);

                var dto = new ProjectDTO
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    Description = project.Description,
                    JobsCount = project.ConversionJobs.Count,
                };

                return dto;

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving project with ID: {Id}. {Message}", id, ex.Message);
                throw;

            }
        }

        /// <summary>
        /// Retrieves a project by its name.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>A read-only DTO of the project.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the project does not exist.</exception>
        public async Task<ProjectReadOnlyDTO?> GetProjectByProjectNameAsync(string projectName)
        {
            try
            {
                Project? project = await unitOfWork.ProjectRepository.GetProjectByProjectNameAsync(projectName);
                if (project == null)

                {
                    throw new EntityNotFoundException("Project", "Project with projectName: " + projectName + " not found.");
                }
                logger.LogInformation("Project found:{ProjectName}", projectName);

                var dto = mapper.Map<ProjectReadOnlyDTO>(project);
                return dto;

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving project by ProjectName: {ProjectName}. {Message}", projectName, ex.Message);
                throw;

            }
        }

        /// <summary>
        /// Retrieves all projects from the database.
        /// </summary>
        /// <returns>A list of all projects ordered by name.</returns>
        public async Task<List<ProjectReadOnlyDTO>> GetAllProjectsAsync()
        {
            var projects = await unitOfWork.ProjectRepository.GetAllAsync();
            var dto = mapper.Map<List<ProjectReadOnlyDTO>>(projects)
                .OrderBy(p => p.ProjectName)
                .ToList();
            logger.LogInformation("Retrieved all projects. Count:{Count}", dto.Count);
            return dto;

        }

        /// <summary>
        /// Retrieves a paginated and filtered list of projects.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="projectFilterDTO">Filter criteria (e.g., project name).</param>
        /// <returns>A paginated result containing the projects.</returns>
        public async Task<PaginatedResult<ProjectDTO>> GetPaginatedProjectsAsync(int pageNumber, int pageSize, ProjectFilterDTO projectFilterDTO)
        {
            try
            {
                List<Project> projects = [];

                List<Expression<Func<Project, bool>>> predicates = [];        

                if (!string.IsNullOrEmpty(projectFilterDTO.ProjectName))
                {
                    predicates.Add(p => p.ProjectName.Contains(projectFilterDTO.ProjectName));
                }

                var result = await unitOfWork.ProjectRepository.GetPaginatedProjectsAsync(pageNumber, pageSize, predicates);

                var dto = new PaginatedResult<ProjectDTO>()
                {
                    Data = result.Data.Select (p => new ProjectDTO
                    {
                        Id = p.Id,
                        ProjectName = p.ProjectName,
                        Description = p.Description,
                        JobsCount = p.ConversionJobs.Count,
                    }).ToList(),

                    TotalRecords = result.TotalRecords,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize
                };
                logger.LogInformation("Retrieved {Count} Projects", dto.Data.Count);
                return dto;
            }
            catch (EntityNotFoundException)
            {
                logger.LogError("Error retrieving paginated projects.");
                throw;
            }
        }

        /// <summary>
        /// Creates a new project in the database.
        /// </summary>
        /// <param name="projectCreateDTO">Data for the new project.</param>
        /// <returns>The created project DTO.</returns>
        /// <exception cref="EntityAlreadyExistsException">Thrown if a project with the same name already exists.</exception>
        public async Task<ProjectDTO> CreateProjectAsync(ProjectCreateDTO projectCreateDTO)
        {
            try
            {
                var existingProject = await unitOfWork.ProjectRepository.GetProjectByProjectNameAsync(projectCreateDTO.ProjectName);
                if (existingProject != null)
                {
                    throw new EntityAlreadyExistsException("Project", $"Project with name {projectCreateDTO.ProjectName} already exists");
                }

                //dto -> entity

                var project = mapper.Map<Project>(projectCreateDTO);


                await unitOfWork.ProjectRepository.AddAsync(project);

                await unitOfWork.SaveAsync(); //commit

                // Δημιουργία φακέλων
                FileHelper.CreateProjectFolders(project.Id, configuration);

                //entity->dto
                var dto = new ProjectDTO
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    Description = project.Description,
                    JobsCount = 0 //0 για νεο project
                };

                logger.LogInformation("Project with ID {id} created successfully", dto.Id);
                return dto;
            }
            catch (AppException ex)
            {
                logger.LogError("Error creating project.Code {Code}, Message: {Message}", ex.Code, ex.Message);
                throw;
            }

        }


        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <param name="id">The project ID.</param>
        /// <param name="projectUpdateDTO">The updated project data.</param>
        /// <returns>True if the update was successful.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the project is not found.</exception>
        /// <exception cref="EntityAlreadyExistsException">Thrown if the new name conflicts with another project.</exception>
        public async Task<bool> UpdateProjectAsync(int id, ProjectUpdateDTO projectUpdateDTO)
        {
            try
            {
                var project = await unitOfWork.ProjectRepository.GetAsync(id);
                //αν δεν υπάρχει
                if (project == null)
                {
                    throw new EntityNotFoundException("Project", $"Project with {id} not found");
                }

                if (!string.IsNullOrEmpty(projectUpdateDTO.ProjectName) && projectUpdateDTO.ProjectName != project.ProjectName)
                {
                    //projectName unique. και ελεγχος αν υπάρχει ήδη
                    var existingProject = await unitOfWork.ProjectRepository.GetProjectByProjectNameAsync(projectUpdateDTO.ProjectName!);

                    if (existingProject != null && existingProject.Id != id)
                    {
                        throw new EntityAlreadyExistsException("Project", $"Project with name {projectUpdateDTO.ProjectName} already exists.");
                    }

                    project.ProjectName = projectUpdateDTO.ProjectName;
                }
                //ελεγχος και του description
                if (projectUpdateDTO.Description != null) project.Description = projectUpdateDTO.Description;

                await unitOfWork.SaveAsync();

                logger.LogInformation("Project with {id} updated successfully", id);
                return true;

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving project with id {id}. {Message}", id, ex.Message);
                throw;
            }
            catch (EntityAlreadyExistsException ex)
            {
                logger.LogError("Error updating project with id {id}. {Message}", id, ex.Message);
                throw;
            }

        }

        /// <summary>
        /// Soft deletes a project and cascades the deletion to its conversion jobs.
        /// </summary>
        /// <param name="id">The project ID.</param>
        /// <returns>True if deletion was successful.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the project is not found.</exception>
        public async Task<bool> DeleteProjectAsync(int id)
        {
            try
            {
                var project = await unitOfWork.ProjectRepository.GetAsync(id);
                if (project == null)
                {
                    throw new EntityNotFoundException("Project", $"Project with id {id} not found");
                }

                //αν υπάρχουν conversion jobs συνδεδεμένα -> warning για cascade soft delete
                var jobs = await unitOfWork.ConversionJobRepository.GetJobsByProjectIdAsync(id);

                if (jobs.Count > 0)
                {
                    logger.LogWarning("Project with {id} and {ProjectName} has {JobCount} jobs assigned. Cascading soft delete", id, project.ProjectName, jobs.Count);
                }
                //iteration στα jobs
                foreach (var job in jobs)
                {
                    await unitOfWork.ConversionJobRepository.DeleteAsync(job.Id); //soft delete τα συνδεδεμένα convertion job
                }
                await unitOfWork.ProjectRepository.DeleteAsync(id); //soft delete το project

                await unitOfWork.SaveAsync();//commit

                logger.LogInformation("Project with {id} deleted successfully. Deleted {jobCount} jobs", id, jobs.Count);

                return true;

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error deleting project with {id}. {Message}", id, ex.Message);
                throw;
            }
        }
    }
}
