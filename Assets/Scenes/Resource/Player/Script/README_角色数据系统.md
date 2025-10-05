# 角色数据跨场景保持系统

## 系统概述

这个系统解决了Unity中角色数值（血量、等级、装备等）在不同地图间保持的问题。通过单例模式和数据持久化，确保角色数据在场景切换时不会丢失。

## 核心组件

### 1. GameDataManager (游戏数据管理器)
- **功能**: 单例模式管理所有持久化数据
- **特点**: 跨场景不被销毁，自动保存/加载数据
- **位置**: `Assets/Scenes/Resource/Player/Script/GameDataManager.cs`

### 2. PlayerData (角色数据结构)
- **功能**: 存储所有需要持久化的角色数据
- **包含**: 血量、等级、经验、装备、背包、金币等
- **位置**: `Assets/Scenes/Resource/Player/Script/PlayerData.cs`

### 3. EquipmentManager (装备管理器)
- **功能**: 管理装备的创建、属性和效果
- **特点**: 支持装备数据库、随机装备生成
- **位置**: `Assets/Scenes/Resource/Player/Script/EquipmentManager.cs`

### 4. SceneTransitionManager (场景切换管理器)
- **功能**: 处理场景切换时的数据保存和恢复
- **特点**: 支持平滑过渡、位置恢复
- **位置**: `Assets/Scenes/Resource/Player/Script/SceneTransitionManager.cs`

## 系统架构图

```
GameDataManager (单例)
├── PlayerData (角色数据)
│   ├── 基础属性 (等级、经验)
│   ├── 生命值系统 (血量、法力)
│   ├── 属性点 (力量、敏捷等)
│   ├── 战斗属性 (攻击力、防御力)
│   ├── 货币系统 (金币、宝石)
│   ├── 装备系统 (武器、护甲等)
│   └── 背包系统 (物品清单)
├── EquipmentManager (装备管理)
└── SceneTransitionManager (场景切换)

FSM_Player (角色控制器)
├── Player_Params (临时数据)
│   ├── 移动参数 (速度、跳跃)
│   ├── 组件引用 (Animator、Rigidbody)
│   └── 临时状态 (地面检测等)
└── 与GameDataManager交互
```

## 使用方法

### 1. 基本设置

在场景中放置以下组件：
- `GameDataManager` (会自动创建)
- `EquipmentManager` (会自动创建)
- `SceneTransitionManager` (会自动创建)
- `PlayerTestController` (用于测试)

### 2. 获取角色数据

```csharp
// 获取游戏数据管理器
GameDataManager gameDataManager = GameDataManager.Instance;

// 获取角色数据
PlayerData playerData = gameDataManager.GetPlayerData();

// 修改生命值
gameDataManager.ModifyHealth(-10f); // 受到伤害
gameDataManager.ModifyHealth(20f);  // 恢复生命值

// 修改法力值
gameDataManager.ModifyMana(-5f);    // 消耗法力
gameDataManager.ModifyMana(10f);    // 恢复法力

// 添加经验值
gameDataManager.AddExperience(50);

// 修改金币
gameDataManager.ModifyGold(100);
```

### 3. 装备系统使用

```csharp
// 获取装备管理器
EquipmentManager equipmentManager = EquipmentManager.Instance;

// 获取装备数据
EquipmentData weapon = equipmentManager.GetEquipmentById("iron_sword");

// 装备物品
gameDataManager.EquipItem(weapon);

// 卸下装备
EquipmentData unequippedWeapon = gameDataManager.UnequipItem(EquipmentType.Weapon);

// 创建随机装备
EquipmentData randomWeapon = equipmentManager.CreateRandomEquipment(EquipmentType.Weapon, 5);
```

### 4. 场景切换

```csharp
// 获取场景切换管理器
SceneTransitionManager sceneManager = SceneTransitionManager.Instance;

// 切换到指定场景
sceneManager.TransitionToScene("Map2", new Vector3(10, 0, 0));

// 快速切换（无过渡效果）
sceneManager.QuickTransitionToScene("Map2");

// 重新加载当前场景
sceneManager.ReloadCurrentScene();
```

### 5. 数据保存/加载

```csharp
// 手动保存数据
gameDataManager.SavePlayerData();

// 手动加载数据
gameDataManager.LoadPlayerData();

// 删除所有存档
gameDataManager.DeleteAllSaveData();
```

## 测试功能

使用 `PlayerTestController` 进行测试：

- **T键**: 造成伤害
- **Y键**: 恢复生命值
- **U键**: 获得经验值
- **I键**: 获得金币
- **O键**: 装备测试武器
- **P键**: 卸下武器
- **F5键**: 保存数据
- **F9键**: 加载数据

## 数据持久化

- **自动保存**: 每30秒自动保存一次
- **手动保存**: 按F5键或调用SavePlayerData()
- **应用暂停**: 应用失去焦点时自动保存
- **场景切换**: 切换场景时自动保存位置和状态

## 事件系统

系统提供事件通知机制：

```csharp
// 订阅事件
GameDataManager.OnHealthChanged += OnHealthChanged;
GameDataManager.OnManaChanged += OnManaChanged;
GameDataManager.OnLevelChanged += OnLevelChanged;
GameDataManager.OnGoldChanged += OnGoldChanged;

// 事件处理方法
private void OnHealthChanged(float currentHealth)
{
    // 更新UI显示
    healthBar.value = currentHealth / maxHealth;
}
```

## 注意事项

1. **单例模式**: GameDataManager、EquipmentManager、SceneTransitionManager 都是单例，确保只有一个实例存在
2. **数据分离**: 持久化数据（血量、等级等）和临时数据（位置、动画状态等）分开管理
3. **自动保存**: 系统会自动保存数据，无需手动调用
4. **场景切换**: 使用SceneTransitionManager进行场景切换，确保数据正确保存和恢复
5. **UI更新**: 通过事件系统自动更新UI，保持数据同步

## 扩展功能

系统设计为可扩展的架构，可以轻松添加：

- 更多装备类型和属性
- 技能系统
- 任务系统
- 成就系统
- 多人游戏支持

## 故障排除

1. **数据未保存**: 检查GameDataManager是否正确初始化
2. **场景切换失败**: 确保使用SceneTransitionManager进行场景切换
3. **UI不更新**: 检查是否正确订阅了事件
4. **装备不生效**: 确保装备数据正确设置并调用EquipItem方法

这个系统为你的Unity项目提供了一个完整、可靠的跨场景数据保持解决方案。
