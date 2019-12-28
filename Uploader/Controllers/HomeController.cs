using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Uploader.Models;

namespace Uploader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private static int imageCounter = 0;
        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index(DTO model = null)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DTO model)
        {
            if (model.File != null)
            {                
                string webPImagePath = null;
                if (model.File != null)
                {
                    string path = Path.Combine(hostingEnvironment.WebRootPath, "files");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string webPFileName = Path.GetFileNameWithoutExtension(model.File.FileName) + ".webp";
                    webPImagePath = Path.Combine(path, webPFileName);
                    if (System.IO.File.Exists(webPImagePath))
                    {
                        imageCounter++;
                        webPImagePath = webPImagePath.Replace(".webp", $"_{imageCounter}.webp");
                    }
                    if (model.File.Length > 0)
                    {
                        var len = Math.Abs(model.File.Length);
                        int quality =
                        len < 2000 ? 100 :
                        len < 5000 ? 90 :
                        len < 10000 ? 70 :
                        len < 100000 ? 50 :
                        len < 200000 ? 45 :
                        len < 300000 ? 40 :
                        len < 600000 ? 30 :
                        len < 1000000 ? 25 :
                        len < 2000000 ? 20 : 10;
                        using (var webPFileStream = new FileStream(webPImagePath, FileMode.Create))
                        {
                            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                            {
                                imageFactory.Load(model.File.OpenReadStream())
                                            .Format(new WebPFormat())
                                            .Quality(quality)
                                            .Save(webPFileStream);
                                model.Path = webPImagePath.Split('\\').LastOrDefault();
                            }
                        }
                    }                    
                }
               
            }           
            return View("Index", model);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }       
    }
}
