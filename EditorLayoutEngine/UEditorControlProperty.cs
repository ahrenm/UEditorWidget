namespace uScaff.UEditorWidgets
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class UEditorControlProperty : UEditorWidgetBase
    {

        public eLayoutStyle LayoutStyle = eLayoutStyle.Horizontal;

        public UEditorWidgetLabel PropertyLabel = new UEditorWidgetLabel();
        public UEditorWidgetTextField PropertyInputField = new UEditorWidgetTextField();

        public override bool BindTo(object Object, string MemberName)
        {
            PropertyLabel.Label = MemberName;
            PropertyInputField.BindTo(Object, MemberName);

            return true;
        }

        //Constructor
        public UEditorControlProperty() : base(eWidgetType.Generic) 
        {
            this.LayoutStyle = eLayoutStyle.Horizontal;
        }

        public override void Render()
        {

            if (LayoutStyle == eLayoutStyle.Horizontal)
            {
                
                base.Render();

                this.Style.stretchWidth = false;

                //EditorGUILayout.BeginHorizontal(GUILayout.Width(this.PropertyInputField.Width + this.PropertyLabel.Width));
                EditorGUILayout.BeginHorizontal(this.Style, GUILayout.Width(this.Style.fixedWidth));
                {
                    this.PropertyLabel.Render();
                    this.PropertyInputField.Render();
                    
                }
                EditorGUILayout.EndHorizontal();
            }

            if (LayoutStyle == eLayoutStyle.Vertical)
            {
                base.Render();

                EditorGUILayout.BeginVertical(this.Style);
                {
                    this.PropertyLabel.Render();
                    this.PropertyInputField.Render();
                }
                EditorGUILayout.EndVertical();
            }


        }

    }
}
