using System.Threading.Tasks;
using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
    public class VideoTranslationController : ControllerTranslationBase<Video, VideoTranslation>
    {
        private readonly ApplicationContext _dbContext;
        public VideoTranslationController(ApplicationContext context, ImageService imageService, ApplicationContext dbContext) : base(context,
            imageService)
        {
            _dbContext = dbContext;
        }

        public override IActionResult RedirectToBaseEntity(int id) =>
            RedirectToAction("Edit", "Video", new { Id = id });

        public override async Task<IActionResult> Create(VideoTranslation translation, IFormFile imageSrc)
        {
            var r = await base.Create(translation, imageSrc);

            if (translation.Id == 0) return r;
            
            var video = await _dbContext.Videos.FindAsync(translation.BaseEntityId);
            if (video == null) return r;
            
            
            
            return r;
        }
    }
}