---
name: "codemap"
description: "代码地图，包含项目目录结构、文件用途说明、关键类位置索引。Invoke when navigating the codebase, locating files, or understanding project structure."
---

# 代码地图

## 项目结构

```
Assets/Resources/Scripts/
│
├── BattleHeroInfo.cs            # 战斗英雄信息
├── BattleHeroInfoGroup.cs       # 战斗英雄信息组
├── CastleHUD.cs                 # 城堡血条HUD
├── ChessHUD.cs                  # 棋子血条HUD
├── CityDetail.cs                # 城市详情
├── MainPanelManager.cs          # 主面板管理器
├── MapDragHandler.cs            # 地图拖拽处理
├── SideBar.cs                   # 侧边栏
├── Tooltip.cs                   # 提示框
├── WorldPieceControl.cs         # 地图块控制
│
├── Combat/                      # 战斗系统
│   ├── Actions/                 # 战斗动作（Command模式）
│   │   ├── ChessAction.cs       # 动作基类（SourceId, Tick, Doing()）
│   │   ├── AttackAction.cs      # 普通攻击
│   │   ├── SkillDamageAction.cs # 技能伤害
│   │   ├── ChessChangeHpAction.cs # 血量变更
│   │   ├── CreateChessAction.cs # 创建棋子
│   │   ├── RemoveChessAction.cs # 移除棋子
│   │   ├── MoveAction.cs        # 移动
│   │   ├── CreateMissileAction.cs # 创建投射物
│   │   ├── RemoveMissileAction.cs # 移除投射物
│   │   ├── AddBuffAction.cs     # 添加Buff
│   │   ├── RemoveBuffAction.cs  # 移除Buff
│   │   ├── CreateEffectAction.cs # 播放特效
│   │   ├── FoodCostAction.cs    # 粮食消耗
│   │   ├── RoundUpdateAction.cs # 回合更新
│   │   └── SkillPlayAction.cs   # 技能播放
│   ├── Buffs/                   # Buff系统
│   │   ├── Buff.cs              # Buff基类
│   │   ├── BuffManager.cs       # Buff管理器（静态工厂+事件分发）
│   │   ├── BuffShield.cs        # 护盾（百分比减伤）
│   │   ├── BuffShieldValue.cs   # 护盾（固定值减伤）
│   │   ├── BuffCoolDown.cs      # 冷却
│   │   ├── BuffNoAction.cs      # 禁行动
│   │   ├── BuffNoMove.cs        # 禁移动
│   │   ├── BuffLock.cs          # 锁定
│   │   ├── BuffSuck.cs          # 吸血
│   │   ├── BuffDamageAddRate.cs # 伤害加成
│   │   ├── BuffDamagedAddRate.cs# 受伤加成
│   │   ├── BuffSpeedDown.cs     # 减速
│   │   └── BuffTimeDamage.cs    # 持续伤害
│   ├── Skills/                  # 技能系统
│   │   ├── Skill.cs             # 技能基类
│   │   ├── SkillManager.cs      # 技能管理器（静态工厂+事件分发）
│   │   ├── SkillAttack*.cs      # 攻击型技能（7个）
│   │   ├── SkillDef*.cs         # 防御型技能（4个）
│   │   ├── SkillHit*.cs         # 命中触发型技能（10个）
│   │   ├── SkillAid*.cs         # 辅助型技能（2个）
│   │   ├── SkillHelp*.cs        # 帮助型技能（2个）
│   │   ├── SkillInit*.cs        # 初始化型技能（3个）
│   │   ├── SkillModify*.cs      # 修改型技能（3个）
│   │   ├── SkillAttacked*.cs    # 被击触发型技能（2个）
│   │   └── SkillDumb.cs         # 空技能/默认
│   ├── Effects/
│   │   └── DissolveEffect.cs    # 溶解特效
│   ├── OOs/
│   │   └── IRecoverable.cs      # 反序列化恢复接口
│   ├── Chess.cs                 # 棋子（战斗单位）
│   ├── ChessViewObj.cs          # 棋子视图对象
│   ├── Missile.cs               # 投射物
│   ├── MissileEffName.cs        # 投射物特效名
│   ├── MissileViewObj.cs        # 投射物视图对象
│   ├── SceneObj.cs              # 场景对象基类
│   ├── EffectManager.cs         # 特效管理器
│   ├── BattleStatManager.cs     # 战斗统计（支持回放）
│   ├── BattleTopForceBar.cs     # 战斗顶部队血条
│   ├── BattleTopInfo.cs         # 战斗顶部信息
│   └── UnityMeshMgr.cs          # 网格管理
│
├── Configs/                     # 配置表系统（静态加载）
│   ├── ConfigManager.cs         # 配置管理器（统一初始化）
│   ├── ArmsConfig_s.cs          # 兵种配置
│   ├── BattleUnitConfig_s.cs    # 战斗单位配置
│   ├── BuffConfig_s.cs          # Buff配置
│   ├── CityAttrConfig_s.cs      # 城市属性配置
│   ├── CityDevConfig_s.cs       # 城市发展配置
│   ├── CityLevelConfig_s.cs     # 城市等级配置
│   ├── ForceConfig_s.cs         # 势力配置
│   ├── FormulaLearnAttrConfig_s.cs # 登用属性公式配置
│   ├── HeroAttrConfig_s.cs      # 英雄属性配置
│   ├── HeroConfig_s.cs          # 英雄配置
│   ├── ItemConfig_s.cs          # 物品配置
│   ├── SeasonConfig_s.cs        # 季节配置
│   ├── ShopConfig_s.cs          # 商店配置
│   ├── SkillConfig_s.cs         # 技能配置
│   └── WorldConfig_s.cs         # 世界/地图配置
│
├── Controls/                    # 控制层
│   ├── AI/                      # AI策略系统
│   │   ├── AI.cs                # AI入口（static class）
│   │   ├── AIStrategyContext.cs # AI上下文数据
│   │   ├── CityEvaluator.cs     # 城市评估器
│   │   ├── CityStrategyState.cs # 城市战略状态枚举（Dev/Def/Atk）
│   │   ├── HeroDispatcher.cs    # 英雄调度器
│   │   ├── HeroTaskMatcher.cs   # 英雄任务匹配器
│   │   ├── StrategicDecider.cs  # 战略决策器
│   │   └── TaskPriorityCalculator.cs # 任务优先级计算器
│   ├── Utils/
│   │   └── GameLog.cs           # 日志系统
│   ├── BattleManager.cs         # 战斗管理器（MonoBehaviour单例）
│   ├── BattleUIManager.cs       # 战斗UI管理
│   ├── GameManager.cs           # 游戏主管理器（MonoBehaviour单例）
│   └── PanelManager.cs          # 面板管理器（信号分发中心）
│
├── Effect/
│   └── GlowBeamController.cs    # 光束特效控制器
│
├── OOs/                         # 接口定义
│   ├── ICityDevNode.cs          # 城市发展节点接口
│   ├── IRankDetailInfo.cs       # 排名详情接口
│   └── IRankDetailInfoHeader.cs # 排名详情头接口
│
├── PO/                          # 纯数据对象 / 枚举
│   ├── ArmsType.cs              # 兵种类型枚举
│   ├── AttrInfo.cs              # 属性信息
│   ├── BattleCardData.cs        # 战斗卡牌数据
│   ├── MapConfig.cs             # 地图配置
│   ├── PanelEvent.cs            # IPanelEvent 接口
│   ├── SignalData.cs            # 信号数据基类及派生类
│   └── TurnPhase.cs             # 回合阶段枚举
│
├── Panels/                      # UI 面板
│   ├── BattleResultPanelManager.cs  # 战斗结果面板
│   ├── CityBattlePanelManager.cs    # 城市战斗面板
│   ├── CityDevPanelManager.cs       # 城市发展面板
│   ├── CityPanelManager.cs          # 城市主面板
│   ├── HeroInfoPanelManager.cs      # 英雄信息面板
│   ├── PickPanelControl.cs          # 选择面板
│   ├── PopArmySetManager.cs         # 兵力设置弹窗
│   ├── PopCitySelectPanelManager.cs # 城市选择弹窗
│   ├── PopHeroBattleSelectPanelManager.cs # 战斗英雄选择弹窗
│   ├── PopHeroSelectPanelManager.cs # 英雄选择弹窗
│   ├── PopResultPanelManager.cs     # 结果弹窗
│   ├── RankPanelManager.cs          # 排名面板
│   ├── ReplayPanelManager.cs        # 回放面板
│   ├── SystemPanelManager.cs        # 系统面板
│   ├── VideoPanelManager.cs         # 视频面板
│   ├── CityDevItem.cs               # 城市发展项
│   ├── CityDevNodeMove.cs           # 城市发展节点移动
│   ├── CityDevPanelCell.cs          # 城市发展面板单元格
│   ├── ResItem.cs                   # 资源项
│   ├── SelectHeroArmyControl.cs     # 英雄兵力选择
│   ├── SelectHeroControl.cs         # 英雄选择控件
│   ├── SideArmsSelector.cs          # 侧边兵种选择器
│   ├── Gismo/
│   │   └── ArmsItemControl.cs       # 兵种项控件
│   └── ListItem/                    # 列表项控件（17个）
│       ├── BattleResultHeroCellControl.cs
│       ├── CityBattleItem.cs
│       ├── CityCellCity.cs / CityCellHero.cs
│       ├── HeroInfoCell.cs
│       ├── PickPanelCellControl.cs
│       ├── PopCitySelectPanelCell.cs
│       ├── PopHeroBattleSelectPanelCell.cs
│       ├── PopHeroSelectPanelCell.cs
│       ├── PopResultCell.cs
│       ├── RankCellForce.cs / RankCellInfo.cs
│       ├── RankCellInfoCity.cs / RankCellInfoForce.cs
│       ├── RankCellMode.cs
│       ├── ReplayCellControl.cs
│       └── SideArmsItem.cs
│
├── SaveDatas/                   # 存档数据类
│   ├── SaveData.cs              # 总存档（forces, cities, heros, round）
│   ├── SaveForceData.cs         # 势力存档
│   ├── SaveCityData.cs          # 城市存档
│   ├── SaveHeroData.cs          # 英雄存档
│   ├── WarTeamData.cs           # 战争队伍数据（3英雄槽位）
│   ├── WarPlanData.cs           # 战争计划数据（运行时，不序列化）
│   └── DevAssignmentData.cs     # 发展指派数据
│
├── SystemTool/                  # 系统工具类
│   ├── SysFormula.cs            # 公式计算（核心，嵌套静态类分类）
│   ├── SystemConst.cs           # 常量定义（核心，嵌套静态类分类）
│   ├── MapTool.cs               # 地图工具（城市邻接、距离、前线判断）
│   ├── BattleRandom.cs          # 战斗层随机数工具
│   ├── SysRandom.cs             # 战略层随机数工具
│   ├── GameLog.cs               # 日志系统
│   ├── BGMPlayer.cs             # 背景音乐播放
│   ├── ConfigNameHelper.cs      # 配置名辅助工具
│   ├── HeroAttrTool.cs          # 英雄属性工具
│   ├── HeroSelectionTool.cs     # 英雄选择工具
│   ├── NLCoroutineManager.cs    # 非MonoBehaviour协程管理器
│   ├── SysSwitch.cs             # 系统开关
│   └── SystemTip.cs             # 系统提示
│
└── UIScripts/                   # UI 通用脚本
    ├── AttrRadarChart.cs        # 属性雷达图
    ├── NLDropDown.cs            # 自定义下拉框
    ├── NLDropDownItem.cs        # 下拉框项
    ├── SideBarAlphaGradient.cs  # 侧边栏透明渐变
    └── UIColorChanger.cs        # 颜色切换器
```

