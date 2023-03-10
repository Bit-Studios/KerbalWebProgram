using System;
using System.Collections.Generic;
using System.Text;
using Castle.Components.DictionaryAdapter.Xml;
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
using KerbalWebProgram;
using KerbalWebProgram.KerbalWebProgram;

namespace ApiEndpoints
{
    public static class ApiEndpoint
    {
        public static void init()
        {
            KerbalWebProgramMod.webAPI.Add("doStage", new doStage());
            KerbalWebProgramMod.webAPI.Add("getAllCelestialBodyData", new getAllCelestialBodyData());
            KerbalWebProgramMod.webAPI.Add("getCelestialBodyData", new getCelestialBodyData());
            KerbalWebProgramMod.webAPI.Add("getCraftFile", new getCraftFile());
            KerbalWebProgramMod.webAPI.Add("getMissionTime", new getMissionTime());
            KerbalWebProgramMod.webAPI.Add("getShipOrbit", new getShipOrbit());
            KerbalWebProgramMod.webAPI.Add("getShiptelemetry", new getShiptelemetry());
            KerbalWebProgramMod.webAPI.Add("getShipThrottle", new getShipThrottle());
            KerbalWebProgramMod.webAPI.Add("getStage", new getStage());
            KerbalWebProgramMod.webAPI.Add("getUniverseTime", new getUniverseTime());
            KerbalWebProgramMod.webAPI.Add("setShipAutoPilotMode", new setShipAutoPilotMode());
            KerbalWebProgramMod.webAPI.Add("setShipThrottle", new setShipThrottle());
        }
    }

    //get Celestial Body Data
    public class getCelestialBodyData : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getCelestialBodyData()
        {
            parameters = new List<KWPParameterType> {
                new StringParameter("name", "Name of the celestial body", true)
            };
            Type = "response";
            Name = "Get Celestial Body Data";
            Description = "Allows you to get the Celestial Body Data of a specified Celestial Body";
            Author = "KWP dev team";
            Tags = new List<string> { "CelestialBody" };
        }

