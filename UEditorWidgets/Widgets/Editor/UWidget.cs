namespace uAssist.UEditorWidgets
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using System.Linq;
    using UEditorWidgets;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Assigned to widget properties to make them editable to the designer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class UWidgetPropertyAttribute : Attribute
    {
        public UWidgetPropertyAttribute(string Label = "")
        {
            this.Label = Label;
        }

        public string[] ListOptions;
        public bool HideInProperties = false;
        public Type CustomEditor = null;
        public string Label = "";
    }

    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited=true)]
    public class UWidgetWidgetAttribute :Attribute
    {
        public UWidgetWidgetAttribute (eUWidgetDesignerCategory Catogery, string Label)
        {
            this.DesignerCatogery = Catogery;
            this.DesignerLabel = Label;
        }

        public eUWidgetDesignerCategory DesignerCatogery = eUWidgetDesignerCategory.NotSet;
        public string DesignerLabel = "";
    }

    public enum eUWidgetDesignerCategory
    {
        NotSet = 0,
        Panels = 1,
        Widgets = 2,
        Controls = 3,
        Decorators = 4,
        Other = 5
    }


    /// <summary>
    /// Widget clickable scaffolding
    /// </summary>
    public interface IUEditorWidgetClickable
    {
        event UEditorWidget_OnClick OnClick;
    }
    public delegate void UEditorWidget_OnClick(IUEditorWidgetClickable sender, EventArgs e);

    //TODO: Get rid of this entirely
    public enum eWidgetType
    {
        NotSet = 0,
        Generic = 1,
        Toggle = 2,
        Label = 3,
        TextField = 4,
        TextArea = 5,
        Button = 6,
        Foldout = 7,
        PanelArea = 20,
        PanelVertical = 21,
        PanelHorizontal = 22,
        PanelScroll = 23,
        PanelToggleArea = 24
    }

    public enum ePositioningLayout
    {
        NotSet = 0,
        WindowSpace = 1,
        RelativeToParent = 2,
        Layout = 3
    }

    

    //My thanks to Unity for making RectOffset both sealed and not marked as seralizable
    //So we do this junk.
    [Serializable]
    public class RectOffsetSeralizable
    {
        private int _top, _bottom, _left, _right;

        public int top
        {
            get
            {
                return this._top;
            }
            set
            {
                if (_top != value)
                {
                    this.IsStyleDirty = true;
                }
                this._top = value;
            }
        }
        public int left
        {
            get
            {
                return this._left;
            }
            set
            {
                if (_left != value)
                {
                    this.IsStyleDirty = true;
                }
                this._left = value;
            }
        }
        public int right
        {
            get
            {
                return this._right;
            }
            set
            {
                if (_right != value)
                {
                    this.IsStyleDirty = true;
                }
                this._right = value;
            }
        }
        public int bottom
        {
            get
            {
                return this._bottom;
            }
            set
            {
                if (_bottom != value)
                {
                    this.IsStyleDirty = true;
                }
                this._bottom = value;
            }
        }

        public RectOffset ToRectOffset()
        {
            return new RectOffset(this.left, this.right, this.top, this.bottom);
        }

        public bool IsStyleDirty = false;

        public void FromRectOffset(RectOffset rectOffset)
        {
            this.top = rectOffset.top;
            this.bottom = rectOffset.bottom;
            this.left = rectOffset.left;
            this.right = rectOffset.right;

            this.IsStyleDirty = true;
        }

        public void FromInt(int Left, int Right, int Top, int Bottom)
        {
            this.top = Top;
            this.bottom = Bottom;
            this.left = Left;
            this.right = Right;

            this.IsStyleDirty = true;
        }
    }


    //Main Widget base class
    //=====================

    /// <summary>
    /// The root of the widget hierarchy.
    /// This class should really be abstract but Unity serilazation doesn't handle abstract bases in generic lists
    /// </summary>
    public class UWidget : ScriptableObject
    {

#region Static Methods

        public static T Create<T>(string Name = "", bool bSuppressBindingWarnings = false) where T : UEditorWidgetBase
        {
            return (T)UWidget.Create(typeof(T), Name,bSuppressBindingWarnings);
        }

        public static UEditorWidgetBase Create(Type WidgetType, string Name = "", bool bSuppressBindingWarnings = false)
        {
            object __retObject = ScriptableObject.CreateInstance(WidgetType);
            UEditorWidgetBase __castWidget = (UEditorWidgetBase)__retObject;
            __castWidget.hideFlags = HideFlags.HideAndDontSave;
            if (Name != "")
            {
                __castWidget.Name = Name;
            }
            __castWidget.SuppressBindingWarnings = bSuppressBindingWarnings;
            return __castWidget;
        }

        public static UEditorWidgetBase FindWidgetById(List<UEditorWidgetBase> RenderableWidgets, string WidgetID)
        {
            UEditorWidgetBase __retValue = null;

            foreach (var __widget in RenderableWidgets)
            {
                if (((UEditorWidgetBase)__widget).WidgetID == WidgetID)
                {
                    return (UEditorWidgetBase)__widget;
                }

                if (__widget.GetType().IsSubclassOf(typeof(UEditorPanelBase)))
                {
                    UEditorWidgetBase __subSearchResult = UWidget.FindWidgetById(((UEditorPanelBase)__widget).Children, WidgetID);
                    if (__subSearchResult != null)
                    {
                        return __subSearchResult;
                    }
                }
            }
            return __retValue;
        }

        public static bool TryGetEnumOptions<T>(out List<string> _enumOptions)
        {
            try
            {
                _enumOptions = Enum.GetNames(typeof(T)).ToList<string>();
            }
            catch
            {
                _enumOptions = new List<string>();
                return false;
            }

            return true;
        }

#endregion

        //The unique ideintifier for thie widget
        public string WidgetID;

        //Public constructor
        public UWidget()
        {
            //TODO: Think about a less cpu intensive way of generating a unique identifier
            WidgetID = Guid.NewGuid().ToString();
        }

        private string _name = "WidgetControl";
        [UWidgetPropertyAttribute]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (this._name != value)
                {
                    //Perform a regex to remove likely candidates for breaking compilation of this Name
                    _name = Regex.Replace(value, "[^a-zA-Z0-9%_]", string.Empty);
                }
                _name = value;
            }
        }

