using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Testing_Console
{
    class ProgramTimer
    {
        private const int DueTime = 15000;
        public static void MyTimer()
        {
            try
            {
                var t = new ProgramTimer();
                t.StartTimer(DueTime);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void StartTimer(int DueTime)
        {
            try
            {
                System.Threading.Timer t = new System.Threading.Timer(new TimerCallback(TimerProc));
                t.Change(DueTime, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void TimerProc(object state)
        {
            try
            {
                // The state object is the Timer object.
                System.Threading.Timer t = (System.Threading.Timer)state;
                t.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static Timer aTimer;

        public static void EventTimer()
        {
            // Create a timer and set a two second interval.
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 2000;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;

            Console.WriteLine("Press the Enter key to exit the program at any time... ");
            Console.ReadLine();
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
        }

    }
}
