using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCManager : MonoBehaviour
{

    [Header("Store Settings")]
    [SerializeField] BoxCollider arrivalArea;
    [SerializeField] NPCTabletteLookerHelper[] myNPCLookerHelpers;
    [SerializeField] NPCCashRegisterHelper myCashRegisteryHelper;

    [Header("NPC Spawn Settings")]
    [SerializeField] private Transform[] npcSpawnLocations;
    [SerializeField] GameObject customerPrefab;
    [SerializeField] float myNPCSpawnProbability = 1f;
    NPCArchetype[] myNPCArchetypes;

    private void Awake()
    {
        if (myNPCLookerHelpers.Length == 0) 
        {
            Debug.LogWarning("No looker helper defined for NPCs, does the scene require it?", this);        
        }

        if (myCashRegisteryHelper == null) Debug.LogWarning("NPC Manager does not have defined cash registery helper. does the scene require it?", this);

        myNPCArchetypes = Resources.LoadAll<NPCArchetype>("CustomerAI/");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnCustomer();
        }
    }

    private void Start()
    {
        InvokeRepeating("RollForNPCSpawn", 5f, 1f);
    }

    private void RollForNPCSpawn()
    {
        float roll = Random.Range(0f, 100f);
        if (roll < myNPCSpawnProbability) SpawnCustomer();
    }
    private void SpawnCustomer()
    {
        Transform spawnPosition = transform;
        if (npcSpawnLocations.Length > 0)
        {
            int chosenIdx = Random.Range(0, npcSpawnLocations.Length);
            spawnPosition = npcSpawnLocations[chosenIdx];
        }


        GameObject newCustomer = GameObject.Instantiate(customerPrefab);
        newCustomer.transform.position = spawnPosition.position;
        
        int randomIdx = Random.Range(0, myNPCArchetypes.Length);
        NPCArchetype goals = myNPCArchetypes[randomIdx];
        NPCController npcController = newCustomer.GetComponent<NPCController>();
        npcController.Init(this, goals);
        npcController.GoToNextGoal();
    }

    public void NotifyGoalReached(NPCController anNPCController)
    {
        if (!anNPCController.GoToNextGoal())
            Debug.Log("implement FORCE LEAVE");
    }

    public Vector3 GetFurthestSpawnPosition(Vector3 aPosition)
    {
        Vector3 position = transform.position;
        float bestSoFar = 0f;

        foreach (Transform spawnerPos  in npcSpawnLocations)
        {
            Vector3 distanceVector = spawnerPos.position - aPosition;
            float distSqr = distanceVector.sqrMagnitude;
            if (distSqr > bestSoFar)
            {
                bestSoFar = distSqr;
                position = spawnerPos.position;
            }
        }

        return position;
    }

    public void NotifyShoppingComplete(NPCController anNPCController)
    {
        if (!anNPCController.myCurrentState.IsTransactionPayed)
        {
            GameManager.Instance.myDayScore.myDisatisfiedCustomers += 1;
        }
        else
        {
            GameManager.Instance.myDayScore.mySatisfiedCustomers += 1;
        }

        GameObject.Destroy(anNPCController.gameObject);
        myNPCSpawnProbability += 2f;
    }
    
    public Vector3 GetArrivalPosition()
    {
        return new Vector3(
        Random.Range(arrivalArea.bounds.min.x, arrivalArea.bounds.max.x),
        0.5f,
            Random.Range(arrivalArea.bounds.min.z, arrivalArea.bounds.max.z)
        );
    }
    
    public bool GetTabletLookerHelpers(out List<NPCTabletteLookerHelper> someLookerHelpers)
    {
        someLookerHelpers = new List<NPCTabletteLookerHelper>(myNPCLookerHelpers);
        return someLookerHelpers.Count > 0;
    }
    
    public NPCCashRegisterHelper GetCashRegisteryHelper()
    {
        return myCashRegisteryHelper;
    }


}
