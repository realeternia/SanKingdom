# Checklist

## 存储功能
- [x] SaveCityData包含可序列化的委派数据字段（DevAssignmentData列表）
- [x] DevAssignmentData类正确包含heroId和devId字段
- [x] 委派数据能够正确序列化和反序列化

## 读取功能
- [x] CityPanel打开时正确读取委派数据
- [x] 切换城市时正确加载对应城市的委派数据
- [x] 委派节点正确显示已分配的hero

## 保存功能
- [x] 分配hero到委派节点时数据保存到SaveCityData
- [x] 移除hero委派时数据从SaveCityData移除
- [x] 数据变更后触发游戏存档

## 清空功能
- [x] hero移动时原城市委派记录被移除
- [x] hero被俘虏时委派记录被移除（通过城市被攻占时清空所有委派）
- [x] 俘虏逃跑时原城市委派记录被移除
- [x] 城市被攻占时所有委派记录被清空
