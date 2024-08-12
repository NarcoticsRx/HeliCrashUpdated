using System;
using System.Collections.Generic;

namespace SamSWAT.HeliCrash.ArysReloaded
{
	// Token: 0x02000004 RID: 4
	public class HeliCrashLocations : Dictionary<string, List<HelicopterSpawnLocation>>
	{
		// Token: 0x0600000D RID: 13 RVA: 0x000022A4 File Offset: 0x000004A4
		public bool TryGetHeliCrashLocation(string map, out HelicopterSpawnLocation location)
		{
			location = null;
			List<HelicopterSpawnLocation> list;
			if (!base.TryGetValue(map, out list))
			{
				return false;
			}
			location = GClass3285.Random<HelicopterSpawnLocation>(list);
			return true;
		}
	}
}
