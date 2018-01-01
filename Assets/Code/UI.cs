using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

    public static float appearSpeed = 0.5f;

    public App app;

    public UnityEngine.UI.Text levelText;
    public UnityEngine.UI.Text checkpointText;
    public GameObject life1;
    public GameObject life2;
    public GameObject life3;
    public GameObject gameOverPanel;
    public UnityEngine.UI.Button retryButton;
    public GameObject levelWonPanel;
    public UnityEngine.UI.Button nextLevelButton;
    public GameObject arrow;
    public GameObject staminaPanelContent;

    private bool canPressEnterToRetry = false;
    private bool canPressEnterToNextLevel = false;

    // Use this for initialization
    void Start () {
        if (this.app == null)
        {
            this.app = GameObject.FindObjectOfType<App>();
        }

        this.retryButton.onClick.AddListener(() => this.clickRetryButton());
        this.nextLevelButton.onClick.AddListener(() => this.clickNextLevelButton());

        this.levelWonPanel.GetComponent<RectTransform>().anchorMin = new Vector2(1.0f, 0.1f);
        this.levelWonPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1.5f, 0.9f);

        this.gameOverPanel.GetComponent<RectTransform>().anchorMin = new Vector2(1.0f, 0.1f);
        this.gameOverPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1.5f, 0.9f);
    }
	
	// Update is called once per frame
	void Update () {
        this.levelText.text = "Level #"+this.app.levelNumber;
        this.checkpointText.text = this.app.currentObjectiveNumber+"/"+this.app.objectives.Count;

        if(this.app.player.lives < 3)
        {
            this.life1.SetActive(false);
        }
        else
        {
            this.life1.SetActive(true);
        }

        if (this.app.player.lives < 2)
        {
            this.life2.SetActive(false);
        }
        else
        {
            this.life2.SetActive(true);
        }

        if (this.app.player.lives < 1)
        {
            this.life3.SetActive(false);
        }
        else
        {
            this.life3.SetActive(true);
        }

        this.staminaPanelContent.GetComponent<RectTransform>().anchorMax = new Vector2(this.app.player.stamina, this.staminaPanelContent.GetComponent<RectTransform>().anchorMax.y);

        //Is objective seen?
        Objective objec = this.app.getObjectiveByNumber(this.app.currentObjectiveNumber);
        if (objec != null) {
            GameObject objectiveObj = objec.gameObject;
            Vector2 viewportPoint = Camera.main.WorldToViewportPoint(objectiveObj.transform.position);
            bool canSeeObj = true;
            if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
            {
                canSeeObj = false;
            }
            if (canSeeObj)
            {
                this.arrow.SetActive(false);
            }
            else
            {
                this.arrow.SetActive(true);

                if (viewportPoint.x < 0)
                {
                    viewportPoint.x = 0;
                }
                else if (viewportPoint.x > 1)
                {
                    viewportPoint.x = 1;
                }

                if (viewportPoint.y < 0)
                {
                    viewportPoint.y = 0;
                }
                else if (viewportPoint.y > 1)
                {
                    viewportPoint.y = 1;
                }

                this.arrow.GetComponent<RectTransform>().anchorMin = new Vector2(viewportPoint.x - 0.05f, viewportPoint.y - 0.1f);
                this.arrow.GetComponent<RectTransform>().anchorMax = new Vector2(viewportPoint.x, viewportPoint.y);

                float angleOnZ = 0.0f;
                angleOnZ = App.signedAngle(Vector3.up, objectiveObj.transform.position - this.app.player.gameObject.transform.position);
                if (Vector3.Cross(Vector3.up, objectiveObj.transform.position - this.app.player.gameObject.transform.position).z < 0)
                {
                    angleOnZ = -angleOnZ;
                }
                this.arrow.transform.rotation = Quaternion.Euler(0, 0, angleOnZ);
            }
        }

        if(this.canPressEnterToRetry)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                this.clickRetryButton();
            }
        }

        if (this.canPressEnterToNextLevel)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                this.clickNextLevelButton();
            }
        }
    }

    public void gameOverAppear()
    {
        StartCoroutine(this.gameOverAppearRoutine());
    }

    private IEnumerator gameOverAppearRoutine()
    {
        bool straight = false;

        while(this.gameOverPanel.GetComponent<RectTransform>().anchorMin.x > 0.25f)
        {
            if(Input.GetKey(KeyCode.Return))
            {
                straight = true;
                break;
            }

            float newValue = this.gameOverPanel.GetComponent<RectTransform>().anchorMin.x - UI.appearSpeed * Time.deltaTime;

            this.gameOverPanel.GetComponent<RectTransform>().anchorMin = new Vector2(newValue, 0.1f);
            this.gameOverPanel.GetComponent<RectTransform>().anchorMax = new Vector2(newValue + 0.5f, 0.9f);

            yield return new WaitForEndOfFrame();
        }

        if(straight)
        {
            clickRetryButton();
        }
        else
        {
            this.canPressEnterToRetry = true;
        }

        yield return null;
    }

    public void levelWonAppear()
    {
        StartCoroutine(this.levelWonAppearRoutine());
    }

    private IEnumerator levelWonAppearRoutine()
    {
        bool straight = false;

        while (this.levelWonPanel.GetComponent<RectTransform>().anchorMin.x > 0.25f)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                straight = true;
                break;
            }

            float newValue = this.levelWonPanel.GetComponent<RectTransform>().anchorMin.x - UI.appearSpeed * Time.deltaTime;

            this.levelWonPanel.GetComponent<RectTransform>().anchorMin = new Vector2(newValue, 0.1f);
            this.levelWonPanel.GetComponent<RectTransform>().anchorMax = new Vector2(newValue + 0.5f, 0.9f);

            yield return new WaitForEndOfFrame();
        }

        if (straight)
        {
            clickNextLevelButton();
        }
        else
        {
            this.canPressEnterToNextLevel = true;
        }

        yield return null;
    }

    private void clickRetryButton()
    {
        SceneManager.LoadScene(1);
    }

    private void clickNextLevelButton()
    {
        if (this.app.levelNumber < 10) {
            SceneManager.LoadScene(this.app.levelNumber + 1);
        }
        else
        {
            this.app.displayMessage("Good game! You have finished Shapeless Dungeon! We hope you liked this Ludum Dare entry, made in 72 hours by Thomas Finholm and Thomas Gainant. The first Thomas did the graphic design, the second one coded the game. Both designed the gameplay. Thank you!");
        }
    }
}
