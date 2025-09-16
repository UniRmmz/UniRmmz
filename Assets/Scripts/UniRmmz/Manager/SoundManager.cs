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

        public void LoadSystemSound(int n)
        {
            if (Rmmz.DataSystem != null)
            {
                Rmmz.AudioManager.LoadStaticSe(Rmmz.DataSystem.Sounds[n]);
            }
        }

        public void PlaySystemSound(int n)
        {
            if (Rmmz.DataSystem != null)
            {
                Rmmz.AudioManager.PlayStaticSe(Rmmz.DataSystem.Sounds[n]);
            }
        }

        public void PlayCursor() => PlaySystemSound(0);
        public void PlayOk() => PlaySystemSound(1);
        public void PlayCancel() => PlaySystemSound(2);
        public void PlayBuzzer() => PlaySystemSound(3);
        public void PlayEquip() => PlaySystemSound(4);
        public void PlaySave() => PlaySystemSound(5);
        public void PlayLoad() => PlaySystemSound(6);
        public void PlayBattleStart() => PlaySystemSound(7);
        public void PlayEscape() => PlaySystemSound(8);
        public void PlayEnemyAttack() => PlaySystemSound(9);
        public void PlayEnemyDamage() => PlaySystemSound(10);
        public void PlayEnemyCollapse() => PlaySystemSound(11);
        public void PlayBossCollapse1() => PlaySystemSound(12);
        public void PlayBossCollapse2() => PlaySystemSound(13);
        public void PlayActorDamage() => PlaySystemSound(14);
        public void PlayActorCollapse() => PlaySystemSound(15);
        public void PlayRecovery() => PlaySystemSound(16);
        public void PlayMiss() => PlaySystemSound(17);
        public void PlayEvasion() => PlaySystemSound(18);
        public void PlayMagicEvasion() => PlaySystemSound(19);
        public void PlayReflection() => PlaySystemSound(20);
        public void PlayShop() => PlaySystemSound(21);
        public void PlayUseItem() => PlaySystemSound(22);
        public void PlayUseSkill() => PlaySystemSound(23);
    }
}