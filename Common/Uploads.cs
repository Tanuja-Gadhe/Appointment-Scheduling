using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Uploads
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Uploads(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadImage(IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProfileImages");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" +ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                
                return "/ProfileImages/" + uniqueFileName;
            }
            return "/ProfileImages/default-profile.png";
        }
        public async Task<string> UploadDocument(IFormFile document)
        {
            if (document != null && document.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Documents");
                var uniqueFileName = Guid.NewGuid().ToString()+".pdf";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await document.CopyToAsync(fileStream);
                }


                return "/Documents/" + uniqueFileName;
            }
            return "";
        }
    }
}
