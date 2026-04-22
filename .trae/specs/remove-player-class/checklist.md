# Checklist

- [x] SaveForceData 包含所有新增字段（phase, warPlans, planConfirmed）
- [x] SaveForceData 包含所有计算属性（Name, LineColor, IconPath）
- [x] SaveForceData 包含所有从 Player 迁移的方法
- [x] GameManager 不再包含 Player 相关字段和方法
- [x] GameManager 使用 currentForceId 进行回合管理
- [x] SaveCityData.GetForce() 返回 SaveForceData 类型
- [x] AI 系统所有方法使用 SaveForceData 参数
- [x] 所有引用 Player 的代码已更新为使用 SaveForceData
- [x] Player.cs 文件已删除
- [x] 代码编译无错误