## 关键类速查

| 类名 | 路径 | 职责 |
|------|------|------|
| `GameManager` | Controls/GameManager.cs | 游戏主管理器（MonoBehaviour单例） |
| `BattleManager` | Controls/BattleManager.cs | 战斗管理器（MonoBehaviour单例） |
| `PanelManager` | Controls/PanelManager.cs | 面板管理器（信号分发中心） |
| `MainPanelManager` | Scripts/MainPanelManager.cs | 主面板管理器 |
| `AI` | Controls/AI/AI.cs | AI入口（static class） |
| `ConfigManager` | Configs/ConfigManager.cs | 配置管理器（static class） |
| `SaveData` | SaveDatas/SaveData.cs | 总存档 |
| `SaveForceData` | SaveDatas/SaveForceData.cs | 势力存档 |
| `SaveCityData` | SaveDatas/SaveCityData.cs | 城市存档 |
| `SaveHeroData` | SaveDatas/SaveHeroData.cs | 英雄存档 |
| `MapTool` | SystemTool/MapTool.cs | 地图工具（static class） |
| `SysFormula` | SystemTool/SysFormula.cs | 公式计算（static class） |
| `SystemConst` | SystemTool/SystemConst.cs | 常量定义（static class） |
| `BattleRandom` | SystemTool/BattleRandom.cs | 战斗层随机数（static class） |
| `SysRandom` | SystemTool/SysRandom.cs | 战略层随机数（static class） |
| `GameLog` | SystemTool/GameLog.cs | 日志系统（static class） |
| `HeroAttrTool` | SystemTool/HeroAttrTool.cs | 英雄属性工具 |
| `HeroSelectionTool` | SystemTool/HeroSelectionTool.cs | 英雄选择工具 |
| `Chess` | Combat/Chess.cs | 棋子（战斗单位） |
| `ChessAction` | Combat/Actions/ChessAction.cs | 动作基类 |
| `SkillManager` | Combat/Skills/SkillManager.cs | 技能管理器（静态工厂+分发） |
| `BuffManager` | Combat/Buffs/BuffManager.cs | Buff管理器（静态工厂+分发） |
| `BattleStatManager` | Combat/BattleStatManager.cs | 战斗统计 |
| `SignalData` | PO/SignalData.cs | 信号数据基类 |
| `TurnPhase` | PO/TurnPhase.cs | 回合阶段枚举 |
| `ArmsType` | PO/ArmsType.cs | 兵种类型枚举 |
| `IPanelEvent` | PO/PanelEvent.cs | 面板事件接口 |
| `IRecoverable` | Combat/OOs/IRecoverable.cs | 反序列化恢复接口 |

## 文件统计

| 目录 | .cs 文件数 | 说明 |
|------|-----------|------|
| Scripts/ (根) | 10 | 主面板管理器、HUD、地图拖拽等 |
| Combat/ (含子目录) | 51 | 战斗系统核心 |
| Combat/Actions/ | 15 | ChessAction 动作队列 |
| Combat/Buffs/ | 13 | Buff 系统及实现 |
| Combat/Skills/ | 38 | 技能基类及实现 |
| Configs/ | 17 | 配置类 + ConfigManager |
| Controls/ (含子目录) | 12 | Manager 层及 AI 系统 |
| Controls/AI/ | 8 | AI 策略系统 |
| Effect/ | 1 | 通用特效 |
| OOs/ | 3 | 接口定义 |
| PO/ | 7 | 纯数据对象、枚举 |
| Panels/ (含子目录) | 30 | UI 面板及列表项控件 |
| SaveDatas/ | 7 | 存档序列化数据类 |
| SystemTool/ | 12 | 系统工具类 |
| UIScripts/ | 5 | UI 通用组件 |
| **总计** | **~139** | |
