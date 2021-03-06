﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrongholdValueShow:MonoBehaviour
{
    //public Stronghold stronghold;

    public RaceValueShow pismireValueShow;
    public RaceValueShow beeValueShow;

    public RaceValueShow nowValueShow;
    public Race nowRace;

    public void showRace(Race race)
    {
        if (nowRace == race)
            return;
        nowRace = race;
        nowValueShow = null;
        if (race == Race.ePismire )
        {
            pismireValueShow.gameObject.SetActiveRecursively(true);
            nowValueShow = pismireValueShow;
        }
        else
        {
            pismireValueShow.gameObject.SetActiveRecursively( false );

        }
        
        if(race == Race.eBee)
        {
            beeValueShow.gameObject.SetActiveRecursively( true);
            nowValueShow = beeValueShow;
        }
        else
        {
            beeValueShow.gameObject.SetActiveRecursively(false);

        }
    }

    //void Start()
    //{
    //    pismireValueShow.gameObject.active = false;
    //    beeValueShow.gameObject.active = false;
    //    nowRace = Race.eNone;
    //}

    public float rate
    {
        set 
        {
            if (nowValueShow)
                nowValueShow.rate = value; 
        }
    }
}