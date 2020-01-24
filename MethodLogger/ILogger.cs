using System;
using System.Collections.Generic;
using System.Text;

namespace MethodLogger
{
    public interface ILogger
    {
        /// <summary>
        /// A string that should be added to the begining of each method name in the output
        /// </summary>
        string Prefix { get; set; }

        /// <summary>
        /// The number of TAB characters to add before the the method name / PreFix + method name
        /// </summary>
        int IndentNumber { get; set; }

        int MaxElementsIfNonNumericList { get; set; }
        int MaxElementsIfNumericList { get; set; }
        int MaxStringLength { get; set; }

        /// <summary>
        /// Used to determine if a "Started" message should be displayed when Start()ing a timer 
        /// </summary>
        bool DisplayStartMessages { get; set; }

        /// <summary>
        /// Starts timing a process based on a method call with parameter values including a visual of Lists of common types
        /// </summary>
        /// <param name="methodName">The name of the method to be represented, set to null and the method name will be obtained from the call stack</param>
        /// <param name="MethodParameterValues">Unlimited parameter values of any type that should be represented in the method call string </param>
        /// <returns>A string that can be used for logging</returns>
        string Start(string methodName, params object[] MethodParameterValues);

        /// <summary>
        /// Stops a running timer for a process with a given MethodName, write the result, and removes it from the stack
        /// </summary>
        /// <param name="MethodName"></param>
        void Stop(string MethodName);

        /// <summary>
        /// Writes a string  
        /// </summary>
        /// <param name="Message"></param>
        void WriteMessage(string Message);

    }
}
