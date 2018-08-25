using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

namespace CodeHelpers
{
    public static class PerformanceTester
    {
        public static void TestWithPrint(Action thisTest, int times = 10000)
        {
            UnityEngine.Debug.Log("This test takes " + Test(thisTest, times).tookMilliseconds + "ms.");
        }

        public static void TestWithPrint(PerformanceTest thisTest)
        {
            UnityEngine.Debug.Log("This test takes " + Test(thisTest).tookMilliseconds + "ms.");
        }

        public static PerformanceTest Test(Action thisTest, int times = 10000)
        {
            Stopwatch thisWatch = new Stopwatch();
            thisWatch.Start();

            for (int i = 0; i < times; i++)
            {
                thisTest();
            }

            thisWatch.Stop();
            return new PerformanceTest(thisTest, times)
            {
                tookMilliseconds = thisWatch.Elapsed.TotalMilliseconds,
                tookNanoseconds = thisWatch.Elapsed.TotalMilliseconds * 1000000
            };
        }

        public static PerformanceTest Test(PerformanceTest thisTest)
        {
            return Test(thisTest.thisTest, thisTest.testTimes);
        }
    }

    public struct PerformanceTest
    {
        public PerformanceTest(Action thisTest, int testTimes = 10000)
        {
            this.thisTest = thisTest;
            this.testTimes = testTimes;

            tookMilliseconds = 0;
            tookNanoseconds = 0;
        }

        public Action thisTest;
        public int testTimes;

        //Results
        public double tookMilliseconds;
        public double tookNanoseconds;
    }

}