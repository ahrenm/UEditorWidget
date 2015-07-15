﻿namespace uAssist.UEditorWidgets
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [UWidgetWidgetAttribute(eUWidgetDesignerCategory.Widgets, "Button")]
    public class UEditorWidgetButton : UEditorWidgetLabel, IUEditorWidgetClickable
    {
        protected object _boundOnClickObject = null;
        protected string _boundOnClickMethodName;


        protected EventArgs ClickArgs = null;

        //Constructor
        public UEditorWidgetButton(): base(eWidgetType.Button)
        {
            this.Alignment = TextAnchor.MiddleCenter;
            this.Name = "WidgetButton";
            this.Border.FromInt(6, 6, 4, 4);
            this.Width = 100;
            this.Height = EditorGUIUtility.singleLineHeight;
        }



        protected void Invoke_OnClick()
        {
            if (OnClick != null)
            {
                OnClick(this, new EventArgs());
            }
        }


        protected override void WidgetRender()
        {

            //Wrap the draw call in a check so derived types don't get called.
            if (this.WidgetType == eWidgetType.Button)
            {
                if (this.LayoutMode == ePositioningLayout.Layout)
                {
                    if (GUILayout.Button(this.Label,
                        this.Style,
                        GUILayout.Height(this.Height),
                        GUILayout.Width(this.Width),
                        GUILayout.ExpandWidth(this.LayoutExpandWidth),
                        GUILayout.ExpandHeight(this.LayoutExpandHeight)))
                    {
                        Invoke_OnClick();
                    }
                }
                else
                {
                    if (GUI.Button(this.RenderRect,this.Label, this.Style))
                    {
                        Invoke_OnClick();
                    }
                }
            }
        }

        public event UEditorWidget_OnClick OnClick;
    }



}