using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
   /// <summary>
   /// The window for displaying the status of party members on the battle screen.
   /// </summary>
   public partial class Window_BattleStatus : Window_StatusBase
   {
       protected int _bitmapsReady;

       public override void Initialize(Rect rect)
       {
           base.Initialize(rect);
           FrameVisible = false;
           Openness = 0;
           _bitmapsReady = 0;
           PreparePartyRefresh();
       }

       protected virtual int ExtraHeight()
       {
           return 10;
       }

       protected override int MaxCols()
       {
           return 4;
       }

       public override int ItemHeight()
       {
           return InnerHeight;
       }

       protected override int MaxItems()
       {
           return Rmmz.gameParty.BattleMembers().Count();
       }

       protected override int RowSpacing()
       {
           return 0;
       }

       protected override void UpdatePadding()
       {
           Padding = 8;
       }

       public virtual Game_Actor Actor(int index)
       {
           var members = Rmmz.gameParty.BattleMembers();
           return members.ElementAtOrDefault(index);
       }

       public virtual void SelectActor(Game_Actor actor)
       {
           var members = Rmmz.gameParty.BattleMembers().ToList();
           Select(members.IndexOf(actor));
       }

       public override void UpdateRmmz()
       {
           base.UpdateRmmz();
           if (Rmmz.gameTemp.IsBattleRefreshRequested())
           {
               PreparePartyRefresh();
           }
       }

       protected virtual void PreparePartyRefresh()
       {
           Rmmz.gameTemp.ClearBattleRefreshRequest();
           _bitmapsReady = 0;
           foreach (Game_Actor actor in Rmmz.gameParty.Members())
           {
               var bitmap = Rmmz.ImageManager.LoadFace(actor.FaceName());
               bitmap.AddLoadListener((Bitmap _) => PerformPartyRefresh());
           }
       }

       protected virtual void PerformPartyRefresh()
       {
           _bitmapsReady++;
           if (_bitmapsReady >= Rmmz.gameParty.Members().Count())
           {
               Refresh();
           }
       }

       public override void DrawItem(int index)
       {
           DrawItemImage(index);
           DrawItemStatus(index);
       }

       protected virtual void DrawItemImage(int index)
       {
           var actor = Actor(index);
           var rect = FaceRect(index);
           DrawActorFace(actor, (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
       }

       protected virtual void DrawItemStatus(int index)
       {
           var actor = Actor(index);
           var rect = ItemRectWithPadding(index);
           int nameX = NameX(rect);
           int nameY = NameY(rect);
           int stateIconX = StateIconX(rect);
           int stateIconY = StateIconY(rect);
           int basicGaugesX = BasicGaugesX(rect);
           int basicGaugesY = BasicGaugesY(rect);
           PlaceTimeGauge(actor, nameX, nameY);
           PlaceActorName(actor, nameX, nameY);
           PlaceStateIcon(actor, stateIconX, stateIconY);
           PlaceBasicGauges(actor, basicGaugesX, basicGaugesY);
       }

       protected virtual Rect FaceRect(int index)
       {
           var rect = ItemRect(index);
           rect.Pad(-1, -1);
           rect.height = NameY(rect) + GaugeLineHeight() / 2 - rect.y;
           return rect;
       }

       protected virtual int NameX(Rect rect)
       {
           return Mathf.RoundToInt(rect.x);
       }

       protected virtual int NameY(Rect rect)
       {
           return BasicGaugesY(rect) - GaugeLineHeight();
       }

       protected virtual int StateIconX(Rect rect)
       {
           return Mathf.RoundToInt(rect.x + rect.width - Rmmz.ImageManager.IconWidth / 2 + 4);
       }

       protected virtual int StateIconY(Rect rect)
       {
           return Mathf.RoundToInt(rect.y + Rmmz.ImageManager.IconHeight / 2 + 4);
       }

       protected virtual int BasicGaugesX(Rect rect)
       {
           return Mathf.RoundToInt(rect.x);
       }

       protected virtual int BasicGaugesY(Rect rect)
       {
           int bottom = Mathf.RoundToInt(rect.y + rect.height - ExtraHeight());
           int numGauges = Rmmz.DataSystem.OptDisplayTp ? 3 : 2;
           return bottom - GaugeLineHeight() * numGauges;
       }
   }
}