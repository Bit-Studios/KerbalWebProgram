using UnityEngine;
using System.Net;
using System.Text;
using KSP.Game;
using Newtonsoft.Json;
using Random = System.Random;
using BepInEx;
using System.Reflection;
using ShadowUtilityLIB;
using Logger = ShadowUtilityLIB.logging.Logger;
using KerbalWebProgram.UI;

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
    [BepInPlugin(KWPmod.ModId, KWPmod.ModName, KWPmod.ModVersion)]
    [BepInDependency(ShadowUtilityLIBMod.ModId, ShadowUtilityLIBMod.ModVersion)]
    [BepInDependency(UitkForKsp2.MyPluginInfo.PLUGIN_GUID, UitkForKsp2.MyPluginInfo.PLUGIN_VERSION)]
    public sealed class KerbalWebProgramMod : BaseUnityPlugin
    {
        public static string ModId = KWPmod.ModId;
        public static string ModName = KWPmod.ModName;
        public static string ModVersion = KWPmod.ModVersion;

        void Awake()
        {
            KWPmod.Awake();
        }
        //void OnDestroy()
        //{
        //    KWPmod.Destroy();
        //}
        void Update()
        {
            KWPmod.Update();
        }
    }
    
    public static class KWPmod
    {
        public static Dictionary<string, Assembly> APIdll = new Dictionary<string, Assembly>();
        public static Dictionary<string, Type> APIdllType = new Dictionary<string, Type>();
        private static string LocationFile = Assembly.GetExecutingAssembly().Location;
        private static string LocationDirectory = Path.GetDirectoryName(LocationFile);
        private static KerbalWebProgramMod Instance { get; set; }
        private static bool IsWebLoaded = false;
        
        public static Dictionary<string, KWPapi> webAPI = new Dictionary<string, KWPapi>();
        public static pageJSON PageJSON = new pageJSON();
        public const string ModId = "kwp_dev_team.kerbal_web_program";
        public const string ModName = "kerbal web program";
        public const string ModVersion = "0.2.0";
        private static Logger logger = new Logger(ModName, ModVersion);

        private static int port;
        private static bool Initialized = false;

        public static List<Browser> browsers = new List<Browser>();

        public static bool FirstTimeLoad = false;

        public static void Awake()
        {


            try
            {

                PageJSON.Pages = new Dictionary<string, string>();


                //init local standalone pages
                string jsonString = string.Empty;
                if (Directory.Exists($"{LocationDirectory}/Server")) { }
                else
                {
                    Directory.CreateDirectory($"{LocationDirectory}/Server");
                }
                if (Directory.Exists($"{LocationDirectory}/Server/apis")) { }
                else
                {
                    Directory.CreateDirectory($"{LocationDirectory}/Server/apis");
                }
                //dynamic load apis

                //uncompiled .cs apis
                string[] uncompiledCsFiles = Directory.GetFiles($"{LocationDirectory}/Server/apis", "*.cs");
                foreach (string fileData in uncompiledCsFiles)
                {

                }

                //compiled .dll apis
                string[] compiledCsFiles = Directory.GetFiles($"{LocationDirectory}/Server/apis", "*.dll");
                foreach (string file in compiledCsFiles)
                {
                    try
                    {
                        Debug.Log($"loading {file}");
                        Assembly apiDll = Assembly.LoadFile(file);
                        Debug.Log($"{file} loaded as {apiDll.GetName()}");
                        APIdll.Add(apiDll.GetName().FullName, apiDll);
                        Type apiType = APIdll[apiDll.GetName().FullName].GetType($"{APIdll[apiDll.GetName().FullName].ExportedTypes.ElementAt(0)}");
                        Debug.Log(apiType.FullName);
                        APIdllType.Add(apiType.FullName, apiType);
                        APIdllType[apiType.FullName].InvokeMember("init", BindingFlags.InvokeMethod, null, null, null);
                        Debug.Log("invoked");
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }


                }

                if (Directory.Exists($"{LocationDirectory}/Server/public")) { }
                else
                {
                    Directory.CreateDirectory($"{LocationDirectory}/Server/public");
                }
                if (Directory.Exists($"{LocationDirectory}/Server/public/assets")) { }
                else
                {
                    Directory.CreateDirectory($"{LocationDirectory}/Server/public/assets");
                }
                if (Directory.Exists($"{LocationDirectory}/Server/public/assets/css")) { }
                else
                {
                    Directory.CreateDirectory($"{LocationDirectory}/Server/public/assets/css");
                }
                if (Directory.Exists($"{LocationDirectory}/Server/public/assets/img")) { }
                else
                {
                    Directory.CreateDirectory($"{LocationDirectory}/Server/public/assets/img");
                }
                if (Directory.Exists($"{LocationDirectory}/Server/public/assets/js")) { }
                else
                {
                    Directory.CreateDirectory($"{LocationDirectory}/Server/public/assets/js");
                }

                if (File.Exists($"{LocationDirectory}/Server/public/pages.json"))
                {
                    jsonString = File.ReadAllText($"{LocationDirectory}/Server/public/pages.json");
                    Debug.Log("Exists");
                    PageJSON = JsonConvert.DeserializeObject<pageJSON>(jsonString);
                }
                else
                {
                    pageJSON tmpPageJSON = new pageJSON();
                    jsonString = JsonConvert.SerializeObject(PageJSON);
                    File.WriteAllText($"{LocationDirectory}/Server/public/pages.json", jsonString);
                    /* V0.2.0
                    Debug.Log("Downloading");
                    WebClient wc = new WebClient();
                    wc.DownloadFile("https://raw.githubusercontent.com/Bit-Studios/KerbalWebProgram/public/pages.json", $"{LocationDirectory}/Server/public/tmppages.json");

                    pageJSON tmpPageJSON = new pageJSON();
                    tmpPageJSON.Pages = new Dictionary<string, string>();
                    string tmpjsonString = File.ReadAllText($"{LocationDirectory}/Server/public/tmppages.json");
                    Debug.Log(tmpjsonString);
                    tmpPageJSON = JsonConvert.DeserializeObject<pageJSON>(tmpjsonString);
                    foreach (var jsonPage in tmpPageJSON.Pages)
                    {
                        if (File.Exists($"{LocationDirectory}/Server/public/{jsonPage.Value}")) {
                            Debug.Log("Exists");
                        }
                        else
                        {
                            Debug.Log($"Getting required web file 'https://raw.githubusercontent.com/Bit-Studios/KerbalWebProgram/public/{jsonPage.Value}' ./Server/public/{jsonPage.Value}");
                            wc.DownloadFile($"https://raw.githubusercontent.com/Bit-Studios/KerbalWebProgram/public/{jsonPage.Value}", $"{LocationDirectory}/Server/public/{jsonPage.Value}");
                        }
                        PageJSON.Pages.Add(jsonPage.Key, jsonPage.Value);
                    }

                    jsonString = JsonConvert.SerializeObject(PageJSON);
                    File.WriteAllText($"{LocationDirectory}/Server/public/pages.json", jsonString);
                    */
                }
                foreach (var jsonPage in PageJSON.Pages)
                {
                    Debug.Log($"{jsonPage.Key} goes to {jsonPage.Value}");
                }

                Debug.Log(Directory.GetCurrentDirectory());
                Initialized = true;
                logger.Log("Mod is initialized");
                port = 8080;

                WebUpdate();
            }
            catch (Exception e)
            {
                logger.Error($"{e}\n{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}\n{e.GetBaseException()}");
            }

        }
        //public static void Destroy()
        //{
        //    browsers.ForEach(browserLoaded => browserLoaded.Close());
        //}
        public static void AddBrowser(string URL, string name, int width, int height, int x,int y)
        {
            browsers.Add(new Browser(name, URL, width, height, x, y));
        }
        public static void RemoveBrowser(string name)
        {
            Browser remBrowser = null;
            browsers.Where((browser) =>
            {
                if (browser.Title == name)
                {
                    remBrowser = browser;
                    browser.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            });
            browsers.Remove(remBrowser);
        }
        public static void Update()
        {
            try
            {
                //if (GameManager.Instance.Game.GlobalGameState.GetState() == GameState.MainMenu && FirstTimeLoad == false)
                //{
                //    FirstTimeLoad = true;
                //    //AddBrowser("https://google.com", "KWPloaded", Screen.width / 2, Screen.height / 2, Screen.width / 4, Screen.height / 4);
                //}

            }
            catch (Exception e)
            {
                //logger.Error($"{e}\n{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}\n{e.GetBaseException()}");
            }
        }
        static void WebUpdate()
        {
            try {
                if (IsWebLoaded == false && Initialized == true)
                {
                    //init documentation
                    if (Directory.Exists($"{LocationDirectory}/Server/public/docs")) { }
                    else
                    {
                        Directory.CreateDirectory($"{LocationDirectory}/Server/public/docs");
                    }
                    string docsPage = $"<html><head><link rel='stylesheet' href='/docs.css'></head><body>";
                    Dictionary<string, string> apiTagType = new Dictionary<string, string>();

                    int cT = 0;

                    foreach (var apiData in webAPI)
                    {
                        Debug.Log($"key:{apiData.Key}");

                        cT = (cT + 42) + ((int)DateTime.Now.Ticks / 2);

                        Debug.Log($"{apiData.Value.Name} api does {apiData.Value.Description}");

                        docsPage = $"{docsPage}<div class='doclink' onclick='document.location=`docs/{apiData.Key}`'><div class='doclinkname'>{apiData.Value.Name}</div><div class='doclinktagarea'>";

                        foreach (var apiTag in apiData.Value.Tags)
                        {

                            if (apiTagType.ContainsKey(apiTag)) { }
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
                        string apiParams = string.Empty;
                        string apiPramList = string.Empty;

                        foreach (var apipram in apiData.Value.parameters)
                        {
                            apiPramList = $@"{apiPramList}<h5>{apipram.Name}</h5>";
                            if (apipram.GetType() == typeof(StringChoicesParameter))
                            {
                                apiPramList = $@"{apiPramList}<ul>";
                                StringChoicesParameter apipramsc = (StringChoicesParameter)apipram;
                                List<string> choices = apipramsc.choices;
                                choices.ForEach(c =>
                                {
                                    apiPramList = $@"{apiPramList}<li>{c}</li>";
                                });
                                apiPramList = $@"{apiPramList}</ul>";
                            }
                            apiParams = $@"{apiParams}""{apipram.Name}"":""{apipram.Description}"",";
                        }
                        if(apiParams.Length > 1)
                        {
                            apiParams = apiParams.Remove(apiParams.Length - 1, 1);
                        }
                        
                        


                        string apiPage = @$"
<html>
    <head>
        <link rel='stylesheet' href='/docs.css'>
        <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.7.0/styles/tomorrow-night-bright.min.css"" integrity=""sha512-kihsljiamrbQ3b3s3TXoAWNSbzbp+gYIeeva81nQwCj/zICdiT/QnKbWTV7DElmAm3mc4vuTR3fo0ToTe2cpNw=="" crossorigin=""anonymous"" referrerpolicy=""no-referrer"" />
        <script src=""https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.7.0/highlight.min.js"" integrity=""sha512-bgHRAiTjGrzHzLyKOnpFvaEpGzJet3z4tZnXGjpsCcqOnAH6VGUx9frc5bcIhKTVLEiCO6vEhNAgx5jtLUYrfA=="" crossorigin=""anonymous"" referrerpolicy=""no-referrer""></script>
    </head>
    <body>
        <h1>{apiData.Value.Name} (by {apiData.Value.Author})</h1>
        <h3>{apiData.Key}</h3>
        <div class='doclinktagarea'>
            {apiPageTags}
        </div>
        <h4>Api Use:</h4>
        <pre style=""background-color: #282828;width:calc(100% - 40px);border-radius:5px;""><code style=""background-color: #282828;"" language='javascript'>
        var data = JSON.stringify(
            {{""ID"":""User Provided ID"",
              ""Action"":""{apiData.Key}"",
              ""parameters"":{{
                    {apiParams}
              }}
            }});
        var xhr = new XMLHttpRequest();

        xhr.addEventListener(""readystatechange"", function() {{
            if(this.readyState === 4) {{
                console.log(this.responseText);
            }}
        }});

        xhr.open(""Post"", ""http://localhost:8080/api"");
        xhr.setRequestHeader(""Content-Type"",""application/json"");
        xhr.send(data);
        </code></pre>
        <h4>Parameters</h4>
        {apiPramList}
<script>hljs.highlightAll();</script>
    </body>
</html>
";

                        File.WriteAllText($"{LocationDirectory}/Server/public/docs/{apiData.Key}.html", apiPage);

                        PageJSON.Pages.Add($"/docs/{apiData.Key}", $"docs/{apiData.Key}.html");


                    }
                    docsPage = $"{docsPage}</body></html>";

                    File.WriteAllText($"{LocationDirectory}/Server/public/docs/docs.html", docsPage);

                    PageJSON.Pages.Add("/docs", "docs/docs.html");

                    //init webserver
                    IsWebLoaded = true;

                    WebServer webServer = new WebServer();

                    webServer.Start();
                    

                }
            }
            catch (Exception e) { Debug.Log($"{e.Message}|{e.InnerException}|{e.StackTrace}|{e.Source}|{e.Data}|{e.TargetSite}"); }
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
                    if(apiResponseData == null)
                    {
                        apiResponseData.ID = apiRequestData.ID;
                        apiResponseData.Type = "error";
                        apiResponseData.Errors = new Dictionary<string, object> { { "Empty","Object Empty"} };
                    }
                }
                return apiResponseData;
            }
            catch (Exception e)
            {

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
                        try
                        {
                            ctx.Response.ContentType = "application/json";
                            data = JsonConvert.DeserializeObject<ApiRequestData>(requestBody);      
                            ApiResponseData responseData = ApiHandler(data);
                            responseString = JsonConvert.SerializeObject(responseData);
                        }
                        catch (Exception e)
                        {

                        }
                        
                    }
                    else {
                        if (PageJSON.Pages.ContainsKey(ctx.Request.Url.AbsolutePath))
                        {
                            responseString = File.ReadAllText($"{LocationDirectory}/Server/public/{PageJSON.Pages[ctx.Request.Url.AbsolutePath]}");
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
                                case "jpeg":
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
                            responseString = File.ReadAllText($"{LocationDirectory}/Server/public/{PageJSON.Pages["/404"]}");
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
