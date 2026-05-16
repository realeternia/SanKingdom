public class SignalData
{
    public string Name;
}

public class CityResChangeSignal : SignalData
{
    public CityResChangeSignal() { Name = "CityResChange"; }
    public int CityId;
    public string ResType;
    public int Value;
}

public class CityForceChangeSignal : SignalData
{
    public CityForceChangeSignal() { Name = "CityForceChange"; }
    public int CityId;
}

public class ForceResChangeSignal : SignalData
{
    public ForceResChangeSignal() { Name = "ForceResChange"; }
    public string ResType;
    public int Value;
    public int Used;
}

public class PhaseChangeSignal : SignalData
{
    public PhaseChangeSignal() { Name = "PhaseChange"; }
    public string PhaseName;
    public int ForceId;
}

public class AICheckSignal : SignalData
{
    public AICheckSignal() { Name = "AICheck"; }
    public string AIName;
    public int ForceId;
}

public class RoundChangeSignal : SignalData
{
    public RoundChangeSignal() { Name = "RoundChange"; }
    public int Round;
}

public class CityAttrChangeSignal : SignalData
{
    public CityAttrChangeSignal() { Name = "CityAttrChange"; }
    public int CityId;
}

public class CityHeroChangeSignal : SignalData
{
    public CityHeroChangeSignal() { Name = "CityHeroChange"; }
    public int CityId;
}

public class CityLevelChangeSignal : SignalData
{
    public CityLevelChangeSignal() { Name = "CityLevelChange"; }
    public int CityId;
}
