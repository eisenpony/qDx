using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Quotidian.Diagnostics.Tests
{
    public class ExtensionTests
    {
        [Test]
        public void TestToTrace()
        {
            var trace = "";

            try
            {
                try
                {
                    throw new ArgumentOutOfRangeException("arg", "Low level error");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new ApplicationException("Wrapping error", e);
                }
            }
            catch (ApplicationException ae)
            {
                trace = ae.ToTrace();
            }

            var expected = File.ReadAllText("ExpectedToTrace.txt");
            trace = Regex.Replace(trace, "(?<=in ).*(?=\r\n)", "***");

            Assert.AreEqual(expected, trace);
        }
    }
}
