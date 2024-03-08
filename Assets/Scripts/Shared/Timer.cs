using System;
using System.Threading;

namespace Shared
{
    public class Timer
    {
        private readonly DateTime endTime;
        private bool isCanceled = false;
        /// <summary>
        /// End time should be UTC
        /// </summary>
        /// <param name="endTime">UTC</param>
        public Timer(DateTime endTime)
        {
            this.endTime = endTime;
        }

        public Timer(DateTime endTime, Action onCompletion)
        {
            this.endTime = endTime;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(this.endTime - DateTime.Now);
                if (!isCanceled)
                {
                    onCompletion.Invoke();
                }
            }));
            thread.Start();
        }
        public string TimeUntil()
        {
            return (endTime - DateTime.UtcNow).ToString(@"mm\:ss");
        }
        public double TimeUntilSeconds()
        {
            return (endTime - DateTime.UtcNow).TotalSeconds;
        }

        public void Cancel()
        {
            isCanceled = true;
        }
    }
}