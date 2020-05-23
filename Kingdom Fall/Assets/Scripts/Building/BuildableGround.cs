using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableGround : MonoBehaviour
{
    public Building building;
    public Building towerToBuild;

    private void OnMouseOver()
    {
        Debug.Log("2");
        if(Input.GetMouseButtonDown(0))
        {
            if (building.currentBuilding == null)
            {
                Destroy(building.gameObject);
                building = Instantiate(towerToBuild,transform.position,Quaternion.identity,transform);
                building.currentBuilding = building;
            }






        }
    }
}
