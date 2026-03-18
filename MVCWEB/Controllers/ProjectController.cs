using Microsoft.AspNetCore.Mvc;
using MVCWEB.DAL.Abstract;
using MVCWEB.Models.Entities;
using MVCWEB.ViewModel.ForProject;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVCWEB.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectRepository _project;
        private readonly ICollabRepository _collab;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(
            IProjectRepository projectRepository,
            ICollabRepository collabRepository,
            ILogger<ProjectController> logger
            )
        {
            _project = projectRepository;
            _collab = collabRepository;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Main(int id,string? tab = null)
        {
            _logger.LogInformation(tab.ToString());
            var GetMainProject = await _project.GetMainProject(id);

            if(GetMainProject == null)
            {
                return NotFound();
            }

            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var MainProject = new ProjectMainViewModel()
            {
                Id = GetMainProject.Project_id,
                Title = GetMainProject.Title!,
                Description = GetMainProject.Description!,
                Categories = GetMainProject.CategoryNames!.Split(",").ToList(),
                MemberSize = GetMainProject.MemberSize,
                Status = GetMainProject.Status!,
                Members = await _collab.GetProjectTeamMembers(id),
                IsUserProjectMember = await _project.IsUserProjectMember(UserId,id),
                IsUserProjectOwner = await _project.IsUserProjectOwner(UserId, id)
                ,TotalMembers = GetMainProject.TotalMembers,
                CreatedAt = GetMainProject.CreatedAt,
                tab = tab

            };


            return View(MainProject);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var CreateProject = new ProjectCreateViewModel()
            {
                Skills = await _collab.GetAllAvailableSkills(),
                Categories = await _collab.GetAllAvailableCategories()
            };
            return View(CreateProject);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateViewModel ccvm)
        {
            ccvm.Skills = await _collab.GetAllAvailableSkills();
            ccvm.Categories = await _collab.GetAllAvailableCategories();

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
                Select(id => new ProjectCategories
                {
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

            return RedirectToAction("Index", "Dashboard");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestToJoin(int projectId)
        {
            
            var Request = new JoinRequests()
            {
                Project_id = projectId,
                User_id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            };
            await _project.RequestToJoin(Request);
            return RedirectToAction("Index","Home");
        }

    }
}
