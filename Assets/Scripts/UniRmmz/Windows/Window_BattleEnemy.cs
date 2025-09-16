using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
   /// <summary>
   /// The window for selecting a target enemy on the battle screen.
   /// </summary>
   public partial class Window_BattleEnemy : Window_Selectable
   {
       protected List<Game_Enemy> _enemies;

       public override void Initialize(Rect rect)
       {
           _enemies = new List<Game_Enemy>();
           base.Initialize(rect);
           Refresh();
           Hide();
       }

       protected override int MaxCols()
       {
           return 2;
       }

       protected override int MaxItems()
       {
           return _enemies.Count;
       }

       public virtual Game_Enemy Enemy()
       {
           return Index() >= 0 && Index() < _enemies.Count ? _enemies[Index()] : null;
       }

       public virtual int EnemyIndex()
       {
           var enemy = Enemy();
           return enemy != null ? enemy.Index() : -1;
       }

       public override void DrawItem(int index)
       {
           ResetTextColor();
           string name = _enemies[index].Name();
           var rect = ItemLineRect(index);
           DrawText(name, (int)rect.x, (int)rect.y, (int)rect.width);
       }

       public override void Show()
       {
           Refresh();
           ForceSelect(0);
           Rmmz.gameTemp.ClearTouchState();
           base.Show();
       }

       public override void Hide()
       {
           base.Hide();
           Rmmz.gameTroop.Select(null);
       }

       public override void Refresh()
       {
           _enemies = Rmmz.gameTroop.AliveMembers().Cast<Game_Enemy>().ToList();
           base.Refresh();
       }

       public override void Select(int index)
       {
           base.Select(index);
           Rmmz.gameTroop.Select(Enemy());
       }

       protected override void ProcessTouch()
       {
           base.ProcessTouch();
           if (IsOpenAndActive())
           {
               var target = Rmmz.gameTemp.TouchTarget();
               if (target != null)
               {
                   if (_enemies.Contains(target as Game_Enemy))
                   {
                       Select(_enemies.IndexOf(target as Game_Enemy));
                       if (Rmmz.gameTemp.TouchState() == "click")
                       {
                           ProcessOk();
                       }
                   }
                   Rmmz.gameTemp.ClearTouchState();
               }
           }
       }
   }
}