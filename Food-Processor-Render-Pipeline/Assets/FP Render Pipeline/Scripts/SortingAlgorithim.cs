using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Jobs;


public struct UnsafeLinkedListNode
{
    public EncodedData value;
    unsafe public UnsafeLinkedListNode* next;
    unsafe public UnsafeLinkedListNode* prev;
}

public class UnsafeLinkedList
{

    unsafe public UnsafeLinkedListNode *head = null;
    unsafe public UnsafeLinkedListNode *tail = null;
    public ulong Count { get; private set; }
    unsafe public void AddLast(EncodedData value)
    {
        UnsafeLinkedListNode temp = new UnsafeLinkedListNode
        {
            value = value
        };
        AddLast(&temp);
    }
    unsafe public void AddLast(UnsafeLinkedListNode* value)
    {
        unsafe
        {
            if(head !=null)
            {
                value->prev = tail;
                value->next = null;
                tail->next = value;
                Count++;
                
            }
            else
            {
                value->next = null;
                value->prev = null;
                head = value;
                tail = value;
                Count++;
            }
        }
    }
    unsafe public void CopyTo(EncodedData[] theArray, int index)
    {
        UnsafeLinkedListNode* currentNode = head;
        for(int i = 0; i< index; i++)
        {
            theArray[i] = currentNode->value;
            currentNode = currentNode->next;
        }
    }
}

public struct EncodedData
{
    public int key;
    public unsafe long* reference;
}

public struct LLContainer
{
    public UnsafeLinkedList anotherLinkedList;

    public void Initialize()
    {
        anotherLinkedList = new UnsafeLinkedList();
    }
}

class SortingAlgorithim : MonoBehaviour
{

    struct RadixCountingSortJob : IJob
    {
        // Jobs declare all data that will be accessed in the job
        // By declaring it as read only, multiple jobs are allowed to access the data in parallel

        [ReadOnly]
        public int capacity;//number of elements(n)
        [ReadOnly]
        public int bitShift;


        [ReadOnly]
        public NativeArray<EncodedData> encodedData;// = new NativeArray<ref>(capacity, Allocator.Persistent);

        // By default containers are assumed to be read & write
        public NativeArray<LLContainer> bucketsData;// = new NativeArray<lLContainer>(digits, Allocator.Persistent);

        [ReadOnly]
        private int theBitShift;
        [ReadOnly]
        private int digits;//base
        [ReadOnly]
        public int pass;//pass

        public void Initialize()
        {
            theBitShift = bitShift;
            digits = GetDigits(theBitShift);
        }

        int GetDigits(int theBitShift)
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
                        bucketsData[i].anotherLinkedList.AddLast(encodedData[j]);
                        location++;
                    }
                }
            }

        }
    }


    int GetDigits(int theBitShift)
    {
        int digits = 1;
        for (int i = 0; i < theBitShift; i++)
        {
            digits *= 2;
        }
        return digits;
    }

    private void ResetForNextPass(NativeArray<EncodedData> theData, NativeArray<LLContainer> BucketsArray)
    {
        int theDataIndex = 0;
        for(int theBucket = 0; theBucket < BucketsArray.Length; theBucket++)
        {
            
            EncodedData[] tempArray = new EncodedData[BucketsArray[theBucket].anotherLinkedList.Count];
            BucketsArray[theBucket].anotherLinkedList.CopyTo(tempArray, 0);
            
            for(int j = 0; j < tempArray.Length; j++)
            {
                theData[j + theDataIndex] = tempArray[j];
            }
            

            theDataIndex += tempArray.Length;
        }
    }


    private void Update()
    {
        List<EncodedData> someList = new List<EncodedData>();
        for(int i = 0; i< 100000; i++)
        {
            GameObject nothing = null;
            unsafe
            {
                EncodedData temp = new EncodedData
                {
                    key = Random.Range(0, int.MaxValue),
                    reference = (long*)((GameObject*)nothing),
                };
                someList.Add(temp);
            }
        }
        Run(someList);


        foreach (EncodedData encodedData in someList)
        {
            print(" " + encodedData.key);
            Debug.LogError(" " + encodedData.key);
            Debug.Log(" " + encodedData.key);
        }

    }

    /// <summary>
    /// with the defults this will consume about 16MB of memory per 500 GameObjects and 4096 digits/possible threads(this results from the bitshift of 12)
    /// At a bitshift = 16 it consumes about 250MB of memory per 500 GameObjects, and so on, so do be carefull altering these values carelessly as the memory costs
    /// increase at a rate of numberOfMegaBytes=(2^bitShift * numberOfGameObjects * 8 / 1024 / 1024)  To calculate the number of threads simply use 2^bitShift
    /// </summary>
    public void Run(List<EncodedData> theValues, int bitShift = 3)//three is uese because we are assuming we have 8 cores, we should really detect this somehow later
    {
        int capacity = theValues.Count;
        int digits = GetDigits(bitShift);

        //put the data in a native array here:
        //====================================
        NativeArray<EncodedData> theData = new NativeArray<EncodedData>(capacity, Allocator.Persistent);
        for (var i = 0; i < theData.Length; i++)
        {
            unsafe
            {
                EncodedData temp = new EncodedData()
                {
                    key = theValues[i].key,
                    reference = theValues[i].reference,
                };

                theData[i] = temp;
            }
        }
        //====================================

        //we should really rewrite this as a NativeArray of LinkedLists later(to fix memory issues and the lengthy time to read a bunch of nulls in reset for th next pass)
        NativeArray<LLContainer> BucketsArray = new NativeArray<LLContainer>(digits, Allocator.Persistent);
        for(int i = 0; i<digits; i++)
        {
            BucketsArray[i].Initialize();
        }


        //start alorithm here
        for (int pass = 0; pass < GetDigits(bitShift); pass++)//can't multithread due to being last pass dependent(need to rebuild data with finalise())
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

            job.Initialize();

            // Schedule the job, returns the JobHandle which can be waited upon later on
            JobHandle jobHandle = job.Schedule();

            // Ensure the job has completed
            // It is not recommended to Complete a job immediately,
            // since that gives you no actual parallelism.
            // You optimally want to schedule a job early in a frame and then wait for it later in the frame.
            jobHandle.Complete();

            //Debug.Log(job.bucketsData[0]);

            ResetForNextPass(theData, BucketsArray);
        }//end algorithim

        //remove the data from the native array here:
        //====================================
        for (var i = 0; i < theData.Length; i++)
        {
            unsafe
            {
                EncodedData temp = new EncodedData
                {
                    key = theValues[i].key,
                    reference = theValues[i].reference
                };
                theData[i] = temp;
            }
        }
        //====================================


        // Native arrays must be disposed manually
        BucketsArray.Dispose();
        theData.Dispose();
    }
}