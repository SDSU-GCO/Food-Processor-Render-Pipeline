using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

class SortingAlgorithim : MonoBehaviour
{
    struct EncodedData
    {
        public int key;
        public GameObject reference;
    }

    struct RadixCountingSortJob : IJob
    {
        // Jobs declare all data that will be accessed in the job
        // By declaring it as read only, multiple jobs are allowed to access the data in parallel

        [ReadOnly]
        public int capacity;//number of elements
        [ReadOnly]
        public int bitShift;


        [ReadOnly]
        public NativeArray<EncodedData> encodedData;// = new NativeArray<ref>(capacity, Allocator.Persistent);

        // By default containers are assumed to be read & write
        public NativeArray<EncodedData> bucketsData;// = new NativeArray<ref>(digits * capacity, Allocator.Persistent);

        [ReadOnly]
        private int theBitShift;
        [ReadOnly]
        private int digits;//base
        [ReadOnly]
        public int pass;//pass

        void Initialize()
        {
            theBitShift = bitShift;
            digits = getDigits(theBitShift);
        }

        int getDigits(int theBitShift)
        {
            int digits = 1;
            for (int i = 0; i < theBitShift; i++)
            {
                digits *= 2;
            }
            return digits;
        }

        // Delta time must be copied to the job since jobs generally don't have concept of a frame.
        // The main thread waits for the job on the same frame or the next frame, but the job should
        // perform work in a deterministic and independent way when running on worker threads.
        //public float deltaTime;

        // The code actually running on the job
        public void Execute()
        {
            Initialize();
            // Move the positions based on delta time and velocity
            for (int i = 0; i < digits; i++)//this gets multithreaded
            {
                int location = 0;
                for (int j = 0; j < encodedData.Length; j++)//can't multithread due to being last pass dependent(location variable needed)
                {
                    int newKey = encodedData[j].key;
                    newKey = newKey >> (theBitShift * (pass + 1));

                    if (newKey % digits == i)
                    {
                        bucketsData[i * location] = encodedData[j];
                        location++;
                    }
                    else
                    {
                        EncodedData temp = new EncodedData();
                        temp.key = 0;
                        temp.reference = null;
                        bucketsData[i * location] = temp;
                    }
                }
            }

        }
    }


    int getDigits(int theBitShift)
    {
        int digits = 1;
        for (int i = 0; i < theBitShift; i++)
        {
            digits *= 2;
        }
        return digits;
    }

    private void resetForNextPass(NativeArray<EncodedData> theData, NativeArray<EncodedData> BucketsArray)
    {
        for (int i = 0; i< BucketsArray.Length; i++)
        {
            int tracker=0;
            if (BucketsArray[i].reference != null)
            {
                theData[tracker] = BucketsArray[i];
                tracker++;
            }
        }
    }

    public void Update()
    {
        //varaibles
        int bitShift = 16;
        int capacity = 500;

        //put the data in a native array here:
        //====================================
        NativeArray<EncodedData> theData = new NativeArray<EncodedData>(capacity, Allocator.Persistent);
        for (var i = 0; i < theData.Length; i++)
        {
            EncodedData temp = new EncodedData();
            temp.key = 0;
            temp.reference = null;
            theData[i] = temp;
        }
        //====================================

        NativeArray<EncodedData> BucketsArray = new NativeArray<EncodedData>(getDigits(bitShift) * capacity, Allocator.Persistent);


        //start alorithm here
        for (int pass = 0; pass < bitShift; pass++)//can't multithread due to being last pass dependent(need to rebuild data with finalise())
        {
            // Initialize the job data
            var job = new RadixCountingSortJob()
            {
                encodedData = theData,
                bucketsData = BucketsArray,
                bitShift = bitShift,
                capacity = theData.Length,
                pass = pass,
            };

            // Schedule the job, returns the JobHandle which can be waited upon later on
            JobHandle jobHandle = job.Schedule();

            // Ensure the job has completed
            // It is not recommended to Complete a job immediately,
            // since that gives you no actual parallelism.
            // You optimally want to schedule a job early in a frame and then wait for it later in the frame.
            jobHandle.Complete();

            //Debug.Log(job.bucketsData[0]);
            resetForNextPass(theData, BucketsArray);
        }//end algorithim


        

        // Native arrays must be disposed manually
        BucketsArray.Dispose();
        theData.Dispose();
    }
}