using UnityEngine;
using System.Collections;

public class MessagePanel : MonoBehaviour {

    public App app;
    public string textToDisplay = "";

    public UnityEngine.UI.Text textDisplayed;
    public UnityEngine.UI.Button okButton = null;

	// Use this for initialization
	void Start () {
        this.app.paused = true;
        this.okButton.onClick.AddListener(() => this.clickOKButton());
	}
	
	// Update is called once per frame
	void Update () {
        this.textDisplayed.text = this.textToDisplay;

        if(Input.GetKeyDown(KeyCode.Return))
        {
            this.clickOKButton();
        }
	}

    private void clickOKButton()
    {
        this.app.paused = false;
        Destroy(this.gameObject);
    }
}
