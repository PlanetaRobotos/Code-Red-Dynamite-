using UnityEngine;

namespace Core.Audio
{
    public enum AudioType
    {
        [Header("Conflicting audio type")]
        None,
        [Header("Sound Tracks")]
        St01,
        St02,
        [Header("Sound Effects")]
        SfxPlayerGround,
        SfxPlayerRope,
        SfxPlayerBullet,
        SfxPlayerBlock,
        SfxPlayerMill,
        SfxPlayerGravity,
        SfxPlayerAppear,
        SfxPlayerTeleport,
        SfxPlayerFinish,
        SfxUiLevelChange,
        SfxUiShakeCamera,
        SfxUiFreezeCamera,
        SfxPlayerStar,
        SfxUiStarFilling,
        SfxUiSwitch,

        // SfxReload,
        //... custom audio types
    }
}