using System;
using System.Collections.Generic;
using Serilog;

namespace Marin.UI.Models;

public class FileManagerHistory
{
    #region Private Fields

    private readonly List<string> _history = [];
    private int _currentIndex = -1;

    #endregion

    #region Public Methods

    public bool IsLast()
    {
        lock (_history)
        {
            return _currentIndex == _history.Count - 1;
        }
    }

    public bool IsFirst()
    {
        lock (_history)
        {
            return _currentIndex == 0;
        }
    }

    public void AddPath(string path)
    {
        Log.Debug("FileManagerHistory: AddPath({Path})", path);
        lock (_history)
        {
            if (_currentIndex < _history.Count - 1)
            {
                _history.RemoveRange(_currentIndex + 1, _history.Count - (_currentIndex + 1));
            }

            _history.Add(path);
            _currentIndex++;

            if (_history.Count <= 100) return; // Example capacity management
            _history.RemoveAt(0);
            _currentIndex--;
        }
    }

    public string GoBack()
    {
        lock (_history)
        {
            if (_currentIndex <= 0)
            {
                _currentIndex = 0;
            }
            else
            {
                _currentIndex--;
            }

            return _history[_currentIndex];
        }
    }

    public string GoForward()
    {
        lock (_history)
        {
            if (_currentIndex >= _history.Count - 1)
            {
                _currentIndex = _history.Count - 1;
            }
            else
            {
                _currentIndex++;
            }

            return _history[_currentIndex];
        }
    }

    public string GetCurrentPath()
    {
        lock (_history)
        {
            return _currentIndex >= 0 && _currentIndex < _history.Count
                ? _history[_currentIndex]
                : throw new IndexOutOfRangeException();
        }
    }

    #endregion
}