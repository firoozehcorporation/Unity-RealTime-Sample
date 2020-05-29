using Newtonsoft.Json;

namespace Models
{
    public enum Action
    {
        Translate , Rotate
    }
    
    public class Vector3{
        [JsonProperty("x")]
        public float X;
        
        [JsonProperty("y")]
        public float Y;
        
        [JsonProperty("z")]
        public float Z;
    }
    
    public class Quaternion{
        [JsonProperty("x")]
        public float X;
        
        [JsonProperty("y")]
        public float Y;
        
        [JsonProperty("z")]
        public float Z;
        
        [JsonProperty("w")]
        public float W;
    }

    
    public class Data
    {
        [JsonProperty("1")]
        public Action Action;
        
        [JsonProperty("2")]
        public Vector3 Vector3;
        
        [JsonProperty("3")]
        public Quaternion Quaternion;
        
    }
}