namespace Base.Locator
{
    public class Locator<T>
    {
        private static T instance;
        public static T Instance => instance;

        public static void SetInstance(T newInstance)
        {
            instance = newInstance;
        }
    }
}