﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
  public class PlaylistController : ControllerBase<Playlist, PlaylistTranslation>
  {
    public PlaylistController(ApplicationContext db, UserService userService) : base(db, userService)
    {
    }
  }
}