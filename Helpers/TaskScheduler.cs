using System;
using System.Collections.Generic;
using System.Threading;

namespace ExtremeInsiders.Helpers
{
    public class TaskScheduler
    {
        private static TaskScheduler _instance;
        private List<Timer> _timers = new List<Timer>();

        private TaskScheduler() { }

        public static TaskScheduler Instance => _instance ??= new TaskScheduler();

        /// <summary>
        /// Schedules task per interval starting from repeatAtUtc 
        /// </summary>
        /// <param name="repeatAtUtc">Only hours and minutes are used</param>
        /// <param name="intervalInHours">Interval in hours to repeat the task</param>
        /// <param name="task">Task to perform</param>
        public void ScheduleTask(TimeSpan repeatAtUtc, double intervalInHours, Action task)
        {
            var now = DateTime.UtcNow;
            var firstRun = new DateTime(now.Year, now.Month, now.Day, repeatAtUtc.Hours, repeatAtUtc.Minutes, 0, 0);
            if (now > firstRun)
                firstRun = firstRun.AddDays(1);

            var timeToGo = firstRun - now;
            if (timeToGo <= TimeSpan.Zero)
                timeToGo = TimeSpan.Zero;

            var timer = new Timer(x =>
            {
                task.Invoke();
            }, null, timeToGo, TimeSpan.FromHours(intervalInHours));
            _timers.Add(timer);
        }
        
        public void RunTaskAt(TimeSpan runAt, Action task)
        {
            var current = DateTime.UtcNow;
            var timeToGo = runAt - current.TimeOfDay;
            
            if (timeToGo < TimeSpan.Zero)
                return;//time already passed
            
            var timer = new Timer(x =>
            {
                task.Invoke();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
            _timers.Add(timer);
        }
    }
}