using System;
using System.Collections.Generic;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for the state of the message window that displays text
    /// or selections, etc.
    /// </summary>
    [Serializable]
    public partial class Game_Message
    {
        private List<string> _texts;
        private List<string> _choices;
        private string _speakerName;
        private string _faceName;
        private int _faceIndex;
        private int _background;
        private int _positionType;
        private int _choiceDefaultType;
        private int _choiceCancelType;
        private int _choiceBackground;
        private int _choicePositionType;
        private int _numInputVariableId;
        private int _numInputMaxDigits;
        private int _itemChoiceVariableId;
        private int _itemChoiceItypeId;
        private bool _scrollMode;
        private int _scrollSpeed;
        private bool _scrollNoFast;
        private Action<int> _choiceCallback;

        protected Game_Message()
        {
            Initialize();
        }

        public void Initialize()
        {
            Clear();
        }

        public void Clear()
        {
            _texts = new List<string>();
            _choices = new List<string>();
            _speakerName = "";
            _faceName = "";
            _faceIndex = 0;
            _background = 0;
            _positionType = 2;
            _choiceDefaultType = 0;
            _choiceCancelType = 0;
            _choiceBackground = 0;
            _choicePositionType = 2;
            _numInputVariableId = 0;
            _numInputMaxDigits = 0;
            _itemChoiceVariableId = 0;
            _itemChoiceItypeId = 0;
            _scrollMode = false;
            _scrollSpeed = 2;
            _scrollNoFast = false;
            _choiceCallback = null;
        }

        public List<string> Choices()
        {
            return _choices;
        }

        public string SpeakerName()
        {
            return _speakerName;
        }

        public string FaceName()
        {
            return _faceName;
        }

        public int FaceIndex()
        {
            return _faceIndex;
        }

        public int Background()
        {
            return _background;
        }

        public int PositionType()
        {
            return _positionType;
        }

        public int ChoiceDefaultType()
        {
            return _choiceDefaultType;
        }

        public int ChoiceCancelType()
        {
            return _choiceCancelType;
        }

        public int ChoiceBackground()
        {
            return _choiceBackground;
        }

        public int ChoicePositionType()
        {
            return _choicePositionType;
        }

        public int NumInputVariableId()
        {
            return _numInputVariableId;
        }

        public int NumInputMaxDigits()
        {
            return _numInputMaxDigits;
        }

        public int ItemChoiceVariableId()
        {
            return _itemChoiceVariableId;
        }

        public int ItemChoiceItypeId()
        {
            return _itemChoiceItypeId;
        }

        public bool ScrollMode()
        {
            return _scrollMode;
        }

        public int ScrollSpeed()
        {
            return _scrollSpeed;
        }

        public bool ScrollNoFast()
        {
            return _scrollNoFast;
        }

        public void Add(string text)
        {
            _texts.Add(text);
        }

        public void SetSpeakerName(string speakerName)
        {
            _speakerName = !string.IsNullOrEmpty(speakerName) ? speakerName : "";
        }

        public void SetFaceImage(string faceName, int faceIndex)
        {
            _faceName = faceName;
            _faceIndex = faceIndex;
        }

        public void SetBackground(int background)
        {
            _background = background;
        }

        public void SetPositionType(int positionType)
        {
            _positionType = positionType;
        }

        public void SetChoices(List<string> choices, int defaultType, int cancelType)
        {
            _choices = choices;
            _choiceDefaultType = defaultType;
            _choiceCancelType = cancelType;
        }

        public void SetChoiceBackground(int background)
        {
            _choiceBackground = background;
        }

        public void SetChoicePositionType(int positionType)
        {
            _choicePositionType = positionType;
        }

        public void SetNumberInput(int variableId, int maxDigits)
        {
            _numInputVariableId = variableId;
            _numInputMaxDigits = maxDigits;
        }

        public void SetItemChoice(int variableId, int itemType)
        {
            _itemChoiceVariableId = variableId;
            _itemChoiceItypeId = itemType;
        }

        public void SetScroll(int speed, bool noFast)
        {
            _scrollMode = true;
            _scrollSpeed = speed;
            _scrollNoFast = noFast;
        }

        public void SetChoiceCallback(Action<int> callback)
        {
            _choiceCallback = callback;
        }

        public void OnChoice(int n)
        {
            if (_choiceCallback != null)
            {
                _choiceCallback(n);
                _choiceCallback = null;
            }
        }

        public bool HasText()
        {
            return _texts.Count > 0;
        }

        public bool IsChoice()
        {
            return _choices.Count > 0;
        }

        public bool IsNumberInput()
        {
            return _numInputVariableId > 0;
        }

        public bool IsItemChoice()
        {
            return _itemChoiceVariableId > 0;
        }

        public bool IsBusy()
        {
            return HasText() || IsChoice() || IsNumberInput() || IsItemChoice();
        }

        public void NewPage()
        {
            if (_texts.Count > 0)
            {
                int lastIndex = _texts.Count - 1;
                _texts[lastIndex] = _texts[lastIndex] + "\f";
            }
        }

        public string AllText()
        {
            return string.Join("\n", _texts);
        }

        public bool IsRTL()
        {
            return Utils.ContainsArabic(AllText());
        }
    }
}