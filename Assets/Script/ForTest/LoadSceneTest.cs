using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneTest : MonoBehaviour
{
    public GameObject prefab;
    public static LoadSceneTest Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(SceneLoadTest("ObjectPoolingTest"));
    }

    IEnumerator SceneLoadTest(string sceneName)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        yield return new WaitForSeconds(2f);
        print("Load2");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

}
