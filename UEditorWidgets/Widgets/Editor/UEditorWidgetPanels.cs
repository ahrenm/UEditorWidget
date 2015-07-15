namespace uAssist.UEditorWidgets
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    
    public class UEditorPanelBase : UEditorWidgetBase
    {

        [UWidgetProperty]
        public bool ChildrenShouldRender = true;


        public List<UEditorWidgetBase> Children = new List<UEditorWidgetBase>();
        
        public void AddChild(UEditorWidgetBase newChild)
        {
            newChild.parent = this;

            if (newChild.GetType().IsSubclassOf(typeof (UEditorWidgetBase)))
            {
                UEditorWidgetBase __widget = (UEditorWidgetBase)newChild;
                if (__widget.Width == -1)
                    __widget.Width = this.Width;
            }
            
            Children.Add(newChild);

        }

        public void ClearChilden()
        {
            this.Children.Clear();
        }

        //public List<IUEditorWidgetRenderable> Children

        //Constructor
        public UEditorPanelBase(eWidgetType type) : base(type) {}


        protected void RenderChildren()
        {
            if (this.ChildrenShouldRender)
            {
                foreach (var item in Children)
                {
                    item.Render();
                }
            }
        }

        protected override void WidgetRender()
        {
          

        }

    }

    [UWidgetWidgetAttribute(eUWidgetDesignerCategory.Panels, "Horizontal Layout")]
    public sealed class UEditorPanelHorizonal : UEditorPanelBase
    {

        public UEditorPanelHorizonal() : base(eWidgetType.PanelHorizontal) 
        {
            //this.LayoutMode = ePositioningLayout.Layout;
            this.Name = "HorizontalLayout";
        }

        protected override void WidgetRender()
        {
            if (this.LayoutMode != ePositioningLayout.Layout)
            {
                GUILayout.BeginArea(new Rect(this.RenderOffsetX, this.RenderOffsetY, this.Width, this.Height), this.Style);
            }
            {
                EditorGUILayout.BeginHorizontal(this.Style, GUILayout.Width(this.Width));
                {
                    RenderChildren();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (this.LayoutMode != ePositioningLayout.Layout)
            {
                GUILayout.EndArea();
            }
        }
    }

    [UWidgetWidgetAttribute(eUWidgetDesignerCategory.Panels, "Vertical Layout")]
    public sealed class UEditorPanelVertical : UEditorPanelBase
    {

        public UEditorPanelVertical() : base(eWidgetType.PanelVertical) 
        {
            //this.LayoutMode = ePositioningLayout.Layout;
            this.Name = "VerticalLayout";
        }

        protected override void WidgetRender()
        {

            if (this.LayoutMode != ePositioningLayout.Layout)
            {
                GUILayout.BeginArea(new Rect(this.RenderOffsetX, this.RenderOffsetY, this.Width, this.Height), this.Style);
            }
            {
                EditorGUILayout.BeginVertical(this.Style, GUILayout.Width(this.Width));
                {
                    RenderChildren();
                }
                EditorGUILayout.EndVertical();
            }
            if (this.LayoutMode != ePositioningLayout.Layout)
            {
                GUILayout.EndArea();
            }
        }
    }

    [UWidgetWidgetAttribute(eUWidgetDesignerCategory.Panels, "Scroll Layout")]
    public sealed class UEditorPanelScroll : UEditorPanelBase
    {

        public UEditorPanelScroll() : base(eWidgetType.PanelScroll) { }

        public Vector2 ScrollPosition = new Vector2();

        protected override void WidgetRender()
        {

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, this.Style, GUILayout.Width(this.parent.Width + 10), GUILayout.Height(this.parent.Height + 10));
            {
                RenderChildren();
            }
            EditorGUILayout.EndScrollView();
        }
    }


    [UWidgetWidgetAttribute(eUWidgetDesignerCategory.Panels, "Area Layout")]
    public sealed class UEditorPanelArea : UEditorPanelBase
    {

        public UEditorPanelArea() : base(eWidgetType.PanelArea) 
        {
            this.Name = "AreaPanel";
        }

        protected override void WidgetRender()
        {
            Rect __areaRect;

            if (this.LayoutMode == ePositioningLayout.Layout)
            {
                __areaRect = EditorGUILayout.GetControlRect(GUILayout.Width(this.Width), GUILayout.Height(this.Height));
            }
            else
            {
                __areaRect = new Rect(this.RenderOffsetX, this.RenderOffsetY, this.Width, this.Height);
            }

            //GUI.BeginGroup(new Rect(this.RenderOffsetX, this.RenderOffsetY, this.Width, this.Height), this.Style);
            GUILayout.BeginArea(__areaRect, this.Style);
            {
                RenderChildren();
            }
            GUILayout.EndArea();
        }
    }


}