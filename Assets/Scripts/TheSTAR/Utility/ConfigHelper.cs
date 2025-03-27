namespace TheSTAR.Utility
{    
    public class ConfigHelper<T> where T : UnityEngine.Object
    {
        private readonly ResourceHelper<T> resourceHelper;

        public ConfigHelper()
        {
            resourceHelper = new($"Configs/{typeof(T).Name}");
        }

        public T Get => resourceHelper.Get;
    }
}