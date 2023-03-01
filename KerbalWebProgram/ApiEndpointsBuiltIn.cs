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
using static Mono.Security.X509.X520;

namespace KerbalWebProgram.KerbalWebProgram
{
    public static class ApiEndpointsBuiltIn
    {
        public static void Init()
        {
            KerbalWebProgramMod.webAPI.Add("getCelestialBodyData", new getCelestialBodyData());
            KerbalWebProgramMod.webAPI.Add("getAllCelestialBodyData", new getAllCelestialBodyData());
            KerbalWebProgramMod.webAPI.Add("getShiptelemetry", new getShiptelemetry());
            KerbalWebProgramMod.webAPI.Add("setShipAutoPilotMode", new setShipAutoPilotMode());
            KerbalWebProgramMod.webAPI.Add("setShipThrottle", new setShipThrottle());
            KerbalWebProgramMod.webAPI.Add("getShipThrottle", new getShipThrottle());
            KerbalWebProgramMod.webAPI.Add("getStage", new getStage());
            KerbalWebProgramMod.webAPI.Add("doStage", new doStage());
            KerbalWebProgramMod.webAPI.Add("getCraftFile", new getCraftFile());
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
            this.parameters = new List<KWPParameterType> {
                new StringParameter("name", "Name of the celestial body", true)
            };
            this.Type = "response";
            this.Name = "Get Celestial Body Data";
            this.Description = "Allows you to get the Celestial Body Data of a specified Celestial Body";
            this.Author = "KWP dev team";
            this.Tags = new List<string> { "CelestialBody" };
        }

        public override Dictionary<string, object> Run(dynamic parameters)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            CelestialBodyCore celestialBodyCore = GameManager.Instance.Game.CelestialBodies.Get(parameters["name"]);
            response.Add("body", celestialBodyCore.data);
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
            this.parameters = new List<KWPParameterType> { };
            this.Type = "response";
            this.Name = "Get All Celestial Body Data";                
            this.Description = "Allows you to get the Celestial Body Data of all Celestial Bodies";
            this.Author = "KWP dev team";                
            this.Tags = new List<string> { "CelestialBody" };
        }

        public override Dictionary<string, object> Run(dynamic parameters)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            var bodys = GameManager.Instance.Game.CelestialBodies.GetAllBodiesData();
            foreach (var body in bodys)
            {
                response.Add(body.Value.data.bodyName, body.Value.data);
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
            this.parameters = new List<KWPParameterType> { };
            this.Type = "response";
            this.Name = "Get Ship Telemetry";
            this.Description = "Allows you to get the ships telemetry data";
            this.Author = "KWP dev team";
            this.Tags = new List<string> { "Vessel" };
        }

        public override Dictionary<string, object> Run(dynamic parameters)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();

            VesselComponent vesselComponent = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimVessel();

