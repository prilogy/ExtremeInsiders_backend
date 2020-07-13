using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using ExtremeInsiders.Services;
using Microsoft.AspNetCore.Authorization;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  public class VideoController : ControllerBase<Video, VideoTranslation>
  {
    public VideoController(ApplicationContext db, UserService userService) : base(db, userService)
    {
    }

    protected override IQueryable<Video> GetRecommendedQueryable() => _db.Videos.OrderByDescending(x => x.Likes.Count);
    
  }
}