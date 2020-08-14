using System;
using System.Device.Gpio;
using System.Threading;

namespace ConsolePIR
{
    internal class Program
    {
        //private const int RedLedPin = 6;
        private const int GreenLedPin = 10;

        private const int PirPin = 12;
        private const int DelayInMilliseconds = 1000;
        private static bool KeepRunning = true;
        private static GpioController gpioController = new GpioController();

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                KeepRunning = false;
            };

            gpioController.OpenPin(GreenLedPin, PinMode.Output);
            //gpioController.OpenPin(RedLedPin, PinMode.Output);
            gpioController.OpenPin(PirPin, PinMode.Input);

            while (KeepRunning)
            {
                var motionStatus = gpioController.Read(PirPin);
                if (motionStatus == PinValue.Low)
                {
                    Console.WriteLine("All clear here...");
                    gpioController.Write(GreenLedPin, PinValue.Low);
                    //gpioController.Write(RedLedPin, PinValue.High);
                }
                else
                {
                    Console.WriteLine("Motion detected!");
                    gpioController.Write(GreenLedPin, PinValue.High);
                    //gpioController.Write(RedLedPin, PinValue.Low);
                }
                Thread.Sleep(DelayInMilliseconds);
            }

            //gpioController.Write(RedLedPin, PinValue.Low);
            //gpioController.ClosePin(RedLedPin);
            gpioController.Write(GreenLedPin, PinValue.Low);
            gpioController.ClosePin(GreenLedPin);
            gpioController.ClosePin(PirPin);
            Console.WriteLine($"{Environment.NewLine}Exited");
        }
    }
}
