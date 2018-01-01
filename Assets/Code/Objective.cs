using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour {

    public App app;
    public int objectiveNumber = 1;
    public bool isFinal = false;

    public GameObject shape;

    public bool alterTile = false;

    public UnityEngine.UI.Text numberText;

	// Use this for initialization
	void Start(){
        if (this.app == null)
        {
            this.app = GameObject.FindObjectOfType<App>();
        }

        this.shape = this.gameObject.transform.FindChild("Shape").gameObject;

        StartCoroutine(spawnNumberText());
        if(this.isFinal)
        {
            StartCoroutine(changeToAlterTile());
        }
	}

    private IEnumerator changeToAlterTile()
    {
        if (this.alterTile)
        {
            while (this.app == null || this.app.alterTiles == null || this.app.alterTiles.Length <= 0)
            {
                yield return new WaitForEndOfFrame();
            }

            foreach (Transform child in this.gameObject.transform)
            {
                if (child.gameObject.name == "Shape")
                {
                    child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[2];
                }
            }
        }

        foreach (Transform child in this.gameObject.transform)
        {
            if (child.gameObject.name == "Shape")
            {
                child.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private IEnumerator spawnNumberText()
    {
        while(this.app.resources.Length <= 0)
        {
            yield return new WaitForEndOfFrame();
        }

        GameObject newNumberTextObj = (GameObject)Instantiate(this.app.resources[5]);
        this.numberText = newNumberTextObj.GetComponent<UnityEngine.UI.Text>();
        this.numberText.gameObject.transform.SetParent(this.app.ui.gameObject.transform);
        this.numberText.gameObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        this.numberText.gameObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {
        if(this.app.currentObjectiveNumber == this.objectiveNumber)
        {
            if (!this.isFinal) {
                this.shape.GetComponent<SpriteRenderer>().material.color = new Color(208.0f / 255.0f, 38.0f / 255.0f, 38.0f / 255.0f);
            }
            else
            {
                this.shape.GetComponent<SpriteRenderer>().material.color = Color.white;
            }

            if (this.numberText != null)
            {
                this.numberText.color = new Color(208.0f / 255.0f, 38.0f / 255.0f, 38.0f / 255.0f);
            }
        }
        else
        {
            this.shape.GetComponent<SpriteRenderer>().material.color = Color.white;
            if (this.numberText != null)
            {
                this.numberText.color = Color.white;
            }
        }

        if(this.numberText != null){
            this.numberText.text = "" + this.objectiveNumber;

            Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(this.gameObject.transform.position + new Vector3(0.125f, 0.125f, 0.0f));
            this.numberText.gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(viewPortPoint.x, viewPortPoint.y);
            this.numberText.gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(viewPortPoint.x + 0.025f, viewPortPoint.y + 0.05f);
        }
	}

    public void disappear()
    {
        Destroy(this.numberText.gameObject);

        if(!this.isFinal){
            Destroy(this.gameObject);
        }
        else
        {
            StartCoroutine(this.movePlayerToCenter());
        }
    }

    private IEnumerator movePlayerToCenter()
    {
        this.app.player.desactivateCollision();

        Vector3 diff = this.app.player.gameObject.transform.position - this.gameObject.transform.position;

        float timeSpent = 0.0f;
        while(diff.magnitude > 0.1f)
        {
            this.app.player.canGameOver = false;
            this.app.player.shouldWalkAnimation = true;

            if(timeSpent >= 2.0f)
            {
                break;
            }

            this.app.player.gameObject.transform.position = Vector3.Lerp(
                this.app.player.gameObject.transform.position,
                this.gameObject.transform.position,
                0.01f
            );

            float alpha = this.app.player.shape.GetComponent<SpriteRenderer>().material.color.a;
            alpha -= 0.9f * Time.deltaTime;
            this.app.player.shape.GetComponent<SpriteRenderer>().material.color = new Color(
                this.app.player.shape.GetComponent<SpriteRenderer>().material.color.r,
                this.app.player.shape.GetComponent<SpriteRenderer>().material.color.g,
                this.app.player.shape.GetComponent<SpriteRenderer>().material.color.b,
                alpha
                );

            diff = this.app.player.gameObject.transform.position - this.gameObject.transform.position;

            timeSpent += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        this.app.levelWon();

        yield return null;
    }
}
