using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPT.Custom.Airdrops.Models;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;

namespace SamSWAT.HeliCrash.ArysReloaded.Helpers
{
	// Token: 0x02000007 RID: 7
	public static class ItemHelper
	{
		// Token: 0x06000018 RID: 24 RVA: 0x000024C4 File Offset: 0x000006C4
		public static async Task AddLoot(StashGridClass grid, AirdropLootResultModel lootToAdd)
		{
			ItemFactory itemFactory = Singleton<ItemFactory>.Instance;
			foreach (AirdropLootModel airdropLootModel in lootToAdd.Loot)
			{
				Item item;
				ResourceKey[] array;
				if (airdropLootModel.IsPreset)
				{
					item = itemFactory.GetPresetItem(airdropLootModel.Tpl);
					item.SpawnedInSession = true;
					GClass761.ExecuteForEach<Item>(GClass2771.GetAllItems(item), delegate(Item x)
					{
						x.SpawnedInSession = true;
					});
					array = (from x in GClass2771.GetAllItems(item)
						select x.Template).SelectMany((ItemTemplate x) => x.AllResources).ToArray<ResourceKey>();
				}
				else
				{
					item = itemFactory.CreateItem(airdropLootModel.ID, airdropLootModel.Tpl, null);
					item.StackObjectsCount = airdropLootModel.StackCount;
					item.SpawnedInSession = true;
					array = item.Template.AllResources.ToArray<ResourceKey>();
				}
				grid.Add(item);
				await Singleton<PoolManager>.Instance.LoadBundlesAndCreatePools(PoolManager.PoolsCategory.Raid, PoolManager.AssemblyType.Local, array, JobPriority.Immediate, null, PoolManager.DefaultCancellationToken);
			}
			IEnumerator<AirdropLootModel> enumerator = null;
		}
	}
}
