using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public App app;

    public Tile currentTile = null;
    public Coroutine movementRoutine = null;
    public bool isMoving = false;
    public Tile targetTile = null;

    public float enemySpeed = 0.5f;

    public float movementToDo = 0.0f;
    public int currentDirection = 0;

	// Use this for initialization
	void Start () {
        this.init();
	}

    public virtual void init()
    {
        if(this.app == null)
        {
            this.app = GameObject.FindObjectOfType<App>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        this.update();
	}

    protected virtual void update()
    {
        if (!this.app.paused)
        {
            if(this.movementToDo <= 0.0f)
            {
                this.currentDirection = Random.Range(0, 4);
                this.movementToDo = Tile.tileSize;
            }
            else
            {
                switch(this.currentDirection)
                {
                    case 0:
                        this.gameObject.transform.position += new Vector3(this.enemySpeed * Time.deltaTime, 0.0f, 0.0f);
                        break;
                    case 1:
                        this.gameObject.transform.position += new Vector3(0.0f, this.enemySpeed * Time.deltaTime, 0.0f);
                        break;
                    case 2:
                        this.gameObject.transform.position += new Vector3(-this.enemySpeed * Time.deltaTime, 0.0f, 0.0f);
                        break;
                    case 3:
                        this.gameObject.transform.position += new Vector3(0.0f, -this.enemySpeed * Time.deltaTime, 0.0f);
                        break;
                }

                this.movementToDo -= this.enemySpeed * Time.deltaTime;
            }

            /*if (this.targetTile != null)
            {
                //Debug.Log("movement routine: "+this.movementRoutine);
                if (!this.isMoving)
                {
                    this.movementRoutine = StartCoroutine(this.goToTileCenter(this.targetTile));
                }
            }
            else
            {
                if (this.currentTile == null)
                {
                    foreach (Tile tile in this.app.tiles)
                    {
                        if (tile.rect.Contains(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y)))
                        {
                            this.currentTile = tile;
                            if (this.movementRoutine != null)
                            {
                                StopCoroutine(this.movementRoutine);
                            }
                            this.movementRoutine = StartCoroutine(this.goToTileCenter(this.currentTile));
                        }
                    }
                }
                else
                {
                    Tile actualTile = null;

                    foreach (Tile tile in this.app.tiles)
                    {
                        if (tile.rect.Contains(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y)))
                        {
                            actualTile = tile;
                        }
                    }

                    if (actualTile != this.currentTile)
                    {
                        this.currentTile = actualTile;
                        if (this.movementRoutine != null)
                        {
                            StopCoroutine(this.movementRoutine);
                        }
                        this.movementRoutine = StartCoroutine(this.goToTileCenter(this.currentTile));
                    }
                }
            }*/
        }
    }

    protected IEnumerator goToTileCenter(Tile tile)
    {
        //Debug.Log("starting movement routine to "+tile);
        this.isMoving = true;
        Vector3 diff = tile.gameObject.transform.position - this.gameObject.transform.position;

        while(diff.magnitude > this.enemySpeed * Time.deltaTime)
        {
            if(this.gameObject.transform.position.x < tile.gameObject.transform.position.x)
            {
                //Debug.Log("go right");
                this.gameObject.transform.position += new Vector3(this.enemySpeed * Time.deltaTime, 0.0f, 0.0f);
                this.gameObject.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            if (this.gameObject.transform.position.x > tile.gameObject.transform.position.x)
            {
                //Debug.Log("go left");
                this.gameObject.transform.position += new Vector3(-this.enemySpeed * Time.deltaTime, 0.0f, 0.0f);
                this.gameObject.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            if (this.gameObject.transform.position.y < tile.gameObject.transform.position.y)
            {
                //Debug.Log("go up");
                this.gameObject.transform.position += new Vector3(0.0f, this.enemySpeed * Time.deltaTime, 0.0f);
            }
            if (this.gameObject.transform.position.y > tile.gameObject.transform.position.y)
            {
                //Debug.Log("go down");
                this.gameObject.transform.position += new Vector3(0.0f, -this.enemySpeed * Time.deltaTime, 0.0f);
            }


            diff = tile.gameObject.transform.position - this.gameObject.transform.position;
            //Debug.Log("diff.magnitude: "+diff.magnitude);
            yield return new WaitForEndOfFrame();
        }

        //Debug.Log("arrived on tile center");

        this.getNextRandomTargetTile();

        //StopCoroutine(this.movementRoutine);
        this.movementRoutine = null;
        this.isMoving = false;

        yield return null;
    }

    public void getNextRandomTargetTile()
    {
        Tile randomTile = this.currentTile.neighbouringTiles[Random.Range(0, this.currentTile.neighbouringTiles.Length)];
        if(randomTile == null)
        {
            randomTile = this.currentTile;
        }

        this.targetTile = randomTile;
        //Debug.Log("target tile set to "+this.targetTile);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        //Debug.Log("collision: "+coll.collider.gameObject);
        if(coll.collider.gameObject.transform.parent != null && coll.collider.gameObject.transform.parent.gameObject.GetComponent<Player>() != null)
        {
            this.app.playSound(1);
            this.app.player.lives--;
            if(this.app.player.lives <= 0){
                this.app.playSound(0);
                this.app.gameOverByEnemy();
            }
        }
    }
}
