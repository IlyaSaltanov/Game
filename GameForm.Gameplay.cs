using System;

namespace WinFormsGame;

public partial class GameForm
{
    /// <summary>
    /// Вычисляет вектор движения на основе текущего состояния клавиш и применяет перемещение.
    /// Почему: Изолирует игровую логику кадра от кода тайминга и отрисовки.
    /// </summary>
    /// <param name="deltaSeconds">Время, прошедшее с предыдущего кадра, в секундах.</param>
    private void UpdateGame(float deltaSeconds)
    {
        if (_isWin || _isLose)
        {
            return;
        }

        _timeRemainingSeconds -= deltaSeconds;
        if (_timeRemainingSeconds <= 0f)
        {
            _timeRemainingSeconds = 0f;
            _isLose = true;
            return;
        }

        (float inputX, float inputY) = _inputController.GetMoveDirection();
        MovePlayer(inputX, inputY, deltaSeconds);

        if (_player.IntersectsWith(_finish))
        {
            _isWin = true;
        }
    }

    /// <summary>
    /// Перемещает игрока с проверкой столкновений с препятствиями раздельно по осям.
    /// Почему: Обеспечивает простое скольжение вдоль стен, сохраняя логику столкновений легко читаемой.
    /// </summary>
    /// <param name="inputX">Горизонтальное направление ввода в диапазоне -1..1.</param>
    /// <param name="inputY">Вертикальное направление ввода в диапазоне -1..1.</param>
    /// <param name="deltaSeconds">Длительность кадра в секундах.</param>
    private void MovePlayer(float inputX, float inputY, float deltaSeconds)
    {
        float moveX = inputX * _playerSpeed * deltaSeconds;
        float moveY = inputY * _playerSpeed * deltaSeconds;

        float oldX = _player.X;
        _player.X = Math.Clamp(_player.X + moveX, 0, ClientSize.Width - _player.Width);
        if (IsPlayerColliding())
        {
            _player.X = oldX;
        }

        float oldY = _player.Y;
        _player.Y = Math.Clamp(_player.Y + moveY, 0, ClientSize.Height - _player.Height);
        if (IsPlayerColliding())
        {
            _player.Y = oldY;
        }
    }

    /// <summary>
    /// Проверяет, пересекается ли игрок с каким-либо препятствием.
    /// Почему: Единая точка входа для правил столкновений позволяет избежать дублирования логики пересечений.
    /// </summary>
    /// <returns>True, если игрок пересекается хотя бы с одним препятствием.</returns>
    private bool IsPlayerColliding()
    {
        for (int i = 0; i < _obstacles.Count; i++)
        {
            if (_player.IntersectsWith(_obstacles[i]))
            {
                return true;
            }
        }

        return false;
    }
}

