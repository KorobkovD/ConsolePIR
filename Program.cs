using System;
using System.Device.Gpio;
using System.Threading;

namespace ConsolePIR
{
    internal class Program
    {
        public static readonly string FAQ =
            $"How to:{Environment.NewLine}" +
            $"\tFirst argument sets pin number for PIR sensor (12 by default){Environment.NewLine}" +
            $"\tSecond argument sets pin number for LED (10 by default){Environment.NewLine}" +
            $"\tThird argument sets PIR sensor delay time for in milliseconds (1000 by default){Environment.NewLine}";

        private static int GreenLedPin = 10;
        private static int PirPin = 12;
        private static int DelayInMilliseconds = 1000;
        private static bool KeepRunning = true;
        private static GpioController gpioController = new GpioController();

        /// <summary>
        /// Проверка запроса помощи
        /// </summary>
        private static bool HelpRequired(string param)
        {
            return param == "-h" || param == "--help" || param == "/?";
        }

        private static bool HandleArguments(string[] args)
        {
            var handleResult = true;

            if (args.Length > 0)
            {
                if (HelpRequired(args[0]))
                {
                    Console.WriteLine(FAQ);
                    handleResult = false;
                }

                if (int.TryParse(args[0], out var pirPin))
                {
                    PirPin = pirPin;
                }

                if (args.Length > 1 && int.TryParse(args[1], out var ledPin))
                {
                    GreenLedPin = ledPin;
                }

                if (args.Length > 2 && int.TryParse(args[2], out var delayTime))
                {
                    DelayInMilliseconds = delayTime;
                }
            }

            return handleResult;
        }

        private static void Main(string[] args)
        {
            if (HandleArguments(args))
            {
                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
                {
                    e.Cancel = true;
                    KeepRunning = false;
                };

                gpioController.OpenPin(GreenLedPin, PinMode.Output);
                gpioController.OpenPin(PirPin, PinMode.Input);

                while (KeepRunning)
                {
                    var motionStatus = gpioController.Read(PirPin);
                    if (motionStatus == PinValue.Low)
                    {
                        Console.WriteLine("All clear here...");
                        gpioController.Write(GreenLedPin, PinValue.Low);
                    }
                    else
                    {
                        Console.WriteLine("Motion detected!");
                        gpioController.Write(GreenLedPin, PinValue.High);
                    }
                    Thread.Sleep(DelayInMilliseconds);
                }

                gpioController.Write(GreenLedPin, PinValue.Low);
                gpioController.ClosePin(GreenLedPin);
                gpioController.ClosePin(PirPin);
                Console.WriteLine($"{Environment.NewLine}Exited");
            }
        }
    }
}
