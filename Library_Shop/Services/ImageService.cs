using ClassLibrary_Shop.Models.Book_m;
using Library_Shop.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Shop.Services
{
    public class ImageService
    {
        private readonly LibraryDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(LibraryDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<int> AddImageFromRelativePathAsync(string relativeImagePath)
        {
            // Завантаження байтів зображення за допомогою LoadImageBytes
            var imageData = LoadImageBytes(relativeImagePath);

            if (imageData == null)
            {
                Console.WriteLine("Failed to load image.");
                return -1; // Або обробіть помилку іншим способом
            }

            var image = new Image
            {
                ImageData = imageData
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return image.Id; // Повертає ID доданого зображення
        }



        public byte[] LoadImageBytes(string relativePath)
        {
            var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
            return File.Exists(absolutePath) ? File.ReadAllBytes(absolutePath) : null;
        }
    }

}
