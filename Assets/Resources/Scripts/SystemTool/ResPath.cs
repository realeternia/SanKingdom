public static class ResPath
{
    public static class Texture
    {
        public static string HeroIcon(string icon)
        {
            return "Textures/Skins/" + icon;
        }

        public static string HeroDefaultIcon()
        {
            return "Textures/Skins/moren";
        }

        public static string HeroBigIcon(string icon)
        {
            return "Textures/SkinsBig/" + icon;
        }

        public static string AttrIcon(string icon)
        {
            return "Textures/Icons/" + icon;
        }

        public static string BuildingIcon(string icon)
        {
            return "Textures/Buildings/" + icon;
        }

        public static string CityView(string viewPrefab)
        {
            return "Textures/CityView/" + viewPrefab;
        }

        public static string TextureByName(string name)
        {
            return "Textures/" + name;
        }

        public static string MapTexture(string mapName)
        {
            return "Textures/Maps/" + mapName;
        }
    }

    public static class Prefab
    {
        public static string Panel(string panelName)
        {
            return "Prefabs/Panels/" + panelName;
        }

        public static string PanelListItem(string itemName)
        {
            return "Prefabs/Panels/ListItem/" + itemName;
        }

        public static string PanelGismo(string itemName)
        {
            return "Prefabs/Panels/Gismo/" + itemName;
        }

        public static string BattleMap(int mapId)
        {
            return "Prefabs/BattleMaps/Map" + mapId;
        }

        public static string BattleItem(string itemName)
        {
            return "Prefabs/BattleItems/" + itemName;
        }

        public static string UnitModel(string model)
        {
            return "Prefabs/BattleItems/" + model;
        }

        public static string Arms(string armsType)
        {
            return "Prefabs/Arms/" + armsType;
        }

        public static string Hud()
        {
            return "Prefabs/Hud";
        }

        public static string HudSmall()
        {
            return "Prefabs/HudSmall";
        }

        public static string BattleTxt()
        {
            return "Prefabs/BattleTxt";
        }

        public static string CityHeroHead()
        {
            return "Prefabs/CityHeroHead";
        }

        public static string MissileCom()
        {
            return "Prefabs/BattleItems/MissileCom";
        }

        public static string MissileEffect(string effectName)
        {
            return "Prefabs/Missile/" + effectName;
        }

        public static string MissileDefaultEffect()
        {
            return "Prefabs/Missile/BulletExplosionFire";
        }

        public static string Effect(string effectName)
        {
            return "Prefabs/Effect/" + effectName;
        }

        public static string ResBase()
        {
            return "Prefabs/ResBase";
        }

        public static string TipItem()
        {
            return "Prefabs/Panels/TipItem/ResTipItem";
        }

        public static string WorldPiece()
        {
            return "Prefabs/WorldPiece";
        }
    }

    public static class Material
    {
        public static string GoldChess()
        {
            return "Materials/GoldChess";
        }

        public static string SilverChess()
        {
            return "Materials/SilverChess";
        }
    }

    public static class Font
    {
        public static string HeiTiSDF()
        {
            return "Fonts/HeiTi SDF";
        }
    }
}
