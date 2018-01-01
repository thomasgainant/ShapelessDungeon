using UnityEngine;
using System.Collections;

public class EnemyFireball : Enemy {

    public int directionOverride = 0;

    // Use this for initialization
    public override void init()
    {
        base.init();

        this.enemySpeed = this.enemySpeed * 2.0f;
    }

    // Update is called once per frame
    protected override void update()
    {
        base.update();

        this.currentDirection = this.directionOverride;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        //Debug.Log("collision: "+coll.collider.gameObject);
        if (coll.collider.gameObject.transform.parent != null && coll.collider.gameObject.transform.parent.gameObject.GetComponent<Player>() != null)
        {
            this.app.playSound(1);
            this.app.player.lives--;
            if (this.app.player.lives <= 0)
            {
                this.app.playSound(0);
                this.app.gameOverByEnemy();
            }
        }

        Destroy(this.gameObject);
    }
}
