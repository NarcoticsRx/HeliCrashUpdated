using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SPT.Custom.Airdrops.Models;
using SPT.Custom.Airdrops.Utils;
using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using EFT.Airdrop;
using EFT.Interactive;
using EFT.InventoryLogic;
using Fika.Core.Coop.Components;
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Networking;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using SamSWAT.HeliCrash.ArysReloaded.Helpers;
using SamSWAT.HeliCrash.ArysReloaded.Packets;
using UnityEngine;

namespace SamSWAT.HeliCrash.ArysReloaded
{
	// Token: 0x02000003 RID: 3
	[BepInPlugin("com.SamSWAT.HeliCrash.ArysReloaded", "SamSWAT.HeliCrash.ArysReloaded", "2.2.1")]
	[BepInDependency("com.fika.core", (BepInDependency.DependencyFlags)1)]
	[BepInDependency("com.SPT.custom", (BepInDependency.DependencyFlags)1)]
	public class Plugin : BaseUnityPlugin
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002090 File Offset: 0x00000290
		private void Awake()
		{
			this._directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string text = this._directory + "/HeliCrashLocations.json";
			if (!File.Exists(text))
			{
				base.Logger.LogWarning("File does not exist '" + text + "'");
				return;
			}
			string text2 = File.ReadAllText(text);
			this._heliCrashLocations = JsonConvert.DeserializeObject<HeliCrashLocations>(text2);
			if (this._heliCrashLocations == null)
			{
				base.Logger.LogError("Failed to deserialize '" + text + "'");
				return;
			}
			FikaEventDispatcher.SubscribeEvent<GameWorldStartedEvent>(new Action<GameWorldStartedEvent>(this.OnGameWorldStarted));
			FikaEventDispatcher.SubscribeEvent<FikaClientCreatedEvent>(new Action<FikaClientCreatedEvent>(this.OnFikaClientCreated));
			this._heliCrashChance = base.Config.Bind<float>("Main Settings", "Helicopter crash site chance", 10f, new ConfigDescription("Percent chance of helicopter crash site appearance", new AcceptableValueRange<float>(0f, 100f), Array.Empty<object>()));
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000217E File Offset: 0x0000037E
		private void OnFikaClientCreated(FikaClientCreatedEvent e)
		{
			e.Client.packetProcessor.SubscribeNetSerializable<CreateHelicopterPacket>(new Action<CreateHelicopterPacket>(this.OnCreateHelicopterPacket));
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000219C File Offset: 0x0000039C
		private async void OnGameWorldStarted(GameWorldStartedEvent e)
		{
			if (Singleton<FikaServer>.Instantiated)
			{
				bool flag = LocationScene.GetAll<AirdropPoint>().Any<AirdropPoint>();
				string locationId = e.GameWorld.LocationId;
				if (!flag)
				{
					base.Logger.LogDebug("No AirdropPoints on '" + locationId + "'");
				}
				else if (global::UnityEngine.Random.Range(0f, 100f) <= this._heliCrashChance.Value)
				{
					base.Logger.LogDebug("Rng pass, continuing...");
					HelicopterSpawnLocation helicopterSpawnLocation;
					if (!this._heliCrashLocations.TryGetHeliCrashLocation(locationId, out helicopterSpawnLocation))
					{
						base.Logger.LogDebug("'" + locationId + "' was not found in HeliCrashLocations");
					}
					else
					{
						Vector3 position = helicopterSpawnLocation.Position;
						Quaternion rotation = Quaternion.Euler(helicopterSpawnLocation.Rotation);
						AirdropLootResultModel loot = new ItemFactoryUtil().GetLoot();
						Item itemCrate = Singleton<ItemFactory>.Instance.CreateItem("HelicopterContainer", "6223349b3136504a544d1608", null);
						await ItemHelper.AddLoot((itemCrate as LootItemClass).Grids[0], loot);
						this.CreateHelicopter(position, rotation, itemCrate);
					}
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021DC File Offset: 0x000003DC
		private async Task<GameObject> GetHelicopterPrefab()
		{
			GameObject gameObject;
			if (this._heliPrefab != null)
			{
				gameObject = this._heliPrefab;
			}
			else
			{
				string path = this._directory + "/sikorsky_uh60_blackhawk.bundle";
				AssetBundle assetBundle = await AssetBundleHelper.LoadAsync(path);
				if (assetBundle == null)
				{
					base.Logger.LogError("Failed to load '" + path + "'");
					gameObject = null;
				}
				else
				{
					this._heliPrefab = assetBundle.LoadAllAssets<GameObject>()[0];
					gameObject = this._heliPrefab;
				}
			}
			return gameObject;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000221F File Offset: 0x0000041F
		private void OnCreateHelicopterPacket(CreateHelicopterPacket packet)
		{
			this.CreateHelicopter(packet.Position, packet.Rotation, packet.RootItem);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002240 File Offset: 0x00000440
		private async Task CreateHelicopter(Vector3 position, Quaternion rotation, Item rootItem)
		{
			GameObject gameObject = await this.GetHelicopterPrefab();
			if (gameObject)
			{
				GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject, position, rotation);
				WorldInteractiveObject[] componentsInChildren = gameObject2.GetComponentsInChildren<WorldInteractiveObject>();
				CoopHandler coopHandler;
				/*
				if (CoopHandler.TryGetCoopHandler(out coopHandler))
				{
					foreach (WorldInteractiveObject worldInteractiveObject in componentsInChildren)
					{
						coopHandler.ListOfInteractiveObjects[worldInteractiveObject.Id] = worldInteractiveObject;
					}
				}
				else
				{
					base.Logger.LogWarning("Failed to get CoopHandler, interactives not registered");
				}
				*/
				LootItem.CreateLootContainer(gameObject2.GetComponentInChildren<LootableContainer>(), rootItem, "Heavy crate", Singleton<GameWorld>.Instance, null);
				if (Singleton<FikaServer>.Instantiated)
				{
					CreateHelicopterPacket createHelicopterPacket = new CreateHelicopterPacket
					{
						Position = position,
						Rotation = rotation,
						RootItem = rootItem
					};
					NetDataWriter netDataWriter = new NetDataWriter();
					Singleton<FikaServer>.Instance.SendDataToAll<CreateHelicopterPacket>(netDataWriter, ref createHelicopterPacket, 0, null);
					base.Logger.LogDebug(string.Format("Sending {0} bytes", netDataWriter.Length));
				}
			}
		}

		// Token: 0x04000003 RID: 3
		private HeliCrashLocations _heliCrashLocations;

		// Token: 0x04000004 RID: 4
		private string _directory;

		// Token: 0x04000005 RID: 5
		private ConfigEntry<float> _heliCrashChance;

		// Token: 0x04000006 RID: 6
		private GameObject _heliPrefab;
	}
}
