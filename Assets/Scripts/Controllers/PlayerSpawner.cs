using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    GameObject previousCamera;

    bool _playerSpawned = false;

    // Update is called once per frame
    void Update()
    {
        if (_playerSpawned)
        {
            gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject newPlayer = GameObject.Instantiate(playerPrefab);
            newPlayer.transform.position = transform.position;
            GameObject[] oldCamera = GameObject.FindGameObjectsWithTag("DeletableCamera");
            if (oldCamera.Length == 0) Debug.LogError("didn't find old camera to delete, add DeletableCamera tag", this);
            else oldCamera[0].SetActive(false);
            _playerSpawned = true;
		}

	}
}
