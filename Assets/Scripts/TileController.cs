using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField] private TileData TileSettings;
    private Pillar _pillar;

    public string GetName()
    {
        return TileSettings.name;
    }
    private void Start()
    {
        if(GetName()!="Gulag")
        {
            _pillar.militaryInfluence = TileSettings.MilitaryInfluence;
            _pillar.economyInfluence = TileSettings.EconomyInfluence;
            _pillar.cultureInfluence = TileSettings.CultureInfluence;
        }
        else
        {
            _pillar.militaryInfluence = Random.Range(-3, 4);
            _pillar.economyInfluence = Random.Range(-3, 4);
            _pillar.cultureInfluence = Random.Range(-3, 4);
        }


    }
}

public struct Pillar
{
    public int militaryInfluence;
    public int economyInfluence;
    public int cultureInfluence;

}
