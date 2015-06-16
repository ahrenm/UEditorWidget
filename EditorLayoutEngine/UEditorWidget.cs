namespace uScaff.UEditorWidgets
{
    using System;
    using System.Reflection;
    using UnityEngine;

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

    public enum eLayoutStyle
    {
        NotSet = 0,
        Horizontal = 1,
        Vertical = 2
    }


    public interface IUEditorWidgetRenderable
    {
        string Name { get; set; }
        void Render();
        UEditorPanelBase parent { get; set; }
    }

    public interface IUEditorWidgetClickable
    {
        event UEditorWidget_OnClick OnClick;
    }

    public delegate void UEditorWidget_OnClick(IUEditorWidgetClickable sender, EventArgs e);


    /// <summary>
    /// The base class renderable, bound objects
    /// </summary>
    public abstract class UEditorBase :IUEditorWidgetRenderable
    {
        public string Name { get; set;}


#region Binding controls

        //Binding
        protected PropertyInfo _boundPropertyInfo = null;
        protected FieldInfo _boundFieldInfo = null;
        protected MethodInfo _boundMethodInfo = null;
        protected object _boundMemberObject = null;
        protected string _boundMemberName;
        protected enum eBindingType
        {
            NotSet = 0,
            Property = 1,
            Field = 2,
            Method = 3
        }
        protected eBindingType BindingType;

        public virtual bool BindTo(object Object, string MemberName)
        {
            if (Object == null || MemberName == null || MemberName == "")
                return false;

            _boundMemberName = MemberName;
            _boundMemberObject = Object;

            _boundPropertyInfo = Object.GetType().GetProperty(_boundMemberName);
            if (_boundPropertyInfo != null)
            {
                this.BindingType = eBindingType.Property;
            }
            else
            {
                _boundFieldInfo = Object.GetType().GetField(_boundMemberName);
                if (_boundFieldInfo != null)
                {
                    this.BindingType = eBindingType.Field;
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

        public abstract void Render();

        private UEditorPanelBase _parent;
        public UEditorPanelBase parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
    }

    /// <summary>
    /// The base class for all widgets
    /// </summary>
    public abstract class UEditorWidgetBase : UEditorBase
    {
        //Used to initalise the cached style data on the first OnGUI call

#region Style Properties
        
        private string _cachedBaseStyle = "";
        public string BaseStyle
        {
            get 
            {
                if (this._cachedBaseStyle == "")
                    return this.Style.name;
                    
                else
                    return this._cachedBaseStyle;
            }
            set
            {
                if (_cachedBaseStyle != value)
                {
                    _cachedBaseStyle = value;
                    this._StyleIsDirty = true;
                }
            }
        }
        

        protected bool _StyleIsDirty = true;
        public bool StyleIsDirty
        {
            get { return _StyleIsDirty; }
        }

        private RectOffset _cachedMargin;
        public RectOffset Margin
        {
            get 
            {
                if (this._cachedMargin == null)
                    _cachedMargin = new RectOffset();
                return this._cachedMargin; 
            }
            set 
            {
                if (_cachedMargin == null)
                    _cachedMargin = new RectOffset();
                if (_cachedMargin != value)
                {
                    this._StyleIsDirty = true;
                    this._cachedMargin = value;
                }
            }
        }

        private RectOffset _cachedPadding;
        public RectOffset Padding
        {
            get 
            {
                if (_cachedPadding == null)
                    _cachedPadding = new RectOffset();
                return _cachedPadding; 
            }
            set 
            {
                if (_cachedPadding == null)
                    _cachedPadding = new RectOffset();
                if ( _cachedPadding != value)
                {
                    this._StyleIsDirty = true;
                    this._cachedPadding = value;
                }
            }
        }

        private RectOffset _cachedBorder;
        public RectOffset Border
        {
            get
            {
                if (_cachedBorder == null)
                    _cachedBorder = new RectOffset();
                return _cachedBorder;
            }
            set
            {
                if (_cachedBorder == null)
                    _cachedBorder = new RectOffset();
                if (_cachedBorder != value)
                {
                    this._StyleIsDirty = true;
                    this._cachedBorder = value;
                }
            }
        }


        private float _cachedHeight = -1;
        public float Height
        {
            get { return this._cachedHeight; }
            set 
            {
                if (_cachedHeight != value)
                {
                    this._StyleIsDirty = true;
                    this._cachedHeight = value;
                }
            }
        }

        private float _cachedWidth = -1;
        public float Width
        {
            get { return this._cachedWidth; }
            set 
            {
                if (_cachedWidth != value)
                {
                    this._StyleIsDirty = true;
                    this._cachedWidth = value;
                }
            }
        }

#endregion

        protected GUIStyle Style;

        protected eWidgetType _widgetType;
        public eWidgetType WidgetType
        {
            get { return _widgetType; }
        }

        public UEditorWidgetBase(eWidgetType type)
        {
            _widgetType = type;
        }


        public override void Render()
        {
            if (_StyleIsDirty)
            {
                if (this._cachedBaseStyle != "")
                    this.Style = new GUIStyle(GUI.skin.FindStyle(_cachedBaseStyle));
                else
                {
                    switch (_widgetType)
                    {
                        case eWidgetType.NotSet:
                            break;
                        case eWidgetType.Generic:
                            this.Style = new GUIStyle();
                            break;
                        case eWidgetType.Toggle:
                            this.Style = new GUIStyle(GUI.skin.toggle);
                            break;
                        case eWidgetType.Label:
                            this.Style = new GUIStyle(GUI.skin.label);
                            break;
                        case eWidgetType.TextField:
                            this.Style = new GUIStyle(GUI.skin.textField);
                            break;
                        case eWidgetType.TextArea:
                            this.Style = new GUIStyle(GUI.skin.textArea);
                            break;
                        case eWidgetType.Button:
                            this.Style = new GUIStyle(GUI.skin.button);
                            break;
                        case eWidgetType.Foldout:
                            this.Style = new GUIStyle(GUI.skin.FindStyle("Foldout"));
                            break;
                        case eWidgetType.PanelArea:
                            this.Style = new GUIStyle();
                            break;
                        case eWidgetType.PanelVertical:
                            this.Style = new GUIStyle();
                            break;
                        case eWidgetType.PanelHorizontal:
                            this.Style = new GUIStyle();
                            break;
                        case eWidgetType.PanelScroll:
                            this.Style = new GUIStyle();
                            break;
                        case eWidgetType.PanelToggleArea:
                            this.Style = new GUIStyle();
                            break;
                        default:
                            break;
                    }
                }

                if (_cachedMargin != null)
                    this.Style.margin = this._cachedMargin;

                if (_cachedPadding != null)
                    this.Style.padding = this._cachedPadding;

                if (_cachedHeight != -1)
                    this.Style.fixedHeight = this._cachedHeight;

                if (_cachedWidth != -1)
                    this.Style.fixedWidth = this._cachedWidth;

            }
        }

    }


    /// <summary>
    /// The base class for text based widgets
    /// </summary>
    public abstract class UEditorWidgetTextBase : UEditorWidgetBase
    {
        private Font _cachedFont;
        public Font Font
        {
            get { return this._cachedFont; }
            set 
            {
                if (_cachedFont == null || _cachedFont != value)
                {
                    this._StyleIsDirty = true;
                    this._cachedFont = value;
                }
            }
        }

        private int _cachedFontSize = -1;
        public int FontSize
        {
            get { return this._cachedFontSize; }
            set 
            {
                if (_cachedFontSize != value)
                {
                    this._StyleIsDirty = true;
                    this._cachedFontSize = value;
                }
            }
        }

        private FontStyle _cachedFontStyle = FontStyle.Normal;
        public FontStyle FontStyle
        {
            get { return this._cachedFontStyle; }
            set 
            { 
                if (_cachedFontStyle != value)
                {
                    this._StyleIsDirty = true;
                    this._cachedFontStyle = value; 
                }
                
            }
        }

        private bool? _cachedWordWrap;
        public bool WordWrap
        {
            get { return this._cachedWordWrap.Value; }
            set
            {
                if (_cachedWordWrap == null || _cachedWordWrap.Value != value)
                {
                    this._StyleIsDirty = true;
                    this._cachedWordWrap = value;
                }
            }
        }

        private TextAnchor _cachedAlighment = TextAnchor.UpperLeft;
        public TextAnchor Alignment
        {
            get { return _cachedAlighment; }
            set 
            {
                if (_cachedAlighment != value)
                {
                    this._StyleIsDirty = true;
                    this._cachedAlighment = value;
                }
            }
        }

        //Constructor
        public UEditorWidgetTextBase(eWidgetType type) :base (type) 
        {
            this.Margin = new RectOffset(4, 4, 2, 2);
            this.Border = new RectOffset(3, 3, 3, 3);
            this.Padding = new RectOffset(3, 3, 1, 2);
        }


        public override void Render()
        {
            if (_StyleIsDirty)
            {
                base.Render();

                if (this._cachedFont != null)
                    this.Style.font = this._cachedFont;

                if (this._cachedFontSize != -1)
                    this.Style.fontSize = this._cachedFontSize;

                this.Style.fontStyle = this._cachedFontStyle;

                if (this._cachedWordWrap != null)
                    this.Style.wordWrap = this._cachedWordWrap.Value;

                this.Style.alignment = this._cachedAlighment;
            }
            else
                base.Render();
            
        }
    }

}