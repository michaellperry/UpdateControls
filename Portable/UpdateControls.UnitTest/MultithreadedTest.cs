using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.UnitTest.MultithreadedData;

namespace UpdateControls.UnitTest
{
    [TestClass]
    public class MultithreadedTest
    {
        [TestMethod]
        public void IsThreadSafe()
        {
            const int ThreadCount = 10;

            // Start source threads.
            SourceThread[] sources = new SourceThread[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                sources[i] = new SourceThread();
                sources[i].Start();
            }

            // Start target threads.
            TargetThread[] targets = new TargetThread[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                targets[i] = new TargetThread(sources);
                targets[i].Start();
            }

            // Wait for all threads to finish.
            for (int i = 0; i < ThreadCount; i++)
            {
                sources[i].Join();
                targets[i].Join();
            }

            // All targets are in the correct state.
            for (int i = 0; i < ThreadCount; i++)
            {
                Assert.AreEqual(ThreadCount * SourceThread.MaxValue, targets[i].Total);
            }
        }
    }
}
