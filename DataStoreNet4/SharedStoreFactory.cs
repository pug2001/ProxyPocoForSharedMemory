using DataStore;

namespace DataStoreNet4
{
    public static class SharedStoreFactory
    {
        public static T Create<T>() where T:class,new()
        {
            var store = new T();
            var sharedStore = new SharedStore<T>(store);
            return sharedStore.GetTransparentProxy() as T;
        }
    }
}
