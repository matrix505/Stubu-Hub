using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCWEB.DAL.Abstract;
using MVCWEB.Models.Entities;
using MVCWEB.Services.Abstract;
using MVCWEB.ViewModel.Account;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVCWEB.Controllers
{
    
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IPasswordHasher _passwordhasher;
        private readonly IAccountRepository _account;
        private readonly IUsersRepository _user;
        private readonly IEmailSenderService _mail;
        public AuthController(
            ILogger<AuthController> logger,
            IPasswordHasher passwordHasher,
            IAccountRepository accountRepository,
            IEmailSenderService emailSenderService,
            IUsersRepository usersRepository
            )
        {
            _logger = logger;
            _account = accountRepository;
            _user = usersRepository;
            _passwordhasher = passwordHasher;
            _mail = emailSenderService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if(!ModelState.IsValid)
            {
                return View(lvm);
            }

             var user = await _account.FindByUsernameAsync(lvm.Username);

            if(user == null || !_passwordhasher.VerifyPassword(lvm.Password,user.HashedPassword ?? ""))
            {
                ModelState.AddModelError(string.Empty, "Incorrect username or password");
                return View(lvm);
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.User_id.ToString()),
                new Claim(ClaimTypes.Name, string.Concat(user.LastName,user.FirstName)),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.AuthorizationRole)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new RegisterViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel rvm)
        {
            if (!ModelState.IsValid)
            {
                return View(rvm);
            }

            var user = new Users() 
            {
                FirstName = rvm.FirstName,
                LastName = rvm.LastName,
                Email = rvm.Email,
                Username = rvm.Username,
                HashedPassword = _passwordhasher.HashPassword(rvm.Password),
                BirthDate = rvm.Birthdate,

            };

            if(await _account.EmailExistsAsync(user.Email))
            {
                ModelState.AddModelError("Email", "Email is already used.");
                return View(rvm);
            }
            if(await _account.UsernameExistsAsync(user.Username))
            {      
                ModelState.AddModelError("Username", "Username is already used. Please try another one.");
                return View(rvm);
            }

            await _account.CreateUserAsync(user);

            return RedirectToAction("Login");
        }
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel fpvm)
        {
            if(!ModelState.IsValid)
            {
                return View(fpvm);
            }
            if(!await _account.EmailExistsAsync(fpvm.Email))
            {
                ModelState.AddModelError("Email", "Email address is not found");
                return View(fpvm);
            }

            return View();
        }
    }
}
