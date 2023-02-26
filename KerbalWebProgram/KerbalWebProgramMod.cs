using UnityEngine;
using KSP.Game;
using System.Collections.Generic;
using System.Linq;
using SpaceWarp.API.Mods;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static VehiclePhysics.TelemetryTemplateBase;


namespace KerbalWebProgram
{
    [MainMod]
    public class KerbalWebProgramMod : Mod
    {
        private bool IsWebLoaded = false;
        public override void OnInitialized()
        {
            
            Logger.Info("Mod is initialized");
        }
        void Awake()
        {
            

        }
        void Update()
        {
            if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.W) ){
                if (IsWebLoaded == false)
                {
                    IsWebLoaded = true;
                    WebServer webServer = new WebServer();
                    webServer.Start();
                }
            }
        }
        void OnGUI()
        {

        }
    }
    internal class WebServer
    {

        public WebServer()
        {

        }

        internal async void Start()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            listener.Start();
            Console.WriteLine("Listening...");

            for (; ; )
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                new Thread(new Worker(ctx).ProcessRequest).Start();
            }
        }
    }
    internal class Worker
    {
        private HttpListenerContext ctx;

        public Worker(HttpListenerContext ctx)
        {
            this.ctx = ctx;
        }

        internal void ProcessRequest()
        {
            ctx.Response.SendChunked = false;
            ctx.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Html;
            
            using (var stream = ctx.Response.OutputStream)
            {
                string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                stream.Write(buffer, 0, buffer.Length);
                stream.Close();
            }

            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
            ctx.Response.StatusDescription = "OK";

        }
    }
}