            response.Add("HorizontalSrfSpeed", vesselComponent.HorizontalSrfSpeed);
            response.Add("VerticalSrfSpeed", vesselComponent.VerticalSrfSpeed);
            response.Add("OrbitalSpeed", vesselComponent.OrbitalSpeed);
            response.Add("TargetSpeed", vesselComponent.TargetSpeed);

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
            this.parameters = new List<KWPParameterType> { new StringChoicesParameter("Mode", "Auto pilot mode", true, new List<string>
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
            this.Type = "response";
            this.Name = "Set Ship Auto Pilot Mode";
            this.Description = "Allows you to set the mode of the ships Autopilot";
            this.Author = "KWP dev team";
            this.Tags = new List<string> { "Vessel", "Control" };
        }
        public override Dictionary<string, object> Run(dynamic parameters)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();

            VesselComponent vesselComponent = GameManager.Instance.Game.ViewController.GetActiveSimVessel();
            switch (parameters["Mode"])
            {
                case "Antinormal":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Antinormal);
                    response.Add("Mode", "Antinormal");
                    break;
                case "AntiTarget":
                    vesselComponent.Autopilot.Activate(AutopilotMode.AntiTarget);
                    response.Add("Mode", "AntiTarget");
                    break;
                case "Autopilot":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Autopilot);
                    response.Add("Mode", "Autopilot");
                    break;
                case "Maneuver":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Maneuver);
                    response.Add("Mode", "Maneuver");
                    break;
                case "Navigation":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Navigation);
                    response.Add("Mode", "Navigation");
                    break;
                case "Normal":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Normal);
                    response.Add("Mode", "Normal");
                    break;
                case "Prograde":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Prograde);
                    response.Add("Mode", "Prograde");
                    break;
                case "RadialIn":
                    vesselComponent.Autopilot.Activate(AutopilotMode.RadialIn);
                    response.Add("Mode", "RadialIn");
                    break;
                case "RadialOut":
                    vesselComponent.Autopilot.Activate(AutopilotMode.RadialOut);
                    response.Add("Mode", "RadialOut");
                    break;
                case "Retrograde":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Retrograde);
                    response.Add("Mode", "Retrograde");
                    break;
                case "StabilityAssist":
                    vesselComponent.Autopilot.Activate(AutopilotMode.StabilityAssist);
                    response.Add("Mode", "StabilityAssist");
                    break;
                case "Target":
                    vesselComponent.Autopilot.Activate(AutopilotMode.Target);
                    response.Add("Mode", "Normal");
                    break;
                default:
                    response.Add("Mode", "Invalid mode");
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
            this.parameters = new List<KWPParameterType> { new FloatParameter("Throttle", "Throttle change amount. 0.1 will change the throttle by 0.1 evey game update", true, 0f, 1f) };
            this.Type = "response";
            this.Name = "Set Ship Throttle";
            this.Description = "Allows you to update the throttle change, Equivalant of holding Shift or Ctrl";
            this.Author = "KWP dev team";
            this.Tags = new List<string> { "Vessel", "Control" };
        }
        public override Dictionary<string, object> Run(dynamic parameters)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();

            GameManager.Instance.Game.ViewController.flightInputHandler.OverrideInputThrottle(parameters["Throttle"]);

            response.Add("Throttle", GameManager.Instance.Game.ViewController.GetActiveVehicle().FlightControlInput.mainThrottle);

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
            this.parameters = new List<KWPParameterType> { };
            this.Type = "response";
            this.Name = "Get Ship Throttle";
            this.Description = "Outputs the current Throttle position";
            this.Author = "KWP dev team";
            this.Tags = new List<string> { "Vessel" };
        }
        public override Dictionary<string, object> Run(dynamic parameters)
        {
            Dictionary<string, object> response =  new Dictionary<string, object>
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
            this.parameters = new List<KWPParameterType> { };
            this.Type = "response";
            this.Name = "Get Stage";
            this.Description = "Gets the current stage that you are on";
            this.Author = "KWP dev team";
            this.Tags = new List<string> { "Vessel" };
        }
        public override Dictionary<string, object> Run(dynamic parameters)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();

            response.Add("Stages", GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().Staging.AvailableStages.Count);

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
            this.parameters = new List<KWPParameterType> { };
            this.Type = "response";
            this.Name = "Do Stage";
            this.Description = "Executes the next stage action. AKA press space";
            this.Author = "KWP dev team";
            this.Tags = new List<string> { "Vessel", "Control" };
        }
        public override Dictionary<string, object> Run(dynamic parameters)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();

            GameManager.Instance.Game.ViewController.GetActiveSimVessel().ActivateNextStage();
            response.Add("Stages", GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().Staging.AvailableStages.Count - 1);

            return response;
        }
    }
    public class getCraftFile: KWPapi
    {
        public override List<KWPParameterType> parameters { get; set; }

        public override string Type { get; set; }

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Author { get; set; }

        public override List<string> Tags { get; set; }
        public getCraftFile()
        {
            this.parameters = new List<KWPParameterType> { };
            this.Type = "response";
            this.Name = "Get Craft File";
            this.Description = "This outputs your crafts parts in a json format";
            this.Author = "KWP dev team";
            this.Tags = new List<string> { "Vessel" };
        }
        public override Dictionary<string, object> Run(dynamic parameters)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();

            var parts = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts;
            List<PartData> partsData = new List<PartData>();
            foreach ( var part in parts )
            {
                partsData.Add(part.PartData);
            }
            response.Add("Parts", partsData);

            return response;
        }
    }
}
