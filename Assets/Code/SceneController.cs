using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    public void Load_Scene(int build_index)
    {
        SceneManager.LoadScene(build_index);
    }
}
