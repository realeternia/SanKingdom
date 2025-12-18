using System.Collections;
using System.Collections.Generic;

// 用于Unity JsonUtility序列化的辅助类
[System.Serializable]
public class SaveData
{
    public List<SaveForceData> forces = new List<SaveForceData>();
    public List<SaveCityData> cities = new List<SaveCityData>();
    public int year;
}
