using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

    public static float tileSize = 3.0f;
    public static float disappearanceSpeed = 0.001f;

    public App app;
    public Rect rect;

    public Objective objectiveOnIt;
    public Tile[] neighbouringTiles;//0 = right, 1 = top, 2 = left, 3 = bottom
    public bool[] blockingWalls = new bool[] { false, false, false, false };

    public bool canDisappear = true;
    public bool goingToDisappear = false;
    public bool alterTile = false;

    // Use this for initialization
    void Start () {
        this.gameObject.transform.FindChild("Shape").transform.localScale = new Vector3(Tile.tileSize, Tile.tileSize, Tile.tileSize);

        this.rect = new Rect(
            new Vector2(this.gameObject.transform.position.x - Tile.tileSize/2.0f, this.gameObject.transform.position.y - Tile.tileSize/2.0f),
            new Vector2(Tile.tileSize, Tile.tileSize)
        );

        this.neighbouringTiles = new Tile[4];
        StartCoroutine(this.refreshNeighbouringTiles());
        StartCoroutine(this.refreshTilesRoutine());
        StartCoroutine(changeToAlterTile());
    }

    private IEnumerator changeToAlterTile()
    {
        if (this.alterTile)
        {
            while (this.app == null || this.app.alterTiles == null)
            {
                yield return new WaitForEndOfFrame();
            }

            foreach (Transform child in this.gameObject.transform)
            {
                if (child.gameObject.name == "Shape")
                {
                    child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[1];
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
	
	// Update is called once per frame
	void Update () {
	
	}

    public void disappear()
    {
        if(!this.goingToDisappear){
            this.goingToDisappear = true;
            StartCoroutine(this.disappearRoutine());
        }
    }

    private IEnumerator disappearRoutine()
    {
        while (this.app == null || this.app.resources[3] == null)
        {
            yield return new WaitForEndOfFrame();
        }

        this.app.playSound(2);

        int numberOfPixels = 0;
        GameObject[] pixels = new GameObject[24*24];

        List<int> usableIndex = new List<int>();
        for(int i = 0; i < 24*24; i++)
        {
            usableIndex.Add(i);
        }

        while(numberOfPixels < 24*24){
            int index = usableIndex[Random.Range(0, usableIndex.Count)];

            pixels[index] = (GameObject)Instantiate(this.app.resources[3]);

            int x = index / 24;
            int y = index % 24;

            pixels[index].transform.position = new Vector3(
                this.gameObject.transform.position.x + x * (Tile.tileSize/24.0f) - (Tile.tileSize / 2.0f) + (Tile.tileSize / 24.0f) / 2.0f,
                this.gameObject.transform.position.y + y * (Tile.tileSize / 24.0f) - (Tile.tileSize / 2.0f) + (Tile.tileSize / 24.0f) / 2.0f,
                this.gameObject.transform.position.z
            );

            pixels[index].transform.SetParent(this.gameObject.transform);

            usableIndex.Remove(index);

            numberOfPixels++;

            yield return new WaitForSeconds(Tile.disappearanceSpeed);
        }

        //Debug.Log(numberOfPixels);

        this.app.tiles.Remove(this);
        Destroy(this.gameObject);

        yield return null;
    }

    public void appear()
    {
        StartCoroutine(this.appearRoutine());
    }

    private IEnumerator appearRoutine()
    {
        GameObject[] pixels = new GameObject[24*24];
        List<GameObject> pixelsDeletable = new List<GameObject>();

        for(int i = 0; i < 24*24; i++)
        {
            pixels[i] = (GameObject)Instantiate(this.app.resources[3]);
            int x = i / 24;
            int y = i % 24;

            pixels[i].transform.position = new Vector3(
                this.gameObject.transform.position.x + x * (Tile.tileSize / 24.0f) - (Tile.tileSize / 2.0f) + (Tile.tileSize/24.0f)/2.0f,
                this.gameObject.transform.position.y + y * (Tile.tileSize / 24.0f) - (Tile.tileSize / 2.0f) + (Tile.tileSize / 24.0f) / 2.0f,
                this.gameObject.transform.position.z
            );

            pixelsDeletable.Add(pixels[i]);

            //yield return new WaitForEndOfFrame();
        }

        this.app.playSound(2);

        while (pixelsDeletable.Count > 0)
        {
            GameObject pixelToDelete = pixelsDeletable[Random.Range(0, pixelsDeletable.Count)];
            pixelsDeletable.Remove(pixelToDelete);
            Destroy(pixelToDelete);

            yield return new WaitForSeconds(Tile.disappearanceSpeed);
        }

        yield return null;
    }

    public Tile addNeighbourTileAt(int index)
    {
        Tile result = null;
        GameObject newTileObj = (GameObject)Instantiate(this.app.resources[0]);
        newTileObj.name = "Tile "+this.app.tiles.Count;

        switch(index)
        {
            case 0: default:
                newTileObj.transform.position = this.gameObject.transform.position + new Vector3(Tile.tileSize, 0.0f, 0.0f);
                break;
            case 1:
                newTileObj.transform.position = this.gameObject.transform.position + new Vector3(0.0f, Tile.tileSize, 0.0f);
                break;
            case 2:
                newTileObj.transform.position = this.gameObject.transform.position + new Vector3(-Tile.tileSize, 0.0f, 0.0f);
                break;
            case 3:
                newTileObj.transform.position = this.gameObject.transform.position + new Vector3(0.0f, -Tile.tileSize, 0.0f);
                break;
        }

        result = newTileObj.GetComponent<Tile>();
        result.app = this.app;
        this.app.tiles.Add(result);

        if(this.alterTile)
        {
            result.alterTile = true;
        }

        StartCoroutine(this.refreshNeighbouringTiles());

        result.appear();

        return result;
    }

    private IEnumerator refreshTilesRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));

        bool continued = true;
        while(continued)
        {
            StartCoroutine(this.refreshNeighbouringTiles());
            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator refreshNeighbouringTiles()
    {
        while (this.app == null || this.app.tiles == null)
        {
            yield return new WaitForEndOfFrame();
        }

        foreach (Tile tile in this.app.tiles)
        {
            if (tile != this)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector3 diff = new Vector3();
                    if (i == 0)
                    {
                        diff = tile.gameObject.transform.position - (this.gameObject.transform.position + new Vector3(Tile.tileSize, 0.0f, 0.0f));
                    }
                    else if (i == 1)
                    {
                        diff = tile.gameObject.transform.position - (this.gameObject.transform.position + new Vector3(0.0f, Tile.tileSize, 0.0f));
                    }
                    else if (i == 2)
                    {
                        diff = tile.gameObject.transform.position - (this.gameObject.transform.position + new Vector3(-Tile.tileSize, 0.0f, 0.0f));
                    }
                    else if (i == 3)
                    {
                        diff = tile.gameObject.transform.position - (this.gameObject.transform.position + new Vector3(0.0f, -Tile.tileSize, 0.0f));
                    }

                    if (diff.magnitude <= Tile.tileSize / 2.0f)
                    //if (diff.magnitude <= 0.1f)
                    {
                        this.neighbouringTiles[i] = tile;
                    }
                }
            }
        }

        this.updateWallsDisplay();
    }

    public void updateWallsDisplay()
    {
        //Clear child named TileBend and TileStraight
        foreach(Transform child in this.gameObject.transform)
        {
            if(child.gameObject.name == "TileBend" || child.gameObject.name == "TileStraight")
            {
                Destroy(child.gameObject);
            }
        }

        //Add TileBend or Tile Wall to the four sides
        bool wallOnRight = true;

        if (this.neighbouringTiles[0] != null)
        {
            if(this.blockingWalls[0] || this.neighbouringTiles[0].blockingWalls[2])
            {
                wallOnRight = true;
            }
            else
            {
                wallOnRight = false;
            }
        }
        else
        {
            wallOnRight = true;
        }

        if(wallOnRight)
        {
            GameObject newWallObj = (GameObject)Instantiate(this.app.resources[2]);
            newWallObj.transform.position = this.gameObject.transform.position;
            newWallObj.transform.Rotate(new Vector3(0, 0, 90));
            newWallObj.transform.SetParent(this.gameObject.transform);
            newWallObj.name = "TileStraight";

            if (this.alterTile)
            {
                foreach (Transform child in newWallObj.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[8];
                    }
                }
            }
        }
        else
        {
            GameObject newWallObj = (GameObject)Instantiate(this.app.resources[1]);
            GameObject newWallObj2 = (GameObject)Instantiate(this.app.resources[1]);

            newWallObj.transform.position = this.gameObject.transform.position;
            newWallObj2.transform.position = this.gameObject.transform.position;

            newWallObj2.transform.Rotate(new Vector3(0, 0, 90));
            //newWallObj2.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = true;

            newWallObj.transform.SetParent(this.gameObject.transform);
            newWallObj2.transform.SetParent(this.gameObject.transform);

            newWallObj.name = "TileBend";
            newWallObj2.name = "TileBend";

            if(this.alterTile)
            {
                foreach (Transform child in newWallObj.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[0];
                    }
                }

                foreach (Transform child in newWallObj2.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[0];
                    }
                }
            }
        }

        bool wallOnTop = true;

        if (this.neighbouringTiles[1] != null)
        {
            if (this.blockingWalls[1] || this.neighbouringTiles[1].blockingWalls[3])
            {
                wallOnTop = true;
            }
            else
            {
                wallOnTop = false;
            }
        }
        else
        {
            wallOnTop = true;
        }

        if (wallOnTop)
        {
            GameObject newWallObj = (GameObject)Instantiate(this.app.resources[2]);
            newWallObj.transform.position = this.gameObject.transform.position;
            newWallObj.transform.Rotate(new Vector3(0, 0, 180));
            newWallObj.transform.SetParent(this.gameObject.transform);
            newWallObj.name = "TileStraight";

            if (this.alterTile)
            {
                foreach (Transform child in newWallObj.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[8];
                    }
                }
            }
        }
        else
        {
            GameObject newWallObj = (GameObject)Instantiate(this.app.resources[1]);
            GameObject newWallObj2 = (GameObject)Instantiate(this.app.resources[1]);

            newWallObj.transform.position = this.gameObject.transform.position;
            newWallObj2.transform.position = this.gameObject.transform.position;

            newWallObj.transform.Rotate(new Vector3(0, 0, 180));
            //newWallObj.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = true;

            newWallObj2.transform.Rotate(new Vector3(0, 0, 90));
            //newWallObj2.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = true;

            newWallObj.transform.SetParent(this.gameObject.transform);
            newWallObj2.transform.SetParent(this.gameObject.transform);

            newWallObj.name = "TileBend";
            newWallObj2.name = "TileBend";

            if (this.alterTile)
            {
                foreach (Transform child in newWallObj.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[0];
                    }
                }

                foreach (Transform child in newWallObj2.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[0];
                    }
                }
            }
        }

        bool wallOnLeft = true;

        if (this.neighbouringTiles[2] != null)
        {
            if (this.blockingWalls[2] || this.neighbouringTiles[2].blockingWalls[0])
            {
                wallOnLeft = true;
            }
            else
            {
                wallOnLeft = false;
            }
        }
        else
        {
            wallOnLeft = true;
        }

        if (wallOnLeft)
        {
            GameObject newWallObj = (GameObject)Instantiate(this.app.resources[2]);
            newWallObj.transform.position = this.gameObject.transform.position;
            newWallObj.transform.Rotate(new Vector3(0, 0, 270));
            newWallObj.transform.SetParent(this.gameObject.transform);
            newWallObj.name = "TileStraight";

            if (this.alterTile)
            {
                foreach (Transform child in newWallObj.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[8];
                    }
                }
            }
        }
        else
        {
            GameObject newWallObj = (GameObject)Instantiate(this.app.resources[1]);
            GameObject newWallObj2 = (GameObject)Instantiate(this.app.resources[1]);

            newWallObj.transform.position = this.gameObject.transform.position;
            newWallObj2.transform.position = this.gameObject.transform.position;

            newWallObj.transform.Rotate(new Vector3(0, 0, 180));
            //newWallObj.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = true;

            newWallObj2.transform.Rotate(new Vector3(0, 0, 270));
            //newWallObj2.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = true;

            newWallObj.transform.SetParent(this.gameObject.transform);
            newWallObj2.transform.SetParent(this.gameObject.transform);

            newWallObj.name = "TileBend";
            newWallObj2.name = "TileBend";

            if (this.alterTile)
            {
                foreach (Transform child in newWallObj.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[0];
                    }
                }

                foreach (Transform child in newWallObj2.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[0];
                    }
                }
            }
        }

        bool wallOnBottom = true;

        if (this.neighbouringTiles[3] != null)
        {
            if (this.blockingWalls[3] || this.neighbouringTiles[3].blockingWalls[1])
            {
                wallOnBottom = true;
            }
            else
            {
                wallOnBottom = false;
            }
        }
        else
        {
            wallOnBottom = true;
        }

        if (wallOnBottom)
        {
            GameObject newWallObj = (GameObject)Instantiate(this.app.resources[2]);
            newWallObj.transform.position = this.gameObject.transform.position;
            newWallObj.transform.Rotate(new Vector3(0, 0, 0));
            newWallObj.transform.SetParent(this.gameObject.transform);
            newWallObj.name = "TileStraight";

            if (this.alterTile)
            {
                foreach (Transform child in newWallObj.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[8];
                    }
                }
            }
        }
        else
        {
            GameObject newWallObj = (GameObject)Instantiate(this.app.resources[1]);
            GameObject newWallObj2 = (GameObject)Instantiate(this.app.resources[1]);

            newWallObj.transform.position = this.gameObject.transform.position;
            newWallObj2.transform.position = this.gameObject.transform.position;

            newWallObj.transform.Rotate(new Vector3(0, 0, 180));
            //newWallObj.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = true;

            newWallObj2.transform.Rotate(new Vector3(0, 0, 270));
            //newWallObj2.transform.FindChild("Shape").gameObject.GetComponent<SpriteRenderer>().flipX = true;

            newWallObj.transform.SetParent(this.gameObject.transform);
            newWallObj2.transform.SetParent(this.gameObject.transform);

            newWallObj.name = "TileBend";
            newWallObj2.name = "TileBend";

            if (this.alterTile)
            {
                foreach (Transform child in newWallObj.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[0];
                    }
                }

                foreach (Transform child in newWallObj2.transform)
                {
                    if (child.gameObject.name == "Shape")
                    {
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = this.app.alterTiles[0];
                    }
                }
            }
        }
    }

    //Editor
    void OnDrawGizmos()
    {
        if (this.blockingWalls[0])
        {
            Gizmos.DrawLine(this.gameObject.transform.position + new Vector3(Tile.tileSize/2.0f, -Tile.tileSize/2.0f, 0.0f), this.gameObject.transform.position + new Vector3(Tile.tileSize / 2.0f, Tile.tileSize / 2.0f, 0.0f));
        }

        if (this.blockingWalls[1])
        {
            Gizmos.DrawLine(this.gameObject.transform.position + new Vector3(Tile.tileSize / 2.0f, Tile.tileSize / 2.0f, 0.0f), this.gameObject.transform.position + new Vector3(-Tile.tileSize / 2.0f, Tile.tileSize / 2.0f, 0.0f));
        }

        if (this.blockingWalls[2])
        {
            Gizmos.DrawLine(this.gameObject.transform.position + new Vector3(-Tile.tileSize / 2.0f, Tile.tileSize / 2.0f, 0.0f), this.gameObject.transform.position + new Vector3(-Tile.tileSize / 2.0f, -Tile.tileSize / 2.0f, 0.0f));
        }

        if (this.blockingWalls[3])
        {
            Gizmos.DrawLine(this.gameObject.transform.position + new Vector3(-Tile.tileSize / 2.0f, -Tile.tileSize / 2.0f, 0.0f), this.gameObject.transform.position + new Vector3(Tile.tileSize / 2.0f, -Tile.tileSize / 2.0f, 0.0f));
        }
    }
}
