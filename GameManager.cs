using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    
    // keep track of game state

    public GameObject[] SystemPrefabs;
    List<GameObject> _instancedSystemPrefabs;
    List<AsyncOperation> _loadOperations;
    string _currentLevelName = string.Empty;



    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _instancedSystemPrefabs = new List<GameObject>();
        _loadOperations = new List<AsyncOperation>();

        InstantiateSystemPrefabs();

        LoadLevel("Main");
    }

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if(_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);
        }
        Debug.Log("Load Complete.");
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload complete.");
    }
    
    void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;

        for (int i=0; i<SystemPrefabs.Length; ++i)
        {
            prefabInstance = Instantiate(SystemPrefabs[i]);
            _instancedSystemPrefabs.Add(prefabInstance);
        }
    }

    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if(ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level " + levelName);
            return;
        }
        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);
        _currentLevelName = levelName;
    }



    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        if(ao == null)
        {
            Debug.LogError("[GameManager] Unable to unload level " + levelName);
            return;
        }
        ao.completed += OnUnloadOperationComplete;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        for (int i = 0; i < _instancedSystemPrefabs.Count; i++)
        {
            Destroy(_instancedSystemPrefabs[i]);
        }
           _instancedSystemPrefabs.Clear();
    }

}