#region Binding controls

        //Binding
        [SerializeField]
        private PropertyInfo _boundPropertyInfo = null;
        [SerializeField]
        private FieldInfo _boundFieldInfo = null;
        [SerializeField]
        private MethodInfo _boundMethodInfo = null;
        [SerializeField]
        private object _boundMemberObject = null;
        [SerializeField]
        private string _boundMemberName = "";
        [SerializeField]
        private bool _boundMemberCanWrite = true;
        [SerializeField]
        private int _boundGameObjectID = -1;

        public bool SuppressBindingWarnings = false;

        protected enum eBindingType
        {
            NotSet = 0,
            Property = 1,
            Field = 2,
            Method = 3,
            This = 4
        }

        [SerializeField]
        protected eBindingType BindingType;

        //Access to binding internals for read only queries
        public object BoundObject
        {
            get
            {
                return _boundMemberObject;
            }
        }

        public string BoundMemberName
        {
            get
            {
                return _boundMemberName;
            }
        }

        public bool BoundMemberCanWrite
        {
            get
            {
                return this._boundMemberCanWrite;
            }
        }

        [SerializeField]
        private object _localBoundMember;


        public T GetBoundValue<T>()
        {

            object __refObject;

            if (this._boundMemberObject == null && this._boundGameObjectID != -1)
            {
                this.BindTo(EditorUtility.InstanceIDToObject(this._boundGameObjectID), this._boundMemberName);
            }

            switch (this.BindingType)
            {
                case eBindingType.NotSet:
                    //If binding is not set, create a local var to store the value
                    if (this._localBoundMember == null)
                    {
                        //String don't take a parameterless constructor
                        if (typeof(T) == typeof(string))
                        {
                            this._localBoundMember = string.Empty;
                        }
                        else
                        {
                            //Create the object
                            this._localBoundMember = (T)Activator.CreateInstance<T>();
                        }

                        //Null strings are bad so lets sort that out.
                        if (this._localBoundMember.GetType() == typeof(string))
                        {
                            this._localBoundMember = string.Empty;
                        }
                    }
                    return (T)_localBoundMember;
                case eBindingType.Property:
                    __refObject = this._boundPropertyInfo.GetValue(this._boundMemberObject, null);
                    break;
                case eBindingType.Field:
                    __refObject = this._boundFieldInfo.GetValue(this._boundMemberObject);
                    break;
                case eBindingType.This:
                    __refObject = this._boundMemberObject;
                    break;
                default:
                    throw new Exception("Unexpected binding type in UEditorWidget.GetBoundValue<T>");
            }


            if (__refObject.GetType() == typeof(T) || __refObject.GetType().IsSubclassOf(typeof(T)))
            {

                return (T)__refObject;
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(__refObject, typeof(T));
                }
                //TODO: Improve this error message
                throw new Exception("Failed implicit cast in UEditorWidget.GetBoundValue<T>");
            }
        }

        public void SetBoundValue(object newValue)
        {
            //If the property is flagged readonly then bail out
            if (this._boundMemberCanWrite == false)
            {
                return;
            }

            //This will fire the first time we set a localy bound piece of data.
            if (this.BindingType == eBindingType.NotSet && this._localBoundMember == null)
            {
                this._localBoundMember = newValue;
                return;
            }

            //Check the type is correct
            if (this.GetBoundValueType() != newValue.GetType())
            {
                //Attempt a Q&N conversion
                object __convertedValue = Convert.ChangeType(newValue, this.GetBoundValueType());

                if (__convertedValue == null)
                {
                    throw new Exception("Input value type does not equal bound member type in UEditorWidget.SetBoundValue");
                }

                //Assume the conversion was succsssful
                newValue = __convertedValue;
            }

            switch (this.BindingType)
            {
                case eBindingType.NotSet:
                    //Update the local var
                    this._localBoundMember = newValue;
                    break;
                case eBindingType.Property:
                    this._boundPropertyInfo.SetValue(this._boundMemberObject, newValue, null);
                    break;
                case eBindingType.Field:
                    this._boundFieldInfo.SetValue(this._boundMemberObject, newValue);
                    break;
                case eBindingType.This:
                    this._boundMemberObject = newValue;
                    break;
                default:
                    throw new Exception("Unexpected binding type in UEditorWidget.SetBoundValue<T>");
            }

        }

        public Type GetBoundValueType()
        {
            switch (this.BindingType)
            {
                case eBindingType.NotSet:
                    if (this._localBoundMember == null)
                    {
                        return null;
                    }
                    else
                    {
                        return this._localBoundMember.GetType();
                    }
                case eBindingType.Property:
                    return this._boundPropertyInfo.GetValue(this._boundMemberObject, null).GetType();
                case eBindingType.Field:
                    return this._boundFieldInfo.GetValue(this._boundMemberObject).GetType();
                case eBindingType.This:
                    if (this._boundMemberObject != null)
                    {
                        return this._boundMemberObject.GetType();
                    }
                    else
                    {
                        return null;
                    }
                default:
                    throw new Exception("Unexpected binding type in UEditorWidget.GetBoundValueType<T>");
            }
        }


        /// <summary>
        /// Binds a member of an object variable into a widget
        /// </summary>
        /// <param name="Object">An object variable to be referenced</param>
        /// <param name="MemberName">The member (property/field) to bind to</param>
        /// <returns></returns>
        public virtual bool BindTo(object Object, string MemberName)
        {
            if (Object == null)
            {
                this.BindingType = eBindingType.NotSet;
                return true;
            }

            if (MemberName == null || MemberName == "")
                return false;

            if (Object.GetType().IsSubclassOf((typeof(UnityEngine.Object))) == true)
            {
                this._boundGameObjectID = ((UnityEngine.Object)Object).GetInstanceID();
            }
            else
            {
                if (this.SuppressBindingWarnings == false)
                {
                    Debug.LogWarning("Bound member " + MemberName + " in Widget " + this.Name + " does not derive from UnityEngine.GameObject.\r\bThis bind may fail seralization");
                }
            }

            _boundMemberName = MemberName;
            _boundMemberObject = Object;

            if (MemberName == "this")
            {
                this.BindingType = eBindingType.This;
                return true;
            }

            _boundPropertyInfo = Object.GetType().GetProperty(_boundMemberName);
            if (_boundPropertyInfo != null)
            {
                this.BindingType = eBindingType.Property;
                if (_boundPropertyInfo.CanWrite != true)
                {
                    this._boundMemberCanWrite = false;
                }
                _localBoundMember = _boundPropertyInfo.GetValue(_boundMemberObject, null);
            }
            else
            {
                _boundFieldInfo = Object.GetType().GetField(_boundMemberName);
                if (_boundFieldInfo != null)
                {
                    this.BindingType = eBindingType.Field;
                    _localBoundMember = _boundFieldInfo.GetValue(_boundMemberObject);
                }
                else
                {
                    _boundMethodInfo = Object.GetType().GetMethod(_boundMemberName);
                    if (_boundMethodInfo != null)
                    {
                        this.BindingType = eBindingType.Method;
                    }
                    else
                    {
                        throw new System.Exception("Unable to derterming property type on " + Object.ToString() + ":" + _boundMemberName);
                    }
                }
            }
            return true;
        }

#endregion

        [SerializeField]
        private UEditorPanelBase _parent;

        public UEditorPanelBase parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(this.WidgetID);
        }

    }
}