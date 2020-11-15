using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using LSL;
using Assets.LSL4Unity.Scripts;

namespace ExciteOMeter
{
    public abstract class ExciteOMeterBaseInlet : MonoBehaviour
    {
        //     StreamID         |    TYPE
        // ====================================
        // "HeartRate" (short)  | ExciteOMeter
        // "RRinterval" (float) | ExciteOMeter
        // "RawECG" (int)       | ExciteOMeter
        // "RawACC" (int)       | ExciteOMeter
        // ====================================

        public string StreamName = "Write_Here_Expected_Stream_Id";
        
        public string StreamType = "ExciteOMeter";

        public ExciteOMeter.Devices Devices = Devices.PolarH10;

        public ExciteOMeter.DataType VariableType = DataType.NONE;
        
        protected liblsl.StreamInlet inlet;

        protected int expectedChannels;

        protected static Resolver resolver;

        private bool pullSamplesContinuously = false;

        /// <summary>
        /// Find a common resolver for all the instances
        /// </summary>
        void Awake()
        {
            if(resolver == null)
                resolver = FindObjectOfType<Resolver>();

            if(VariableType == DataType.NONE)
                ExciteOMeterManager.LogError("Please define the type of measurement of this LSL inlet in GameObject: " + gameObject.name);
                
        }

        /// <summary>
        /// Call this method when your inlet implementation got created at runtime
        /// </summary>
        protected virtual void registerAndLookUpStream()
        {
            if(resolver != null)
            {
                resolver.onStreamFound.AddListener(new UnityAction<LSLStreamInfoWrapper>(AStreamIsFound));
                resolver.onStreamLost.AddListener(new UnityAction<LSLStreamInfoWrapper>(AStreamGotLost));
            }
            
            if (resolver.knownStreams.Any(isTheExpected))
            {
                var stream = resolver.knownStreams.First(isTheExpected);
                AStreamIsFound(stream);
            }
        }

        /// <summary>
        /// Callback method for the Resolver gets called each time the resolver found a stream
        /// </summary>
        /// <param name="stream"></param>
        public virtual void AStreamIsFound(LSLStreamInfoWrapper stream)
        {
            if (!isTheExpected(stream))
                return;

            ExciteOMeterManager.DebugLog(string.Format("LSL Stream {0} found for {1}", stream.Name, name));

            // In case the signal emulator is running. Deactivate it.
            EoM_SignalEmulator.DisableEmulator();

            inlet = new LSL.liblsl.StreamInlet(stream.Item);
            expectedChannels = stream.ChannelCount;

            OnStreamAvailable();
        }

        /// <summary>
        /// Callback method for the Resolver gets called each time the resolver misses a stream within its cache
        /// </summary>
        /// <param name="stream"></param>
        public virtual void AStreamGotLost(LSLStreamInfoWrapper stream)
        {
            if (!isTheExpected(stream))
                return;

            ExciteOMeterManager.DebugLog(string.Format("LSL Stream {0} Lost for {1}", stream.Name, name));

            OnStreamLost();
        }

        protected virtual bool isTheExpected(LSLStreamInfoWrapper stream)
        {
            bool predicate = StreamName.Equals(stream.Name);
            predicate &= StreamType.Equals(stream.Type);
            // add a more specific description for your stream here specifying hostname etc.
            //predicate &= stream.HostName.Equals("Expected Hostname");

            return predicate;
        }

        protected abstract void pullSamples();

        protected virtual void OnStreamAvailable()
        {
            EoM_Events.Send_OnStreamConnected(VariableType);

            // decide what happens when the stream gets available
            pullSamplesContinuously = true;
        }

        protected virtual void OnStreamLost()
        {
            // decide what happens when the stream gets lost
            pullSamplesContinuously = false;
            EoM_Events.Send_OnStreamDisconnected(VariableType);
        }

        private void Update()
        {
            // Do not collect signals from LSL when ExciteOMeter is postProcessing stage
            // Otherwise it will continue adding data on the CSV while wrapping up.
            if(pullSamplesContinuously)
                pullSamples();
        }

    }

    public abstract class InletFloatSamples : ExciteOMeterBaseInlet
    {
        protected abstract void Process(float[] newSample, double timeStamp);

        protected float[] sample;

        protected override void pullSamples()
        {
            sample = new float[expectedChannels];

            try
            {
                double lastTimeStamp = inlet.pull_sample(sample, 0.0f);

                if (lastTimeStamp != 0.0)
                {
                    // do not miss the first one found
                    Process(sample, lastTimeStamp);
                    // pull as long samples are available
                    while ((lastTimeStamp = inlet.pull_sample(sample, 0.0f)) != 0)
                    {
                        Process(sample, lastTimeStamp);
                    }

                }
            }
            catch (ArgumentException aex)
            {
                Debug.LogError("An Error on pulling samples deactivating LSL inlet on...", this);
                this.enabled = false;
                Debug.LogException(aex, this);
            }

        }
    }

    public abstract class InletDoubleSamples : ExciteOMeterBaseInlet
    {
        protected abstract void Process(double[] newSample, double timeStamp);

        protected double[] sample;

