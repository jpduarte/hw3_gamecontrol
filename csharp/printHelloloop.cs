using System;


namespace printHelloloop {

  class printHelloloop {
    private static System.Timers.Timer aTimer;

    public static void Main(string[] args){
      System.Console.WriteLine("Hello Print Juan");

      //https://msdn.microsoft.com/en-us/library/393k7sb1.aspx
      // Create a timer and set a two second interval.
      aTimer = new System.Timers.Timer();
      aTimer.Interval = 200;

      // Alternate method: create a Timer with an interval argument to the constructor.
      //aTimer = new System.Timers.Timer(2000);

      // Create a timer with a two second interval.
      //aTimer = new System.Timers.Timer(2000);

      // Hook up the Elapsed event for the timer.
      aTimer.Elapsed += OnTimedEvent(5);

      // Have the timer fire repeated events (true is the default)
      aTimer.AutoReset = true;

      // Start the timer
      aTimer.Enabled = true;

      Console.WriteLine("Press the Enter key to exit the program at any time... ");
      Console.ReadLine();
    }

    private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e,int a)
    {
        Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
    }

  }
}
