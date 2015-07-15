namespace uAssist.UEditorWidgets
{
    using System;
    using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using uAssist.UEditorWidgets;

    public class frmBase : EditorWindow
    {
        
        public List<UEditorWidgetBase> Components = new List<UEditorWidgetBase>();
        public bool InspectorUpdateRender = false;

        public string FormTitle
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }
        public float MaxHeight = -1;
        public float MaxWidth = -1;

        private bool _eventsEnabled = true;
        public bool EventsEnabled
        {
            get
            {
                return _eventsEnabled;
            }
            set
            {
                if (value = true && this._eventsEnabled == false)
                {
                    this.EnableEvents();
                }

                if (value == false && this._eventsEnabled == true)
                {
                    this.DisableEvents();
                }

                this._eventsEnabled = value;
            }

        }

        public virtual void OnEnable()
        {
            if (this.EventsEnabled)
            {
                this.EnableEvents();
            }
        }


        protected virtual void EnableEvents(){}
        protected virtual void DisableEvents(){}

        public frmBase() : base()
        {
            this.maxSize = new Vector2(MaxWidth, MaxHeight);
        }

        public virtual void OnInspectorUpdate()
        {
            if (this.InspectorUpdateRender)
            {
                this.Repaint();
            }
        }

        public virtual void OnGUI()
        {
            foreach (UEditorWidgetBase __widget in Components)
            {
                if (__widget != null)
                {
                    __widget.Render();
                }
            }
        }

    }
}