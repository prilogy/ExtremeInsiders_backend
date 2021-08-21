using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Helpers;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Services
{
    /// <summary>
    /// Сервис Firebase Cloud Messaging - обрабатывает токены и шлет сообщения(пуши) на сервер Google
    /// </summary>
    public class FcmService
    {
        //private readonly IServiceScopeFactory _scopeFactory;
        protected readonly ApplicationContext DbContext;
        private readonly FirebaseMessaging _fcmInstance;

        public FcmService(ApplicationContext dbContext)
        {
            DbContext = dbContext;
            //_scopeFactory = scopeFactory;
            _fcmInstance = FirebaseMessaging.DefaultInstance;
        }

        public async Task<bool> HandleTokenAsync(User forUser, string newToken, string oldToken = null)
        {
            try
            {
                if (oldToken != null)
                {
                    var oldTokenEntity =
                        await DbContext.FcmTokens.FirstOrDefaultAsync(x =>
                            x.Token == oldToken && forUser.Id == x.UserId);
                    if (oldTokenEntity != null)
                    {
                        DbContext.Remove(oldTokenEntity);
                        await DbContext.SaveChangesAsync();
                    }
                }

                if (newToken != null)
                {
                    var newTokenEntity =
                        await DbContext.FcmTokens.FirstOrDefaultAsync(x =>
                            x.Token == newToken && forUser.Id == x.UserId);
                    if (newTokenEntity != null) return true;

                    await DbContext.AddAsync(new FcmToken
                    {
                        Token = newToken,
                        UserId = forUser.Id
                    });
                    await DbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendNotificationAsync(List<string> tokens, NotificationBase notification)
        {
            var n = notification.ToNotification();
            await SendNotificationAsync(tokens, n, notification.Data);
            return true;
        }

        public Task<bool> SendNotificationAsync(UserNotification n, List<string> tokens = null)
        {
            var nn = n.ToNotificationBase();
            return SendNotificationAsync(nn, n.CultureId, tokens);
        }

        public async Task<bool> SendNotificationAsync(NotificationBase n, int? cultureId, List<string> _tokens = null)
        {
            if (cultureId != null) n.Localize(Culture.All.FirstOrDefault(x => x.Id == cultureId) ?? Culture.Fallback);
            var tokens = _tokens ?? await ((DbContext.FcmTokens, cultureId) switch
            {
                (var x, null) => x,
                var (x, y) => x.Where(o => o.User.CultureId == y)
            }).AsNoTracking().Select(x => x.Token).ToListAsync();

            if(tokens.Any()) await SendNotificationAsync(tokens, n);
            return true;
        }

        public async Task<bool> SendNotificationAsync(List<string> tokens, Notification notification,
            Dictionary<string, string> data = null)
        {
            if (tokens == null || tokens.Count == 0) return false;

            try
            {
                var message = new MulticastMessage
                {
                    Tokens = tokens,
                    Data = data,
                    Notification = notification,
                };

                var r = await _fcmInstance.SendMulticastAsync(message);
                if (r.FailureCount > 0) await ValidateTokensAsync(tokens);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendNotificationAsync(List<User> users, NotificationBase notification)
        {
            foreach (var user in users)
                await SendNotificationAsync(user, notification);
            return true;
        }

        public async Task<bool> SendNotificationAsync(User user, NotificationBase notification)
            => user?.FcmTokens?.Count > 0 && (notification.Content == null
                    ? notification.Localize(user.Culture ?? Culture.Fallback)
                    : notification) switch
                {
                    var x => await SendNotificationAsync(
                        user.FcmTokens.Select(y => y.Token).ToList(), x.ToNotification(),
                        x.Data)
                };

        public async Task<bool> SendNotificationAsync(int? userId, NotificationBase notification)
            => userId != null && await DbContext.Users.FindAsync(userId) switch
            {
                null => false,
                var x => await SendNotificationAsync(x, notification)
            };

        #region [ Helpers ]

        private async Task ValidateTokensAsync(IReadOnlyCollection<string> tokens)
        {
            if (tokens == null) return;

            var tokensToDelete = new List<string>();

            foreach (var token in tokens)
            {
                try
                {
                    await _fcmInstance.SendAsync(new Message { Token = token }, true);
                }
                catch (Exception)
                {
                    tokensToDelete.Add(token);
                }
            }

            if (!tokensToDelete.Any()) return;

            DbContext.RemoveRange(
                await DbContext.FcmTokens.Where(x => tokensToDelete.Contains(x.Token)).ToListAsync());
            await DbContext.SaveChangesAsync();
        }

        #endregion
    }
}