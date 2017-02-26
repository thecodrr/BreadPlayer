using System.Collections.Generic;

namespace BreadPlayer.MomentoPattern
{
	public class UndoRedoStack<T>
    {
        private Stack<T> _Undo;
        private Stack<T> _Redo;

        public int UndoCount
        {
            get
            {
                return _Undo.Count;
            }
        }
        public int RedoCount
        {
            get
            {
                return _Redo.Count;
            }
        }

        public UndoRedoStack()
        {
            Reset();
        }
        public void Reset()
        {
            _Undo = new Stack<T>();
            _Redo = new Stack<T>();
        }
	
	public T SemiUndo(T input)
        {
            if (_Undo.Count > 0)
            {
                T cmd = _Undo.Peek();
                T output = cmd;
                return output;
            }
                return input;
        }
	
        public T Do(T sub)
        {
            T output = sub;
            _Undo.Push(sub);
            _Redo.Clear(); // Once we issue a new command, the redo stack clears
            return output;
        }
        public T Undo(T input)
        {
            if (_Undo.Count > 0)
            {
                T cmd = _Undo.Pop();
                T output = cmd;
                _Redo.Push(cmd);
                return output;
            }
            else
            {
                return Redo(input);
            }
        }
        public T Redo(T input)
        {
            if (_Redo.Count > 0)
            {
                T cmd = _Redo.Pop();
                T output = cmd;
                _Undo.Push(cmd);
                return output;
            }
            else
            {
                return input;
            }
        }


    }
}
