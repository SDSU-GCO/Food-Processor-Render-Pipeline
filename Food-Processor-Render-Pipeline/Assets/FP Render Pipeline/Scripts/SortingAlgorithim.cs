using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Jobs;

//Alex: please note a lot of the math in the comments might be wrong, but the code whould be right.  I was dead tired writing this and got 
//Alex: logbase(2)n confused with (2^n) ...  #quikMafs



unsafe public struct UnsafeLinkedListNode
{
    public EncodedData value;
    unsafe public UnsafeLinkedListNode* next;
    unsafe public UnsafeLinkedListNode* prev;
}

public struct UnsafeLinkedList
{

    unsafe public UnsafeLinkedListNode* head;
    unsafe public UnsafeLinkedListNode* tail;
    public ulong Count { get; private set; }

    public unsafe void Initialize()
    {
        head = null;
        tail = null;
    }

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

    unsafe public void DeleteHead()
    {
        UnsafeLinkedListNode* temp = head;
        temp->next->prev = null;
        head = temp->next;
        temp->delete();//destroy itself, broken
    }
}

public struct LLContainer
{
    public UnsafeLinkedList anotherLinkedList;

    public void Initialize()
    {
        anotherLinkedList = new UnsafeLinkedList();
        anotherLinkedList.Initialize();
    }
}

class SortingAlgorithim : MonoBehaviour
{

    struct RadixCountingSortJob : IJob 
    {
        //Alex:  Jobs declare all data that will be accessed in the job
        //Alex:  By declaring it as read only, multiple jobs are allowed to access the data in parallel

        [ReadOnly]
        public int capacity;//Alex: number of elements(n)


        [ReadOnly]
        public NativeArray<EncodedData> encodedData;//Alex:  = new NativeArray<ref>(capacity, Allocator.Persistent);

        //Alex:  By default containers are assumed to be read & write
        public NativeArray<EncodedData> outputData;//Alex:  = new NativeArray<lLContainer>(digits, Allocator.Persistent);
        

        // Delta time must be copied to the job since jobs generally don't have concept of a frame.
        // The main thread waits for the job on the same frame or the next frame, but the job should
        // perform work in a deterministic and independent way when running on worker threads.
        //public float deltaTime;

        // The code actually running on the job
        public void Execute()
        {
            
        }
    }

    public struct EncodedData
    {
        public int key;
        public ulong index;
    }

    private void Update()
    {
        List<GameObject> inView = new List < GameObject >();

    }

    /// <summary>
    /// Alex: Does a parrallel QuickSort
    /// </summary>
    public void quickSort(NativeList<GameObject> inView)//Alex: three is uese because we are assuming we have 8 cores, we should really detect this somehow later
    {


        List<EncodedData> theValues = new List<EncodedData>();
        List<GameObject> sortedList = new List<GameObject>();

        for (ulong i = 0; i < (ulong)inView.Length; i++)
        {
            EncodedData temp = new EncodedData
            {
                key = Random.Range(0, int.MaxValue),
                index = i,
            };
            someList.Add(temp);
        }

        quickSort(someList);

        for (int i = 0; i < inView.Length; i++)
        {
            sortedList[i] = inView[(int)someList[i].index];
            print(someList[i].index + " " + someList[i].key);
        }

        //put the data in a native array here:
        //Alex: ====================================
        NativeArray<EncodedData> theData = new NativeArray<EncodedData>(theValues.Count, Allocator.Persistent);
        for (var i = 0; i < theData.Length; i++)
        {
            theData[i] = theValues[i];
        }
        //Alex: ====================================

        //Alex: we should really rewrite this as a NativeArray of LinkedLists later(to fix memory issues and the lengthy time to read a bunch of nulls in reset for th next pass)
        NativeArray<EncodedData> outputArray = new NativeArray<EncodedData>(theValues.Count, Allocator.Persistent);
        for(int i = 0; i< theValues.Count; i++)
        {

        }
        
        //Alex:  Initialize the job data
        var job = new RadixCountingSortJob()
        {
            encodedData = theData,
            bucketsData = outputArray,
            capacity = theData.Length,
        };

        // Schedule the job, returns the JobHandle which can be waited upon later on
        JobHandle jobHandle = job.Schedule();

        // Ensure the job has completed
        // It is not recommended to Complete a job immediately,
        // since that gives you no actual parallelism.
        // You optimally want to schedule a job early in a frame and then wait for it later in the frame.
        jobHandle.Complete();

        //Alex: remove the data from the native array here:
        //Alex: ====================================
        for (var i = 0; i < theData.Length; i++)
        {
            theValues[i] = outputArray[i];
        }
        //Alex: ====================================


        // Native arrays must be disposed manually
        outputArray.Dispose();
        theData.Dispose();
    }
}