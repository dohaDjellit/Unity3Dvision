using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject myCube;
    public GameObject mySphere;
    public GameObject myCylinder;
    public GameObject myPlane;
    public GameObject mycheckboard;
    //public GameObject myCamera; // Assign the camera prefab in the Inspector
      



    public void SpawnCheckboard()
    {
        Instantiate(mycheckboard);



    }
        
    public void SpawnCube()
    {
        Instantiate(myCube);


    }
    public void SpawnSphere()
    {
        
        Instantiate(mySphere);
    }
    public void SpawnCylinder()
    {
        
   
        Instantiate(myCylinder);

    }
    public void SpawnPlane()
    {


        Instantiate(myPlane);
    }
    

}
