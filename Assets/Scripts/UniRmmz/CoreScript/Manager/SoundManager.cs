namespace UniRmmz
{
    /// <summary>
    /// The static class that plays sound effects defined in the database.
    /// </summary>
    public partial class SoundManager
    {
        public void PreloadImportantSounds()
        {
            LoadSystemSound(0);
            LoadSystemSound(1);
            LoadSystemSound(2);
            LoadSystemSound(3);
        }

        public virtual void LoadSystemSound(int n)
        {
            if (Rmmz.dataSystem != null)
            {
                Rmmz.AudioManager.LoadStaticSe(Rmmz.dataSystem.Sounds[n]);
            }
        }

        public virtual void PlaySystemSound(int n)
        {
            if (Rmmz.dataSystem != null)
            {
                Rmmz.AudioManager.PlayStaticSe(Rmmz.dataSystem.Sounds[n]);
            }
        }

        public virtual void PlayCursor() => PlaySystemSound(0);
        public virtual void PlayOk() => PlaySystemSound(1);
        public virtual void PlayCancel() => PlaySystemSound(2);
        public virtual void PlayBuzzer() => PlaySystemSound(3);
        public virtual void PlayEquip() => PlaySystemSound(4);
        public virtual void PlaySave() => PlaySystemSound(5);
        public virtual void PlayLoad() => PlaySystemSound(6);
        public virtual void PlayBattleStart() => PlaySystemSound(7);
        public virtual void PlayEscape() => PlaySystemSound(8);
        public virtual void PlayEnemyAttack() => PlaySystemSound(9);
        public virtual void PlayEnemyDamage() => PlaySystemSound(10);
        public virtual void PlayEnemyCollapse() => PlaySystemSound(11);
        public virtual void PlayBossCollapse1() => PlaySystemSound(12);
        public virtual void PlayBossCollapse2() => PlaySystemSound(13);
        public virtual void PlayActorDamage() => PlaySystemSound(14);
        public virtual void PlayActorCollapse() => PlaySystemSound(15);
        public virtual void PlayRecovery() => PlaySystemSound(16);
        public virtual void PlayMiss() => PlaySystemSound(17);
        public virtual void PlayEvasion() => PlaySystemSound(18);
        public virtual void PlayMagicEvasion() => PlaySystemSound(19);
        public virtual void PlayReflection() => PlaySystemSound(20);
        public virtual void PlayShop() => PlaySystemSound(21);
        public virtual void PlayUseItem() => PlaySystemSound(22);
        public virtual void PlayUseSkill() => PlaySystemSound(23);
    }
}