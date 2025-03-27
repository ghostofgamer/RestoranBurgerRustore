using UnityEngine;

namespace TheSTAR.Utility
{
    public class ResourceHelper<T> where T : UnityEngine.Object
    {
        private readonly string path;

        public ResourceHelper(string path) => this.path = path;

        private bool loaded = false;

        private T element;
        public T Get
        {
            get
            {
                if (!loaded)
                {
                    element = Resources.Load<T>(path);
                    loaded = true;
                }

                return element;
            }
        }
    }
}