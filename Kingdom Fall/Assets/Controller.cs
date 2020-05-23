using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Transform alliesBase;
    public Transform enermyBase;
    public List<Unit> units = new List<Unit>();

    public void SummonUnit(Unit unit)
    {
        float offset = Random.value /2;
        Unit newUnit = Instantiate(unit, new Vector2(alliesBase.position.x - 0.25f + offset, alliesBase.position.y + offset),Quaternion.identity);
        newUnit.enermyBase = enermyBase;
    }
}
