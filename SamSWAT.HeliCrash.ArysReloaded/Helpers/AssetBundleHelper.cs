using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SamSWAT.HeliCrash.ArysReloaded.Helpers
{
	// Token: 0x02000006 RID: 6
	public static class AssetBundleHelper
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002480 File Offset: 0x00000680
		public static async Task<AssetBundle> LoadAsync(string path)
		{
			AssetBundleCreateRequest heliBundleRequest = AssetBundle.LoadFromFileAsync(path);
			AssetBundle assetBundle;
			if (heliBundleRequest == null)
			{
				assetBundle = null;
			}
			else
			{
				while (!heliBundleRequest.isDone)
				{
					await Task.Yield();
				}
				assetBundle = heliBundleRequest.assetBundle;
			}
			return assetBundle;
		}
	}
}
