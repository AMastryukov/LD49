using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TileData data;

    public string Name => data.Name;
    public Pillars Pillars { get; set; }

    public void Awake()
    {
        Pillars = new Pillars();

        // A Gulag is a special case where all pillar values are random
        if (Name.Equals("Gulag"))
        {
            Pillars.Military = Random.Range(-3, 4);
            Pillars.Economy = Random.Range(-3, 4);
            Pillars.Culture = Random.Range(-3, 4);

            return;
        }

        Pillars.Military = data.Military;
        Pillars.Economy = data.Economy;
        Pillars.Culture = data.Culture;
    }
}