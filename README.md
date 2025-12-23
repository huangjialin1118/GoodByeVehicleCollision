# Collision Be Gone

[![Cities: Skylines 2](https://img.shields.io/badge/Cities%3A%20Skylines%202-Mod-blue)](https://www.paradoxinteractive.com/games/cities-skylines-ii)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Collision Be Gone** is a Cities: Skylines 2 mod that intelligently removes vehicle-to-vehicle collision blocking, allowing cars to pass through each other while preserving essential traffic rules.

---

**Collision Be Gone** 是一个城市：天际线2的模组，它智能地移除车辆之间的碰撞阻塞，让车辆可以相互穿过，同时保留必要的交通规则。

---

## Features | 功能特性

### English

- **Smart Collision Removal**: Vehicles no longer stop and wait for other vehicles, dramatically improving traffic flow
- **Preserves Traffic Signals**: Red lights, speed limits, and other traffic signals are fully respected
- **Emergency Vehicle Priority**: Police cars, fire engines, and ambulances still have right of way
- **Train Crossing Safety**: Vehicles properly stop at railway crossings
- **Parking Behavior**: Normal parking mechanics remain intact
- **Gradual Speed Recovery**: Vehicles smoothly accelerate back to lane speed instead of sudden jumps
- **In-Game Toggle**: Enable/disable the mod anytime through the game's options menu

### 中文

- **智能碰撞移除**：车辆不再因其他车辆而停车等待，大幅改善交通流量
- **保留交通信号**：红灯、限速等交通信号完全遵守
- **紧急车辆优先**：警车、消防车、救护车仍然享有优先通行权
- **铁路道口安全**：车辆在铁路道口正常等待火车通过
- **停车行为**：正常的停车机制保持不变
- **渐进式速度恢复**：车辆平滑加速恢复到车道速度，而非突然加速
- **游戏内开关**：可随时通过游戏选项菜单启用/禁用此模组

---

## How It Works | 工作原理

### English

The mod operates as a post-processing system that runs between `CarNavigationSystem` and `CarMoveSystem` in the game's simulation loop.

**Technical Details:**
1. The game's `CarLaneSpeedIterator` detects vehicles ahead and sets a "blocker" component, causing vehicles to slow down
2. This mod identifies blockers that are regular vehicles, bicycles, or pedestrians
3. It clears these blockers and gradually restores the vehicle's speed
4. Critical safety blockers (signals, trains, emergency vehicles, parking) are preserved

**Blocker Types Cleared:**
- Regular cars and trucks
- Bicycles
- Pedestrians

**Blocker Types Preserved:**
- Traffic signals (red lights, caution signs)
- Speed limits
- Trains at railway crossings
- Emergency vehicles (police, fire, ambulance)
- Parking maneuvers

### 中文

该模组作为后处理系统运行，在游戏模拟循环中位于 `CarNavigationSystem` 之后、`CarMoveSystem` 之前执行。

**技术细节：**
1. 游戏的 `CarLaneSpeedIterator` 检测到前方车辆后会设置"阻碍者"组件，导致车辆减速
2. 本模组识别出阻碍者为普通车辆、自行车或行人的情况
3. 清除这些阻碍并逐步恢复车辆速度
4. 关键的安全阻碍（信号灯、火车、紧急车辆、停车）被保留

**被清除的阻碍类型：**
- 普通汽车和卡车
- 自行车
- 行人

**被保留的阻碍类型：**
- 交通信号（红灯、警示标志）
- 速度限制
- 铁路道口的火车
- 紧急车辆（警车、消防车、救护车）
- 停车动作

---

## Installation | 安装

### English

1. Subscribe to the mod on Paradox Mods
2. Enable the mod in the game's Content Manager
3. The mod is enabled by default when loaded

### 中文

1. 在 Paradox Mods 上订阅此模组
2. 在游戏的内容管理器中启用模组
3. 模组加载后默认启用

---

## Configuration | 配置

### English

1. Open the game's **Options** menu
2. Navigate to **Collision Be Gone** section
3. Toggle **Enable Collision Be Gone** to turn the mod on/off

### 中文

1. 打开游戏的**选项**菜单
2. 导航到 **Collision Be Gone** 部分
3. 切换**Enable Collision Be Gone**来开启/关闭模组

---

## Compatibility | 兼容性

### English

- This mod modifies vehicle navigation behavior at runtime
- Should be compatible with most other mods
- May conflict with other mods that modify the `Blocker` or `CarNavigation` components

### 中文

- 此模组在运行时修改车辆导航行为
- 应与大多数其他模组兼容
- 可能与其他修改 `Blocker` 或 `CarNavigation` 组件的模组冲突

---

## Building from Source | 从源码构建

### Requirements | 需求

- Cities: Skylines 2 Modding Toolchain
- .NET SDK (version as specified in project)
- `CSII_TOOLPATH` environment variable configured

### Build | 构建

```bash
dotnet build
```

---

## License | 许可证

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

本项目基于 MIT 许可证开源 - 详情请查看 [LICENSE](LICENSE) 文件。

---

## Credits | 致谢

- Colossal Order for Cities: Skylines 2
- The CS2 modding community

---

## Changelog | 更新日志

### v1.0.0
- Initial release | 首次发布
- Smart collision removal for vehicles | 智能车辆碰撞移除
- Preserves traffic signals, emergency vehicles, and trains | 保留交通信号、紧急车辆和火车
