using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour{

    public Canvas canvas;
    public UnityEngine.UI.Button playButton;
    public GameObject mainScreen;

	// Use this for initialization
	void Start () {
        this.playButton.onClick.AddListener(() => this.clickPlayButton());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            clickPlayButton();
        }

        if(this.mainScreen.GetComponent<CanvasGroup>().alpha < 1)
        {
            this.mainScreen.GetComponent<CanvasGroup>().alpha += 0.5f * Time.deltaTime;
        }
    }

    private void clickPlayButton()
    {
        SceneManager.LoadScene(1);
    }
}
