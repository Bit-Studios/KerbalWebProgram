using SpaceWarp.API.Configuration;
using Newtonsoft.Json;

namespace KerbalWebProgram
{
    [JsonObject(MemberSerialization.OptOut)]
    [ModConfig]
    public class KerbalWebProgramConfig
    {
         [ConfigField("port")] [ConfigDefaultValue(8080)] public int port;
    }
}