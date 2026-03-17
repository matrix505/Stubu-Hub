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
        private readonly IProjectRepository _project;
        private readonly ILogger<CollabController> _logger;

        public CollabController(
              IProjectRepository project,
              ILogger<CollabController> logger

            )
        {
            _project = project;
            _logger = logger;
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
                Projects = await _project.BrowseAllProjects(page, pageSize, search),
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
                Projects = await _project.GetJoinedProjects(userId, page, pageSize, search)
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
                Projects = await _project.
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
        public async Task<IActionResult> Details(int id)
        {
            var project = new CollabViewModel() { Project = await _project.GetByIdAsync(id) };

            if(project is null || !Request.Headers.ContainsKey("X-Requested-With"))
            {
                return NotFound();
            }

            project.Members = await _project.GetProjectTeamMembers(id);
            

            return PartialView("Partials/Project/_ProjectModalViewPartial",project);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var CreateProject = new CollabCreateViewModel()
            {
                Skills = await _project.GetAllAvailableSkills(),
                Categories = await _project.GetAllAvailableCategories()
            };
            return View(CreateProject);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CollabCreateViewModel ccvm)
        {
            ccvm.Skills = await _project.GetAllAvailableSkills();
            ccvm.Categories = await _project.GetAllAvailableCategories();

            if (!ModelState.IsValid)
            {
                return View(ccvm);
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var NewProject = new Project()
            {
                Title = ccvm.Title,
                Description = ccvm.Description,
                MemberSize = ccvm.MemberSize,

                Categories = ccvm.SelectedCategoryIds!.
                Select(id => new ProjectCategories { 
                    Category_id = int.Parse(id),
                
                }).ToList()
                ,
                Skills = ccvm.SelectedSkillIds!.
                Select(id => new ProjectSkills
                {
                    Skill_id = id

                }).ToList(),
            };

            await _project.CreateProject(userId, NewProject);

            return RedirectToAction("Index","Dashboard");
        }
        public IActionResult JoinRequests()
        {   
            return View();
        }

    }
}
