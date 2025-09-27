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
        protected List<string> _texts;
        protected List<string> _choices;
        protected string _speakerName;
        protected string _faceName;
        protected int _faceIndex;
        protected int _background;
        protected int _positionType;
        protected int _choiceDefaultType;
        protected int _choiceCancelType;
        protected int _choiceBackground;
        protected int _choicePositionType;
        protected int _numInputVariableId;
        protected int _numInputMaxDigits;
        protected int _itemChoiceVariableId;
        protected int _itemChoiceItypeId;
        protected bool _scrollMode;
        protected int _scrollSpeed;
        protected bool _scrollNoFast;
        protected Action<int> _choiceCallback;

        protected Game_Message()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            Clear();
        }

        public virtual void Clear()
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

        public virtual List<string> Choices()
        {
            return _choices;
        }

        public virtual string SpeakerName()
        {
            return _speakerName;
        }

        public virtual string FaceName()
        {
            return _faceName;
        }

        public virtual int FaceIndex()
        {
            return _faceIndex;
        }

        public virtual int Background()
        {
            return _background;
        }

        public virtual int PositionType()
        {
            return _positionType;
        }

        public virtual int ChoiceDefaultType()
        {
            return _choiceDefaultType;
        }

        public virtual int ChoiceCancelType()
        {
            return _choiceCancelType;
        }

        public virtual int ChoiceBackground()
        {
            return _choiceBackground;
        }

        public virtual int ChoicePositionType()
        {
            return _choicePositionType;
        }

        public virtual int NumInputVariableId()
        {
            return _numInputVariableId;
        }

        public virtual int NumInputMaxDigits()
        {
            return _numInputMaxDigits;
        }

        public virtual int ItemChoiceVariableId()
        {
            return _itemChoiceVariableId;
        }

        public virtual int ItemChoiceItypeId()
        {
            return _itemChoiceItypeId;
        }

        public virtual bool ScrollMode()
        {
            return _scrollMode;
        }

        public virtual int ScrollSpeed()
        {
            return _scrollSpeed;
        }

        public virtual bool ScrollNoFast()
        {
            return _scrollNoFast;
        }

        public virtual void Add(string text)
        {
            _texts.Add(text);
        }

        public virtual void SetSpeakerName(string speakerName)
        {
            _speakerName = !string.IsNullOrEmpty(speakerName) ? speakerName : "";
        }

        public virtual void SetFaceImage(string faceName, int faceIndex)
        {
            _faceName = faceName;
            _faceIndex = faceIndex;
        }

        public virtual void SetBackground(int background)
        {
            _background = background;
        }

        public virtual void SetPositionType(int positionType)
        {
            _positionType = positionType;
        }

        public virtual void SetChoices(List<string> choices, int defaultType, int cancelType)
        {
            _choices = choices;
            _choiceDefaultType = defaultType;
            _choiceCancelType = cancelType;
        }

        public virtual void SetChoiceBackground(int background)
        {
            _choiceBackground = background;
        }

        public virtual void SetChoicePositionType(int positionType)
        {
            _choicePositionType = positionType;
        }

        public virtual void SetNumberInput(int variableId, int maxDigits)
        {
            _numInputVariableId = variableId;
            _numInputMaxDigits = maxDigits;
        }

        public virtual void SetItemChoice(int variableId, int itemType)
        {
            _itemChoiceVariableId = variableId;
            _itemChoiceItypeId = itemType;
        }

        public virtual void SetScroll(int speed, bool noFast)
        {
            _scrollMode = true;
            _scrollSpeed = speed;
            _scrollNoFast = noFast;
        }

        public virtual void SetChoiceCallback(Action<int> callback)
        {
            _choiceCallback = callback;
        }

        public virtual void OnChoice(int n)
        {
            if (_choiceCallback != null)
            {
                _choiceCallback(n);
                _choiceCallback = null;
            }
        }

        public virtual bool HasText()
        {
            return _texts.Count > 0;
        }

        public virtual bool IsChoice()
        {
            return _choices.Count > 0;
        }

        public virtual bool IsNumberInput()
        {
            return _numInputVariableId > 0;
        }

        public virtual bool IsItemChoice()
        {
            return _itemChoiceVariableId > 0;
        }

        public virtual bool IsBusy()
        {
            return HasText() || IsChoice() || IsNumberInput() || IsItemChoice();
        }

        public virtual void NewPage()
        {
            if (_texts.Count > 0)
            {
                int lastIndex = _texts.Count - 1;
                _texts[lastIndex] = _texts[lastIndex] + "\f";
            }
        }

        public virtual string AllText()
        {
            return string.Join("\n", _texts);
        }

        public virtual bool IsRTL()
        {
            return Utils.ContainsArabic(AllText());
        }
    }
}