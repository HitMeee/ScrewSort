using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotlBase : MonoBehaviour
{
    [SerializeField] List<PostBolt> postBolts;
    [SerializeField] ScrewBase screwBase;
    [SerializeField] List<ScrewBase> screwBases;


    void Start()
    {
        foreach( var iteam in postBolts )
        {
            var Screw = Instantiate(screwBase);
            Screw.transform.position = iteam.transform.position;

            screwBases.Add(Screw);

        }
    }

}
