using System;
using System.Collections.Generic;

namespace WaifuGallery.Models;

public class FileExplorerHistory
{
    private readonly List<string> _history = [];
    private int _currentIndex = -1;
    private readonly object _lockObj = new();

    public bool IsLast()
    {
        lock (_lockObj)
        {
            return _currentIndex == _history.Count - 1;
        }
    }
    
    public bool IsFirst()
    {
        lock (_lockObj)
        {
            return _currentIndex == 0;
        }
    }

    public void AddPath(string path)
    {
        lock (_lockObj)
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
        lock (_lockObj)
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
        lock (_lockObj)
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
        lock (_lockObj)
        {
            return _currentIndex >= 0 && _currentIndex < _history.Count
                ? _history[_currentIndex]
                : throw new IndexOutOfRangeException();
        }
    }
}