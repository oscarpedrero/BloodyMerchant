using Bloodstone.API;
using BloodyMerchant.Systems;
using Il2CppInterop.Runtime;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace BloodyMerchant.Utils
{
    internal class Helper
    {
        public static NativeArray<Entity> GetEntitiesByComponentTypes<T1, T2>(EntityQueryOptions queryOptions = default)
        {
            EntityQueryDesc queryDesc = new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                new ComponentType(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite),
                new ComponentType(Il2CppType.Of<T2>(), ComponentType.AccessMode.ReadWrite)
                },
                Options = queryOptions
            };

            var query = VWorld.Server.EntityManager.CreateEntityQuery(queryDesc);

            var entities = query.ToEntityArray(Allocator.Temp);
            query.Dispose();
            return entities;
        }
        public static NativeArray<Entity> GetEntitiesByOneComponentTypes<T1>(EntityQueryOptions queryOptions = default)
        {
            EntityQueryDesc queryDesc = new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                new ComponentType(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite)
                },
                Options = queryOptions
            };

            var query = VWorld.Server.EntityManager.CreateEntityQuery(queryDesc);

            var entities = query.ToEntityArray(Allocator.Temp);
            query.Dispose();
            return entities;
        }

        public static Entity GetAnyUser()
        {
            var _entities = Il2cppService.GetEntitiesByComponentTypes<User>();
            foreach (var _entity in _entities)
            {
                return _entity;
            }
            return Entity.Null;
        }
    }
}
