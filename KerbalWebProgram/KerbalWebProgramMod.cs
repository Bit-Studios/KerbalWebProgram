using UnityEngine;
using SpaceWarp.API.Mods;
using System.Net;
using System.Text;
using KerbalWebProgram;
using KerbalWebProgram.KerbalWebProgram;
using Newtonsoft.Json;
using Shapes;

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
        public Dictionary<string, string> parameters  { get; set; }
    }
    public class ApiResponseData
    {
        public string Type { get; set; }
        public string ID { get; set; }
        public Dictionary<string, object> Data { get; set; }
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
        public abstract List<KWPapiParameter> parameters { get; set; }
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
        public abstract ApiResponseData Run(ApiRequestData apiRequestData);
    }
    [MainMod]
    public class KerbalWebProgramMod : Mod
    {
        private bool IsWebLoaded = false;

        public static Dictionary<string, KWPapi> webAPI = new Dictionary<string, KWPapi>();
        public static pageJSON PageJSON = new pageJSON();

        public override void OnInitialized()
        {
            ApiEndpointsBuiltIn.Init();

            PageJSON.Pages = new Dictionary<string, string>();



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
                PageJSON = JsonConvert.DeserializeObject<pageJSON>(jsonString);
            }
            else
            {
                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        wc.DownloadFile("https://raw.githubusercontent.com/Bit-Studios/KerbalWebProgram/public/pages.json", "./KerbalWebProgram/public/tmppages.json");
                        pageJSON tmpPageJSON = new pageJSON();
                        tmpPageJSON.Pages = new Dictionary<string, string>();
                        string tmpjsonString = File.ReadAllText("./KerbalWebProgram/public/tmppages.json");
                        tmpPageJSON = JsonConvert.DeserializeObject<pageJSON>(tmpjsonString);
                        foreach (var jsonPage in PageJSON.Pages)
                        {
                            if (File.Exists($"./KerbalWebProgram/public/{jsonPage.Value}")) { }
                            else
                            {
                                wc.DownloadFile($"https://raw.githubusercontent.com/Bit-Studios/KerbalWebProgram/public/{jsonPage.Value}", $"./KerbalWebProgram/public/{jsonPage.Value}");
                            }
                            PageJSON.Pages.Add(jsonPage.Key, jsonPage.Value);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }

                jsonString = JsonConvert.SerializeObject(PageJSON);
                File.WriteAllText("./KerbalWebProgram/public/pages.json", jsonString);
            }
            foreach (var jsonPage in PageJSON.Pages)
            {
                Debug.Log($"{jsonPage.Key} goes to {jsonPage.Value}");
            }

            Debug.Log(Directory.GetCurrentDirectory());

            Logger.Info("Mod is initialized");
        }
        void Awake()
        {
            

        }
        void Update()
        {
            if (IsWebLoaded == false)
            {
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
                apiResponseData.Data = new Dictionary<string, object>();
                bool hasError = false;
                ApiEndpoint.parameters.ForEach(param =>
                {
                    if (apiRequestData.parameters.Keys.Contains(param.Name)){}
                    else
                    {
                        apiResponseData.Data.Add("error", $"Missing parameters, Required {ApiEndpoint.parameters.ToArray()}");
                        hasError = true;
                        return;
                    }
                    if(param.min == null ||  param.max == null) { } else
                    {
                        if(apiRequestData.parameters[param.Name].Length < param.min || apiRequestData.parameters[param.Name].Length > param.max)
                        {
                            apiResponseData.Data.Add("error", $"parameter {param.Name} has a minimum length of {param.min} and a maximum length of {param.max}");
                            hasError = true;
                            return;
                        }
                        
                    }
                });
                if (hasError)
                {
                    apiResponseData.Data = new Dictionary<string, object>();
                    apiResponseData.Type = "error";
                }
                else
                {
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
