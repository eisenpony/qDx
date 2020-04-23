using qdx.Samples.FileCreator.Diagnostics;
using Quotidian.Diagnostics.Source;
using Quotidian.Diagnostics.Source.Domain;
using System;
using System.IO;

namespace qdx.Samples.FileCreator
{
    public class Creator
    {

        public Creator(ITrace trace) // This class sends trace messages, so it requires an ITrace dependency to be injected.
        {
            Trace = trace ?? throw new ArgumentNullException(nameof(trace));
        }

        private ITrace Trace { get; }

        // SecureTrace is an optional dependency which allows the user of this class to recieve "sensitive" messages on a secondary channel.
        // Optional dependencies should be implemented as "popsicle" properties -- they can only be set once, and then they "freeze".
        // Allowing dependencies to change at runtime can lead to surprising behaviour.
        private ITrace secureTrace;
        public ITrace SecureTrace
        { 
            // If secureTrace is null when needed, just set it to the standard trace.
            get => secureTrace ?? (secureTrace = Trace);

            // If secureTrace already has a value, don't change it. Send a trace message or throw an exception.
            set
            {
                if (secureTrace != null)
                    throw new InvalidOperationException($"The property '{nameof(SecureTrace)}' cannot be changed once set");

                secureTrace = value;
            }
        }
        
        public void MakeFiles(string targetDirectory, int num)
        {
            // Use exceptions when you run into a problem you don't want to recover from. Don't send a trace message in addition to throwing the exception.
            // Here, a custom ErrorCode exception type is used to map problems to unique identifiers.
            // This pattern supports globalization and localization by removing the need for translated error messages to be kept in code.
            if (num < 0)
                throw new ErrorCode(EventCodes.Khopesh,
                    new ArgumentOutOfRangeException(nameof(num)));

            if (!Directory.Exists(targetDirectory))
                throw new ErrorCode(EventCodes.Shotel,
                    new DirectoryNotFoundException(targetDirectory));

            // They may not be pretty, but verbose and information trace messages can be useful when tracking down tough runtime errors.
            // They are most useful in deep domain level algorithms whose behaviour might be invisible otherwise. Use them to explain what is being done.
            Trace.Information($"Starting to create {num} files");

            int i = 0;
            for (int j = 0; i < num; j++)
            {
                try
                {
                    Trace.Verbose($"Attempting to create file {j}.txt");

                    using (File.Open(Path.Combine(targetDirectory, $"{j}.txt"), FileMode.CreateNew, FileAccess.Write)) ; // don't do anything with the file, just create it.
                    Trace.Verbose($"Created file {j}.txt");

                    // You're not limited to sending strings and Exceptions as trace messages.
                    // Passing objects is a common approach for getting information to trace adapters that "instrument" an application, providing rich performance counter capabilities.
                    // The code parameter can be used to help identify trace messages that are intended for specific uses.
                    Trace.Information(new FileInfo(Path.Combine(targetDirectory, $"{j}.txt")), code: EventCodes.Ballista);

                    i++;
                }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is NotSupportedException)
                {
                    // Warning trace messages are useful for explaining how an algorithm is recovering from a problem it detected

                    // When you recover from an exception and want to add some additional information, create a new, wrapping Exception.
                    // Trace.Warning(new ApplicationException($"Tried to create file {j}.txt but failed. Will skip this file name and continue trying", e));

                    // Alternatively, if the captured exception may contain sensitive information, you can send a replacement exception to the standard trace and the original exception to a "secure" trace
                    // The correlation parameter can be used to help correlate the message a user recieves with the secure logs.
                    var c = Guid.NewGuid().ToString();
                    Trace.Warning($"Tried to create a file but failed. Will skip this file name and continue trying", correlation: c);
                    SecureTrace.Warning(new ApplicationException($"Tried to create file {j}.txt but failed. Will skip this file name and continue trying", e), correlation: c);
                }

                if (j == int.MaxValue)
                {
                    Trace.Warning($"Can't make more than {int.MaxValue} files. Giving up.");
                    return;
                }
            }

            Trace.Information($"Done creating {i} files");
        }
    }
}
