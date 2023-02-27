using System;
using System.Collections.Generic;
using System.Text;
using Castle.Components.DictionaryAdapter.Xml;
using KerbalWebProgram;
using KSP;
using KSP.Game;
using KSP.Iteration.UI.Binding;
using KSP.Sim;
using KSP.Sim.Definitions;
using KSP.Sim.impl;
using KSP.Sim.State;
using Newtonsoft.Json;
using Shapes;
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
            KerbalWebProgramMod.webAPI.Add("setShipAutoPilotMode", new setShipAutoPilotMode());
            KerbalWebProgramMod.webAPI.Add("setShipThrottle", new setShipThrottle());
            KerbalWebProgramMod.webAPI.Add("getShipThrottle", new setShipThrottle());
            KerbalWebProgramMod.webAPI.Add("getStage", new getStage());
            KerbalWebProgramMod.webAPI.Add("doStage", new doStage());
            KerbalWebProgramMod.webAPI.Add("getCraftFile", new getCraftFile());
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
            CelestialBodyCore celestialBodyCore = GameManager.Instance.Game.CelestialBodies.Get(apiRequestData.parameters["name"].ToString());
            apiResponseData.Data.Add("body", celestialBodyCore.data);
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
                apiResponseData.Data.Add(body.Value.data.bodyName, body.Value.data);
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

            VesselComponent vesselComponent = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimVessel();
            
            apiResponseData.Data.Add("HorizontalSrfSpeed", vesselComponent.HorizontalSrfSpeed);
            apiResponseData.Data.Add("VerticalSrfSpeed", vesselComponent.VerticalSrfSpeed);
            apiResponseData.Data.Add("OrbitalSpeed", vesselComponent.OrbitalSpeed);
            apiResponseData.Data.Add("TargetSpeed", vesselComponent.TargetSpeed);

            return apiResponseData;
        }
    }

    public class setShipAutoPilotMode : KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();

            VesselComponent vesselComponent = GameManager.Instance.Game.ViewController.GetActiveSimVessel();
            switch (apiRequestData.parameters["Mode"])
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
    public class setShipThrottle : KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();

            GameManager.Instance.Game.ViewController.flightInputHandler.OverrideInputThrottle(float.Parse(apiRequestData.parameters["Throttle"]));

            apiResponseData.Data.Add("Throttle", GameManager.Instance.Game.ViewController.GetActiveVehicle().FlightControlInput.mainThrottle);

            return apiResponseData;
        }
    }

    public class getShipThrottle : KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>
            {
                { "Throttle", GameManager.Instance.Game.ViewController.GetActiveVehicle().FlightControlInput.mainThrottle }
            };

            return apiResponseData;
        }
    }

    public class getStage : KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();
            apiResponseData.Data.Add("Stages", GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().Staging.AvailableStages.Count);

            return apiResponseData;
        }
    }

    public class doStage : KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();

            GameManager.Instance.Game.ViewController.GetActiveSimVessel().ActivateNextStage();
            apiResponseData.Data.Add("Stages", GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().Staging.AvailableStages.Count - 1);

            return apiResponseData;
        }
    }
    public class getCraftFile: KWPapi
    {
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();
            var parts = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts;
            List<PartData> partsData = new List<PartData>();
            foreach ( var part in parts )
            {
                partsData.Add(part.PartData);
            }
            apiResponseData.Data.Add("Parts", partsData);

            return apiResponseData;
        }
    }
}
