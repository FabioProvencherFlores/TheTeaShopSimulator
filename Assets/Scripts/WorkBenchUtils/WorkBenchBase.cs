using UnityEngine;

public class WorkBenchBase : MonoBehaviour
{
    protected bool myIsRunning;

    public virtual void StartWorkBench() { myIsRunning = true; }
    public virtual void StopWorkBench() { myIsRunning = false; }
}
