using System;
using Newtonsoft.Json;

namespace Models
{
    [Serializable]
    public enum Action
    {
        Translate , Rotate
    }
    
    [Serializable]
    public class Vector3{
        [JsonProperty("x")]
        public float x;
        
        [JsonProperty("y")]
        public float y;
        
        [JsonProperty("z")]
        public float z;
    }
    
    [Serializable]
    public class Quaternion{
        [JsonProperty("x")]
        public float x;
        
        [JsonProperty("y")]
        public float y;
        
        [JsonProperty("z")]
        public float z;
        
        [JsonProperty("w")]
        public float w;
    }
    
    [Serializable]
    public class Data
    {
        [JsonProperty("1")]
        public Action action;
        
        [JsonProperty("2")]
        public Vector3 position;
        
        [JsonProperty("3")]
        public Quaternion rotation;
    }
}