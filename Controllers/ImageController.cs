using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EliteForce.Data;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Hosting; // .IWebHostEnvironment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IImageRepository _imageRepo;
        private readonly IMapper _mapper;
        private readonly int MAX_BYTES = 10 * 1024 * 1024;
        private readonly string[] ACCEPTED_FILE_TYPES = new[] {".jpg", ".jpeg", ".png" };
        public ImageController(IWebHostEnvironment env, IImageRepository imageRepo, IMapper mapper)
        {
            _environment = env ?? throw new ArgumentNullException(nameof(env));
            _imageRepo = imageRepo ?? throw new ArgumentNullException(nameof(imageRepo));
            _mapper = mapper;
        }

        // GET: api/Image
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Image/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Image/upload
        //[HttpPost]
        //public async Task<IActionResult> UploadMultiple(IFormCollection files)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string uniqueFileName = UploadedFile(photoModel);
        //        MissionPhoto photo = new MissionPhoto 
        //        {
        //            UniquePhotoName = uniqueFileName,
        //            UploadDate = photoModel.UploadDate,
        //            UploadedBy = photoModel.UploadedBy
        //        };

        //        await _imageRepo.Add(photo);
        //    }
            

        //    return Ok("Photo-Upload-Success");
        //}

       // POST: api/Image/upload
       [HttpPost]
        public async Task<IActionResult> UploadOne([FromBody] ImageUploadDto photoModel)
        {
            // var photoEntity = _mapper.Map<MissionPhoto>(photoModel);
            // Validations:
            if (photoModel.UploadedImage == null) return BadRequest("Null file");
            if (photoModel.UploadedImage.Length == 0) return BadRequest("Empty file");
            if (photoModel.UploadedImage.Length > MAX_BYTES) return BadRequest("Max file size exceeded"); // MAX_BYTES could be stored in a configuraton file
            if (ACCEPTED_FILE_TYPES.Any(s => s == Path.GetExtension(photoModel.UploadedImage.FileName).ToLower())) return BadRequest("Invalid file type");


            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(photoModel.UploadedImage);
                MissionPhoto photo = new MissionPhoto
                {
                    UniquePhotoName = uniqueFileName,
                    UploadDate = DateTime.Now,
                    UploadedBy = photoModel.UploadedBy
                };

                await _imageRepo.Add(photo);
            }

            return Ok("Photo-Upload-Success");
        }


        // POST: api/Image/upload
        //[HttpPost]
        //public async Task<IActionResult> Upload([FromBody] ImageUploadDto photoModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string uniqueFileName = UploadedFile(photoModel);
        //        MissionPhoto photo = new MissionPhoto
        //        {
        //            UniquePhotoName = uniqueFileName,
        //            UploadDate = photoModel.UploadDate,
        //            UploadedBy = photoModel.UploadedBy
        //        };

        //        await _imageRepo.Add(photo);
        //    }


        //    return Ok("Photo-Upload-Success");
        //}

        // PUT: api/Image/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private string UploadedFile(IFormFile uploadedImage)
        {
            string uniqueFileName = null;

            if (uploadedImage != null)
            {
                // @params _environment.WebRootPath points to the wwwroot directory, then combines it with 'uploaded'
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploaded");
                // If this directory is not available, then create it. 
                if(!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Now generate a new unique image file name of the file, since a hacker can pass malicious code in the name
                var nameAndExtensin = Guid.NewGuid().ToString() + "_" + Path.GetExtension(uploadedImage.FileName);
                uniqueFileName = Path.Combine(uploadsFolder, nameAndExtensin);

                // uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadedImageModel.UploadedImage.FileName;
                // string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Next, we would read the file as a stream and store it in the variable "fileStream"
                using (var fileStream = new FileStream(uniqueFileName, FileMode.Create))
                {
                    uploadedImage.CopyTo(fileStream);
                }

                // Alternatively a thumbnail can be created at this stage and added somewhere in the app.
            }
            return uniqueFileName;
        }
    }
}
