namespace UniRmmz
{
    /// <summary>
    /// The scene class of the load screen.
    /// </summary>
    public partial class Scene_Load //: Scene_File
    {
        protected bool _loadSuccess = false;

        public override void Initialize()
        {
            base.Initialize();
            _loadSuccess = false;
        }

        public override void Terminate()
        {
            base.Terminate();
            if (_loadSuccess)
            {
                Rmmz.gameSystem.OnAfterLoad();
            }
        }

        protected override string Mode()
        {
            return "load";
        }

        protected override string HelpWindowText()
        {
            return Rmmz.TextManager.LoadMessage;
        }

        protected override int FirstSavefileId()
        {
            return Rmmz.DataManager.LatestSavefileId();
        }

        protected override void OnSavefileOk()
        {
            base.OnSavefileOk();
            int savefileId = SavefileId();
            if (IsSavefileEnabled(savefileId))
            {
                ExecuteLoad(savefileId);
            }
            else
            {
                OnLoadFailure();
            }
        }

        protected virtual void ExecuteLoad(int savefileId)
        {
            Rmmz.DataManager.LoadGame(savefileId, OnLoadSuccess, OnLoadFailure);
        }

        protected virtual void OnLoadSuccess()
        {
            Rmmz.SoundManager.PlayLoad();
            FadeOutAll();
            ReloadMapIfUpdated();
            Scene_Map.Goto();
            _loadSuccess = true;
        }

        protected virtual void OnLoadFailure()
        {
            Rmmz.SoundManager.PlayBuzzer();
            ActivateListWindow();
        }

        protected virtual void ReloadMapIfUpdated()
        {
            if (Rmmz.gameSystem.VersionId() != Rmmz.dataSystem.VersionId)
            {
                int mapId = Rmmz.gameMap.MapId();
                int x = Rmmz.gamePlayer.X;
                int y = Rmmz.gamePlayer.Y;
                int d = Rmmz.gamePlayer.Direction();
                Rmmz.gamePlayer.ReserveTransfer(mapId, x, y, d, 0);
                Rmmz.gamePlayer.RequestMapReload();
            }
        }
    }
}