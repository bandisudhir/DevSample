using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DevSample
{
    class Program
    {
        //Note: these settings should not be modified
        //hopefully we have more than 1 core to work with, run cores/2 cycles with a max of 4 cycles;

        static readonly int _cyclesToRun= Environment.ProcessorCount > 1 ? 
                                          Environment.ProcessorCount / 2 > 4?4: Environment.ProcessorCount / 2 :
                                          1; 
        static readonly int _samplesToLoad = 222222;
        static readonly DateTime _sampleStartDate= new DateTime(1990, 1, 1, 1, 1, 1, 1);
        static readonly TimeSpan _sampleIncrement= new TimeSpan(0, 5, 0);
        static readonly string _fileName= string.Format("DevSample_{0}.{1}", DateTime.Now.Ticks, "txt");

        static void Main(string[] args)
        {


            Stopwatch totalMonitor = new Stopwatch();
            totalMonitor.Start();

            LogMessage($"Starting Execution on a {Environment.ProcessorCount} core system. A total of {_cyclesToRun} cycles will be run");


            //sudhir: Used Parallel for to run multiple threads for the cpu cycles
            Parallel.For(0, _cyclesToRun, i =>
            {
                try
                {

                    TimeSpan cycleElapsedTime;
                    TimeSpan elapsedTiimeTillSampleLoad;

                    Stopwatch cycleTimer = new Stopwatch();

                    SampleGenerator sampleGenerator = new SampleGenerator(_sampleStartDate, _sampleIncrement);

                    LogMessage($"Cycle {i} Started Sample Load.");


                    cycleTimer.Start();

                    sampleGenerator.LoadSamples(_samplesToLoad);

                    cycleTimer.Stop();
                    elapsedTiimeTillSampleLoad  = cycleElapsedTime = cycleTimer.Elapsed;
                    LogMessage($"Cycle {i} Finished Sample Load. Load Time: {cycleElapsedTime.TotalMilliseconds.ToString("N")} ms.");
                    LogMessage($"Cycle {i} Started Sample Validation.");


                    cycleTimer.Restart();
                    sampleGenerator.ValidateSamples();

                    cycleTimer.Stop();
                    cycleElapsedTime = cycleTimer.Elapsed;

                    LogMessage($"Cycle {i} Finished Sample Validation. Total Samples Validated: {sampleGenerator.SamplesValidated}. Validation Time: {cycleElapsedTime.TotalMilliseconds.ToString("N")} ms.");

                    float valueSum = 0;
                    foreach (Sample s in sampleGenerator.Samples)
                        valueSum += s.Value;

                    //TODO: why do we only seem to get 7 digits of precision? The CEO wants to see at least 20!

                    //Sudhir: When i ran the program i got a precision of more than 20 digits eg: 1,39,31,21,00,00,00,00,00,000.00.
                    LogMessage($"Cycle {i} Sum of All Samples: {valueSum.ToString("N")}.");

                    //Sudhir: Fixed the incorrect total cycle time
                    LogMessage($"Cycle {i} Finished. Total Cycle Time: {(elapsedTiimeTillSampleLoad.TotalMilliseconds + cycleElapsedTime.TotalMilliseconds).ToString("N")} ms.");


                }
                catch (Exception ex)
                {
                    LogMessage($"Execution Failed!\n{ex.ToString()}");
                }


            });

            totalMonitor.Stop();

            LogMessage("-----");
            LogMessage($"Execution Finished. Total Elapsed Time: {totalMonitor.Elapsed.TotalMilliseconds.ToString("N")} ms.");


            Console.Read();

        }



        static void LogMessage(string message)
        {

            LogToFile(message);
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fffff")} - {message}");
        }

        static void LogToFile(string message)
        {
            //TODO: implement this when someone complains about it not working... 
            //everything written to the console should also be written to a log under C:\Temp. A new log with a unique file name should be created each time the application is run.

            //sudhir: Implemented logging to a text file
            try
            {
                using (FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", "C:/TEMP/", _fileName), FileMode.Append, FileAccess.Write))
                {
                    StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                    objStreamWriter.WriteLine(message);
                    objStreamWriter.Close();
                    objFilestream.Close();
                }
            }
            catch (Exception)
            {
                //
            }
        }
    }
}
