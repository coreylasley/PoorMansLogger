using System;
using System.Collections.Generic;
using PoorMansLogger.Interface;
using PoorMansLogger.Implementation;

namespace PoorMansLogger
{
    class Program
    {
        /* This is part of a concept I came up with that would facilitate the generation of a string
         * representation of a method call with parameter values, used for logging method calls, their parameter values, and execution time
         * within an application with bare minimal code.
         * The Main() method is the test of this functionality.
         */

        static void Main(string[] args)
        {

            // Lets create some Lists and objects to use in our Method Call generator
            // -----------------------------------------------------------------------------------
            List<string> list = new List<string> { 
                "This", "Is", "A", "Test" 
            };

            Dude c = new Dude();

            List<DateTime> dts = new List<DateTime> {
            DateTime.Now, DateTime.Now.AddDays(150)
            };

            List<int> ints = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                ints.Add(i);
            }
            // ------------------------------------------------------------------------------------


            // Now on to the beef, and an example of the MethodLogger....

            Console.WriteLine("Look at the Output window in the IDE to see the output of the logger.....");

            ILogger dl = new DebugLogger { Mode = Base.LoggerBase.Modes.Debug, Prefix = "Program", MaxElementsIfNonNumericList = 5, MaxElementsIfNumericList = 25, MaxStringLength = 100 };

            double totalMS = 0;

            int loops = 250;

            Console.WriteLine("Starting Tests. Each test will average the time cost of the logger over " + loops + " loops...");

            for (int z = 0; z < 10; z++)
            {
                // Lets run the test xxx times
                for (int y = 0; y < loops; y++)
                {
                    // We are going to pass in null as the first parameter, so that the method name will be pulled off the stack, additionally a bunch of parameter values
                    string method = dl.Start(null, 12345, 987.432, "A test string!!", c, dts, ints, null, list);

                    // We are going to pass in a method name in this instance, with a bunch of random parameter values
                    string method2 = dl.Start("MyMethod", "another test", 1, 2, 3, null, c, list, 3939.29983, dts);

                    string method3 = dl.Start("MethodWithNoParams");

                    totalMS += dl.Stop(method);

                    totalMS += dl.Stop(method2);

                    totalMS += dl.Stop(method3);
                }

                Console.WriteLine("3 Different Calls per loop = " + totalMS / (loops * 3) + " Average ms");


                totalMS = 0;
                // Lets run the test xxx times
                for (int y = 0; y < loops; y++)
                {
                    string method = dl.Start(null, 12345, 987.432, "A test string!!", c, dts, ints, null, list, true, false, ints);
                    totalMS += dl.Stop(method);
                }

                Console.WriteLine("1 call with code block name pulled from stack trace with 11 parameters per loop = " + totalMS / (loops * 3) + " Average ms");


                totalMS = 0;
                // Lets run the test xxx times
                for (int y = 0; y < loops; y++)
                {
                    string method = dl.Start("MethodWithNoParams");
                    totalMS += dl.Stop(method);
                }

                Console.WriteLine("1 call with 8 parameters per loop = " + totalMS / (loops * 3) + " Average ms");
            }
        }
      
    }

    /// <summary>
    /// Just a class for testing
    /// </summary>
    public class Dude
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
