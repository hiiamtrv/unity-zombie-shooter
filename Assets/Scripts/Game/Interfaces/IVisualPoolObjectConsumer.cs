using Game.SkinPool;

namespace Game.Interfaces
{
    public interface IVisualPoolObjectConsumer
    {
        public void LoadVisualPoolObject(VisualPoolObject visual);
        public void UnloadVisualPoolObject();
    }
}