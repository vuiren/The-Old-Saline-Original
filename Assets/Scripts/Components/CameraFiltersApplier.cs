using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Assets.Scripts.Components
{
    class CameraFiltersApplier
    {
        Transform playerHead;

        PostProcessProfile airProfile;
        PostProcessProfile waterProfile;

        PostProcessVolume postProcessVolume;

        bool isNewEffect;

        Water water;
        public CameraFiltersApplier(Transform playerHead, PostProcessProfile airProfile, PostProcessProfile waterProfile, PostProcessVolume postProcessVolume)
        {
            this.playerHead = playerHead;
            this.airProfile = airProfile;
            this.waterProfile = waterProfile;
            this.postProcessVolume = postProcessVolume;
        }

        public void ApplyOnAirFilter()
        {
            postProcessVolume.profile = airProfile;
        }

        public void TryToApplyWaterFilter(Water water)
        {
            this.water = water;
            if (playerHead.position.y <= water.WaterLevelY)
            {
                if (!isNewEffect)
                {
                    isNewEffect = true;
                    RenderSettings.fog = true;
                    RenderSettings.fogDensity = 0.1f;
                    postProcessVolume.profile = waterProfile;
                }
            }
            else
            {
                if (isNewEffect)
                {
                    isNewEffect = false;
                    RenderSettings.fog = false;
                    ApplyOnAirFilter();
                }
            }
        }

        public void Update()
        {
            if (water)
                TryToApplyWaterFilter(water);
        }
    }
}
