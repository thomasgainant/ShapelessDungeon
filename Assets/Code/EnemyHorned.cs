using UnityEngine;
using System.Collections;

public class EnemyHorned : Enemy {

    // Use this for initialization
    public override void init()
    {
        base.init();

        this.enemySpeed = this.enemySpeed * 1.5f;
    }

    // Update is called once per frame
    protected override void update()
    {
        base.update();
    }
}
