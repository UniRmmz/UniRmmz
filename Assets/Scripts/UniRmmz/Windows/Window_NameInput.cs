using UnityEngine;

namespace UniRmmz
{
   public partial class Window_NameInput : Window_Selectable
   {
       public static readonly string[] LATIN1 =
       {
           "A","B","C","D","E",  "a","b","c","d","e",
           "F","G","H","I","J",  "f","g","h","i","j",
           "K","L","M","N","O",  "k","l","m","n","o",
           "P","Q","R","S","T",  "p","q","r","s","t",
           "U","V","W","X","Y",  "u","v","w","x","y",
           "Z","[","]","^","_",  "z","{","}","|","~",
           "0","1","2","3","4",  "!","#","$","%","&",
           "5","6","7","8","9",  "(",")","*","+","-",
           "/","=","@","<",">",  ":",";"," ","Page","OK"
       };

       public static readonly string[] LATIN2 =
       {
           "Á","É","Í","Ó","Ú",  "á","é","í","ó","ú",
           "À","È","Ì","Ò","Ù",  "à","è","ì","ò","ù",
           "Â","Ê","Î","Ô","Û",  "â","ê","î","ô","û",
           "Ä","Ë","Ï","Ö","Ü",  "ä","ë","ï","ö","ü",
           "Ā","Ē","Ī","Ō","Ū",  "ā","ē","ī","ō","ū",
           "Ã","Å","Æ","Ç","Ð",  "ã","å","æ","ç","ð",
           "Ñ","Õ","Ø","Š","Ŵ",  "ñ","õ","ø","š","ŵ",
           "Ý","Ŷ","Ÿ","Ž","Þ",  "ý","ÿ","ŷ","ž","þ",
           "Ĳ","Œ","ĳ","œ","ß",  "«","»"," ","Page","OK"
       };

       public static readonly string[] RUSSIA =
       {
           "А","Б","В","Г","Д",  "а","б","в","г","д",
           "Е","Ё","Ж","З","И",  "е","ё","ж","з","и",
           "Й","К","Л","М","Н",  "й","к","л","м","н",
           "О","П","Р","С","Т",  "о","п","р","с","т",
           "У","Ф","Х","Ц","Ч",  "у","ф","х","ц","ч",
           "Ш","Щ","Ъ","Ы","Ь",  "ш","щ","ъ","ы","ь",
           "Э","Ю","Я","^","_",  "э","ю","я","%","&",
           "0","1","2","3","4",  "(",")","*","+","-",
           "5","6","7","8","9",  ":",";"," ","","OK"
       };

       public static readonly string[] JAPAN1 =
       {
           "あ","い","う","え","お",  "が","ぎ","ぐ","げ","ご",
           "か","き","く","け","こ",  "ざ","じ","ず","ぜ","ぞ",
           "さ","し","す","せ","そ",  "だ","ぢ","づ","で","ど",
           "た","ち","つ","て","と",  "ば","び","ぶ","べ","ぼ",
           "な","に","ぬ","ね","の",  "ぱ","ぴ","ぷ","ぺ","ぽ",
           "は","ひ","ふ","へ","ほ",  "ぁ","ぃ","ぅ","ぇ","ぉ",
           "ま","み","む","め","も",  "っ","ゃ","ゅ","ょ","ゎ",
           "や","ゆ","よ","わ","ん",  "ー","～","・","＝","☆",
           "ら","り","る","れ","ろ",  "ゔ","を","　","カナ","決定"
       };

       public static readonly string[] JAPAN2 =
       {
           "ア","イ","ウ","エ","オ",  "ガ","ギ","グ","ゲ","ゴ",
           "カ","キ","ク","ケ","コ",  "ザ","ジ","ズ","ゼ","ゾ",
           "サ","シ","ス","セ","ソ",  "ダ","ヂ","ヅ","デ","ド",
           "タ","チ","ツ","テ","ト",  "バ","ビ","ブ","ベ","ボ",
           "ナ","ニ","ヌ","ネ","ノ",  "パ","ピ","プ","ペ","ポ",
           "ハ","ヒ","フ","ヘ","ホ",  "ァ","ィ","ゥ","ェ","ォ",
           "マ","ミ","ム","メ","モ",  "ッ","ャ","ュ","ョ","ヮ",
           "ヤ","ユ","ヨ","ワ","ン",  "ー","～","・","＝","☆",
           "ラ","リ","ル","レ","ロ",  "ヴ","ヲ","　","英数","決定"
       };

       public static readonly string[] JAPAN3 =
       {
           "Ａ","Ｂ","Ｃ","Ｄ","Ｅ",  "ａ","ｂ","ｃ","ｄ","ｅ",
           "Ｆ","Ｇ","Ｈ","Ｉ","Ｊ",  "ｆ","ｇ","ｈ","ｉ","ｊ",
           "Ｋ","Ｌ","Ｍ","Ｎ","Ｏ",  "ｋ","ｌ","ｍ","ｎ","ｏ",
           "Ｐ","Ｑ","Ｒ","Ｓ","Ｔ",  "ｐ","ｑ","ｒ","ｓ","ｔ",
           "Ｕ","Ｖ","Ｗ","Ｘ","Ｙ",  "ｕ","ｖ","ｗ","ｘ","ｙ",
           "Ｚ","［","］","＾","＿",  "ｚ","｛","｝","｜","～",
           "０","１","２","３","４",  "！","＃","＄","％","＆",
           "５","６","７","８","９",  "（","）","＊","＋","－",
           "／","＝","＠","＜","＞",  "：","；","　","かな","決定"
       };

