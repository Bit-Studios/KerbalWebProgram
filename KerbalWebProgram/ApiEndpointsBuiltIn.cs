using System;
using System.Collections.Generic;
using System.Text;
using KerbalWebProgram;
using KSP;
using KSP.Game;
using KSP.Sim;
using KSP.Sim.Definitions;
using KSP.Sim.impl;
using Newtonsoft.Json;
using UnityEngine;

namespace KerbalWebProgram.KerbalWebProgram
{
    public static class ApiEndpointsBuiltIn
    {
        public static void Init()
        {
            KerbalWebProgramMod.webAPI.Add("getCelestialBodyData", new getCelestialBodyData());
            KerbalWebProgramMod.webAPI.Add("getAllCelestialBodyData", new getALLCelestialBodyData());
            KerbalWebProgramMod.webAPI.Add("getShiptelemetry", new getShiptelemetry());
        }
    }

    //get Celestial Body Data
    public class getCelestialBodyData : KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();
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
            return apiResponseData;
        }
    }
    public class getALLCelestialBodyData : KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();
            var bodys = GameManager.Instance.Game.CelestialBodies.GetAllBodiesData();
            foreach (var body in bodys)
            {
                apiResponseData.Data.Add(body.Value.data.bodyName, JsonConvert.SerializeObject(body.Value.data));
            }
            return apiResponseData;
        }
    }


    //Ship data

    public class getShiptelemetry : KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();

            VesselComponent vesselComponent = GameManager.Instance.Game.ViewController.GetActiveSimVessel();

            apiResponseData.Data.Add(vesselComponent.DisplayName, JsonConvert.SerializeObject(vesselComponent));

            return apiResponseData;
        }
    }

    public class setShipAutoPilotMode: KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();

            VesselComponent vesselComponent = GameManager.Instance.Game.ViewController.GetActiveSimVessel();
            switch (apiRequestData.paramters["Mode"])
            {
                case "Antinormal":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Antinormal);
                    apiResponseData.Data.Add("Mode", "Antinormal");
                    break;
                case "AntiTarget":
                    vesselComponent.Autopilot.Activate(AutopilotMode.AntiTarget);
                    apiResponseData.Data.Add("Mode", "AntiTarget");
                    break;
                case "Autopilot":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Autopilot);
                    apiResponseData.Data.Add("Mode", "Autopilot");
                    break;
                case "Maneuver":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Maneuver);
                    apiResponseData.Data.Add("Mode", "Maneuver");
                    break;
                case "Navigation":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Navigation);
                    apiResponseData.Data.Add("Mode", "Navigation");
                    break;
                case "Normal":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Normal);
                    apiResponseData.Data.Add("Mode", "Normal");
                    break;
                case "Prograde":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Prograde);
                    apiResponseData.Data.Add("Mode", "Prograde");
                    break;
                case "RadialIn":
                    vesselComponent.Autopilot.Activate(AutopilotMode.RadialIn);
                    apiResponseData.Data.Add("Mode", "RadialIn");
                    break;
                case "RadialOut":
                    vesselComponent.Autopilot.Activate(AutopilotMode.RadialOut);
                    apiResponseData.Data.Add("Mode", "RadialOut");
                    break;
                case "Retrograde":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Retrograde);
                    apiResponseData.Data.Add("Mode", "Retrograde");
                    break;
                case "StabilityAssist":
                    vesselComponent.Autopilot.Activate(AutopilotMode.StabilityAssist);
                    apiResponseData.Data.Add("Mode", "StabilityAssist");
                    break;
                case "Target":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Target);
                    apiResponseData.Data.Add("Mode", "Normal");
                    break;
                default:
                    apiResponseData.Data.Add("Mode", "Invalid mode");
                    break;
            }
            

            return apiResponseData;
        }
    }
}
