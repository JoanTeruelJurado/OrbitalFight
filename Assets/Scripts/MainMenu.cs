using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioSource source;
    private bool Muted = false;

    public Sprite[] spriteArray;



    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void MuteButton(Button button) {
       if (Muted) Muted = false;
       else Muted = true;
        
       var State = button.spriteState;
       
       if (Muted) { 
            button.image.sprite = spriteArray[0];
            State.pressedSprite = spriteArray[1];
            source.Pause(); 
       }
       else { 
            button.image.sprite = spriteArray[2];
            State.pressedSprite = spriteArray[3];
            source.Play();
       }
       button.spriteState = State;
    }

    public void QuitApp() {
        Application.Quit();
        Debug.Log("Closing Application.");
    }
    


}
