using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldControl : MonoBehaviour
{
    public int numLevels;
    public int currentLevel = 0;
    public WorldGen worldGen;
    
    public GameObject congratsUI1;
    public GameObject congratsUI2;

    public int[] numCrowsPerLevel;

    private int numCrowsKilledThisLevel;

    void Start()
    {
        this.numCrowsKilledThisLevel = 0;
        this.worldGen.GenerateMap(0);
    }

    public void CrowKilled() {
        this.numCrowsKilledThisLevel++;
        Debug.Log("crow killed!");
        if (this.numCrowsKilledThisLevel >= this.numCrowsPerLevel[this.currentLevel]) {

            //display UI

            if (this.currentLevel < this.numLevels - 1)
                this.congratsUI1.SetActive(true);
            Invoke("NextLevel", 3);
        }
    }

    private void NextLevel() {
        this.congratsUI1.SetActive(false);
        if (this.currentLevel + 1 == this.numLevels) {
            //player wins!
            //display UI text
            this.congratsUI2.SetActive(true);
            return;
        }

        this.currentLevel++;

        this.numCrowsKilledThisLevel = 0;
        
        object[] obj = GameObject.FindSceneObjectsOfType(typeof (GameObject));
        foreach (object o in obj) {
            GameObject g = (GameObject) o;

            if (g.layer != LayerMask.NameToLayer("Permanent")){
                Destroy(g);
            }
        }

        this.worldGen.GenerateMap(this.currentLevel);
        StartCoroutine(Wait1Frame());
        
    }

    //coroutine to wait a frame

    IEnumerator Wait1Frame()
    {
        yield return 0;
        yield return 0;
        Camera.main.GetComponent<CameraControl>().StartCamera();
    }
}
