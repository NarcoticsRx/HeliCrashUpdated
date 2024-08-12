using System;
using UnityEngine;

namespace SamSWAT.HeliCrash.ArysReloaded
{
	// Token: 0x02000002 RID: 2
	public class HelicopterSpawnLocation
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public Vector3 Position { get; set; } = Vector3.zero;

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002069 File Offset: 0x00000269
		public Vector3 Rotation { get; set; } = Vector3.zero;
	}
}
