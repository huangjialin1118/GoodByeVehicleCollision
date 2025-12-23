using Game;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace CollisionBeGone
{
    /// <summary>
    /// 后处理系统：清除车辆之间的碰撞阻碍，保留交通信号、火车、行人等
    /// 在 CarNavigationSystem 之后、CarMoveSystem 之前执行
    ///
    /// 核心原理：
    /// - CarLaneSpeedIterator 检测到前车后会设置 blocker 并降低速度
    /// - 我们清除 blocker 并渐进式恢复速度（避免突然加速）
    /// </summary>
    public partial class CollisionBeGoneSystem : GameSystemBase
    {
        private EntityQuery m_VehicleQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_VehicleQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<CarNavigation>(),
                    ComponentType.ReadWrite<Blocker>(),
                    ComponentType.ReadOnly<CarCurrentLane>(),
                    ComponentType.ReadOnly<Car>(),
                    ComponentType.ReadOnly<PrefabRef>()
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Game.Tools.Temp>()
                }
            });

            RequireForUpdate(m_VehicleQuery);
        }

        protected override void OnUpdate()
        {
            if (!Mod.m_Setting.Enabled)
                return;

            var entities = m_VehicleQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];

                var blocker = EntityManager.GetComponentData<Blocker>(entity);
                var currentLane = EntityManager.GetComponentData<CarCurrentLane>(entity);

                if (ShouldClearBlocker(currentLane, blocker))
                {
                    var navigation = EntityManager.GetComponentData<CarNavigation>(entity);
                    var prefabRef = EntityManager.GetComponentData<PrefabRef>(entity);

                    // 计算车道允许的最大速度
                    float targetSpeed = CalculateLaneMaxSpeed(currentLane, prefabRef);
                    float currentSpeed = math.abs(navigation.m_MaxSpeed);

                    // 恢复速度：确保最低速度足够穿过障碍
                    // 最低速度 = 目标速度的50%（至少5 m/s），这样能快速穿过
                    // 然后渐进式增加到目标速度
                    float minSpeed = math.max(5f, targetSpeed * 0.5f);
                    float restoredSpeed = math.min(targetSpeed, math.max(minSpeed, currentSpeed * 1.1f + 1f));

                    // 保持速度符号（正向/倒车）
                    float sign = math.sign(navigation.m_MaxSpeed);
                    if (sign == 0) sign = 1f;

                    navigation.m_MaxSpeed = sign * restoredSpeed;
                    EntityManager.SetComponentData(entity, navigation);

                    // 清除阻碍者信息
                    blocker.m_Blocker = Entity.Null;
                    blocker.m_Type = BlockerType.None;
                    blocker.m_MaxSpeed = 255;
                    EntityManager.SetComponentData(entity, blocker);
                }
            }

            entities.Dispose();
        }

        private bool ShouldClearBlocker(CarCurrentLane currentLane, Blocker blocker)
        {
            // 1. 如果没有阻碍者，不需要处理
            if (blocker.m_Blocker == Entity.Null)
                return false;

            // 2. 保留交通信号（红灯、限速等）
            if (blocker.m_Type == BlockerType.Signal ||
                blocker.m_Type == BlockerType.Limit ||
                blocker.m_Type == BlockerType.Caution ||
                blocker.m_Type == BlockerType.Spawn)
            {
                return false;
            }

            // 3. 保留停车场景
            if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.ParkingSpace) != 0)
            {
                return false;
            }

            // 4. 保留火车（铁路道口）
            if (EntityManager.HasComponent<Train>(blocker.m_Blocker))
            {
                return false;
            }

            // 5. 如果阻碍者是汽车，需要进一步检查是否为紧急车辆
            if (EntityManager.HasComponent<Car>(blocker.m_Blocker))
            {
                // 检查是否为紧急车辆（警车、消防车、救护车）
                if (IsEmergencyVehicle(blocker.m_Blocker))
                {
                    return false; // 保留紧急车辆的阻碍
                }
                return true;
            }

            // 6. 如果阻碍者是自行车，清除
            if (EntityManager.HasComponent<Bicycle>(blocker.m_Blocker))
            {
                return true;
            }

            // 7. 如果阻碍者是行人（Human/Creature），清除
            if (EntityManager.HasComponent<Game.Creatures.Human>(blocker.m_Blocker) ||
                EntityManager.HasComponent<Game.Creatures.Creature>(blocker.m_Blocker))
            {
                return true;
            }

            // 8. 其他未知情况保留
            return false;
        }

        private float CalculateLaneMaxSpeed(CarCurrentLane currentLane, PrefabRef prefabRef)
        {
            // 获取车辆的基础参数
            if (!EntityManager.HasComponent<CarData>(prefabRef.m_Prefab))
            {
                return 20f; // 默认速度
            }

            var carData = EntityManager.GetComponentData<CarData>(prefabRef.m_Prefab);

            // 获取车道速度限制
            if (EntityManager.HasComponent<Game.Net.CarLane>(currentLane.m_Lane))
            {
                var carLane = EntityManager.GetComponentData<Game.Net.CarLane>(currentLane.m_Lane);
                float maxDriveSpeed = CalculateMaxDriveSpeed(carData, carLane.m_SpeedLimit, carLane.m_Curviness);
                return math.min(maxDriveSpeed, carData.m_MaxSpeed);
            }

            // 如果没有 CarLane 组件（特殊车道），使用保守的默认速度
            // 15 m/s ≈ 54 km/h，比较合理的城市道路速度
            return math.min(15f, carData.m_MaxSpeed);
        }

        /// <summary>
        /// 检查实体是否为紧急车辆（警车、消防车、救护车）
        /// 直接检查车辆实体的运行时组件，而不是通过预制件
        /// </summary>
        private bool IsEmergencyVehicle(Entity vehicleEntity)
        {
            // 直接检查车辆实体是否有紧急车辆运行时组件
            // 使用完全限定名避免与 Game.Prefabs 命名空间冲突
            if (EntityManager.HasComponent<Game.Vehicles.PoliceCar>(vehicleEntity))
            {
                return true;
            }

            if (EntityManager.HasComponent<Game.Vehicles.FireEngine>(vehicleEntity))
            {
                return true;
            }

            if (EntityManager.HasComponent<Game.Vehicles.Ambulance>(vehicleEntity))
            {
                return true;
            }

            return false;
        }

        private float CalculateMaxDriveSpeed(CarData carData, float speedLimit, float curviness)
        {
            if (curviness < 0.001f)
            {
                return speedLimit;
            }

            // 根据弯道曲率计算安全速度
            float turningSpeed = carData.m_Turning.x * carData.m_MaxSpeed /
                math.max(0.000001f, curviness * carData.m_MaxSpeed + carData.m_Turning.x - carData.m_Turning.y);
            turningSpeed = math.max(1f, turningSpeed);

            return math.min(speedLimit, turningSpeed);
        }
    }
}
