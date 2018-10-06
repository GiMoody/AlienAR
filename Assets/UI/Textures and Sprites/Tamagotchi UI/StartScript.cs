using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour {

    public void ContinuaClicked() {
        SceneManager.LoadSceneAsync("AlienAR");
    }

    public void StartClicked()
    {
        string saveGameFileName = "saves";
        string filePath = Path.Combine(Application.persistentDataPath, "data");
        filePath = Path.Combine(filePath, saveGameFileName + ".binary");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        SceneManager.LoadSceneAsync("AlienAR");
    }
}
