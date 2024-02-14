using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;
namespace InfiniteShotgunReload;

[HarmonyPatch(typeof(ShotgunItem))]
public class ShotgunPatch
{
	/**
	 * Patch to alllow to reload the shotgun when it has 2 bullets already loaded.
	 */
	[HarmonyPatch(nameof(ShotgunItem.ItemInteractLeftRight))]
	[HarmonyPostfix]
	public static void ItemInteractLeftRightPatch(ShotgunItem __instance, bool right)
	{
		if (right && !__instance.isReloading && __instance.shellsLoaded >= 2)
			__instance.StartReloadGun();
	}

	[HarmonyPatch(nameof(ShotgunItem.ShootGun))]
	[HarmonyPrefix]
	[HarmonyPriority(Priority.High)]
	public static void SaveShellsPatch(ShotgunItem __instance, out int __state)
	{
		__state = __instance.shellsLoaded;
	}

	[HarmonyPatch(nameof(ShotgunItem.ShootGun))]
	[HarmonyPostfix]
	public static void ShootGunPatch(ShotgunItem __instance, int __state)
	{
		__instance.shellsLoaded = Math.Max(0, __state - 1);
	}

	[HarmonyPatch(nameof(ShotgunItem.reloadGunAnimation))]
	[HarmonyPrefix]
	public static bool ReloadGunAnimationPatch(ShotgunItem __instance, ref IEnumerator __result)
	{
		__result = ReloadGunAnimationPatchCoroutine(__instance);
		return false;
	}

	static IEnumerator ReloadGunAnimationPatchCoroutine(ShotgunItem __instance)
	{
		ShotgunItem shotgunItem = __instance;
		shotgunItem.isReloading = true;
		if (shotgunItem.shellsLoaded <= 0)
		{
			shotgunItem.playerHeldBy.playerBodyAnimator.SetBool("ReloadShotgun", true);
			shotgunItem.shotgunShellLeft.enabled = false;
			shotgunItem.shotgunShellRight.enabled = false;
		}
		else
		{
			shotgunItem.playerHeldBy.playerBodyAnimator.SetBool("ReloadShotgun2", true);
			shotgunItem.shotgunShellRight.enabled = false;
		}
		yield return new WaitForSeconds(0.3f);
		shotgunItem.gunAudio.PlayOneShot(shotgunItem.gunReloadSFX);
		shotgunItem.gunAnimator.SetBool("Reloading", true);
		shotgunItem.ReloadGunEffectsServerRpc();
		yield return new WaitForSeconds(0.95f);
		shotgunItem.shotgunShellInHand.enabled = true;
		shotgunItem.shotgunShellInHandTransform.SetParent(shotgunItem.playerHeldBy.leftHandItemTarget);
		shotgunItem.shotgunShellInHandTransform.localPosition = new Vector3(-0.0555f, 0.1469f, -0.0655f);
		shotgunItem.shotgunShellInHandTransform.localEulerAngles = new Vector3(-1.956f, 143.856f, -16.427f);
		yield return new WaitForSeconds(0.95f);
		shotgunItem.playerHeldBy.DestroyItemInSlotAndSync(shotgunItem.ammoSlotToUse);
		shotgunItem.ammoSlotToUse = -1;
		shotgunItem.shellsLoaded += 1;
		shotgunItem.shotgunShellLeft.enabled = true;
		if(shotgunItem.shellsLoaded >= 2)
			shotgunItem.shotgunShellRight.enabled = true;
		shotgunItem.shotgunShellInHand.enabled = false;
		shotgunItem.shotgunShellInHandTransform.SetParent(shotgunItem.transform);
		yield return new WaitForSeconds(0.45f);
		shotgunItem.gunAudio.PlayOneShot(shotgunItem.gunReloadFinishSFX);
		shotgunItem.gunAnimator.SetBool("Reloading", false);
		shotgunItem.playerHeldBy.playerBodyAnimator.SetBool("ReloadShotgun", false);
		shotgunItem.playerHeldBy.playerBodyAnimator.SetBool("ReloadShotgun2", false);
		shotgunItem.isReloading = false;
		shotgunItem.ReloadGunEffectsServerRpc(false);
	}
}