
using System.Diagnostics;

namespace PoorMansLogger.Interface
{
    public interface ILogger
    {
        
        /// <summary>
        /// A string that should be added to the begining of each code block name in the log
        /// </summary>
        string Prefix { get; set; }

        /// <summary>
        /// The number of TAB characters to add before the the code block name / PreFix + code block name
        /// </summary>
        int IndentNumber { get; set; }

        /// <summary>
        /// The maximum number of elements from an IList to render in the log for non-numeric types
        /// </summary>
        int MaxElementsIfNonNumericList { get; set; }

        /// <summary>
        /// The maximum number of elements from a parameter of IList to render in the log for numeric types
        /// </summary>
        int MaxElementsIfNumericList { get; set; }

        /// <summary>
        /// The maximum length of a parameter of non-numeric types to render in the log before it is trimmed
        /// </summary>
        int MaxStringLength { get; set; }

        /// <summary>
        /// Used to determine if a "Started" message should be displayed when Start()ing a timer 
        /// </summary>
        bool DisplayStartMessages { get; set; }

        //// <summary>
        /// Starts the stop watch for a given code block. NOTE: Adding parameterValuesToBeBlogged introduces what is known as boxing which can degrade performance 
        /// </summary>
        /// <param name="codeBlockName">The name of the code block, set as null to pull the calling method name from the Stack Trace</param>
        /// <param name="parameterValuesToLog">unlimited list of object values to be listed in the log</param>
        /// <returns>A string to be used when calling the Stop() method</returns>
        long Start(string codeBlockName, params object[] parameterValuesToBLogged);

        /// <summary>
        /// Stops the stop watch on a given code block and logs the execution length in milliseconds
        /// </summary>
        /// <param name="codeBlockName">The name of the code block, leave as null to pull the calling method's name from the Stack Trace</param>
        double Stop(long ID);

        /// <summary>
        /// Returns an unlimited list of object values as a string for common types
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        string ParamValuesToString(params object[] parameterValues);

        /// <summary>
        /// Writes an unlimited number of object values as a string to the log
        /// </summary>
        /// <param name="parameterValues"></param>
        void WriteParamValuesToString(params object[] parameterValues);

        /// <summary>
        /// Writes a string  
        /// </summary>
        /// <param name="Message"></param>
        void WriteMessage(string Message);

    }
}