       protected Window_NameEdit _editWindow;
       protected int _page;

       public override void Initialize(Rect rect)
       {
           base.Initialize(rect);
           _editWindow = null;
           _page = 0;
           _index = 0;
       }

       public virtual void SetEditWindow(Window_NameEdit editWindow)
       {
           _editWindow = editWindow;
           Refresh();
           UpdateCursor();
           Activate();
       }

       protected virtual string[][] Table()
       {
           if (Rmmz.gameSystem.IsJapanese())
           {
               return new string[][] { JAPAN1, JAPAN2, JAPAN3 };
           }
           else if (Rmmz.gameSystem.IsRussian())
           {
               return new string[][] { RUSSIA };
           }
           else
           {
               return new string[][] { LATIN1, LATIN2 };
           }
       }

       protected override int MaxCols()
       {
           return 10;
       }

       protected override int MaxItems()
       {
           return 90;
       }

       public override int ItemWidth()
       {
           return Mathf.FloorToInt((InnerWidth - GroupSpacing()) / 10f);
       }

       protected virtual int GroupSpacing()
       {
           return 24;
       }

       protected virtual string Character()
       {
           return _index < 88 ? Table()[_page][_index] : "";
       }

       protected virtual bool IsPageChange()
       {
           return _index == 88;
       }

       protected virtual bool IsOk()
       {
           return _index == 89;
       }

       protected override Rect ItemRect(int index)
       {
           int itemWidth = ItemWidth();
           int itemHeight = ItemHeight();
           int colSpacing = ColSpacing();
           int rowSpacing = RowSpacing();
           int groupSpacing = GroupSpacing();
           int col = index % 10;
           int group = Mathf.FloorToInt(col / 5f);
           float x = col * itemWidth + group * groupSpacing + colSpacing / 2f;
           float y = Mathf.FloorToInt(index / 10f) * itemHeight + rowSpacing / 2f;
           float width = itemWidth - colSpacing;
           float height = itemHeight - rowSpacing;
           return new Rect(x, y, width, height);
       }

       public override void DrawItem(int index)
       {
           string[][] table = Table();
           string character = table[_page][index];
           Rect rect = ItemLineRect(index);
           DrawText(character, (int)rect.x, (int)rect.y, (int)rect.width, Bitmap.TextAlign.Center);
       }

       protected virtual void UpdateCursor()
       {
           Rect rect = ItemRect(_index);
           SetCursorRect(rect.x, rect.y, rect.width, rect.height);
       }

       protected override bool IsCursorMovable()
       {
           return Active;
       }

       protected override void CursorDown(bool wrap)
       {
           if (_index < 80 || wrap)
           {
               _index = (_index + 10) % 90;
           }
       }

       protected override void CursorUp(bool wrap)
       {
           if (_index >= 10 || wrap)
           {
               _index = (_index + 80) % 90;
           }
       }

       protected override void CursorRight(bool wrap)
       {
           if (_index % 10 < 9)
           {
               _index++;
           }
           else if (wrap)
           {
               _index -= 9;
           }
       }

       protected override void CursorLeft(bool wrap)
       {
           if (_index % 10 > 0)
           {
               _index--;
           }
           else if (wrap)
           {
               _index += 9;
           }
       }

       protected override void CursorPagedown()
       {
           _page = (_page + 1) % Table().Length;
           Refresh();
       }

       protected override void CursorPageup()
       {
           _page = (_page + Table().Length - 1) % Table().Length;
           Refresh();
       }

       protected override void ProcessCursorMove()
       {
           int lastPage = _page;
           base.ProcessCursorMove();
           UpdateCursor();
           if (_page != lastPage)
           {
               PlayCursorSound();
           }
       }

       protected override void ProcessHandling()
       {
           if (IsOpen() && Active)
           {
               if (Input.IsTriggered("shift"))
               {
                   ProcessJump();
               }
               if (Input.IsRepeated("cancel"))
               {
                   ProcessBack();
               }
               if (Input.IsRepeated("ok"))
               {
                   ProcessOk();
               }
           }
       }

       protected override bool IsCancelEnabled()
       {
           return true;
       }

       protected override void ProcessCancel()
       {
           ProcessBack();
       }

       protected virtual void ProcessJump()
       {
           if (_index != 89)
           {
               _index = 89;
               PlayCursorSound();
           }
       }

       protected virtual void ProcessBack()
       {
           if (_editWindow.Back())
           {
               Rmmz.SoundManager.PlayCancel();
           }
       }

       protected override void ProcessOk()
       {
           if (!string.IsNullOrEmpty(Character()))
           {
               OnNameAdd();
           }
           else if (IsPageChange())
           {
               PlayOkSound();
               CursorPagedown();
           }
           else if (IsOk())
           {
               OnNameOk();
           }
       }

       protected virtual void OnNameAdd()
       {
           if (_editWindow.Add(Character()))
           {
               PlayOkSound();
           }
           else
           {
               PlayBuzzerSound();
           }
       }

       protected virtual void OnNameOk()
       {
           if (string.IsNullOrEmpty(_editWindow.Name()))
           {
               if (_editWindow.RestoreDefault())
               {
                   PlayOkSound();
               }
               else
               {
                   PlayBuzzerSound();
               }
           }
           else
           {
               PlayOkSound();
               CallOkHandler();
           }
       }
   }
}