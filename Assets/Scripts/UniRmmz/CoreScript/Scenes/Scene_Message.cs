using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of Scene_Map and Scene_Battle.
    /// </summary>
    public partial class Scene_Message //: Scene_Base
    {
        protected Window_Message _messageWindow;
        protected Window_ScrollText _scrollTextWindow;
        protected Window_Gold _goldWindow;
        protected Window_NameBox _nameBoxWindow;
        protected Window_ChoiceList _choiceListWindow;
        protected Window_NumberInput _numberInputWindow;
        protected Window_EventItem _eventItemWindow;

        public virtual bool IsMessageWindowClosing()
        {
            return _messageWindow.IsClosing();
        }

        protected virtual void CreateAllWindows()
        {
            CreateMessageWindow();
            CreateScrollTextWindow();
            CreateGoldWindow();
            CreateNameBoxWindow();
            CreateChoiceListWindow();
            CreateNumberInputWindow();
            CreateEventItemWindow();
            AssociateWindows();
        }

        protected virtual void CreateMessageWindow()
        {
            Rect rect = MessageWindowRect();
            _messageWindow = Window_Message.Create(rect, "message");
            AddWindow(_messageWindow);
        }

        protected virtual Rect MessageWindowRect()
        {
            float ww = Graphics.BoxWidth;
            float wh = CalcWindowHeight(4, false) + 8;
            float wx = (Graphics.BoxWidth - ww) / 2;
            float wy = 0;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateScrollTextWindow()
        {
            Rect rect = ScrollTextWindowRect();
            _scrollTextWindow = Window_ScrollText.Create(rect, "scrollText");
            AddWindow(_scrollTextWindow);
        }

        protected virtual Rect ScrollTextWindowRect()
        {
            float wx = 0;
            float wy = 0;
            float ww = Graphics.BoxWidth;
            float wh = Graphics.BoxHeight;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateGoldWindow()
        {
            Rect rect = GoldWindowRect();
            _goldWindow = Window_Gold.Create(rect, "gold");
            _goldWindow.Openness = 0;
            AddWindow(_goldWindow);
        }

        protected virtual Rect GoldWindowRect()
        {
            float ww = MainCommandWidth();
            float wh = CalcWindowHeight(1, true);
            float wx = Graphics.BoxWidth - ww;
            float wy = 0;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateNameBoxWindow()
        {
            _nameBoxWindow = Window_NameBox.Create(new Rect(), "nameBox");
            AddWindow(_nameBoxWindow);
        }
        
        protected virtual void CreateChoiceListWindow()
        {
            _choiceListWindow = Window_ChoiceList.Create(new Rect(), "choiceList");
            AddWindow(_choiceListWindow);
        }

        protected virtual void CreateNumberInputWindow()
        {
            _numberInputWindow = Window_NumberInput.Create(new Rect(), "numberInput");
            AddWindow(_numberInputWindow);
        }

        protected virtual void CreateEventItemWindow()
        {
            Rect rect = EventItemWindowRect();
            _eventItemWindow = Window_EventItem.Create(rect, "eventItem");
            AddWindow(_eventItemWindow);
        }

        protected virtual Rect EventItemWindowRect()
        {
            float wx = 0;
            float wy = 0;
            float ww = Graphics.BoxWidth;
            float wh = CalcWindowHeight(4, true);
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void AssociateWindows()
        {
            Window_Message messageWindow = _messageWindow;
            messageWindow.SetGoldWindow(_goldWindow);
            messageWindow.SetNameBoxWindow(_nameBoxWindow);
            messageWindow.SetChoiceListWindow(_choiceListWindow);
            messageWindow.SetNumberInputWindow(_numberInputWindow);
            messageWindow.SetEventItemWindow(_eventItemWindow);
            _nameBoxWindow.SetMessageWindow(messageWindow);
            _choiceListWindow.SetMessageWindow(messageWindow);
            _numberInputWindow.SetMessageWindow(messageWindow);
            _eventItemWindow.SetMessageWindow(messageWindow);
        }
    }
}
