using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameMarker
{
    public class UndoEngine
    {
        bool _dirty = false;
        int _capacity = 50;
        LinkedList<UndoAction> _undo_actions = new LinkedList<UndoAction>();
        LinkedList<UndoAction> _redo_actions = new LinkedList<UndoAction>();

        public UndoEngine()
        {

        }

        public UndoEngine(int capacity)
        {
            _capacity = capacity;
        }

        //----------------------------------------------------------------------
        // EVENTS
        //----------------------------------------------------------------------
        public event EventHandler UndoListChanged;

        //----------------------------------------------------------------------
        // UNDO FUNC
        //----------------------------------------------------------------------
        public bool IsDirty()
        {
            return _dirty;
        }
        public void SetDirty(bool dirty)
        {
            _dirty = dirty;
            UndoListChanged(this, null);
        }

        public bool CanUndo()
        {
            return (_undo_actions.Count > 0);
        }

        public bool CanRedo()
        {
            return (_redo_actions.Count > 0);
        }

        public IEnumerable<UndoAction> GetUndoList()
        {
            return _undo_actions;
        }

        public void ClearUndoRedoList()
        {
            _undo_actions.Clear();
            _redo_actions.Clear();
            OnUndoListChanged();
        }

        public void MarkAllOtherStatesDirty()
        {
            foreach (var s in _undo_actions)
            {
                s.after_dirty = true;
                s.before_dirty = true;
            }
            foreach (var s in _redo_actions)
            {
                s.after_dirty = true;
                s.before_dirty = true;
            }

            if (_undo_actions.Count > 0)
                _undo_actions.Last().after_dirty = false;

            if (_redo_actions.Count > 0)
                _redo_actions.Last().before_dirty = false;
        }

        public void PerformAction(UndoAction act, bool skipredo)
        {
            act.before_dirty = _dirty;
            act.after_dirty = true;
            _dirty = true;

            _redo_actions.Clear();
            _undo_actions.AddLast(act);

            if (!skipredo)
            {
                _undo_actions.Last().Redo();
            }

            if (_undo_actions.Count > _capacity)
            {
                _undo_actions.RemoveFirst();
            }

            OnUndoListChanged();
        }

        public void MergeWithLast()
        {
            if (_undo_actions.Count < 2)
                throw new Exception("There has to be at least two undo actions to be merged with");

            var last = _undo_actions.Last;
            _undo_actions.RemoveLast();
            var previous = _undo_actions.Last;
            _undo_actions.RemoveLast();

            var act = new SimpleAction(
                () =>
                {
                    previous.Value.Redo();
                    last.Value.Redo();
                },
                () =>
                {
                    last.Value.Undo();
                    previous.Value.Undo();
                });

            act.before_dirty = previous.Value.before_dirty;
            act.after_dirty = previous.Value.after_dirty;

            _undo_actions.AddLast(act);
        }

        public void PerformSimpleAction(Action doit, Action undoit)
        {
            PerformAction(new SimpleAction(doit, undoit), false);
        }

        public void PerformSimpleAction(Action doit, Action undoit, bool skipRedo)
        {
            PerformAction(new SimpleAction(doit, undoit), skipRedo);
        }

        public void OnUndoListChanged()
        {
            OnUndoListChanged(EventArgs.Empty);
        }

        void OnUndoListChanged(EventArgs e)
        {
            if (UndoListChanged != null)
            {
                UndoListChanged(this, e);
            }
        }

        //----------------------------------------------------------------------
        // GUI FUNC
        //----------------------------------------------------------------------                                     
        public void Op_Undo()
        {
            if (CanUndo())
            {
                _undo_actions.Last().Undo();
                _dirty = _undo_actions.Last().before_dirty;
                _redo_actions.AddLast(_undo_actions.Last());
                _undo_actions.RemoveLast();

                OnUndoListChanged();
            }
        }

        public void Op_Redo()
        {
            if (CanRedo())
            {
                _redo_actions.Last().Redo();
                _dirty = _redo_actions.Last().after_dirty;
                _undo_actions.AddLast(_redo_actions.Last());
                _redo_actions.RemoveLast();

                OnUndoListChanged();
            }
        }
    }

    public abstract class UndoAction
    {
        protected abstract void DoRedo();

        protected abstract void DoUndo();

        bool reverse;

        public bool before_dirty;
        public bool after_dirty;

        protected UndoAction(bool rev)
        {
            reverse = rev;
        }

        protected UndoAction()
        {
            reverse = false;
        }

        public void Redo()
        {
            if (reverse)
                DoUndo();
            else
                DoRedo();
        }

        public void Undo()
        {
            if (reverse)
                DoRedo();
            else
                DoUndo();
        }
    }

    public class SimpleAction : UndoAction
    {
        Action doIt;
        Action undoIt;

        public SimpleAction(Action doIt, Action undoIt)
            : this(doIt, undoIt, false)
        {
        }
        public SimpleAction(Action doIt, Action undoIt, bool reverse)
            : base(reverse)
        {
            this.doIt = doIt;
            this.undoIt = undoIt;
        }
        protected override void DoRedo()
        {
            doIt();
        }
        protected override void DoUndo()
        {
            undoIt();
        }
    }
}
