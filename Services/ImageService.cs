using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ExtremeInsiders.Services
{
  public class ImageService
  {
    private readonly ApplicationContext _db;
    private readonly IWebHostEnvironment _appEnvironment;

    public ImageService(ApplicationContext db, IWebHostEnvironment appEnvironment)
    {
      _db = db;
      _appEnvironment = appEnvironment;
    }

    public async Task<Image> AddImage(IFormFile file)
    {
      if (file == null) return null;

      try
      {
        using var algorithm = new Rfc2898DeriveBytes(
          DateTime.UtcNow.ToString(),
          8,
          10,
          HashAlgorithmName.SHA512);
        
        var name = Regex.Replace(Convert.ToBase64String(algorithm.GetBytes(5)) + file.FileName, @"(\s|\\.|\/|\\)+", "");
        
        var path = "/static/" + name;
        await using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
        {
          await file.CopyToAsync(fileStream);
        }

        var image = new Image {Path = path};
        _db.Images.Add(image);
        await _db.SaveChangesAsync();
        
        return image;
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }

    }
  }
}