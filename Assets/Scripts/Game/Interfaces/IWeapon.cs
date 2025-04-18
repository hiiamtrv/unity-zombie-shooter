namespace Game.Interfaces
{
    public interface IWeapon
    {
        public void TryAttack();
        public void TryReload();

        public string WeaponName { get; }
        public int BulletsInMag { get; }
        public int BulletsLeft { get; }
        public bool InfiniteBullet { get; }
    }
}