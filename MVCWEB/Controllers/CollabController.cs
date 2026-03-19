using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCWEB.DAL.Abstract;
using MVCWEB.Models.Entities;
using MVCWEB.ViewModel.Collab;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVCWEB.Controllers
{
    [Authorize]
    public class CollabController : Controller
    {
        private readonly ICollabRepository _collab;
        private readonly IProjectRepository _project;
        private readonly ILogger<CollabController> _logger;

        public CollabController(
              ICollabRepository collab,
              ILogger<CollabController> logger,
              IProjectRepository project

            )
        {
            _collab = collab;
            _logger = logger;
            _project = project;
        }
        public IActionResult Index(
            
            )
        {
          
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> BrowseProjects(int page =1, string? search = null)
        {
            int pageSize = 9; // 4 x 5;
            
            var projects = new CollabViewModel()
            {
                Projects = await _collab.BrowseAllProjects(page, pageSize, search),
                Search = search
            };
            return View(projects);
        }
        public async Task<IActionResult> JoinedProjects(int page = 1, string? search = null)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            int pageSize = 9;
            var projects = new CollabViewModel()
            {
                Projects = await _collab.GetJoinedProjects(userId, page, pageSize, search)
            };

            return View(projects);
        }
        [HttpGet]
        public async Task<IActionResult> MyProjects(int page = 1, string? search = null)
        {
            int pageSize = 9; // TO DO: make static helper to set fixed values 

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var MyProjects = new CollabViewModel()
            {
                Projects = await _collab.
                GetOwnedProjects(
                    userId,
                    page,
                    pageSize,
                    search
                    ),
                isSearchAllowed = false
            };
            return View(MyProjects);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int ProjectId)
        {
         
                var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var project = new CollabViewModel()
                {

                    Project = await _collab.GetByIdAsync(ProjectId),
                    isProjectMember = await _project.IsUserProjectMember(UserId, ProjectId),
                    isOwnerMember = await _project.IsUserProjectOwner(UserId, ProjectId),
                    isRequested = await _project.IsUserInRequest(UserId, ProjectId)

                };

                if (project is null || !Request.Headers.ContainsKey("X-Requested-With"))
                {
                    return NotFound();
                }

                project.Members = await _collab.GetProjectTeamMembers(ProjectId);


                return PartialView("Partials/Project/_ProjectModalViewPartial", project);
           

            
        }


       

    }
}
