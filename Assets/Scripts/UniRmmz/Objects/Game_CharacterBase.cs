using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of Game_Character. It handles basic information, such as
    /// coordinates and images, shared by all characters.
    /// </summary>
    [Serializable]
    public abstract partial class Game_CharacterBase : IAnimationTarget, IBalloonTarget
    {
    protected int _x;
    protected int _y;
    protected float _realX;
    protected float _realY;
    protected int _moveSpeed = 4;
    protected int _moveFrequency = 6;
    protected int _opacity = 255;
    protected int _blendMode;
    protected int _direction = 2;
    protected int _pattern = 1;
    protected int _priorityType = 1;
    protected int _tileId;
    protected string _characterName = "";
    protected int _characterIndex;
    protected bool _isObjectCharacter;
    protected bool _walkAnime = true;
    protected bool _stepAnime;
    protected bool _directionFix;
    protected bool _through;
    protected bool _transparent;
    protected float _bushDepth;
    protected int _animationId;
    protected int _balloonId;
    protected bool _animationPlaying;
    protected bool _balloonPlaying;
    protected float _animationCount;
    protected int _stopCount;
    protected int _jumpCount;
    protected int _jumpPeak;
    protected bool _movementSuccess = true;

    protected Game_CharacterBase()
    {
        InitMembers();
    }

    public int X => _x;
    public int Y => _y;

    public virtual void InitMembers()
    {
        _x = 0;
        _y = 0;
        _realX = 0;
        _realY = 0;
        _moveSpeed = 4;
        _moveFrequency = 6;
        _opacity = 255;
        _blendMode = 0;
        _direction = 2;
        _pattern = 1;
        _priorityType = 1;
        _tileId = 0;
        _characterName = "";
        _characterIndex = 0;
        _isObjectCharacter = false;
        _walkAnime = true;
        _stepAnime = false;
        _directionFix = false;
        _through = false;
        _transparent = false;
        _bushDepth = 0;
        _animationId = 0;
        _balloonId = 0;
        _animationPlaying = false;
        _balloonPlaying = false;
        _animationCount = 0;
        _stopCount = 0;
        _jumpCount = 0;
        _jumpPeak = 0;
        _movementSuccess = true;
    }

    public virtual bool Pos(int x, int y) => _x == x && _y == y;
    public virtual bool PosNt(int x, int y) => Pos(x, y) && !IsThrough();

    public virtual int MoveSpeed() => _moveSpeed;

    public virtual void SetMoveSpeed(int value)
    {
        _moveSpeed = value;
    }

    public virtual int MoveFrequency() => _moveFrequency;

    public virtual void SetMoveFrequency(int value)
    {
        _moveFrequency = value;
    }

    public virtual int Opacity() => _opacity;

    public virtual void SetOpacity(int value)
    {
        _opacity = value;
    }

    public virtual int BlendMode() => _blendMode;

    public virtual void SetBlendMode(int value)
    {
        _blendMode = value;
    }

    public virtual bool IsNormalPriority() => _priorityType == 1;

    public virtual void SetPriorityType(int value)
    {
        _priorityType = value;
    }

    public virtual bool IsMoving() => _realX != _x || _realY != _y;
    public virtual bool IsJumping() => _jumpCount > 0;

    public virtual float JumpHeight()
    {
        return (_jumpPeak * _jumpPeak -
                Mathf.Pow(Mathf.Abs(_jumpCount - _jumpPeak), 2)) /
               2f;
    }

    public virtual bool IsStopping() => !IsMoving() && !IsJumping();
    public virtual bool CheckStop(int threshold) => _stopCount > threshold;

    public virtual void ResetStopCount()
    {
        _stopCount = 0;
    }

    public virtual int RealMoveSpeed() => _moveSpeed + (IsDashing() ? 1 : 0);
    public virtual float DistancePerFrame() => Mathf.Pow(2, RealMoveSpeed()) / 256f;

    public virtual bool IsDashing() => false;
    public virtual bool IsDebugThrough() => false;

    public virtual void Straighten()
    {
        if (HasWalkAnime() || HasStepAnime())
        {
            _pattern = 1;
        }

        _animationCount = 0;
    }

    public virtual int ReverseDir(int d) => 10 - d;

    public virtual bool CanPass(int x, int y, int d)
    {
        int x2 = Rmmz.gameMap.RoundXWithDirection(x, d);
        int y2 = Rmmz.gameMap.RoundYWithDirection(y, d);
        if (!Rmmz.gameMap.IsValid(x2, y2))
        {
            return false;
        }

        if (IsThrough() || IsDebugThrough())
        {
            return true;
        }

        if (!IsMapPassable(x, y, d))
        {
            return false;
        }

        if (IsCollidedWithCharacters(x2, y2))
        {
            return false;
        }

        return true;
    }

    public virtual bool CanPassDiagonally(int x, int y, int horz, int vert)
    {
        int x2 = Rmmz.gameMap.RoundXWithDirection(x, horz);
        int y2 = Rmmz.gameMap.RoundYWithDirection(y, vert);
        if (CanPass(x, y, vert) && CanPass(x, y2, horz))
        {
            return true;
        }

        if (CanPass(x, y, horz) && CanPass(x2, y, vert))
        {
            return true;
        }

        return false;
    }

    public virtual bool IsMapPassable(int x, int y, int d)
    {
        int x2 = Rmmz.gameMap.RoundXWithDirection(x, d);
        int y2 = Rmmz.gameMap.RoundYWithDirection(y, d);
        int d2 = ReverseDir(d);
        return Rmmz.gameMap.IsPassable(x, y, d) && Rmmz.gameMap.IsPassable(x2, y2, d2);
    }

    public virtual bool IsCollidedWithCharacters(int x, int y)
    {
        return IsCollidedWithEvents(x, y) || IsCollidedWithVehicles(x, y);
    }

    public virtual bool IsCollidedWithEvents(int x, int y)
    {
        var events = Rmmz.gameMap.EventsXyNt(x, y);
        return events.Exists(ev => ev.IsNormalPriority());
    }

    public virtual bool IsCollidedWithVehicles(int x, int y)
    {
        return false;
        //return Rmmz.gameMap.Boat().PosNt(x, y) || Rmmz.gameMap.Ship().PosNt(x, y);
    }

    public virtual void SetPosition(float x, float y)
    {
        _x = Mathf.RoundToInt(x);
        _y = Mathf.RoundToInt(y);
        _realX = x;
        _realY = y;
    }

    public virtual void CopyPosition(Game_CharacterBase character)
    {
        _x = character._x;
        _y = character._y;
        _realX = character._realX;
        _realY = character._realY;
        _direction = character._direction;
    }

    public virtual void Locate(int x, int y)
    {
        SetPosition(x, y);
        Straighten();
        RefreshBushDepth();
    }

    public virtual int Direction() => _direction;

    public virtual void SetDirection(int d)
    {
        if (!IsDirectionFixed() && d != 0)
        {
            _direction = d;
        }

        ResetStopCount();
    }

    public virtual bool IsTile() => _tileId > 0 && _priorityType == 0;

    public virtual bool IsObjectCharacter() => _isObjectCharacter;

    public virtual int ShiftY() => IsObjectCharacter() ? 0 : 6;

    public virtual float ScrolledX() => Rmmz.gameMap.AdjustX(_realX);

    public virtual float ScrolledY() => Rmmz.gameMap.AdjustY(_realY);

    public virtual int ScreenX()
    {
        var tw = Rmmz.gameMap.TileWidth();
        return Mathf.FloorToInt(ScrolledX() * tw + (float)tw / 2);
    }

    public virtual int ScreenY()
    {
        var th = Rmmz.gameMap.TileHeight();
        return Mathf.FloorToInt(ScrolledY() * th + th - ShiftY() - JumpHeight());
    }

    public virtual int ScreenZ() => _priorityType * 2 + 1;

    public virtual bool IsNearTheScreen()
    {
        var gw = Graphics.Width;
        var gh = Graphics.Height;
        var tw = Rmmz.gameMap.TileWidth();
        var th = Rmmz.gameMap.TileHeight();
        var px = ScrolledX() * tw + (float)tw / 2 - (float)gw / 2;
        var py = ScrolledY() * th + (float)th / 2 - (float)gh / 2;
        return px >= -gw && px <= gw && py >= -gh && py <= gh;
    }

    public virtual void Update()
    {
        if (IsStopping())
        {
            UpdateStop();
        }

        if (IsJumping())
        {
            UpdateJump();
        }
        else if (IsMoving())
        {
            UpdateMove();
        }

        UpdateAnimation();
    }

    public virtual void UpdateStop() => _stopCount++;

    public virtual void UpdateJump()
    {
        _jumpCount--;
        _realX = (_realX * _jumpCount + _x) / (_jumpCount + 1.0f);
        _realY = (_realY * _jumpCount + _y) / (_jumpCount + 1.0f);
        RefreshBushDepth();
        if (_jumpCount == 0)
        {
            _realX = _x = Rmmz.gameMap.RoundX(_x);
            _realY = _y = Rmmz.gameMap.RoundY(_y);
        }
    }

    public virtual void UpdateMove()
    {
        if (_x < _realX)
        {
            _realX = Mathf.Max(_realX - DistancePerFrame(), _x);
        }

        if (_x > _realX)
        {
            _realX = Mathf.Min(_realX + DistancePerFrame(), _x);
        }

        if (_y < _realY)
        {
            _realY = Mathf.Max(_realY - DistancePerFrame(), _y);
        }

        if (_y > _realY)
        {
            _realY = Mathf.Min(_realY + DistancePerFrame(), _y);
        }

        if (!IsMoving())
        {
            RefreshBushDepth();
        }
    }

    public virtual void UpdateAnimation()
    {
        UpdateAnimationCount();
        if (_animationCount >= AnimationWait())
        {
            UpdatePattern();
            _animationCount = 0;
        }
    }

    public virtual int AnimationWait() => (9 - RealMoveSpeed()) * 3;

    public virtual void UpdateAnimationCount()
    {
        if (IsMoving() && HasWalkAnime())
        {
            _animationCount += 1.5f;
        }
        else if (HasStepAnime() || !IsOriginalPattern())
        {
            _animationCount++;
        }
    }

    public virtual void UpdatePattern()
    {
        if (!HasStepAnime() && _stopCount > 0)
        {
            ResetPattern();
        }
        else
        {
            _pattern = (_pattern + 1) % MaxPattern();
        }
    }

    public virtual int MaxPattern() => 4;

    public virtual int Pattern() => _pattern < 3 ? _pattern : 1;

    public virtual void SetPattern(int pattern) => _pattern = pattern;

    public virtual bool IsOriginalPattern() => Pattern() == 1;

    public virtual void ResetPattern() => SetPattern(1);

    public virtual void RefreshBushDepth()
    {
        if (IsNormalPriority() && !IsObjectCharacter() && IsOnBush() && !IsJumping())
        {
            if (!IsMoving())
            {
                _bushDepth = Rmmz.gameMap.BushDepth();
            }
        }
        else
        {
            _bushDepth = 0;
        }
    }

    public virtual bool IsOnLadder() => Rmmz.gameMap.IsLadder(_x, _y);

    public virtual bool IsOnBush() => Rmmz.gameMap.IsBush(_x, _y);

    public virtual int TerrainTag() => Rmmz.gameMap.TerrainTag(_x, _y);

    public virtual int RegionId() => Rmmz.gameMap.RegionId(_x, _y);

    public virtual void IncreaseSteps()
    {
        if (IsOnLadder())
        {
            SetDirection(8);
        }

        ResetStopCount();
        RefreshBushDepth();
    }

    public virtual int TileId() => _tileId;

    public virtual string CharacterName() => _characterName;

    public virtual int CharacterIndex() => _characterIndex;

    public virtual void SetImage(string characterName, int characterIndex)
    {
        _tileId = 0;
        _characterName = characterName;
        _characterIndex = characterIndex;
        _isObjectCharacter = Rmmz.ImageManager.IsObjectCharacter(characterName);
    }

    public virtual void SetTileImage(int tileId)
    {
        _tileId = tileId;
        _characterName = "";
        _characterIndex = 0;
        _isObjectCharacter = true;
    }

    public virtual void CheckEventTriggerTouchFront(int d)
    {
        int x2 = Rmmz.gameMap.RoundXWithDirection(_x, d);
        int y2 = Rmmz.gameMap.RoundYWithDirection(_y, d);
        CheckEventTriggerTouch(x2, y2);
    }

    public virtual void CheckEventTriggerTouch(int x, int y)
    {
    }

    public virtual bool IsMovementSucceeded() => _movementSuccess;

    public virtual void SetMovementSuccess(bool success) => _movementSuccess = success;

    public virtual void MoveStraight(int d)
    {
        SetMovementSuccess(CanPass(_x, _y, d));
        if (IsMovementSucceeded())
        {
            SetDirection(d);
            _x = Rmmz.gameMap.RoundXWithDirection(_x, d);
            _y = Rmmz.gameMap.RoundYWithDirection(_y, d);
            _realX = Rmmz.gameMap.XWithDirection(_x, ReverseDir(d));
            _realY = Rmmz.gameMap.YWithDirection(_y, ReverseDir(d));
            IncreaseSteps();
        }
        else
        {
            SetDirection(d);
            CheckEventTriggerTouchFront(d);
        }
    }

    public virtual void MoveDiagonally(int horz, int vert)
    {
        SetMovementSuccess(CanPassDiagonally(_x, _y, horz, vert));
        if (IsMovementSucceeded())
        {
            _x = Rmmz.gameMap.RoundXWithDirection(_x, horz);
            _y = Rmmz.gameMap.RoundYWithDirection(_y, vert);
            _realX = Rmmz.gameMap.XWithDirection(_x, ReverseDir(horz));
            _realY = Rmmz.gameMap.YWithDirection(_y, ReverseDir(vert));
            IncreaseSteps();
        }

        if (_direction == ReverseDir(horz))
        {
            SetDirection(horz);
        }

        if (_direction == ReverseDir(vert))
        {
            SetDirection(vert);
        }
    }

    public virtual void Jump(int xPlus, int yPlus)
    {
        if (Mathf.Abs(xPlus) > Mathf.Abs(yPlus))
        {
            if (xPlus != 0)
            {
                SetDirection(xPlus < 0 ? 4 : 6);
            }
        }
        else
        {
            if (yPlus != 0)
            {
                SetDirection(yPlus < 0 ? 8 : 2);
            }
        }

        _x += xPlus;
        _y += yPlus;
        int distance = Mathf.RoundToInt(Mathf.Sqrt(xPlus * xPlus + yPlus * yPlus));
        _jumpPeak = 10 + distance - _moveSpeed;
        _jumpCount = _jumpPeak * 2;
        ResetStopCount();
        Straighten();
    }

    public virtual bool HasWalkAnime() => _walkAnime;

    public virtual void SetWalkAnime(bool walkAnime)
    {
        _walkAnime = walkAnime;
    }

    public virtual bool HasStepAnime() => _stepAnime;

    public virtual void SetStepAnime(bool stepAnime)
    {
        _stepAnime = stepAnime;
    }

    public virtual bool IsDirectionFixed() => _directionFix;

    public virtual void SetDirectionFix(bool directionFix)
    {
        _directionFix = directionFix;
    }

    public virtual bool IsThrough() => _through;

    public virtual void SetThrough(bool through)
    {
        _through = through;
    }

    public virtual bool IsTransparent() => _transparent;

    public virtual float BushDepth() => _bushDepth;

    public virtual void SetTransparent(bool transparent)
    {
        _transparent = transparent;
    }

    public virtual void StartAnimation()
    {
        _animationPlaying = true;
    }

    public virtual void StartBalloon()
    {
        _balloonPlaying = true;
    }

    public virtual bool IsAnimationPlaying() => _animationPlaying;

    public virtual bool IsBalloonPlaying() => _balloonPlaying;

    public virtual void EndAnimation()
    {
        _animationPlaying = false;
    }

    public virtual void EndBalloon()
    {
        _balloonPlaying = false;
    }
    }
}