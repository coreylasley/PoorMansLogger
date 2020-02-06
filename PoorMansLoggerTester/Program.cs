using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoorMansLogger.Interface;

namespace PoorMansLogger
{
    class Program
    {
        /* This is part of a concept I came up with that would facilitate the generation of a string
         * representation of a method call with parameter values, used for logging method calls, their parameter values, and execution time
         * within an application with bare minimal code.
         * The Main() method is the test of this functionality.
         */

        private static DebugLogger dl = new DebugLogger { Prefix = "Program", MaxElementsIfNonNumericList = 5, MaxElementsIfNumericList = 25, MaxStringLength = 100 };


        static void Main(string[] args)
        {

            Console.WriteLine("Setting up dummy data...");

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
            Console.WriteLine("Look at the Output window in the IDE to see the output of the logger (if running in Debug mode).....");


            // Lets execute this test 10x 
            for(int x = 0; x < 10; x++)
               ListToStringListCompare();


            Test(123, new List<string> { "This", "Is", "A", "Test" }, new List<DateTime> { DateTime.Now, DateTime.Now.AddDays(5) }, new List<int> { 9, 8, 7, 6, 5, 4, 3, 2, 1 });

            /*

            int loops = 25;

            dl.StartDebug("MAIN");
            for (int z = 0; z < loops; z++)
            {
                Test(z, list, dts, ints, c);
                
                Console.WriteLine(z + " - " + DateTime.Now.ToLongTimeString() + " - Test Complete!");
            }
            dl.StopDebug("MAIN");

            dl.StartDebug("MAIN");
            for (int z = 0; z < loops; z++)
            {                
                Test2(z, list, dts, ints, c);

                Console.WriteLine(z + " - " + DateTime.Now.ToLongTimeString() + " - Test Complete!");
            }
            dl.StopDebug("MAIN");

            Console.WriteLine("Testing has been completed. Hit [ENTER] to exit");
            Console.ReadLine();
            Environment.Exit(0);
            */

        }

        private static void ListToStringListCompare()
        {
            List<int> ints = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            
            // Box the ints object
            object obj = ints;

            List<string> stringList = new List<string>();

            dl.WriteMessage("Performing JSON Serialize/Parse to List<string>");
            dl.StartDebug("JSON Serialize");
            // using Newtonsoft.Json;
            // using Newtonsoft.Json.Linq;
            JObject j = JObject.Parse("{ items: " + JsonConvert.SerializeObject(obj) + "}");
            stringList = (j["items"]).Select(x => x.ToString()).ToList();
            dl.StopDebug("JSON Serialize");


            dl.WriteMessage("Performing Direct Cast to List<int> -> List<string>");
            dl.StartDebug("Direct Test/Cast");
            Type type = obj.GetType();
            if (type == typeof(List<int>)) stringList = ((List<int>)obj).Select(x => x.ToString()).ToList();
            dl.StopDebug("Direct Test/Cast");
        }

        private static void Test(int a, List<string> b, List<DateTime> c, List<int> d)
        {
            // Start a stopwatch on the method
            dl.StartDebug("TestMethod");
            
            for (int x = 0; x < 5; x++)
            {
                // Start a stopwatch and log some values
                dl.StartDebug("loop" + x, x, a, b, c, d);
                
                // Some code here … 
                
                dl.StopDebug("loop" + x);
            }

            dl.StopDebug("TestMethod");
        }

        private static void Test2(int a, List<string> b, List<DateTime> c, List<int> d)
        {
            // Start a stopwatch on the method, let StartDebug get the name
            dl.StartDebug(null);

            // Since we didnt pass a name, lets get the ID so we can use it when stopping the stopwatch
            long p1 = dl.LastStartID;

            for (int x = 0; x < 10; x++)
            {
                // Start a stopwatch for this iteration of the loop but no values
                dl.StartDebug("loop" + x);

                // Some code here … 

                dl.StopDebug("loop" + x);
            }

            dl.StopDebug(p1);
        }


    }


    /// <summary>
    /// Just a class for testing
    /// </summary>
    public class Dude
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return "Dude {Id = " + Id + ", Name = " + Name + "}";
        }
    }
}
