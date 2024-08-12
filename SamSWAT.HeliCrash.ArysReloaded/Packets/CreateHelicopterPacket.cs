using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using EFT.SynchronizableObjects;
using Fika.Core.Networking;
using LiteNetLib.Utils;
using UnityEngine;

namespace SamSWAT.HeliCrash.ArysReloaded.Packets
{
	// Token: 0x02000005 RID: 5
	public struct CreateHelicopterPacket : INetSerializable
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000022D2 File Offset: 0x000004D2
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000022DA File Offset: 0x000004DA
		public Vector3 Position { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000022E3 File Offset: 0x000004E3
		// (set) Token: 0x06000012 RID: 18 RVA: 0x000022EB File Offset: 0x000004EB
		public Quaternion Rotation { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000022F4 File Offset: 0x000004F4
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000022FC File Offset: 0x000004FC
		public Item RootItem { get; set; }

		// Token: 0x06000015 RID: 21 RVA: 0x00002308 File Offset: 0x00000508
		public void Deserialize(NetDataReader reader)
		{
			this.Position = FikaSerializationExtensions.GetVector3(reader);
			this.Rotation = FikaSerializationExtensions.GetQuaternion(reader);
			using (MemoryStream memoryStream = new MemoryStream(FikaSerializationExtensions.GetByteArray(reader)))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					this.RootItem = GClass1535.DeserializeItem(Singleton<ItemFactory>.Instance, new Dictionary<string, Item>(), GClass2972.ReadEFTItemDescriptor(binaryReader));
				}
			}
			ContainerCollection[] array = new ContainerCollection[] { this.RootItem as ContainerCollection };
			ResourceKey[] array2 = GClass2771.GetAllItemsFromCollections(array).Concat(array.Where(new Func<Item, bool>(AirdropSynchronizableObject.Class1832.class1832_0.method_2))).SelectMany(new Func<Item, IEnumerable<ResourceKey>>(AirdropSynchronizableObject.Class1832.class1832_0.method_3))
				.ToArray<ResourceKey>();
			Singleton<PoolManager>.Instance.LoadBundlesAndCreatePools(PoolManager.PoolsCategory.Raid, PoolManager.AssemblyType.Online, array2, JobPriority.Immediate, null, default(CancellationToken));
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000023FC File Offset: 0x000005FC
		public void Serialize(NetDataWriter writer)
		{
			FikaSerializationExtensions.Put(writer, this.Position);
			FikaSerializationExtensions.Put(writer, this.Rotation);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					GClass2972.Write(binaryWriter, GClass1535.SerializeItem(this.RootItem));
				}
				FikaSerializationExtensions.PutByteArray(writer, memoryStream.ToArray());
			}
		}
	}
}
