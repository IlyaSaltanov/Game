using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsGame;

public partial class GameForm : Form
{
    private readonly System.Windows.Forms.Timer _timer;
    private DateTime _lastTick;

    private readonly InputController _inputController = new InputController();

    private RectangleF _player;
    private readonly float _playerSpeed = 220f;
    private readonly List<RectangleF> _obstacles = new List<RectangleF>();

    private RectangleF _finish;
    private float _timeRemainingSeconds;
    private bool _isWin;
    private bool _isLose;

    private RectangleF _playerStart;
    private bool _isMazeBuilt;

    public GameForm()
    {
        InitializeComponent();
        _timer = new System.Windows.Forms.Timer { Interval = 16 };

        ConfigureWindow();
        InitializeGameWorld();
        InitializeInputHandling();
        InitializeGameLoop();
    }
}

