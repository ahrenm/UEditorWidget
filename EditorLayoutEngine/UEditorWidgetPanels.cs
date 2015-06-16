namespace uScaff.UEditorWidgets
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;


    public abstract class UEditorPanelBase : UEditorWidgetBase
    {

        protected List<IUEditorWidgetRenderable> Children = new List<IUEditorWidgetRenderable>();
        public void AddChild(IUEditorWidgetRenderable newChild)
        {
            newChild.parent = this;

            if (newChild.GetType().IsSubclassOf(typeof (UEditorWidgetBase)))
            {
                UEditorWidgetBase __widget = (UEditorWidgetBase)newChild;
                if (__widget.Width == -1)
                    __widget.Width = this.Width;
            }
            
            Children.Add(newChild);

            if (AutoCalcWidth)
                this.Width = CalcWidth;
        }

        private eLayoutStyle _layoutStyle = eLayoutStyle.NotSet;
        public eLayoutStyle LayoutStyle
        {
            get { return _layoutStyle; }
            set
            {
                _layoutStyle = value;

                if (AutoCalcWidth)
                    this.Width = CalcWidth;
            }
        }

        public bool AutoCalcWidth = true;
        public float CalcWidth
        {
            get
            {
                float __retValue = -1; 

                if (this.LayoutStyle == eLayoutStyle.Vertical)
                {
                    foreach (var item in Children)
                    {
                        if (item.GetType().IsSubclassOf(typeof(UEditorWidgetBase)))
                        {
                            UEditorWidgetBase __castItem = (UEditorWidgetBase)item;
                            if (__castItem.Width > __retValue)
                                __retValue = __castItem.Width;
                        }
                    }
                }

                if (this.LayoutStyle == eLayoutStyle.Horizontal)
                {
                    foreach (var item in Children)
                    {
                        if (item.GetType().IsSubclassOf(typeof(UEditorWidgetBase)))
                        {
                            UEditorWidgetBase __castItem = (UEditorWidgetBase)item;
                            __retValue += __castItem.Width;
                        }
                    }
                }


                return __retValue;
            }

        }

        //Constructor
        public UEditorPanelBase(eWidgetType type) : base(type) {}


        protected void RenderChildren()
        {
            foreach (var item in Children)
            {
                item.Render();
            }
        }

        public override void Render()
        {
            base.Render();

        }
    }

    public sealed class UEditorPanelHorizonal : UEditorPanelBase
    {

        public UEditorPanelHorizonal() : base(eWidgetType.PanelHorizontal) 
        {
            this.LayoutStyle = eLayoutStyle.Horizontal;
        }

        public override void Render()
        {
            base.Render();

            EditorGUILayout.BeginHorizontal(this.Style, GUILayout.Width(this.Width));
            {
                RenderChildren();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    public sealed class UEditorPanelVertical : UEditorPanelBase
    {

        public UEditorPanelVertical() : base(eWidgetType.PanelVertical) 
        { 
            this.LayoutStyle = eLayoutStyle.Vertical; 
        }

        public override void Render()
        {
            base.Render();

            EditorGUILayout.BeginVertical(this.Style, GUILayout.Width(this.Width));
            {
                RenderChildren();
            }
            EditorGUILayout.EndVertical();
        }
    }

    public sealed class UEditorPanelScroll : UEditorPanelBase
    {

        public UEditorPanelScroll() : base(eWidgetType.PanelScroll) { }

        public Vector2 ScrollPosition = new Vector2();

        public override void Render()
        {
            base.Render();

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, this.Style);

            RenderChildren();

            EditorGUILayout.EndVertical();
        }
    }

    public sealed class UEditorPanelToggleArea : UEditorPanelBase
    {

        public UEditorPanelToggleArea() : base(eWidgetType.PanelToggleArea) { }

        public bool AreaEnabled = true;

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

        public override void Render()
        {
            base.Render();

            this.AreaEnabled = EditorGUILayout.BeginToggleGroup(this.Label, this.AreaEnabled);

            RenderChildren();

            EditorGUILayout.EndToggleGroup();
        }
    }

    public sealed class UEditorPanelArea : UEditorPanelBase
    {

        public UEditorPanelArea() : base(eWidgetType.PanelArea) { }

        public Rect ScreenAreaRect = new Rect();

        public override void Render()
        {
            base.Render();

            GUILayout.BeginArea(ScreenAreaRect, this.Style);

            RenderChildren();

            EditorGUILayout.EndVertical();
        }
    }


}