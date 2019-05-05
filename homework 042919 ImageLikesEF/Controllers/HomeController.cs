using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using homework_042919_ImageLikesEF.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using ImageLikesEF.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace homework_042919_ImageLikesEF.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment _environment;
        private string _connectionString;

        public HomeController(IHostingEnvironment environment,
            IConfiguration configuration)
        {
            _environment = environment;
            _connectionString = configuration.GetConnectionString("ConStr");
        }

        public IActionResult Index()
        {
            ImageRepository repo = new ImageRepository(_connectionString);
            IEnumerable<Image> images = repo.GetImages();
            return View(images);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile image, string title)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            string fullPath = Path.Combine(_environment.WebRootPath, "images/uploads", fileName);
            using (FileStream stream = new FileStream(fullPath, FileMode.CreateNew))
            {
                image.CopyTo(stream);
            }

            ImageRepository repo = new ImageRepository(_connectionString);
            repo.AddImage(new Image
            {
                FileName = fileName,
                Title = title,
                Likes = 0,
                Date = DateTime.Now
            });

            return Redirect("/");
        }

        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            ViewImageViewModel vm = new ViewImageViewModel();
            vm.Image = repo.GetImage(id);
            List<int> likes = HttpContext.Session.Get<List<int>>("likes");
            if (likes != null)
            {
                if (likes.Contains(id))
                {
                    vm.Like = true;
                }
                else
                {
                    vm.Like = false;
                }
            }
            else
            {
                vm.Like = false;
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult Like(int id)
        {
            var repo = new ImageRepository(_connectionString);
            repo.SetLike(id);
            List<int> likes = HttpContext.Session.Get<List<int>>("likes");
            
            if(likes == null)
            {
                likes = new List<int>();
            }

            likes.Add(id);
            HttpContext.Session.Set<List<int>>("likes", likes);
            return Json(id);
        }

        public IActionResult GetLikes(int id)
        {
            var repo = new ImageRepository(_connectionString);
            int likes = repo.GetLikes(id);
            return Json(likes);
        }

    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
