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
using System.Text.RegularExpressions;


namespace KerbalWebProgram
{
    [MainMod]
    public class KerbalWebProgramMod : Mod
    {
        private bool IsWebLoaded = false;

        public Dictionary<string,string> webAPI = new Dictionary<string,string>();
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
                    WebSocketServer webSocketServer = new WebSocketServer();
                    webSocketServer.start();
                }
            }
        }
        void OnGUI()
        {

        }
        internal class WebSocketServer
        {
            List<TcpClient> clients = new List<TcpClient>();
            public WebSocketServer() { }
            internal async void start()
            {

                TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);
                server.Start();
                Debug.Log("Listening");

                for (; ; )
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    clients.Add(client);
                    new Thread(new WebSocketWorker(client).ProcessRequest).Start();
                }
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
                Debug.Log("Listening");

                for (; ; )
                {
                    HttpListenerContext ctx = await listener.GetContextAsync();
                    new Thread(new WebWorker(ctx).ProcessRequest).Start();
                }
            }
        }
        internal class WebSocketWorker
        {
            TcpClient client { get; set; }
            

            public WebSocketWorker(TcpClient client)
            {
                this.client = client;
            }

            internal void ProcessRequest()
            {
                NetworkStream stream = client.GetStream();
                while (true)
                {
                    
                    while (!stream.DataAvailable) ;
                    while (client.Available < 3) ;


                    byte[] bytes = new byte[client.Available];
                    stream.Read(bytes, 0, client.Available);
                    string clientData = Encoding.UTF8.GetString(bytes);


                    if (Regex.IsMatch(clientData, "^GET", RegexOptions.IgnoreCase))
                    {
                        string swk = Regex.Match(clientData, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                        string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                        byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                        string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);
                        byte[] response = Encoding.UTF8.GetBytes(
                       "HTTP/1.1 101 Switching Protocols\r\n" +
                       "Connection: Upgrade\r\n" +
                       "Upgrade: websocket\r\n" +
                       "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n");

                        stream.Write(response, 0, response.Length);
                    }
                    else
                    {
                        bool fin = (bytes[0] & 0b10000000) != 0,
                        mask = (bytes[1] & 0b10000000) != 0;

                        int opcode = bytes[0] & 0b00001111,
                        offset = 2;

                        ulong msglen = (ulong)(bytes[1] & 0b01111111);

                        byte[] decoded = new byte[msglen];
                        byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                        offset += 4;

                        for (ulong i = 0; i < msglen; ++i)
                            decoded[i] = (byte)(bytes[offset + (int)i] ^ masks[i % 4]);

                        string ClientOutputData = Encoding.UTF8.GetString(decoded);
                        ApiHandler(client, ClientOutputData, stream);
                    }
                }

            }
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
        public class ApiRequestData
        {
            public string ID { get; set; }
            public string Action { get; set; }
            public Dictionary<string, dynamic> paramters { get; set; }
        }
        public class ApiResponseData
        {
            public string Type { get; set; }
            public string ID { get; set; }
            public Dictionary<string, dynamic> Data { get; set; }
        }
        static byte[] GenerateBytes(string data)
        {
            byte[] response = Encoding.UTF8.GetBytes(data);
            return response;
        }
        static void ApiHandler(TcpClient client, string ClientOutputData, NetworkStream stream)
        {
            Debug.Log("got to api");
            stream = client.GetStream();
            
            byte[] response = GenerateBytes("aaaaaaaaaaaaa");
            Debug.Log("sending");
            stream.Write(response,0, response.Length);
        }

    }

}
