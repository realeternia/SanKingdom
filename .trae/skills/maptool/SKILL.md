---
name: "maptool"
description: "MapTool地图工具规则，包含城市邻接查询、前线判断、距离计算等规范。Invoke when working with city adjacency, frontline detection, distance calculation, or map-related logic."
---

# MapTool 地图工具规则

`MapTool` 是 `SystemTool/` 下的静态工具类，统一管理所有基于 `WorldConfig.WorldNearIds` 的城市邻接查询逻辑。

## 核心原则

**禁止在业务代码中直接访问 `WorldConfig.WorldNearIds` 进行邻接判断**，必须使用 `MapTool` 提供的方法。

## 方法列表

| 方法 | 返回值 | 说明 |
|------|--------|------|
| `GetAdjacentCityIds(int cityId)` | `List<int>` | 获取某城市的所有相邻城市ID（纯配置查询，无势力参数） |
| `GetAdjacentFriendlyCityIds(int cityId, int forceId)` | `List<int>` | 获取某城市相邻的同势力城市 |
| `GetAdjacentEnemyCityIdsForCity(int cityId, int forceId)` | `List<int>` | 获取某城市相邻的敌对势力城市（针对单个城市） |
| `GetAdjacentEnemyCityIds(int forceId)` | `List<int>` | 获取某势力所有城市的相邻敌方城市并集（`HashSet` 去重） |
| `GetOwnCityIds(int forceId)` | `List<int>` | 获取某势力所有己方城市ID |
| `IsAdjacentCity(int cityId1, int cityId2)` | `bool` | 判断两个城市是否相邻 |
| `IsFrontlineCity(int cityId)` | `bool` | 判断某城市是否为前线城市（存在邻接敌方城市） |
| `GetFrontlineCityIds(int forceId)` | `List<int>` | 获取某势力所有前线城市ID |
| `GetRearCityIds(int forceId)` | `List<int>` | 获取某势力所有后方城市ID（非前线） |
| `GetRandomAdjacentCityId(int cityId)` | `int` | 随机获取一个相邻城市ID（无邻接时返回 `0`） |
| `CalculateCityDistance(int cityId1, int cityId2)` | `int` | 计算两城市间距离 |

## 方法命名约定

- `GetAdjacentEnemyCityIdsForCity` 带 `ForCity` 后缀 = 针对特定城市+势力的查询
- `GetAdjacentEnemyCityIds` 无 `ForCity` 后缀 = 势力级别的全局查询（遍历所有己方城市，`HashSet` 去重）
- `GetAdjacentCityIds` 无势力参数 = 纯配置查询，直接返回 `WorldConfig.WorldNearIds`

## 内部委托关系

- `IsFrontlineCity` 内部调用 `GetAdjacentEnemyCityIdsForCity`，判断结果 Count > 0
- `GetFrontlineCityIds` / `GetRearCityIds` 内部调用 `IsFrontlineCity`
- `GetOwnCityIds` 委托给 `GameManager.Instance.GetCitiesByForce(forceId)`
- `CalculateCityDistance` 委托给 `SysFormula.City.CalculateDistance(cfg1.X, cfg1.Y, cfg2.X, cfg2.Y)`，使用 `WorldConfig` 中的 X/Y 坐标
- `GetRandomAdjacentCityId` 使用 `SysRandom.Range`，符合战略层随机数规范

## 返回值约定

- `GetRandomAdjacentCityId` 无邻接城市时返回 `0`（0 不是合法 cityId，调用方需注意）
- 其余方法在配置不存在时返回空列表（`new List<int>()`）或 `false`

## 前线判定定义

`IsFrontlineCity` 的判定标准：只要存在一个非同势力的邻接城市就算前线。即 `GetAdjacentEnemyCityIdsForCity(cityId, city.forceId).Count > 0`。

## 邻接数据来源

所有邻接关系来自 `WorldConfig.WorldNearIds`（配置层数组），是地图拓扑的静态配置，非运行时动态计算。

## 使用规则

- 城市邻接查询必须使用 `MapTool`，禁止直接访问 `WorldConfig.GetConfig(cityId).WorldNearIds`
- 前线/后方城市判断必须使用 `MapTool.IsFrontlineCity` / `MapTool.GetFrontlineCityIds` / `MapTool.GetRearCityIds`
- 城市间距离计算必须使用 `MapTool.CalculateCityDistance`，`SaveCityData.CalculateDistanceTo` 内部已委托给它

## 禁止事项

- 不要在业务代码中直接访问 `WorldConfig.WorldNearIds` 进行城市邻接判断，必须使用 `MapTool` 的方法
