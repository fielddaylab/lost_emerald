using UnityEngine;
using FieldDay;
using Shipwreck;
using System.Runtime.InteropServices;

/// <summary>
/// Note: This class is designed to be used with the `vault-floating-dropdown` jslib pluggin. 
/// This script will remove the dropdown elment from the DOM after the target scene has been unloaded.
/// </summary>
public class VaultDropdownToggle : MonoBehaviour {
    [DllImport("__Internal")]
    private static extern void DisableVaultButton();

    private void Awake() {
#if UNITY_WEBGL && !UNITY_EDITOR
        GameMgr.Events.Register(GameEvents.ExitTitleScreen, DisableVaultButton);
#endif
    }
}