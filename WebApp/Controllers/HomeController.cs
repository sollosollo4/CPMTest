using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using WebApp.Data;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, IHostingEnvironment appEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
                return View(_context.UserFiles.Where(x => x.UserId == currentUser.Id).ToList());
            else
                return View();
        }

        
        public IActionResult AdminIndex()
        {
            return View(_context.UserFiles.ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string path = "/Files/" + uploadedFile.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                var _currentUser = await _userManager.GetUserAsync(HttpContext.User);
                UserFile file = new UserFile { FileName = uploadedFile.FileName, FilePath = _appEnvironment.WebRootPath + path, User = _currentUser };
                _context.UserFiles.Add(file);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserFiles()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            return View(_context.UserFiles.Where(x => x.UserId == currentUser.Id).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> DownoloadFile(int FileId)
        {
            var file = await _context.UserFiles.FindAsync(FileId);
            if (file != null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(file.FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
            }
            else 
                return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DownoloadFileByUserId(Guid UserId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == UserId);

            var file = _context.UserFiles.FirstOrDefault(x => x.UserId == user.Id);
            if (file != null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(file.FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
            }
            else
                return RedirectToAction("Index");
        }
    }
}
