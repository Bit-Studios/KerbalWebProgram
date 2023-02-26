using SpaceWarp.API.Configuration;
using Newtonsoft.Json;

namespace KerbalWebProgram
{
    [JsonObject(MemberSerialization.OptOut)]
    [ModConfig]
    public class KerbalWebProgramConfig
    {
         [ConfigField("pi")] [ConfigDefaultValue(3.14159)] public double pi;
    }
}