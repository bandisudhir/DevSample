using System;
using System.Collections.Generic;

namespace DevSample
{
    class SampleGenerator
    {


        private readonly DateTime _sampleStartDate;
        private readonly TimeSpan _sampleIncrement;

        public SampleGenerator(DateTime sampleStartDate, TimeSpan sampleIncrement)
        {
            Samples = new List<Sample>();
            _sampleStartDate = sampleStartDate;
            _sampleIncrement = sampleIncrement;
        }


        /// <summary>
        /// Samples should be a time-descending ordered list
        /// </summary>
        public List<Sample> Samples { get; }


        public int SamplesValidated { get; private set; }


        public void LoadSamples(int samplesToGenerate)
        {

            //TODO: can we load samples faster?

            Samples.Clear();

            DateTime date = _sampleStartDate;

            for (int i = 0; i < samplesToGenerate; i++)
            {
                Sample s = new Sample(i == 0);
                s.LoadSampleAtTime(date);

                //_sampleList.Insert(0, s);
                //sudhir: Used Add instead of Insert in the list. The complexity for insert is O(n) snice it needs to move all elements and for add it is O(1)
                Samples.Add(s);
                date += _sampleIncrement;
            }


        }


        public void ValidateSamples()
        {
            //TODO: can we validate samples faster?

            //Validating elements from the end since we are validaing the previous elements added in the list using Add()
            int samplesValidated = 0;
            for (int i = Samples.Count; i > 0; i--)
            {
                if (Samples[i - 1].ValidateSample(i > 1 ? Samples[i - 2] : null, _sampleIncrement)) //in this sample the ValidateSample is always true but assume that's not always the case
                    samplesValidated++;
            }

            SamplesValidated = samplesValidated;
        }

    }
}

