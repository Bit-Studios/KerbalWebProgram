using System;
using System.Collections.Generic;
using System.Text;
using KerbalWebProgram;
using KSP;
using KSP.Game;
using KSP.Sim.Definitions;

namespace KerbalWebProgram.KerbalWebProgram
{
    public static class ApiEndpointsBuiltIn
    {
        public static void Init()
        {
            KerbalWebProgramMod.webAPI.Add("getCelestialBodyData", new getCelestialBodyData());
        }
    }
    internal class getCelestialBodyData : KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";

            CelestialBodyCore celestialBodyCore = GameManager.Instance.Game.CelestialBodies.Get(apiRequestData.paramters["name"].ToString());
            
            apiResponseData.Data.Add("bodyName", celestialBodyCore.data.bodyName);
            apiResponseData.Data.Add("assetKeyScaled", celestialBodyCore.data.assetKeyScaled);
            apiResponseData.Data.Add("assetKeySimulation", celestialBodyCore.data.assetKeySimulation);
            apiResponseData.Data.Add("bodyDisplayName", celestialBodyCore.data.bodyDisplayName);
            apiResponseData.Data.Add("bodyDescription", celestialBodyCore.data.bodyDescription);
            apiResponseData.Data.Add("isStar", celestialBodyCore.data.isStar);
            apiResponseData.Data.Add("isHomeWorld", celestialBodyCore.data.isHomeWorld);
            apiResponseData.Data.Add("navballSwitchAltitudeHigh", celestialBodyCore.data.navballSwitchAltitudeHigh);
            apiResponseData.Data.Add("navballSwitchAltitudeLow", celestialBodyCore.data.navballSwitchAltitudeLow);
            apiResponseData.Data.Add("hasSolidSurface", celestialBodyCore.data.hasSolidSurface);
            apiResponseData.Data.Add("hasOcean", celestialBodyCore.data.hasOcean);
            apiResponseData.Data.Add("HasLocalSpace", celestialBodyCore.data.HasLocalSpace);
            apiResponseData.Data.Add("radius", celestialBodyCore.data.radius);
            apiResponseData.Data.Add("gravityASL", celestialBodyCore.data.gravityASL);
            apiResponseData.Data.Add("oceanAltitude", celestialBodyCore.data.oceanAltitude);
            apiResponseData.Data.Add("oceanDensity", celestialBodyCore.data.oceanDensity);
            apiResponseData.Data.Add("MinTerrainHeight", celestialBodyCore.data.MinTerrainHeight);
            apiResponseData.Data.Add("MaxTerrainHeight", celestialBodyCore.data.MaxTerrainHeight);
            apiResponseData.Data.Add("TerrainHeightScale", celestialBodyCore.data.TerrainHeightScale);
            apiResponseData.Data.Add("TimeWarpAltitudeOffset", celestialBodyCore.data.TimeWarpAltitudeOffset);
            apiResponseData.Data.Add("SphereOfInfluenceCalculationType", celestialBodyCore.data.SphereOfInfluenceCalculationType);
            apiResponseData.Data.Add("hasSolarRotationPeriod", celestialBodyCore.data.hasSolarRotationPeriod);

            return new ApiResponseData();
        }
    }
}
