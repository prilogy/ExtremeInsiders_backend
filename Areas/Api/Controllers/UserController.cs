﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Api.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using ExtremeInsiders.Helpers;
using ExtremeInsiders.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Areas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly UserService _userService;
        private readonly IEnumerable<SocialAuthService> _authServices;
        private readonly ConfirmationService _confirmationService;
        private readonly ImageService _imageService;
        private readonly FcmService _fcmService;

        public UserController(ApplicationContext db, UserService userService,
            IEnumerable<SocialAuthService> authServices,
            ConfirmationService confirmationService, ImageService imageService, FcmService fcmService)
        {
            _db = db;
            _userService = userService;
            _authServices = authServices;
            _confirmationService = confirmationService;
            _imageService = imageService;
            _fcmService = fcmService;
        }

        /// <summary>
        /// Handles FCM token refresh scenario
        /// </summary>
        /// <response code="200">If everything went OK</response>
        /// <response code="400">If anything went BAD</response>
        /// <returns>Action result</returns>
        [HttpPost]
        public async Task<IActionResult> Fcm(FcmTokenRefreshModel model)
            => await _fcmService.HandleTokenAsync(_userService.User, model.NewToken, model.OldToken) switch
            {
                true => Ok(),
                _ => BadRequest()
            };

        [HttpPut("{type}")]
        public async Task<IActionResult> SocialAccount(string type, AuthenticationModels.SocialLogIn model)
        {
            var handler = _authServices.FirstOrDefault(s => s.ProviderName == type);
            if (handler != null)
            {
                var user = _userService.User;
                var account = await handler.CreateAccount(model.Token);
                if (account != null)
                {
                    user.SocialAccounts.Add(account);
                    _db.SaveChanges();
                    return Ok(user.WithoutSensitive(false));
                }

                ModelState.AddModelError("Auth", $"Не удалось привязать {type} аккаунт.");
                return BadRequest(ModelState);
            }

            ModelState.AddModelError("Auth", $"Неправильный тип социальной сети.");
            return NotFound(ModelState);
        }

        [HttpDelete("{type}")]
        public IActionResult SocialAccount(string type)
        {
            var user = _userService.User;
            var toRemove = user.SocialAccounts.SingleOrDefault(a => a.Provider.Name == type);

            if (toRemove != null)
            {
                user.SocialAccounts.Remove(toRemove);
                _db.SaveChanges();
                return Ok(user.WithoutSensitive(false));
            }

            ModelState.AddModelError("Auth", $"Неправильный тип социальной сети или аккаунт не найден.");
            return NotFound(ModelState);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Like(int id)
        {
            var userAction = new UserAction
            {
                EntityIdItem = new EntityIdItem { EntityId = id }
            };

            var like = await _db.Likes.FirstOrDefaultAsync(l => l.EntityId == id && l.UserId == _userService.UserId);
            if (like == null)
            {
                var entity = await _db.EntitiesLikeable.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return NotFound();

                like = new Like
                {
                    EntityId = entity.Id,
                    UserId = _userService.UserId,
                };
                await _db.Likes.AddAsync(like);
                userAction.Status = true;
                userAction.Entity = like.Entity;
            }
            else
            {
                userAction.Entity = like.Entity;
                userAction.EntityIdItem.Id = like.Id;
                userAction.Status = false;
                _db.Remove(like);
            }

            await _db.SaveChangesAsync();
            if (like.Id != 0) userAction.EntityIdItem.Id = like.Id;
            return Ok(userAction);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Favorite(int id)
        {
            var userAction = new UserAction
            {
                EntityIdItem = new EntityIdItem { EntityId = id }
            };

            var favorite =
                await _db.Favorites.FirstOrDefaultAsync(l => l.EntityId == id && l.UserId == _userService.UserId);
            if (favorite == null)
            {
                var entity = await _db.EntitiesBase.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return NotFound();

                favorite = new Favorite
                {
                    EntityId = entity.Id,
                    UserId = _userService.UserId,
                };
                await _db.Favorites.AddAsync(favorite);
                userAction.Entity = favorite.Entity;
                userAction.Status = true;
            }
            else
            {
                userAction.EntityIdItem.Id = favorite.Id;
                userAction.Entity = favorite.Entity;
                userAction.Status = false;
                _db.Remove(favorite);
            }

            await _db.SaveChangesAsync();
            if (favorite.Id != 0) userAction.EntityIdItem.Id = favorite.Id;
            return Ok(userAction);
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail()
        {
            await _confirmationService.SendEmailConfirmationAsync(_userService.User);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail([FromBody] string code)
        {
            var confirmationCode = _userService.User.ConfirmationCodes.FirstOrDefault(x =>
                ConfirmationCode.CanBeUsed(x, code, ConfirmationCode.Types.EmailConfirmation));

            if (confirmationCode == null)
                return BadRequest();

            confirmationCode.IsConfirmed = true;
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            email = email.ToLower();
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return BadRequest();

            await _confirmationService.SendPasswordReset(user);

            return Ok();
        }

        [HttpPost("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] string code, bool verify)
        {
            var confirmationCode =
                await _db.ConfirmationCodes.FirstOrDefaultAsync(ConfirmationCode.CanBeUsed(code,
                    ConfirmationCode.Types.PasswordReset));
            if (confirmationCode == null) return NotFound();
            return Ok();
        }

        [HttpPatch]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] UserModels.PasswordReset model)
        {
            var confirmationCode =
                await _db.ConfirmationCodes.FirstOrDefaultAsync(ConfirmationCode.CanBeUsed(model.Code,
                    ConfirmationCode.Types.PasswordReset));
            if (confirmationCode == null) return BadRequest();

            confirmationCode.IsConfirmed = true;

            var user = confirmationCode.User;
            user.Password = _userService.HashPassword(user, model.Password);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> Edit([FromForm] UserModels.ProfileEdit model)
        {
            var user = await _db.Users.FindAsync(_userService.UserId);
            if (user.Email == null || (model.Email != null && user.Email.ToLower() != model.Email.ToLower()))
            {
                if (await _db.Users.AnyAsync(x => x.Email.ToLower() == model.Email.ToLower()))
                    return BadRequest("Email is already being used");
                user.Email = model.Email;
                user.ConfirmationCodes.RemoveAll(x => x.Type == ConfirmationCode.Types.EmailConfirmation);
            }

            if (model.Name != null)
                user.Name = model.Name;

            if (model.AvatarSrc != null)
                user.AvatarId = (await _imageService.AddImage(model.AvatarSrc)).Id;

            if (model.PhoneNumber != null)
                user.PhoneNumber = model.PhoneNumber;

            await _db.SaveChangesAsync();
            return Ok(_userService.User.WithoutSensitive(false, true, true, true));
        }
    }
    
    public class FcmTokenRefreshModel
    {
        public string NewToken { get; set; }
        public string OldToken { get; set; }
    }
}