using UnityEngine;
using System.Collections;

public class EnemyWizard : Enemy {

    public float fireTimer;

    // Use this for initialization
    public override void init()
    {
        base.init();

        this.enemySpeed = this.enemySpeed * 1.0f;
        this.fireTimer = 5.0f;
    }

    // Update is called once per frame
    protected override void update()
    {
        base.update();

        if(!this.app.paused){
            this.fireTimer -= Time.deltaTime;
            if (this.fireTimer <= 0.0f)
            {
                this.fireTimer = 5.0f;

                GameObject fireballObj = (GameObject)Instantiate(this.app.resources[9]);
                switch (this.currentDirection)
                {
                    case 0:
                        fireballObj.transform.position = this.gameObject.transform.position + new Vector3(1.0f, 0.0f, 0.0f);
                        break;
                    case 1:
                        fireballObj.transform.position = this.gameObject.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
                        break;
                    case 2:
                        fireballObj.transform.position = this.gameObject.transform.position + new Vector3(-1.0f, 0.0f, 0.0f);
                        break;
                    case 3:
                        fireballObj.transform.position = this.gameObject.transform.position + new Vector3(0.0f, -1.0f, 0.0f);
                        break;
                }

                fireballObj.GetComponent<EnemyFireball>().directionOverride = this.currentDirection;
            }
        }
    }
}