        protected override void pullSamples()
        {
            sample = new double[expectedChannels];

            try
            {
                double lastTimeStamp = inlet.pull_sample(sample, 0.0f);

                if (lastTimeStamp != 0.0)
                {
                    // do not miss the first one found
                    Process(sample, lastTimeStamp);
                    // pull as long samples are available
                    while ((lastTimeStamp = inlet.pull_sample(sample, 0.0f)) != 0)
                    {
                        Process(sample, lastTimeStamp);
                    }

                }
            }
            catch (ArgumentException aex)
            {
                Debug.LogError("An Error on pulling samples deactivating LSL inlet on...", this);
                this.enabled = false;
                Debug.LogException(aex, this);
            }

        }
    }

    public abstract class InletIntSamples : ExciteOMeterBaseInlet
    {
        protected abstract void Process(int[] newSample, double timeStamp);

        protected int[] sample;

        protected override void pullSamples()
        {
            sample = new int[expectedChannels];
                        
            try
            {
                double lastTimeStamp = inlet.pull_sample(sample, 0.0f);

                if (lastTimeStamp != 0.0)
                {
                    // do not miss the first one found
                    Process(sample, lastTimeStamp);

                    // pull as long samples are available
                    while ((lastTimeStamp = inlet.pull_sample(sample, 0.0f)) != 0)  
                    {
                        Process(sample, lastTimeStamp);
                    }

                }
            }
            catch (ArgumentException aex)
            {
                Debug.LogError("An Error on pulling samples deactivating LSL inlet on...", this);
                this.enabled = false;
                Debug.LogException(aex, this);
            }

        }
    }

    public abstract class InletCharSamples : ExciteOMeterBaseInlet
    {
        protected abstract void Process(char[] newSample, double timeStamp);

        protected char[] sample;

        protected override void pullSamples()
        {
            sample = new char[expectedChannels];

            try
            {
                double lastTimeStamp = inlet.pull_sample(sample, 0.0f);

                if (lastTimeStamp != 0.0)
                {
                    // do not miss the first one found
                    Process(sample, lastTimeStamp);
                    // pull as long samples are available
                    while ((lastTimeStamp = inlet.pull_sample(sample, 0.0f)) != 0)
                    {
                        Process(sample, lastTimeStamp);
                    }

                }
            }
            catch (ArgumentException aex)
            {
                Debug.LogError("An Error on pulling samples deactivating LSL inlet on...", this);
                this.enabled = false;
                Debug.LogException(aex, this);
            }

        }
    }

    public abstract class InletStringSamples : ExciteOMeterBaseInlet
    {
        protected abstract void Process(String[] newSample, double timeStamp);

        protected String[] sample;

        protected override void pullSamples()
        {
            sample = new String[expectedChannels];

            try
            {
                double lastTimeStamp = inlet.pull_sample(sample, 0.0f);

                if (lastTimeStamp != 0.0)
                {
                    // do not miss the first one found
                    Process(sample, lastTimeStamp);
                    // pull as long samples are available
                    while ((lastTimeStamp = inlet.pull_sample(sample, 0.0f)) != 0)
                    {
                        Process(sample, lastTimeStamp);
                    }

                }
            }
            catch (ArgumentException aex)
            {
                Debug.LogError("An Error on pulling samples deactivating LSL inlet on...", this);
                this.enabled = false;
                Debug.LogException(aex, this);
            }

        }
    }

    public abstract class InletShortSamples : ExciteOMeterBaseInlet
    {
        protected abstract void Process(short[] newSample, double timeStamp);

        protected short[] sample;

        protected override void pullSamples()
        {
            sample = new short[expectedChannels];

            try
            {
                double lastTimeStamp = inlet.pull_sample(sample, 0.0f);

                if (lastTimeStamp != 0.0)
                {
                    // do not miss the first one found
                    Process(sample, lastTimeStamp);
                    // pull as long samples are available
                    while ((lastTimeStamp = inlet.pull_sample(sample, 0.0f)) != 0)
                    {
                        Process(sample, lastTimeStamp);
                    }

                }
            }
            catch (ArgumentException aex)
            {
                Debug.LogError("An Error on pulling samples deactivating LSL inlet on...", this);
                this.enabled = false;
                Debug.LogException(aex, this);
            }

        }
    }


    public abstract class InletIntChunk : ExciteOMeterBaseInlet
    {
        private readonly int MAX_CHUNK_SIZE = 90;

        protected abstract void Process(int numSamples, int[,] newSamples, double[] timeStamp);

        protected int[,] samples;
        protected double[] timestamps;

        int numberSamples;

        
        protected override void OnStreamAvailable()
        {
            base.OnStreamAvailable();
                    
            samples = new int[expectedChannels,MAX_CHUNK_SIZE];
            timestamps = new double[MAX_CHUNK_SIZE];        
        }

        protected override void pullSamples()
        {
            try
            {
                numberSamples = inlet.pull_chunk(samples, timestamps, 5.0f);

                if (numberSamples != 0)
                {
                    Process(numberSamples, samples, timestamps);
                }
            }
            catch (ArgumentException aex)
            {
                Debug.LogError("An Error on pulling samples deactivating LSL inlet on...", this);
                this.enabled = false;
                Debug.LogException(aex, this);
            }

        }
    }
}