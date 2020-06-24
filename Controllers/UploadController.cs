using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EliteForce.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EliteForce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {


        //[HttpPost]
        //public async Task<IActionResult> UploadOne([FromBody] NewsImage photoModel)
        //{
        //    // var photoEntity = _mapper.Map<MissionPhoto>(photoModel);
        //    // Validations:
        //    if (photoModel.UploadedImage == null) return BadRequest("Null file");
        //    if (photoModel.UploadedImage.Length == 0) return BadRequest("Empty file");
        //    if (photoModel.UploadedImage.Length > MAX_BYTES) return BadRequest("Max file size exceeded"); // MAX_BYTES could be stored in a configuraton file
        //    if (ACCEPTED_FILE_TYPES.Any(s => s == Path.GetExtension(photoModel.UploadedImage.FileName).ToLower())) return BadRequest("Invalid file type");


        //    if (ModelState.IsValid)
        //    {
        //        string uniqueFileName = UploadedFile(photoModel.UploadedImage);
        //        MissionPhoto photo = new MissionPhoto
        //        {
        //            UniquePhotoName = uniqueFileName,
        //            UploadDate = DateTime.Now,
        //            UploadedBy = photoModel.UploadedBy
        //        };

        //        await _imageRepo.Add(photo);
        //    }

        //    return Ok("Photo-Upload-Success");
        //}




        // POST: api/Upload
        [HttpPost("fileUpload/{topFolder}/{innerFolder}"), DisableRequestSizeLimit]
        public IActionResult Upload(string topFolder, string innerFolder)
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", topFolder, innerFolder); // Individual paths separated by comas
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    return Ok(new { dbPath });
                } 
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        // PUT: api/Upload/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
