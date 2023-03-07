using UnityEngine;
using SpaceWarp.API.Mods;
using System.Net;
using System.Text;
using KerbalWebProgram;
using KerbalWebProgram.KerbalWebProgram;
using Newtonsoft.Json;
using Shapes;
using Random = System.Random;
using System.Dynamic;
using KSP.OAB;
using BepInEx;
using BepInEx.Configuration;
using SpaceWarp;

namespace KerbalWebProgram
{
    public class pageJSON
    {
        public Dictionary<string, string> Pages { get; set; }
    }
    public class ApiRequestData
    {
        public string ID { get; set; }
        public string Action { get; set; }
        public Dictionary<string, dynamic> parameters  { get; set; }
    }
    public class ApiResponseData
    {
        public string Type { get; set; }
        public string ID { get; set; }
        public Dictionary<string, object> Data { get; set; } = null;
        public Dictionary<string, object> Errors { get; set; } = null;
    }
    public class KWPapiParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int? min { get; set; }
        public int? max { get; set; }
        public KWPapiParameter(string name, string description, string type, int? min = null, int? max = null)
        {
            this.Name = name;
            this.Description = description;
            this.Type = type;
            this.min = min;
            this.max = max;

        }
    }
    public abstract class KWPapi
    {
        public abstract List<KWPParameterType> parameters { get; set; }
        //Api parameters

        public abstract string Type { get; set; }
        //Api type

        public abstract string Name { get; set; }
        //Api name

        public abstract string Description { get; set; }
        //Api Description

        public abstract string Author { get; set; }
        //Api Author

        public abstract List<string> Tags { get; set; }
        //Api tags
        public abstract ApiResponseData Run(ApiRequestData request);
    }
    [BepInPlugin("kwp_dev_team.kerbal_web_program", "kerbal_web_program", "0.0.1")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class KerbalWebProgramMod : BaseSpaceWarpPlugin
    {
        private static KerbalWebProgramMod Instance { get; set; }
        private bool IsWebLoaded = false;

        public static Dictionary<string, KWPapi> webAPI = new Dictionary<string, KWPapi>();
        public static pageJSON PageJSON = new pageJSON();

        private ConfigEntry<int> port;

        public override void OnInitialized()
        {
            Instance = this;
            //init built in apis
            ApiEndpointsBuiltIn.Init();

            PageJSON.Pages = new Dictionary<string, string>();


            //init local standalone pages
            string jsonString = string.Empty;
            if (Directory.Exists("./KerbalWebProgram")){}
            else
            {
                Directory.CreateDirectory("./KerbalWebProgram");
            }
            if (Directory.Exists("./KerbalWebProgram/public")){}
            else
            {
                Directory.CreateDirectory("./KerbalWebProgram/public");
            }
            if (Directory.Exists("./KerbalWebProgram/public/assets")) { }
            else
            {
                Directory.CreateDirectory("./KerbalWebProgram/public/assets");
            }
            if (Directory.Exists("./KerbalWebProgram/public/assets/css")) { }
            else
            {
                Directory.CreateDirectory("./KerbalWebProgram/public/assets/css");
            }
            if (Directory.Exists("./KerbalWebProgram/public/assets/img")) { }
            else
            {
                Directory.CreateDirectory("./KerbalWebProgram/public/assets/img");
            }
            if (Directory.Exists("./KerbalWebProgram/public/assets/js")) { }
            else
            {
                Directory.CreateDirectory("./KerbalWebProgram/public/assets/js");
            }

            if (File.Exists("./KerbalWebProgram/public/pages.json"))
            {
                jsonString = File.ReadAllText("./KerbalWebProgram/public/pages.json");
                Debug.Log("Exists");
                PageJSON = JsonConvert.DeserializeObject<pageJSON>(jsonString);
            }
            else
            {
                Debug.Log("Downloading");
                WebClient wc = new WebClient();
                wc.DownloadFile("https://raw.githubusercontent.com/Bit-Studios/KerbalWebProgram/public/pages.json", "./KerbalWebProgram/public/tmppages.json");
                
                pageJSON tmpPageJSON = new pageJSON();
                tmpPageJSON.Pages = new Dictionary<string, string>();
                string tmpjsonString = File.ReadAllText("./KerbalWebProgram/public/tmppages.json");
                Debug.Log(tmpjsonString);
                tmpPageJSON = JsonConvert.DeserializeObject<pageJSON>(tmpjsonString);
                foreach (var jsonPage in tmpPageJSON.Pages)
                {
                    if (File.Exists($"./KerbalWebProgram/public/{jsonPage.Value}")) {
                        Debug.Log("Exists");
                    }
                    else
                    {
                        Debug.Log($"Getting required web file 'https://raw.githubusercontent.com/Bit-Studios/KerbalWebProgram/public/{jsonPage.Value}' ./KerbalWebProgram/public/{jsonPage.Value}");
                        wc.DownloadFile($"https://raw.githubusercontent.com/Bit-Studios/KerbalWebProgram/public/{jsonPage.Value}", $"./KerbalWebProgram/public/{jsonPage.Value}");
                    }
                    PageJSON.Pages.Add(jsonPage.Key, jsonPage.Value);
                }

                jsonString = JsonConvert.SerializeObject(PageJSON);
                File.WriteAllText("./KerbalWebProgram/public/pages.json", jsonString);
            }
            foreach (var jsonPage in PageJSON.Pages)
            {
                Debug.Log($"{jsonPage.Key} goes to {jsonPage.Value}");
            }

            Debug.Log(Directory.GetCurrentDirectory());

            Logger.LogInfo("Mod is initialized");
        }
        void Awake()
        {
            port = Config.Bind("Webserver", "Port", 8080, "The port that the internal webserver will run on");

        }
        void Update()
        {
            if (IsWebLoaded == false)
            {
                //init documentation
                if (Directory.Exists("./KerbalWebProgram/public/docs")) { }
                else
                {
                    Directory.CreateDirectory("./KerbalWebProgram/public/docs");
                }
                string docsPage = $"<html><head><link rel='stylesheet' href='/docs.css'></head><body>";
                Dictionary<string, string> apiTagType = new();
                int cT = 0;
                foreach (var apiData in webAPI)
                {
                    cT = (cT + 42) + ((int)DateTime.Now.Ticks / 2) ;
                    Debug.Log($"{apiData.Value.Name} api does {apiData.Value.Description}");
                    docsPage = $"{docsPage}<div class='doclink' onclick='document.location=`docs/{apiData.Key}`'><div class='doclinkname'>{apiData.Value.Name}</div><div class='doclinktagarea'>";
                    
                    foreach(var apiTag in apiData.Value.Tags)
                    {
                        if (apiTagType.ContainsKey(apiTag)){}
                        else
                        {
                            Random rnd = new Random((int)DateTime.Now.Ticks + cT);
                            string randomColor = $"{rnd.Next(0, 255)},{rnd.Next(0, 255)},{rnd.Next(0, 255)}";
                            apiTagType.Add(apiTag, randomColor);
                            Debug.Log($"new tag color {apiTagType[apiTag]}");
                        }
                        docsPage = $"{docsPage}<div class='doclinktag' style='background-color:rgba({apiTagType[apiTag]},0.4);border-color:rgb({apiTagType[apiTag]})'>{apiTag}</div>";
                    }
                    docsPage = $"{docsPage}</div></div>";
                    //generate api doc page
                    string apiPageTags = string.Empty;
                    foreach (var apiTag in apiData.Value.Tags)
                    {
                        apiPageTags = $"{apiPageTags}<div class='doclinktag' style='background-color:rgba({apiTagType[apiTag]},0.4);border-color:rgb({apiTagType[apiTag]})'>{apiTag}</div>";
                    }
                    string apiPage = @$"
<html>
    <head>
        <link rel='stylesheet' href='/docs.css'>
    </head>
    <body>
        <h1>{apiData.Value.Name} (by {apiData.Value.Author})</h1>
        <h3>{apiData.Key}</h3>
        <div class='doclinktagarea'>
            {apiPageTags}
        </div>
        <p>{apiData.Value.Description}</p>
    </body>
</html>
";
                    File.WriteAllText($"./KerbalWebProgram/public/docs/{apiData.Key}.html", apiPage);
                    PageJSON.Pages.Add($"/docs/{apiData.Key}", $"docs/{apiData.Key}.html");

                }
                docsPage = $"{docsPage}</body></html>";
                File.WriteAllText("./KerbalWebProgram/public/docs/docs.html", docsPage);
                PageJSON.Pages.Add("/docs", "docs/docs.html");
                //init webserver
                IsWebLoaded = true;
                WebServer webServer = new WebServer();
                webServer.Start();
            }
        }
        void OnGUI()
        {

        }
        
        internal class WebServer
        {
            public WebServer(){}

            internal async void Start()
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://*:8080/");
                listener.Start();
                Debug.Log("Listening");

                for (; ; )
                {
                    HttpListenerContext ctx = await listener.GetContextAsync();
                    new Thread(new WebWorker(ctx).ProcessRequest).Start();
                }
            }
        }
        
        
        static ApiResponseData ApiHandler(ApiRequestData apiRequestData)
        {
            try
            {
                KWPapi ApiEndpoint = webAPI[apiRequestData.Action];
                ApiResponseData apiResponseData = new ApiResponseData();
                Dictionary<string, object> errors = new Dictionary<string, object>();
                Dictionary<string, object> transformedParams = new Dictionary<string, object>();
                ApiEndpoint.parameters.ForEach(param =>
                {
                    var value = apiRequestData.parameters.ContainsKey(param.Name) ? apiRequestData.parameters[param.Name] : null;
                    try
                    {
                        param.validate(value);
                        transformedParams.Add(param.Name, param.transformValue(value));
                    } catch (ParameterValidationException ex)
                    {
                        errors.Add(param.Name, ex.Message);
                    }
                });
                if (errors.Keys.Count > 0)
                {
                    apiResponseData.ID = apiRequestData.ID;
                    apiResponseData.Type = "error";
                    apiResponseData.Errors = errors;
                }
                else
                {
                    apiRequestData.parameters = transformedParams;
                    apiResponseData = ApiEndpoint.Run(apiRequestData);
                }
                return apiResponseData;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return null;
        }
        internal class WebWorker
        {
            private HttpListenerContext ctx;

            public WebWorker(HttpListenerContext ctx)
            {
                this.ctx = ctx;
            }
            
            internal void ProcessRequest()
            {
                Stream body = ctx.Request.InputStream;
                Encoding encoding = ctx.Request.ContentEncoding;
                StreamReader reader = new StreamReader(body, encoding);
                string requestBody = reader.ReadToEnd();
                ctx.Response.SendChunked = false;
                ctx.Response.Headers.Add("Access-Control-Allow-Origin: *");
                ctx.Response.Headers.Add("Access-Control-Allow-Headers: *");

                using (var stream = ctx.Response.OutputStream)
                {
                    ApiRequestData data = new ApiRequestData();
                    string responseString = string.Empty;
                    if (ctx.Request.Url.AbsolutePath.StartsWith("/api") && ctx.Request.ContentType == "application/json")
                    {

                        Debug.Log("=================="); 
                        Debug.Log(requestBody);
                        ctx.Response.ContentType = "application/json";
                        data = JsonConvert.DeserializeObject<ApiRequestData>(requestBody);
                        Debug.Log("==================");
                        Debug.Log(data.ID);
                        Debug.Log(data.Action);
                        Debug.Log(string.Join(",", data.parameters));
                        ApiResponseData responseData = ApiHandler(data);
                        Debug.Log("==================");
                        Debug.Log(responseData.ID);
                        Debug.Log(responseData.Data);
                        Debug.Log(responseData.Type);
                        responseString = JsonConvert.SerializeObject(responseData);
                    }
                    else {
                        if(PageJSON.Pages.ContainsKey(ctx.Request.Url.AbsolutePath))
                        {
                            responseString = File.ReadAllText($"./KerbalWebProgram/public/{PageJSON.Pages[ctx.Request.Url.AbsolutePath]}");
                            switch (PageJSON.Pages[ctx.Request.Url.AbsolutePath].Split('.')[PageJSON.Pages[ctx.Request.Url.AbsolutePath].Split('.').Length - 1])
                            {
                                case "html":
                                    ctx.Response.ContentType = "text/html";
                                    break;
                                case "js":
                                    ctx.Response.ContentType = "text/javascript";
                                    break;
                                case "json":
                                    ctx.Response.ContentType = "application/json";
                                    break;
                                case "css":
                                    ctx.Response.ContentType = "text/css";
                                    break;
                                case "png":
                                    ctx.Response.ContentType = "image/png";
                                    break;
                                case "svg":
                                    ctx.Response.ContentType = "image/svg+xml";
                                    break;
                                case "jpg":
                                    ctx.Response.ContentType = "image/jpeg";
                                    break;
                                case "jepg":
                                    ctx.Response.ContentType = "image/jpeg";
                                    break;
                                case "gif":
                                    ctx.Response.ContentType = "image/gif";
                                    break;
                                default:
                                    ctx.Response.ContentType = "text/html";
                                    break;
                            }
                        }
                        else //404
                        {
                            ctx.Response.ContentType = "text/html";
                            responseString = File.ReadAllText($"./KerbalWebProgram/public/{PageJSON.Pages["/404"]}");
                        }
                        
                    }



                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Close();
                }

                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.StatusDescription = "OK";

            }

        }
    }

}
