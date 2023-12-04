using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource source;
    private bool Muted = false;





    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void MuteButton(GameObject button) {
       if (Muted) Muted = false;
       else Muted = true;

       button.S

       if (Muted) { source.Pause(); Debug.Log("MUTING"); }
       else { source.Play(); Debug.Log("PLAYING"); }

    }

    public void QuitApp() {
        Application.Quit();
        Debug.Log("Closing Application.");
    }
    


}
