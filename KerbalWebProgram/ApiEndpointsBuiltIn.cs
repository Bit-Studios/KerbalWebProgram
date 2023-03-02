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
            KerbalWebProgramMod.webAPI.Add("serverPing", new getShipOrbit(
                new List<KWPapiParameter> { },
                "pong",
                "Is server alive",
                "This outputs the SessionGuidString",
                "KWP dev team",
                new List<string> { "Server" }
                ));
            KerbalWebProgramMod.webAPI.Add("getCelestialBodyData", new getCelestialBodyData(
                new List<KWPapiParameter> { new KWPapiParameter("name", "Name of the Celestial Body","String") },
                "response",
                "Get Celestial Body Data",
                "Allows you to get the Celestial Body Data of a specified Celestial Body",
                "KWP dev team",
                new List<string> { "CelestialBody" }
                ));

            KerbalWebProgramMod.webAPI.Add("getAllCelestialBodyData", new getALLCelestialBodyData(
                new List<KWPapiParameter> { },
                "response",
                "Get All Celestial Body Data",
                "Allows you to get the Celestial Body Data of all Celestial Bodies",
                "KWP dev team",
                new List<string> { "CelestialBody" }
                ));

            KerbalWebProgramMod.webAPI.Add("getShiptelemetry", new getShiptelemetry(
                new List<KWPapiParameter> {},
                "response",
                "Get Ship Telemetry",
                "Allows you to get the ships telemetry data",
                "KWP dev team",
                new List<string> { "Vessel" }
                ));

            KerbalWebProgramMod.webAPI.Add("setShipAutoPilotMode", new setShipAutoPilotMode(
                new List<KWPapiParameter> { new KWPapiParameter("Mode", "Auto pilot mode", "String") },
                "response",
                "Set Ship Auto Pilot Mode",
                "Allows you to set the mode of the ships Autopilot",
                "KWP dev team",
                new List<string> { "Vessel", "Control" }
                ));

            KerbalWebProgramMod.webAPI.Add("setShipThrottle", new setShipThrottle(
                new List<KWPapiParameter> { new KWPapiParameter("Throttle", "Throttle change amount. 0.1 will change the throttle by 0.1 evey game update", "Float") },
                "response",
                "Set Ship Throttle",
                "Allows you to update the throttle change, Equivalant of holding Shift or Ctrl",
                "KWP dev team",
                new List<string> { "Vessel", "Control" }
                ));

            KerbalWebProgramMod.webAPI.Add("getShipThrottle", new setShipThrottle(
                new List<KWPapiParameter> { },
                "response",
                "Get Ship Throttle",
                "Outputs the current Throttle position",
                "KWP dev team",
                new List<string> { "Vessel" }
                ));

            KerbalWebProgramMod.webAPI.Add("getStage", new getStage(
                new List<KWPapiParameter> {  },
                "response",
                "Get Stage",
                "Gets the current stage that you are on",
                "KWP dev team",
                new List<string> { "Vessel" }
                ));

            KerbalWebProgramMod.webAPI.Add("doStage", new doStage(
                new List<KWPapiParameter> { },
                "response",
                "Do Stage",
                "Executes the next stage action. AKA press space",
                "KWP dev team",
                new List<string> { "Vessel", "Control" }
                ));

            KerbalWebProgramMod.webAPI.Add("getCraftFile", new getCraftFile(
                new List<KWPapiParameter> { },
                "response",
                "Get Craft File",
                "This outputs your crafts parts in a json format",
                "KWP dev team",
                new List<string> { "Vessel" }
                ));

            KerbalWebProgramMod.webAPI.Add("getUniverseTime", new getUniverseTime(
                new List<KWPapiParameter> { },
                "response",
                "Gets the universe time",
                "This outputs the current universe time",
                "KWP dev team",
                new List<string> { "time" }
                ));
            KerbalWebProgramMod.webAPI.Add("getShipOrbit", new getShipOrbit(
                new List<KWPapiParameter> { },
                "response",
                "Gets the current orbit of the active vessel",
                "This outputs the current orbital data of the active vessel and what body it is currently orbiting",
                "KWP dev team",
                new List<string> { "Vessel", "CelestialBody", "Orbit" }
                ));
        }
    }
    public class serverPing : KWPapi
    {
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public serverPing(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
        }
        public override ApiResponseData Run(ApiRequestData apiRequestData)
        {
            ApiResponseData apiResponseData = new ApiResponseData();
            apiResponseData.ID = apiRequestData.ID;
            apiResponseData.Type = "pong";
            apiResponseData.Data = new Dictionary<string, object>();

            apiResponseData.Data.Add("pong", GameManager.Instance.Game.SessionGuidString);

            return apiResponseData;
        }
    }

    //Ship data

    public class getShiptelemetry : KWPapi
    {
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getShiptelemetry(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
        }
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
            apiResponseData.Data.Add("Heading", vesselComponent.Heading);
            apiResponseData.Data.Add("AltitudeFromTerrain", vesselComponent.AltitudeFromTerrain);
            apiResponseData.Data.Add("AltitudeFromSeaLevel", vesselComponent.AltitudeFromSeaLevel);
            apiResponseData.Data.Add("AltitudeFromRadius", vesselComponent.AltitudeFromRadius);

            return apiResponseData;
        }
    }

    public class setShipAutoPilotMode : KWPapi
    {
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public setShipAutoPilotMode(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
        }
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
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public setShipThrottle(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
        }
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
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getShipThrottle(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
        }
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
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getStage(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
        }
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
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public doStage(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
        }
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
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getCraftFile(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
        }
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
    public class getShipOrbit : KWPapi
    {
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getShipOrbit(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
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
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getUniverseTime(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
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
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getMissionTime(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
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
    public class getCelestialBodyData : KWPapi
    {
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getCelestialBodyData(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
        }

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
        public override List<KWPapiParameter> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getALLCelestialBodyData(List<KWPapiParameter> parameters, string type, string name, string description, string author, List<string> tags)
        {
            this.parameters = parameters;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Tags = tags;
        }
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
}
