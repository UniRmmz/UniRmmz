namespace UniRmmz
{
    /// <summary>
    /// The scene class of the save screen.
    /// </summary>
    public partial class Scene_Save //: Scene_File
    {
        protected override string Mode()
        {
            return "save";
        }

        protected override string HelpWindowText()
        {
            return Rmmz.TextManager.SaveMessage;
        }

        protected override int FirstSavefileId()
        {
            return Rmmz.gameSystem.SavefileId();
        }

        protected override void OnSavefileOk()
        {
            base.OnSavefileOk();
            int savefileId = SavefileId();
            if (IsSavefileEnabled(savefileId))
            {
                ExecuteSave(savefileId);
            }
            else
            {
                OnSaveFailure();
            }
        }

        protected virtual void ExecuteSave(int savefileId)
        {
            Rmmz.gameSystem.SetSavefileId(savefileId);
            Rmmz.gameSystem.OnBeforeSave();
            Rmmz.DataManager.SaveGame(savefileId, OnSaveSuccess, OnSaveFailure);
        }

        protected virtual void OnSaveSuccess()
        {
            Rmmz.SoundManager.PlaySave();
            PopScene();
        }

        protected virtual void OnSaveFailure()
        {
            Rmmz.SoundManager.PlayBuzzer();
            ActivateListWindow();
        }
    }

}