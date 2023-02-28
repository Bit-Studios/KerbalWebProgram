using UnityEngine;
using SpaceWarp.API.Mods;
using System.Net;
using System.Text;
using KerbalWebProgram;
using KerbalWebProgram.KerbalWebProgram;
using Newtonsoft.Json;
using I2.Loc.SimpleJSON;

namespace KerbalWebProgram
{
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
    public abstract class KWPapi
    {
        public abstract ApiResponseData Run(ApiRequestData apiRequestData);
    }
    [MainMod]
    public class KerbalWebProgramMod : Mod
    {
        private bool IsWebLoaded = false;

        public static Dictionary<string, KWPapi> webAPI = new Dictionary<string, KWPapi>();

        public override void OnInitialized()
        {
            ApiEndpointsBuiltIn.Init();
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
                ApiResponseData apiResponseData = webAPI[apiRequestData.Action].Run(apiRequestData);
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
            internal class pageJSON
            {
                public Dictionary<string,string> Pages {  get; set; }
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

                        pageJSON jsonData = new pageJSON();
                        jsonData.Pages = new Dictionary<string, string>();
                        jsonData.Pages.Add("/", "index.html");
                        string jsonString = JsonConvert.SerializeObject(jsonData);
                        if (Directory.Exists("./KerbalWebProgram"))
                        {
                            if (Directory.Exists("./KerbalWebProgram/public"))
                            {
                                File.WriteAllText("./KerbalWebProgram/public/pages.json", jsonString);
                            }
                            else
                            {
                                Directory.CreateDirectory("./KerbalWebProgram/public");
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory("./KerbalWebProgram");
                        }


                        Debug.Log(Directory.GetCurrentDirectory());
                        //string jsonString = File.ReadAllText("./Frontend/Standalone/pages.json");
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
