namespace uAssist.UEditorWidgets
{
    using System;
    using UnityEngine;
    using UnityEditor;
    using System.Collections;

    [UWidgetWidgetAttribute(eUWidgetDesignerCategory.Widgets, "Toggle button")]
    public class UEditorWidgetToggle : UEditorWidgetBase
    {

        public UEditorWidgetToggle() : base(eWidgetType.Generic)
        {
            this.Name = "WidgetToggle";
        }

        public override bool BindTo(object Object, string MemberName)
        {
            if (Object.GetType() != typeof(bool))
            {
                return false;
            }

            if (base.BindTo(Object, MemberName) == false)
            {
                return false;
            }

            return true;
        }


        public bool Value
        {
            get
            {
                return this.GetBoundValue<bool>();
            }
            set
            {
                this.SetBoundValue(value);
            }
        }

        protected override void WidgetRender()
        {
            this.SetBoundValue(EditorGUILayout.Toggle(this.GetBoundValue<bool>(), GUILayout.Height(this.Height), GUILayout.Width(this.Width) ));
        }


    }
}