        public override ApiResponseData Run(ApiRequestData request)
        {
            ApiResponseData response = new ApiResponseData();
            response.ID = request.ID;
            response.Type = "response";
            response.Data = new Dictionary<string, object>();
            CelestialBodyCore celestialBodyCore = GameManager.Instance.Game.CelestialBodies.Get(request.parameters["name"]);
            response.Data.Add("body", celestialBodyCore.data);
            return response;
        }
    }
    public class getAllCelestialBodyData : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getAllCelestialBodyData()
        {
            parameters = new List<KWPParameterType> { };
            Type = "response";
            Name = "Get All Celestial Body Data";
            Description = "Allows you to get the Celestial Body Data of all Celestial Bodies";
            Author = "KWP dev team";
            Tags = new List<string> { "CelestialBody" };
        }

        public override ApiResponseData Run(ApiRequestData request)
        {
            ApiResponseData response = new ApiResponseData();
            response.ID = request.ID;
            response.Type = "response";
            response.Data = new Dictionary<string, object>();
            var bodys = GameManager.Instance.Game.CelestialBodies.GetAllBodiesData();
            foreach (var body in bodys)
            {
                response.Data.Add(body.Value.data.bodyName, body.Value.data);
            }
            return response;
        }
    }


    //Ship data

    public class getShiptelemetry : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }

        public getShiptelemetry()
        {
            parameters = new List<KWPParameterType> { };
            Type = "response";
            Name = "Get Ship Telemetry";
            Description = "Allows you to get the ships telemetry data";
            Author = "KWP dev team";
            Tags = new List<string> { "Vessel" };
        }

        public override ApiResponseData Run(ApiRequestData request)
        {
            ApiResponseData response = new ApiResponseData();
            response.ID = request.ID;
            response.Type = "response";
            response.Data = new Dictionary<string, object>();

            VesselComponent vesselComponent = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimVessel();

            response.Data.Add("HorizontalSrfSpeed", vesselComponent.HorizontalSrfSpeed);
            response.Data.Add("VerticalSrfSpeed", vesselComponent.VerticalSrfSpeed);
            response.Data.Add("OrbitalSpeed", vesselComponent.OrbitalSpeed);
            response.Data.Add("TargetSpeed", vesselComponent.TargetSpeed);
            response.Data.Add("Heading", vesselComponent.Heading);
            response.Data.Add("AltitudeFromTerrain", vesselComponent.AltitudeFromTerrain);
            response.Data.Add("AltitudeFromSeaLevel", vesselComponent.AltitudeFromSeaLevel);
            response.Data.Add("AltitudeFromRadius", vesselComponent.AltitudeFromRadius);

            return response;
        }
    }

    public class setShipAutoPilotMode : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }

        public setShipAutoPilotMode()
        {
            parameters = new List<KWPParameterType> { new StringChoicesParameter("Mode", "Auto pilot mode", true, new List<string>
            {
                "Antinormal",
                "AntiTarget",
                "Autopilot",
                "Maneuver",
                "Navigation",
                "Normal",
                "Prograde",
                "RadialIn",
                "RadialOut",
                "Retrograde",
                "StabilityAssist",
                "Target"
            }) };
            Type = "response";
            Name = "Set Ship Auto Pilot Mode";
            Description = "Allows you to set the mode of the ships Autopilot";
            Author = "KWP dev team";
            Tags = new List<string> { "Vessel", "Control" };
        }
        public override ApiResponseData Run(ApiRequestData request)
        {
            ApiResponseData response = new ApiResponseData();
            response.ID = request.ID;
            response.Type = "response";
            response.Data = new Dictionary<string, object>();

            VesselComponent vesselComponent = GameManager.Instance.Game.ViewController.GetActiveSimVessel();
            switch (request.parameters["Mode"])
            {
                case "Antinormal":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Antinormal);
                    response.Data.Add("Mode", "Antinormal");
                    break;
                case "AntiTarget":
                    vesselComponent.Autopilot.Activate(AutopilotMode.AntiTarget);
                    response.Data.Add("Mode", "AntiTarget");
                    break;
                case "Autopilot":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Autopilot);
                    response.Data.Add("Mode", "Autopilot");
                    break;
                case "Maneuver":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Maneuver);
                    response.Data.Add("Mode", "Maneuver");
                    break;
                case "Navigation":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Navigation);
                    response.Data.Add("Mode", "Navigation");
                    break;
                case "Normal":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Normal);
                    response.Data.Add("Mode", "Normal");
                    break;
                case "Prograde":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Prograde);
                    response.Data.Add("Mode", "Prograde");
                    break;
                case "RadialIn":
                    vesselComponent.Autopilot.Activate(AutopilotMode.RadialIn);
                    response.Data.Add("Mode", "RadialIn");
                    break;
                case "RadialOut":
                    vesselComponent.Autopilot.Activate(AutopilotMode.RadialOut);
                    response.Data.Add("Mode", "RadialOut");
                    break;
                case "Retrograde":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Retrograde);
                    response.Data.Add("Mode", "Retrograde");
                    break;
                case "StabilityAssist":
                    vesselComponent.Autopilot.Activate(AutopilotMode.StabilityAssist);
                    response.Data.Add("Mode", "StabilityAssist");
                    break;
                case "Target":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Target);
                    response.Data.Add("Mode", "Normal");
                    break;
                default:
                    response.Data.Add("Mode", "Invalid mode");
                    break;
            }


            return response;
        }
    }
    public class setShipThrottle : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }


        public setShipThrottle()
        {
            parameters = new List<KWPParameterType> { new FloatParameter("Throttle", "Throttle change amount. 0.1 will change the throttle by 0.1 evey game update", true, 0f, 1f) };
            Type = "response";
            Name = "Set Ship Throttle";
            Description = "Allows you to update the throttle change, Equivalant of holding Shift or Ctrl";
            Author = "KWP dev team";
            Tags = new List<string> { "Vessel", "Control" };
        }
        public override ApiResponseData Run(ApiRequestData request)
        {
            ApiResponseData response = new ApiResponseData();
            response.ID = request.ID;
            response.Type = "response";
            response.Data = new Dictionary<string, object>();

            GameManager.Instance.Game.ViewController.TryGetActiveVehicle(out var vessel);
            var thisvessel = vessel as VesselVehicle;

            thisvessel.AtomicSet((float)request.parameters["Throttle"], null, null, null, null, null, null, null, null, null, null, null, null,
        null, null, null, null, null);
            response.Data.Add("Throttle", GameManager.Instance.Game.ViewController.GetActiveVehicle().FlightControlInput.mainThrottle);

            return response;
        }
    }

    public class getShipThrottle : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getShipThrottle()
        {
            parameters = new List<KWPParameterType> { };
            Type = "response";
            Name = "Get Ship Throttle";
            Description = "Outputs the current Throttle position";
            Author = "KWP dev team";
            Tags = new List<string> { "Vessel" };
        }
        public override ApiResponseData Run(ApiRequestData request)
        {
            ApiResponseData response = new ApiResponseData();
            response.ID = request.ID;
            response.Type = "response";
            response.Data = new Dictionary<string, object>
            {
                { "Throttle", GameManager.Instance.Game.ViewController.GetActiveVehicle().FlightControlInput.mainThrottle }
            };

            return response;
        }
    }

    public class getStage : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getStage()
        {
            parameters = new List<KWPParameterType> { };
            Type = "response";
            Name = "Get Stage";
            Description = "Gets the current stage that you are on";
            Author = "KWP dev team";
            Tags = new List<string> { "Vessel" };
        }
        public override ApiResponseData Run(ApiRequestData request)
        {
            ApiResponseData response = new ApiResponseData();
            response.ID = request.ID;
            response.Type = "response";
            response.Data = new Dictionary<string, object>();

            response.Data.Add("Stages", GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().Staging.AvailableStages.Count);

            return response;
        }
    }

    public class doStage : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public doStage()
        {
            parameters = new List<KWPParameterType> { };
            Type = "response";
            Name = "Do Stage";
            Description = "Executes the next stage action. AKA press space";
            Author = "KWP dev team";
            Tags = new List<string> { "Vessel", "Control" };
        }
        public override ApiResponseData Run(ApiRequestData request)
        {
            ApiResponseData response = new ApiResponseData();
            response.ID = request.ID;
            response.Type = "response";
            response.Data = new Dictionary<string, object>();

            GameManager.Instance.Game.ViewController.GetActiveSimVessel().ActivateNextStage();
            response.Data.Add("Stages", GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().Staging.AvailableStages.Count - 1);

            return response;
        }
    }
    public class getCraftFile : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getCraftFile()
        {
            parameters = new List<KWPParameterType> { };
            Type = "response";
            Name = "Get Craft File";
            Description = "This outputs your crafts parts in a json format";
            Author = "KWP dev team";
            Tags = new List<string> { "Vessel" };
        }
        public override ApiResponseData Run(ApiRequestData request)
        {
            ApiResponseData response = new ApiResponseData();
            response.ID = request.ID;
            response.Type = "response";
            response.Data = new Dictionary<string, object>();

            var parts = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts;
            List<PartData> partsData = new List<PartData>();
            foreach (var part in parts)
            {
                partsData.Add(part.PartData);
            }
            response.Data.Add("Parts", partsData);

            return response;
        }
    }
    public class getShipOrbit : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getShipOrbit()
        {
            parameters = new List<KWPParameterType> { };
            Type = "response";
            Name = "Get the current orbit of the active vessel";
            Description = "This outputs the current orbital data of the active vessel and what body it is currently orbiting";
            Author = "KWP dev team";
            Tags = new List<string> { "Vessel", "CelestialBody", "Orbit" };
        }
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();

            VesselComponent vesselComponent = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimVessel();

            apiResponseData.Data.Add("OrbitalSpeed", vesselComponent.OrbitalSpeed);
            apiResponseData.Data.Add("OrbitalVelocity", vesselComponent.OrbitalVelocity);
            apiResponseData.Data.Add("altitude", vesselComponent.Orbit.altitude);
            apiResponseData.Data.Add("an", vesselComponent.Orbit.an);
            apiResponseData.Data.Add("collisionPointUT", vesselComponent.Orbit.collisionPointUT);
            apiResponseData.Data.Add("Apoapsis", vesselComponent.Orbit.Apoapsis);
            apiResponseData.Data.Add("Periapsis", vesselComponent.Orbit.Periapsis);
            apiResponseData.Data.Add("Velocity", vesselComponent.Orbit.Velocity);
            apiResponseData.Data.Add("eccentricity", vesselComponent.Orbit.eccentricity);
            apiResponseData.Data.Add("inclination", vesselComponent.Orbit.inclination);
            apiResponseData.Data.Add("Apoapsis", vesselComponent.Orbit.period);
            apiResponseData.Data.Add("radius", vesselComponent.Orbit.radius);
            apiResponseData.Data.Add("body", vesselComponent.Orbit.referenceBody.Name);

            return apiResponseData;
        }
    }
    //Universe Data
    public class getUniverseTime : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getUniverseTime()
        {
            parameters = new List<KWPParameterType> { };
            Type = "response";
            Name = "Get universe time";
            Description = "This outputs the current universe time";
            Author = "KWP dev team";
            Tags = new List<string> { "time" };
        }
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();

            apiResponseData.Data.Add("time", GameManager.Instance.Game.ViewController.universalTime);

            return apiResponseData;
        }
    }
    public class getMissionTime : KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getMissionTime()
        {
            parameters = new List<KWPParameterType> { };
            Type = "response";
            Name = "Get mission time";
            Description = "This outputs the current mission time of the active vessel";
            Author = "KWP dev team";
            Tags = new List<string> { "time" };
        }
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "response";
            apiResponseData.Data = new Dictionary<string, object>();

            apiResponseData.Data.Add("time", GameManager.Instance.Game.ViewController);

            return apiResponseData;
        }
    }
}
