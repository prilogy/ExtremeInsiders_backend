using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Admin.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExtremeInsiders.Areas.Admin.Controllers
{
    public class VideoTranslationController : ControllerTranslationBase<Video, VideoTranslation>
    {
        private readonly ApplicationContext _dbContext;
        private readonly FcmService _fcmService;

        public VideoTranslationController(ApplicationContext context, ImageService imageService,
            ApplicationContext dbContext, FcmService fcmService) : base(context,
            imageService)
        {
            _dbContext = dbContext;
            _fcmService = fcmService;
        }

        public override IActionResult RedirectToBaseEntity(int id) =>
            RedirectToAction("Edit", "Video", new { Id = id });

        public override async Task<IActionResult> Create(VideoTranslation translation, IFormFile imageSrc)
        {
            var r = await base.Create(translation, imageSrc);

            if (translation.Id == 0) return r;

            var video = await _dbContext.Videos.FindAsync(translation.BaseEntityId);
            if (video == null) return r;

            var n = DefaultNotification.NewVideo(video.OfCulture(Culture.All.FirstOrDefault(x => x.Id == translation.CultureId)));
            await _fcmService.SendNotificationAsync(n, translation.CultureId);

            return r;
        }
    }
}