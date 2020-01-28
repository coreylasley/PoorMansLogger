﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using PoorMansLogger;
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

            Console.WriteLine("Look at the Output window in the IDE.....");

            ILogger dl = new DebugLogger { Prefix = "Program", MaxElementsIfNonNumericList = 5, MaxElementsIfNumericList = 25, MaxStringLength = 100 };

            // Lets run the test 25 times
            for (int y = 0; y < 25; y++)
            {

                // We are going to pass in null as the first parameter, so that the method name will be pulled off the stack, additionally a bunch of parameter values
                string method = dl.Start(null, 12345, 987.432, "A test string!!", c, dts, ints, null, list);

                // We are going to pass in a method name in this instance, with a bunch of random parameter values
                string method2 = dl.Start("MyMethod", "another test", 1, 2, 3, null, c, list, 3939.29983, dts);

                string method3 = dl.Start("MethodWithNoParams");

                System.Threading.Thread.Sleep(500);                
                
                dl.Stop(method);

                System.Threading.Thread.Sleep(250);

                dl.Stop(method3);

                System.Threading.Thread.Sleep(250);

                dl.Stop(method2);                
                                
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