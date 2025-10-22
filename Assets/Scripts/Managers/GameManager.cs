using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public struct EndDayScore
{
	public int mySatisfiedCustomers;
	public float myTotalProfits;
	public int myDisatisfiedCustomers;

	public void Reset()
	{
		mySatisfiedCustomers = 0;
		myTotalProfits = 0f;
		myDisatisfiedCustomers = 0;
    }
}

public class GameManager : MonoBehaviour
{
    #region Instance
    private static GameManager _instance;
	public static GameManager Instance
	{
		get
		{
			if (_instance is null)
				Debug.LogError("No Game Manager is found");

			return _instance;
		}
	}
	#endregion

	[SerializeField] private GameObject _computerUIController;


    ResourceManager _resourceManager;
	InteractionManager _interactionManager;
	NPCManager _npcManager;

    private UnityEvent<bool> OnWorldMovementLockedChange = new UnityEvent<bool>();
	private bool _worldMovementIsLocked;

	string loseReason;
	public string GetLoseReason() { return loseReason; }

	public void RegisterToWorldMovementLock(UnityAction<bool> aCallback) { OnWorldMovementLockedChange.AddListener(aCallback); }
	public void DeregisterFromWorldMovementLock(UnityAction<bool> aCallback) { OnWorldMovementLockedChange.RemoveListener(aCallback); }

	private float timeEllapsedSinceStartOfDayMS = 0f;

	public EndDayScore myDayScore;

	[Header("Time Management")]
	[SerializeField] int myDayStartInMinutes = 540;
	[SerializeField] int myDayEndInMinutes = 1020;
	public float TimeOfDayInMinutes { get; private set; }

    void Awake()
	{
		_instance = this;
		if (_computerUIController == null) Debug.LogError("No Computer UI controller provided in this scene", this);

        DontDestroyOnLoad(this.gameObject);
    }

	void Start()
	{
		_interactionManager = GetComponent<InteractionManager>();
		if (_interactionManager == null) Debug.LogWarning("No Interaction Manager, does your scene require one?", this);
		_resourceManager = GetComponent<ResourceManager>();
		if (_resourceManager == null) Debug.LogWarning("No Resource Manager, does your scene require one?", this);
        _npcManager = GetComponent<NPCManager>();
		if (_npcManager == null) Debug.LogWarning("No NPC Manager, does your scene require one?", this);
		StartNewDay();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) Application.Quit();

        timeEllapsedSinceStartOfDayMS += Time.deltaTime;
		TimeOfDayInMinutes += Time.deltaTime;

		if (TimeOfDayInMinutes > myDayEndInMinutes) GoToEndOfDayScore();
    }

	public void RequestGoToNextDay()
	{
        SceneManager.LoadScene("Floor_V1");
		StartNewDay();
	}

	private void GoToEndOfDayScore()
	{
        SceneManager.LoadScene("Score Screen");
	}

	private void StartNewDay()
	{
		myDayScore.Reset();
        timeEllapsedSinceStartOfDayMS = 0f;
		TimeOfDayInMinutes = myDayStartInMinutes;
		GoToRegularGameplay();
	}

	public void GoToRegularGameplay()
	{
		ShouldUnlockWorldMovement();
		_computerUIController.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

	public void GoToComputerInteractionGameplay()
	{
		ShouldLockWorldMovement();
		_computerUIController.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

	private void ShouldLockWorldMovement()
	{
		bool previousValue = _worldMovementIsLocked;
        _worldMovementIsLocked = true;
		if (previousValue != _worldMovementIsLocked) OnWorldMovementLockedChange.Invoke(_worldMovementIsLocked);
    }

	private void ShouldUnlockWorldMovement()
	{
		bool previousValue = _worldMovementIsLocked;
        _worldMovementIsLocked = false;
		if (previousValue != _worldMovementIsLocked) OnWorldMovementLockedChange.Invoke(_worldMovementIsLocked);
	}
}
