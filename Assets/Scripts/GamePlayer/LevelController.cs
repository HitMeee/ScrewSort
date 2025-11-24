using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] List<Transform> PostCreateBolts;
    [SerializeField] BotlBase botlBase;
    [SerializeField] List<BotlBase> botlBases;
    [SerializeField] LevelData levelDatas;
    public void Init()
    {
        for (int i = 0; i < levelDatas.lsDataBolt.Count && i < PostCreateBolts.Count; i++)
        {
            var dataBolt = levelDatas.lsDataBolt[i];

            var bolt = Instantiate(botlBase);
            bolt.transform.position = PostCreateBolts[i].transform.position;
            bolt.Init(dataBolt.lsIdScrew);
            botlBases.Add(bolt);
        }
    }
}

[System.Serializable]
public class LevelData
{

    public List<DataBolt> lsDataBolt;

}
 
[System.Serializable]
public class DataBolt
{
    public int idBolt;
    public List<int> lsIdScrew;
}
