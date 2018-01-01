using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public App app;

    public GameObject shape;

    private Rigidbody2D body;
    private float playerSpeed = 1.6f;
    public float stamina = 1.0f;
    public float sprintStaminaDecrease = 0.25f;
    public float sprintAmount = 2.5f;

    public int lives = 3;

    public bool canGameOver = true;

    public bool shouldWalkAnimation = false;
    public Sprite[] sprites;
    private float walkTimer = 0.0f;
    private int currentWalkSprite = 2;

    public AudioSource footstepSource = null;

	// Use this for initialization
	void Start () {
        this.shape = this.gameObject.transform.FindChild("Shape").gameObject;
        this.body = this.gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        this.shouldWalkAnimation = false;

        if(!this.app.paused){
            //Handle controls
            bool sprinting = false;
            if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                this.stamina -= this.sprintStaminaDecrease * Time.deltaTime;
                if(this.stamina < 0.0f)
                {
                    this.stamina = 0.0f;
                }

                if(this.stamina <= 0.0f)
                {
                    sprinting = false;
                }
                else
                {
                    sprinting = true;
                }
            }
            else
            {
                this.stamina += this.sprintStaminaDecrease * 0.5f * Time.deltaTime;
                if (this.stamina > 1.0f)
                {
                    this.stamina = 1.0f;
                }
            }

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z))
            {
                float currentSpeed = this.playerSpeed;
                if(sprinting)
                {
                    currentSpeed = currentSpeed * this.sprintAmount;
                }
                this.gameObject.transform.position += new Vector3(0.0f, currentSpeed * Time.deltaTime, 0.0f);
                this.shouldWalkAnimation = true;
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                float currentSpeed = this.playerSpeed;
                if (sprinting)
                {
                    currentSpeed = currentSpeed * this.sprintAmount;
                }
                this.gameObject.transform.position += new Vector3(0.0f, -currentSpeed * Time.deltaTime, 0.0f);
                this.shouldWalkAnimation = true;
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
            {
                float currentSpeed = this.playerSpeed;
                if (sprinting)
                {
                    currentSpeed = currentSpeed * this.sprintAmount;
                }
                this.gameObject.transform.position += new Vector3(-currentSpeed * Time.deltaTime, 0.0f, 0.0f);
                this.gameObject.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = true;
                this.shouldWalkAnimation = true;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                float currentSpeed = this.playerSpeed;
                if (sprinting)
                {
                    currentSpeed = currentSpeed * this.sprintAmount;
                }
                this.gameObject.transform.position += new Vector3(currentSpeed * Time.deltaTime, 0.0f, 0.0f);
                this.gameObject.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = false;
                this.shouldWalkAnimation = true;
            }

            if (this.footstepSource != null && sprinting)
            {
                this.footstepSource.pitch = 1.5f;
            }
            else if(this.footstepSource != null)
            {
                this.footstepSource.pitch = 1.0f;
            }

            //Handle walk animation
            if (this.shouldWalkAnimation)
            {
                this.walkTimer -= Time.deltaTime;
                if(this.walkTimer <= 0.0f)
                {
                    this.walkTimer = 0.125f;
                    this.currentWalkSprite++;
                    if(this.currentWalkSprite > 2)
                    {
                        this.currentWalkSprite = 1;
                    }
                    this.shape.gameObject.GetComponent<SpriteRenderer>().sprite = this.sprites[this.currentWalkSprite];
                }

                if(this.footstepSource == null)
                {
                    this.footstepSource = this.app.playSound(5, true);
                }
            }
            else
            {
                this.shape.gameObject.GetComponent<SpriteRenderer>().sprite = this.sprites[0];

                if(this.footstepSource != null)
                {
                    Destroy(this.footstepSource);
                }
            }

            //Handle game over
            if(this.canGameOver){
                bool isOnATile = false;
                foreach (Tile tile in this.app.tiles)
                {
                    if (tile.rect.Contains(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y)))
                    {
                        //Debug.Log("on tile "+tile);
                        isOnATile = true;
                    }
                }
                if (!isOnATile)
                {
                    this.app.gameOverByDarkness();
                }
            }
        }
    }

    public void desactivateCollision()
    {
        this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        //this.gameObject.transform.FindChild("Shape").gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.gameObject.GetComponent<Objective>() != null)
        {
            //Debug.Log("Player on obj");
            this.app.handleObjectiveTouch(other.gameObject.transform.parent.gameObject.GetComponent<Objective>());
        }
    }
}
