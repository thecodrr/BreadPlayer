using System.Collections.Generic;

namespace BreadPlayer.MomentoPattern
{
    public class UndoRedoStack<T>
    {
        private Stack<T> _undo;
        private Stack<T> _redo;

        public int UndoCount => _undo.Count;
        public int RedoCount => _redo.Count;

        public UndoRedoStack()
        {
            Reset();
        }

        public void Reset()
        {
            _undo = new Stack<T>();
            _redo = new Stack<T>();
        }

        public T SemiUndo(T input)
        {
            if (_undo.Count > 0)
            {
                T cmd = _undo.Peek();
                T output = cmd;
                return output;
            }
            return input;
        }

        public T Do(T sub)
        {
            T output = sub;
            _undo.Push(sub);
            _redo.Clear(); // Once we issue a new command, the redo stack clears
            return output;
        }

        public T Undo(T input)
        {
            if (_undo.Count > 0)
            {
                T cmd = _undo.Pop();
                T output = cmd;
                _redo.Push(cmd);
                return output;
            }
            return Redo(input);
        }

        public T Redo(T input)
        {
            if (_redo.Count > 0)
            {
                T cmd = _redo.Pop();
                T output = cmd;
                _undo.Push(cmd);
                return output;
            }
            return input;
        }
    }
}