using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for editing an actor's name on the name input screen.
    /// </summary>
   public partial class Window_NameEdit : Window_StatusBase
   {
       protected Game_Actor _actor;
       protected int _maxLength;
       protected string _name;
       protected string _defaultName;

       public override void Initialize(Rect rect)
       {
           base.Initialize(rect);
           _actor = null;
           _maxLength = 0;
           _name = "";
           _index = 0;
           _defaultName = "";
           Deactivate();
       }

       public virtual void Setup(Game_Actor actor, int maxLength)
       {
           _actor = actor;
           _maxLength = maxLength;
           _name = actor.Name().Substring(0, Mathf.Min(actor.Name().Length, _maxLength));
           _index = _name.Length;
           _defaultName = _name;
           Rmmz.ImageManager.LoadFace(actor.FaceName());
       }

       public virtual string Name()
       {
           return _name;
       }

       public virtual bool RestoreDefault()
       {
           _name = _defaultName;
           _index = _name.Length;
           Refresh();
           return _name.Length > 0;
       }

       public virtual bool Add(string ch)
       {
           if (_index < _maxLength)
           {
               _name += ch;
               _index++;
               Refresh();
               return true;
           }
           else
           {
               return false;
           }
       }

       public virtual bool Back()
       {
           if (_index > 0)
           {
               _index--;
               _name = _name.Substring(0, _index);
               Refresh();
               return true;
           }
           else
           {
               return false;
           }
       }

       protected virtual float FaceWidth()
       {
           return 144f;
       }

       protected virtual float CharWidth()
       {
           string text = Rmmz.gameSystem.IsJapanese() ? "Ａ" : "A";
           return TextWidth(text);
       }

       protected virtual float Left()
       {
           float nameCenter = (InnerWidth + FaceWidth()) / 2f;
           float nameWidth = (_maxLength + 1) * CharWidth();
           return Mathf.Min(nameCenter - nameWidth / 2f, InnerWidth - nameWidth);
       }

       protected override Rect ItemRect(int index)
       {
           float x = Left() + index * CharWidth();
           float y = 54f;
           float width = CharWidth();
           float height = LineHeight();
           return new Rect(x, y, width, height);
       }

       protected virtual Rect UnderlineRect(int index)
       {
           Rect rect = ItemRect(index);
           rect.x += 1f;
           rect.y += rect.height - 4f;
           rect.width -= 2f;
           rect.height = 2f;
           return rect;
       }

       protected virtual Color UnderlineColor()
       {
           return Rmmz.ColorManager.NormalColor();
       }

       protected virtual void DrawUnderline(int index)
       {
           Rect rect = UnderlineRect(index);
           Color color = UnderlineColor();
           Contents.PaintOpacity = 48;
           Contents.FillRect(rect.x, rect.y, rect.width, rect.height, color);
           Contents.PaintOpacity = 255;
       }

       protected virtual void DrawChar(int index)
       {
           Rect rect = ItemRect(index);
           ResetTextColor();
           var character = _name.ElementAtOrDefault(index).ToString();
           DrawText(character, (int)rect.x, (int)rect.y);
       }

       public override void Refresh()
       {
           Contents.Clear();
           DrawActorFace(_actor, 0, 0);
           
           for (int i = 0; i < _maxLength; i++)
           {
               DrawUnderline(i);
           }
           
           for (int j = 0; j < _name.Length; j++)
           {
               DrawChar(j);
           }
           
           Rect rect = ItemRect(_index);
           SetCursorRect(rect.x, rect.y, rect.width, rect.height);
       }
   }
}