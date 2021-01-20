using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace {project}.Server.Controllers
{

    [ApiController]
    [Route("[controller]")] // /Filesave
    public class FilesaveController : ControllerBase
    {

        private readonly IWebHostEnvironment env;
        public FilesaveController(IWebHostEnvironment env) => this.env = env;

        [HttpPost]
        public async Task<IActionResult> PostFile([FromForm] IFormFile file)
        {
            /* Note:
                This demo shows how to save a file to the wwwroot folder.
                1. wwwroot must be present in the application or WebRootPath will return null.
                2. This is for demo purposes, use caution when providing users with the ability
                    to upload files to a server. 
                    See: https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0#security-considerations
            */
            
            // Create a BadRequestResult (400 Bad Request) when data is missing.
            if (file == null || file.Length == 0) return new BadRequestResult(); 

            // Ex. path: "wwwroot/uploads/filename.jpg"
            string path = Path.Combine(env.WebRootPath, "uploads", file.FileName);

            using MemoryStream ms = new();
            await Task.WhenAll(
                file.CopyToAsync(ms),
                System.IO.File.WriteAllBytesAsync(path, ms.ToArray())
            );

            Uri resourcePath = new Uri($"{Request.Scheme}://{Request.Host}//uploads//{file.FileName}");

            // Create a (201) response with a Location header indicating the path to the resource created.
            return new CreatedResult(resourcePath, null);
        }

    }

}