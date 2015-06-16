namespace uScaff.UEditorWidgets
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class UEditorWidgetLabel : UEditorWidgetTextBase
    {
        private string _label;
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


        //Basic constructor
        public UEditorWidgetLabel(): this(eWidgetType.Label) { }


        //Used by widgets that derive from this to pass their type down to the base class
        public UEditorWidgetLabel(eWidgetType type) : base(type) { }


        public override void Render()
        {
            base.Render();

            //Wrap the draw call in a check so derived types don't get called.
            if (this.WidgetType == eWidgetType.Label)
                EditorGUILayout.LabelField(this.Label, this.Style, GUILayout.Width(this.Style.fixedWidth));
        }
    }

    public class UEditorWidgetTextField : UEditorWidgetBase
    {
        private string _text;
        public string Text
        {
            get
            {
                switch (this.BindingType)
                {
                    case eBindingType.NotSet:
                        return _text;
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
                        _text = value;
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


        public UEditorWidgetTextField()
            : base(eWidgetType.TextField)
        {

        }

        public override void Render()
        {
            base.Render();

            //Wrap the draw call in a check so derived types don't get called.
            if (this.WidgetType == eWidgetType.TextField)
                this.Text = EditorGUILayout.TextField(this.Text, this.Style, GUILayout.Width(this.Style.fixedWidth));
        }

    }

    public class UEditorButton : UEditorWidgetLabel, IUEditorWidgetClickable
    {
        protected object _boundOnClickObject = null;
        protected string _boundOnClickMethodName;


        protected EventArgs ClickArgs = null;

        //Constructor
        public UEditorButton()
            : base(eWidgetType.Button)
        {
            this.Alignment = TextAnchor.MiddleCenter;
        }



        protected void Invoke_OnClick()
        {
            if (OnClick != null)
            {
                OnClick(this, new EventArgs());
            }
        }


        public override void Render()
        {
            base.Render();

            //Wrap the draw call in a check so derived types don't get called.
            if (this.WidgetType == eWidgetType.Button)
            {
                if (GUILayout.Button(this.Label, this.Style))
                    Invoke_OnClick();
            }
        }

        public event UEditorWidget_OnClick OnClick;
    }

    public class UEditorDecoratorSeperator : IUEditorWidgetRenderable
    {
        public string Name { get; set; }

        public int SeperatorLines = 1;

        public UEditorDecoratorSeperator(int LineCount)
        {
            SeperatorLines = LineCount;
        }

        public UEditorDecoratorSeperator() { }

        public void Render()
        {
            for (int i = 0; i < SeperatorLines; i++)
            {
                EditorGUILayout.Separator();
            }
        }

        private UEditorPanelBase _parent;
        public UEditorPanelBase parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
    }

    public class UEditorDecoratorGUIEnable : UEditorBase
    {
        private bool _editorGUIEnabled = true;
        public bool EditorGUIEnabled
        {
            get
            {
                //TODO: Write proper checking of the return type to prevent/handle exceptions
                switch (this.BindingType)
                {
                    case eBindingType.NotSet:
                        return _editorGUIEnabled;
                    case eBindingType.Property:
                        return (bool)_boundPropertyInfo.GetValue(_boundMemberObject, null);
                    case eBindingType.Field:
                        return (bool)_boundFieldInfo.GetValue(_boundMemberObject);
                    default:
                        return true;
                }
            }
            set
            {
                _editorGUIEnabled = value;
            }
        }

        public override void Render()
        {
            GUI.enabled = this.EditorGUIEnabled;
        }

    }

    public class UEditorDecoratorHorizontalLine : IUEditorWidgetRenderable
    {
        public string Name { get; set; }

        public void Render()
        {
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        }

        private UEditorPanelBase _parent;
        public UEditorPanelBase parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
    }

    
}