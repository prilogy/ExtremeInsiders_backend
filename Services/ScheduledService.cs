using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExtremeInsiders.Services
{
    public class ScheduledService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ScheduledService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        #region [ IHostedService implementation ]

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await SendNotificationsAsync();
            Helpers.TaskScheduler.Instance.ScheduleTask(DateTime.UtcNow switch
                {
                    var x => x.AddMinutes(60 - x.Minute).TimeOfDay
                }, 1,
                async () => { await SendNotificationsAsync(); });
        }

        public Task StopAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;

        #endregion

        #region [ Implementation ]

        public async Task SendNotificationsAsync()
        {
            var from = DateTime.UtcNow;
            var to = from.AddMinutes(60 - from.Minute);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            try
            {
                var notifications = await db.UserNotifications
                    .Where(x => x.DateCreated >= from && x.DateCreated < to)
                    .ToListAsync();

                var tokens = new Dictionary<string, List<string>>
                {
                    {
                        Culture.Russian.Key, new List<string>()
                    },
                    {
                        Culture.English.Key, new List<string>()
                    },
                };

                if (notifications.Any())
                {
                    tokens[Culture.Russian.Key] = await db.FcmTokens.Where(x => x.User.CultureId == Culture.Russian.Id)
                        .Select(x => x.Token).ToListAsync();
                    tokens[Culture.English.Key] = await db.FcmTokens.Where(x => x.User.CultureId == Culture.English.Id)
                        .Select(x => x.Token).ToListAsync();
                    if (tokens.Any(x => x.Value.Any()))
                    {
                        foreach (var n in notifications)
                            Helpers.TaskScheduler.Instance.RunTaskAt(n.DateCreated!.Value.TimeOfDay,
                                async () => await SendNotificationAsync(n, tokens));
                    }
                }

                Console.WriteLine("_________________________________________\n" +
                                  "| NOTIFICATIONS REPORT \n" +
                                  "| UTCNOW(used): " + from + "\n" +
                                  "| Notifications: " + notifications.Count + "\n" +
                                  "| Tokens: " + tokens.Aggregate("", (acc, x) => x.Key + " - " + x.Value.Count + ",") + "\n"
                                  + "_________________________________________");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("_________________________________________");
                Console.WriteLine("| NOTIFICATIONS REPORT \n");
                Console.WriteLine("| UTCNOW(used): " + from);
                Console.WriteLine("| UNEXPECTED ERROR OCCURED, NO NOTIFICATIONS WILL BE SENT!");
                Console.WriteLine("_________________________________________");
            }
        }

        public async Task SendNotificationAsync(UserNotification n, Dictionary<string, List<string>> d)
        {
            using var scope = _scopeFactory.CreateScope();
            var fcmService = scope.ServiceProvider.GetRequiredService<FcmService>();
            var tokens = n.CultureId == null ? d.SelectMany(x => x.Value).ToList() : d[n.Culture.Key];
            await fcmService.SendNotificationAsync(n, tokens);
        }

        #endregion
    }
}