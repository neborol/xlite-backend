using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using EliteForce.AppWideHelpers;
using EliteForce.Data;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Hosting; // .IWebHostEnvironment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IImageRepository _imageRepo;
        private readonly IMissionPhotosRepository _missionImageRepo;
        private readonly IMapper _mapper;
        private readonly IConfirmResp _confirmResp;
        private readonly int MAX_BYTES = 10 * 1024 * 1024;
        private readonly string[] ACCEPTED_FILE_TYPES = new[] {".jpg", ".jpeg", ".png" };
        public ImageController(IWebHostEnvironment env, IImageRepository imageRepo, IMapper mapper, IConfirmResp confirmResp, IMissionPhotosRepository missionImageRepo)
        {
            _environment = env ?? throw new ArgumentNullException(nameof(env));
            _imageRepo = imageRepo ?? throw new ArgumentNullException(nameof(imageRepo));
            _missionImageRepo = missionImageRepo ?? throw new ArgumentNullException(nameof(missionImageRepo));
            _mapper = mapper;
            _confirmResp = confirmResp;
        }

        // GET: api/Image
        [HttpGet("getAllImages")]
        public async Task<IEnumerable<MissionPhoto>> GetImages()
        {
            var images = await _missionImageRepo.GetAllImages();
            return images;
        }

        // GET: api/Image/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }



       [HttpPost("uploadOne")]
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



        [HttpPost("uploadMulti")]
        public async Task<ActionResult<List<string>>> UploadMulti()
        {
            // var singleFile = Request.Form.Files[0];
            var files = Request.Form.Files;
            
            // string webRootPath = _environment.WebRootPath;
            string rootDir = _environment.ContentRootPath;
            string rootFolder = "Resources";
            string subFolder = "MissionPhotos";
            
            // string newPath = Path.Combine(webRootPath, folderName);
            string newPath = Path.Combine(rootDir, rootFolder, subFolder);

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            if (files.Count > 25)
            {
                throw new Exception("Number of files limit exceeded.");
            }

            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                ContentDisposition contentDisposition = new ContentDisposition(file.ContentDisposition);
                string fileName = contentDisposition.FileName.Trim('"');

                string fullPath = Path.Combine(newPath, fileName);
                string relPath = Path.Combine(rootFolder, subFolder, fileName);
                uploadedUrls.Add(relPath);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            // Send the image URLs to the database.
            var numberAffected = await _missionImageRepo.AddAMissionPhoto(uploadedUrls);

            if (numberAffected == 0)
            {
                throw new Exception("Image path insertion failed");
            }

            var resp = _confirmResp.ConfirmResponse(true, "Image creation was Successful");

            return Ok(resp);
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
