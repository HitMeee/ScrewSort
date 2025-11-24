using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotlBase : MonoBehaviour
{
    [SerializeField] List<PostBolt> postBolts;
    [SerializeField] ScrewBase screwBase;
    [SerializeField] List<ScrewBase> screwBases;
    [SerializeField] List<int> lsId;

    public void Init(List<int> idLs)
    {
        foreach (var iteam in idLs)
        {
            lsId.Add(iteam);
        }
        for (int i = 0; i < postBolts.Count && i < lsId.Count; i++)
        {
            var Screw = Instantiate(screwBase);
            Screw.transform.position = postBolts[i].transform.position;
            Screw.Init(lsId[i]);
            screwBases.Add(Screw);
        }
    }
}