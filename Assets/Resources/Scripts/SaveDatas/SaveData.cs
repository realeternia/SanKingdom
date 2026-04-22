using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<SaveForceData> forces = new List<SaveForceData>();
    public List<SaveCityData> cities = new List<SaveCityData>();
    public List<SaveHeroData> heros = new List<SaveHeroData>();
    public BattleStatManager battleStatManager = new BattleStatManager();
    public int round;
    public int currentForceIndex;
}
