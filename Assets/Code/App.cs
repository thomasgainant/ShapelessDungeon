using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour {

    public Object[] resources;

    public int levelNumber = 1;

    public Camera camera;

    public bool paused = false;

    public Player player;
    public List<Tile> tiles = new List<Tile>();
    public List<Objective> objectives = new List<Objective>();
    public int currentObjectiveNumber = 1;

    public bool levelCanChange = true;
    public float delayBetweenChange = 3.0f;

    public UI ui;

    public Sprite[] alterTiles;

    // Use this for initialization
    void Start () {
        this.resources = new Object[] {
            Resources.Load("Tile"),
            Resources.Load("TileBend"),
            Resources.Load("TileStraight"),
            Resources.Load("Pixel"),
            Resources.Load("MessagePanel"),
            Resources.Load("NumberText"),
            Resources.Load<AudioClip>("GameOver"),
            Resources.Load<AudioClip>("PlayerDeath"),
            Resources.Load<AudioClip>("Tilesdestroy"),
            Resources.Load("EnemyFireball 1"),
            Resources.Load<AudioClip>("CompleteGame"),//pass objective
            Resources.Load<AudioClip>("CompleteGame"),//End level
            Resources.Load<AudioClip>("step2"),//footstep
            null
        };

        this.alterTiles = Resources.LoadAll<Sprite>("AlterTiles");

        this.camera = GameObject.Find("Main Camera").gameObject.GetComponent<Camera>();

        this.player = GameObject.FindObjectOfType<Player>();
        this.player.app = this;

        Tile[] rawTiles = GameObject.FindObjectsOfType<Tile>();
        foreach(Tile tile in rawTiles)
        {
            this.tiles.Add(tile);
            tile.app = this;
        }

        Objective[] rawObjectives = GameObject.FindObjectsOfType<Objective>();
        foreach (Objective objective in rawObjectives)
        {
            this.objectives.Add(objective);
            objective.app = this;
        }

        this.ui = GameObject.FindObjectOfType<UI>();

        if(this.levelCanChange){
            StartCoroutine(this.levelGenerationRoutine());
        }

        switch(this.levelNumber)
        {
            case 1:
                this.displayMessage("Welcome to the Shapeless Dungeon! Your goal is too reach the final gate and step deeper into this dreadful dungeon. Press Z, Q, S and D or use your keyboard arrows to move around.");
                break;
            case 2:
                this.displayMessage("You will have to pass through a number of checkpoints before going to the final door. Step on the different checkpoints, following their specific order and get to the final stairs.");
                break;
            case 3:
                this.displayMessage("You can sprint using the Shift key. You will run faster but your stamina will lower. Use it wisely!");
                break;
            case 4:
                this.displayMessage("This creepy dungeon is cursed. A mighty wizard put a powerful spell on it which makes the shape of the dungeon shift without stopping. Some part of the dungeon will disappear, some more will appear. Avoid the disappearing parts or your soul will be sent to the Dark Void!");
                break;
            case 6:
                this.displayMessage("This shapeless dungeon is haunted. It is inhabited by the souls of adventurers like you. These ghosts will try to devour your soul, avoid them at all cost!");
                break;
            case 8:
                this.displayMessage("The lost souls on this floor are more dangerous. Watch out!");
                break;
            case 10:
                this.displayMessage("This level is way harder! It is the end of your journey. The final gate will lead you straight to Hell. Watch your step, adventurer!");
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
        this.camera.gameObject.transform.position = Vector3.Lerp(
            this.camera.gameObject.transform.position,
            new Vector3(this.player.gameObject.transform.position.x, this.player.gameObject.transform.position.y, this.camera.gameObject.transform.position.z),
            0.1f);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    public void handleObjectiveTouch(Objective objective)
    {
        if(objective.objectiveNumber == this.currentObjectiveNumber)
        {
            this.currentObjectiveNumber++;
            objective.disappear();

            if(objective.isFinal)
            {
                this.playSound(4);
            }
            else
            {
                this.playSound(3);
            }

            //Debug.Log(this.currentObjectiveNumber+"/"+ this.objectives.Count);

            /*if(this.currentObjectiveNumber > this.objectives.Count)
            {
                this.levelWon();
            }*/
        }
    }

    public Objective getObjectiveByNumber(int number)
    {
        Objective result = null;

        foreach(Objective objective in this.objectives)
        {
            if(objective.objectiveNumber == number)
            {
                result = objective;
            }
        }

        return result;
    }

    public void levelWon()
    {
        Debug.Log("level won");
        this.ui.levelWonAppear();
        //SceneManager.LoadScene(this.levelNumber+1);
    }

    public void gameOverByDarkness()
    {
        Debug.Log("game over by darkness");
        this.gameOver();
    }

    public void gameOverByEnemy()
    {
        Debug.Log("game over by enemy");
        this.gameOver();
    }

    public void gameOver()
    {
        Debug.Log("game over");
        this.ui.gameOverAppear();
    }

    public void displayMessage(string text)
    {
        GameObject newMessageObj = (GameObject)Instantiate(this.resources[4]);
        newMessageObj.transform.SetParent(this.ui.gameObject.transform);
        newMessageObj.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        newMessageObj.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        MessagePanel panel = newMessageObj.GetComponent<MessagePanel>();
        panel.app = this;
        panel.textToDisplay = text;
    }

    public AudioSource playSound(int type)
    {
        return this.playSound(type, false);
    }

    public AudioSource playSound(int type, bool looped)
    {
        //0: game over, 1: player death, 2: tile destroy, 3: pass objective, 4: end level, 5: footstep
        AudioSource source = Camera.main.gameObject.AddComponent<AudioSource>();
        if(type == 0)
        {
            source.clip = (AudioClip)Instantiate(this.resources[6]);
        }
        else if (type == 1)
        {
            source.clip = (AudioClip)Instantiate(this.resources[7]);
        }
        else if (type == 2)
        {
            source.clip = (AudioClip)Instantiate(this.resources[8]);
            source.volume = 0.3f;
            source.pitch = Random.Range(0.5f, 1.1f);
        }
        else if (type == 3)
        {
            source.clip = (AudioClip)Instantiate(this.resources[10]);
        }
        else if (type == 4)
        {
            source.clip = (AudioClip)Instantiate(this.resources[11]);
        }
        else if (type == 5)
        {
            source.clip = (AudioClip)Instantiate(this.resources[12]);
            source.volume = 0.75f;
        }

        if (!looped){
            source.loop = false;
        }
        else
        {
            source.loop = true;
        }
        source.Play();

        return source;
    }

    private IEnumerator levelGenerationRoutine()
    {
        bool continued = true;
        while (continued)
        {
            if (!this.paused)
            {
                yield return new WaitForSeconds(this.delayBetweenChange);

                Tile randomTile = null;

                foreach (Tile tile in this.tiles)
                {
                    if (Random.Range(0.0f, 1.0f) > 0.75f
                        && tile.objectiveOnIt == null
                        && !tile.goingToDisappear
                        && tile.canDisappear)
                    {
                        randomTile = tile;
                    }
                }

                if (randomTile == null)
                {
                    //Create tile
                    Tile addRandomTile = this.tiles[Random.Range(0, this.tiles.Count)];
                    for (int i = 0; i < 4; i++)
                    {
                        Tile neighbourTile = addRandomTile.neighbouringTiles[i];
                        if (neighbourTile == null)
                        {
                            Tile newTile = addRandomTile.addNeighbourTileAt(i);
                            //Debug.Log("add tile "+newTile);
                            break;
                        }
                    }
                }
                else
                {
                    randomTile.disappear();
                }
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public static float signedAngle(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle(a, b); // calculate angle
                                           // assume the sign of the cross product's Y component:
        return angle * Mathf.Sign(Vector3.Cross(a, b).y);
    }
}
