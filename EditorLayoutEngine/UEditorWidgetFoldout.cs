namespace uScaff.UEditorWidgets
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;


    public class UEditorWidgetFoldout : UEditorPanelBase, IUEditorWidgetClickable
    {

        public event UEditorWidget_OnClick OnClick;

        protected bool _foldOutOpen = false;
        public bool FoldoutOpen
        {
            get { return _foldOutOpen; }
            set { _foldOutOpen = value; }
        }

        public int IndentLevel = 0;
        public int IndentDepth = 10;
        public float DefaultWidth = 200f;
        public bool ToggleOnLabelClick = true;

        public object ReferenceObject
        {
            get
            {
                return this._boundMemberObject;
            }
        }

        //Constructor
        public UEditorWidgetFoldout() : base(eWidgetType.Foldout) { }

        private string _label = "";
        public string Label
        {
            get
            {
                switch (this.BindingType)
                {
                    case eBindingType.NotSet:
                        return _label;
                    case eBindingType.Property:
                        return (string)_boundPropertyInfo.GetValue(_boundMemberObject, null);
                    case eBindingType.Field:
                        return (string)_boundFieldInfo.GetValue(_boundMemberObject);
                    case eBindingType.Method:
                        return null;
                    default:
                        return null;
                }
            }
            set
            {
                switch (this.BindingType)
                {
                    case eBindingType.NotSet:
                        _label = value;
                        break;
                    case eBindingType.Property:
                        _boundPropertyInfo.SetValue(_boundMemberObject, value, null);
                        break;
                    case eBindingType.Field:
                        _boundFieldInfo.SetValue(_boundMemberObject, value);
                        break;
                    case eBindingType.Method:
                        break;
                    default:
                        break;
                }
            }
        }

        public override void Render()
        {
            if (this.Label == "")
                return;

            if (this.Width == -1)
            {
                base.Render();
                this.Width = DefaultWidth;
            }

            base.Render();

            Rect __foldoutRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, this.Style, GUILayout.MaxWidth(this.Width));
            __foldoutRect.x += (this.IndentLevel * this.IndentDepth);

            bool __newStatus = EditorGUI.Foldout(__foldoutRect, this.FoldoutOpen, this.Label, ToggleOnLabelClick);

            if (__newStatus != this.FoldoutOpen)
            {
                this.FoldoutOpen = __newStatus;
                Invoke_OnClick();
            }
            else
                this.FoldoutOpen = __newStatus;

            if (FoldoutOpen)
                this.RenderChildren();

        }

        protected void Invoke_OnClick()
        {
            if (OnClick != null)
            {
                OnClick(this, new EventArgs());
            }
        }

        
    }


}