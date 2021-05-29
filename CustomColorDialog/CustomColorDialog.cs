using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using AlphaSliderControl;
using System.Windows.Forms;
    

namespace CustomCommonDialog
{
    /// <summary>
    /// Summary description for CustomColorDialog.
    /// </summary>
    public class CustomColorDialog : IDisposable
    {
        // the CHOOSECOLOR structure, used to control the appearance and behaviour of the OpenFileDialog
        private ChooseColor							_cc;
        // the slider control to change the alpha values
        private AlphaSlider							_sliderAlpha;
        // the textbox to show current alpha value
        private TextBox								_textAlpha;
        // The label to show 'Alpha'
        private Label _labelAlpha;
        // the panel to contain the slider and text box
        private Panel								_panelAlpha;

        private int[] _customColors = new int[] { 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff, 0xffffff };

        // final color value
        private System.Drawing.Color _initialColor = Color.FromArgb(255, Color.Black);   // Initialize with a solid black color

        // COLOROKSTRING message code
        private UInt32 _colorOKMessagecode = 0;

        /// <summary>
        /// Sets up the data structures necessary to display the CustomColorDialog
        /// </summary>
        public CustomColorDialog( IntPtr	handle )
        {
            // create the ChooseColor structure to pass to WinAPI
            _cc.lStructSize = Marshal.SizeOf( _cc );
            _cc.lpfnHook = new CCHookProc(MyHookProc);								//hook function 
            _cc.Flags = ChooseColorFlags.FullOpen | ChooseColorFlags.EnableHook | ChooseColorFlags.RGBInit /* | ChooseColorFlags.SoliDcolor */;	//enable hook
            _cc.hwndOwner = handle;													//set the owner window
            Int32	temp = 0;
            int size = Marshal.SizeOf(temp);
            IntPtr CustColor = Marshal.AllocCoTaskMem( 16 * size);

            int cnt = 0;
            for (int i = 0; i < (16 * size); i += size)
            {
                IntPtr ptr = (IntPtr)(CustColor.ToInt32() + i);
                Marshal.WriteInt32(ptr, _customColors[cnt]);
                cnt++;
            }

            //fill in the value to load custom colors with these values
            _cc.lpCustColors = CustColor;											//set the custom color buffer

            _colorOKMessagecode = NativeMethods.RegisterWindowMessage(new StringBuilder("commdlg_ColorOK"));

            // create the container window to keep the alpha slider and text box controls
            _panelAlpha = new Panel();
            _sliderAlpha = new AlphaSlider();
            _textAlpha	= new TextBox();
            _labelAlpha = new Label();

            _panelAlpha.Controls.Add( _sliderAlpha );
            _panelAlpha.Controls.Add(_labelAlpha);
            _panelAlpha.Controls.Add( _textAlpha );

            _panelAlpha.BorderStyle = BorderStyle.None;
            _panelAlpha.Width = 36;
            _panelAlpha.Height = 300;//268;

            _sliderAlpha.Height = 240;
            _sliderAlpha.Dock = DockStyle.Fill;

            _labelAlpha.Height = 16;
            _labelAlpha.Width = 6;
            _labelAlpha.Text = "Alpha";
            _labelAlpha.Dock = DockStyle.Bottom;

            _textAlpha.Height = 16;
            _textAlpha.Width  = 10;
            _textAlpha.MaxLength = 3;
            _textAlpha.Dock = DockStyle.Bottom;
        }

        // Initialize the color. Alpha value in color will initialize alpha component.
        public CustomColorDialog( IntPtr handle, Color initialColor)
            : this(handle)
        {
            IntialColor = initialColor;
            _sliderAlpha.InitAlphaValue = _initialColor.A;
        }

        public CustomColorDialog(IntPtr handle, Color initialColor, int[] customColor)
            : this(handle)
        {
            IntialColor = initialColor;
            _sliderAlpha.InitAlphaValue = _initialColor.A;

            InitCustomColors(customColor);
        }

        // Initialize the color. Additional alpha value will overwrite the alpha value specified in initialColor.
        public CustomColorDialog(IntPtr handle, Color initialColor, int initialAlphaValue)
            : this(handle, initialColor)
        {
            _sliderAlpha.InitAlphaValue = initialAlphaValue;
        }

        // Initialize the color. Additional alpha value will overwrite the alpha value specified in initialColor.
        public CustomColorDialog(IntPtr handle, Color initialColor, int initialAlphaValue, int[] customColor)
            : this(handle, initialColor, initialAlphaValue)
        {
            InitCustomColors(customColor);
        }

        void InitCustomColors(int[] customColor)
        {
            int customColorCount = customColor.GetLength(0);
            int minCustomColorcount = Math.Min(16, customColorCount);

            for (int i = 0; i < minCustomColorcount; i++)
            {
                _customColors[i] = customColor[i];
            }

            Int32 temp = 0;
            int size = Marshal.SizeOf(temp);
            IntPtr CustColor = Marshal.AllocCoTaskMem(16 * size);

            int cnt = 0;
            for (int i = 0; i < (16 * size); i += size)
            {
                IntPtr ptr = (IntPtr)(CustColor.ToInt32() + i);
                Marshal.WriteInt32(ptr, _customColors[cnt]);
                cnt++;
            }

            //fill in the value to load custom colors with these values
            _cc.lpCustColors = CustColor;											//set the custom color buffer
        }

        /// <summary>
        /// The finalizer will release the unmanaged memory, if I should forget to call Dispose
        /// </summary>
        ~CustomColorDialog()
        {
            Dispose( false );
        }

        /// <summary>
        /// Display the ChooseColor dialog and allow user interaction
        /// </summary>
        /// <returns>true if the user clicked OK, false if they clicked cancel (or close)</returns>
        public bool ShowDialog()
        {
            _cc.rgbResult = ColorToCOLORREF(_initialColor);
            _sliderAlpha.InitAlphaValue = _initialColor.A;
            
            bool result = NativeMethods.ChooseColor( ref _cc );

            Marshal.Copy(_cc.lpCustColors, _customColors, 0, 16);

            return result;
        }

        /// <summary>
        /// Convert color to COLORREF(int:0x00bbggrr)
        /// </summary>
        private int ColorToCOLORREF(Color c)
        {
            int cr = c.B;
            cr = ((cr << 8) | c.G);
            cr = ((cr << 8) | c.R);
            return cr;
        }

        /// <summary>
        /// Convert COLORREF(int:0x00bbggrr) to Color
        /// </summary>
        /// <param name="cr">give me COLORREF</param>
        private Color COLORREFToColor(int cr)
        {
            int R = (cr & 0xFF);
            int G = ((cr >> 8) & 0xFF);
            int B = ((cr >> 16) & 0xFF);
            return Color.FromArgb(R, G, B);
        }

        /// <summary>
        /// The hook procedure for window messages generated by the FileOpenDialog
        /// </summary>
        /// <param name="hWnd">the handle of the window at which this message is targeted</param>
        /// <param name="msg">the message identifier</param>
        /// <param name="wParam">message-specific parameter data</param>
        /// <param name="lParam">mess-specific parameter data</param>
        /// <returns></returns>

const UInt16 WM_CTLCOLORMSGBOX = 0x132;
const UInt16 WM_CTLCOLOREDIT = 0x133;
const UInt16 WM_CTLCOLORLISTBOX = 0x134;
const UInt16 WM_CTLCOLORBTN = 0x135;
const UInt16 WM_CTLCOLORDLG = 0x136;
const UInt16 WM_CTLCOLORSCROLLBAR = 0x137;
const UInt16 WM_CTLCOLORSTATIC = 0x138;

const UInt16 WM_DESTROY = 0x02;
const UInt16 WM_MOVE = 0x03;
const UInt16 WM_SIZE = 0x05;
const UInt16 WM_ACTIVATE = 0x06;
const UInt16 WM_SETFOCUS = 0x07;
const UInt16 WM_KILLFOCUS = 0x08;

const UInt16 WM_CLOSE = 0x10;
const UInt16 WM_QUIT = 0x12;
const UInt16 WM_SETCURSOR = 0x20;
const UInt16 WM_WINDOWPOSCHANGING = 0x46;
const UInt16 WM_WINDOWPOSCHANGED = 0x47;

const UInt16 WM_MOUSEFIRST = 0x200;
const UInt16 WM_MOUSEMOVE = 0x200;
const UInt16 WM_LBUTTONDOWN = 0x201;
const UInt16 WM_LBUTTONUP = 0x202;
const UInt16 WM_LBUTTONDBLCLK = 0x203;
const UInt16 WM_RBUTTONDOWN = 0x204;
const UInt16 WM_RBUTTONUP = 0x205;
const UInt16 WM_RBUTTONDBLCLK = 0x206;
const UInt16 WM_MBUTTONDOWN = 0x207;
const UInt16 WM_MBUTTONUP = 0x208;
const UInt16 WM_MBUTTONDBLCLK = 0x209;
const UInt16 WM_MOUSEWHEEL = 0x20A;
const UInt16 WM_MOUSEHWHEEL = 0x20E;

        
        public IntPtr MyHookProc( IntPtr hWnd, UInt16 msg, Int32 wParam, Int32 lParam )
        {
            Trace.WriteLine("MyHookProc: " + msg.ToString("X"));
            // return if invalid window
            if (hWnd == IntPtr.Zero)
                return IntPtr.Zero;

            //the message passed by AlphaSlider control
            if( msg == 0x5050 )
            {
                //update the text box value
                Trace.WriteLine("AA: " + wParam.ToString());
                _textAlpha.Text = wParam.ToString();
            }
                
            if (msg == _colorOKMessagecode)
            {
                Trace.WriteLine("COLOROKSTRNG: " + wParam.ToString());

            }

            // Behaviour is dependant on the message received
            switch( msg )
            {
                    // We're not interested in every possible message; just return a NULL for those we don't care about
                default:
                {
                        Trace.WriteLine("Unhandled: " + msg.ToString("X"));
                    return IntPtr.Zero;
                }
                case WM_DESTROY:
                    System.Diagnostics.Trace.WriteLine("WM_DESTROY");
                    On_WM_DESTROY_Event(hWnd, wParam, lParam);
                    break;
                case WM_MOVE:
                case WM_ACTIVATE:
                case WM_SETFOCUS:
                case WM_KILLFOCUS:
                case 0x20:
                case 0x46:
                case 0x47:
                case 0x7F:
                case 0x82:
                case 0x84:
                case 0x86:
                case 0x90:
                case 0xA0:
                case 0x111:
                case 0x281:
                case 0x282:
                    break;
            
                case WM_MOUSEMOVE:
                case WM_LBUTTONDOWN:
                case WM_LBUTTONUP:
                case WM_LBUTTONDBLCLK:
                case WM_RBUTTONDOWN:
                case WM_RBUTTONUP:
                case WM_RBUTTONDBLCLK:
                case WM_MBUTTONDOWN:
                case WM_MBUTTONUP:
                case WM_MBUTTONDBLCLK:
                case WM_MOUSEWHEEL:
                case WM_MOUSEHWHEEL:
                    break;

                case WM_CTLCOLORMSGBOX:
                    //System.Diagnostics.Trace.WriteLine("WM_CTLCOLORMSGBOX");
                    break;
                case WM_CTLCOLOREDIT:
                    System.Diagnostics.Trace.WriteLine("WM_CTLCOLOREDIT");
                    break;
                case WM_CTLCOLORLISTBOX:
                    //System.Diagnostics.Trace.WriteLine("WM_CTLCOLORLISTBOX");
                    break;
                case WM_CTLCOLORBTN:
                    //System.Diagnostics.Trace.WriteLine("WM_CTLCOLORBTN");
                    //On_WM_CTLCOLORBTN_Event(hWnd, wParam, lParam);
                    break;
                case WM_CTLCOLORDLG:
                    System.Diagnostics.Trace.WriteLine("WM_CTLCOLORDLG");
                    break;
                case WM_CTLCOLORSCROLLBAR:
                    System.Diagnostics.Trace.WriteLine("WM_CTLCOLORSCROLLBAR");
                    break;
                case WM_CTLCOLORSTATIC:
                    System.Diagnostics.Trace.WriteLine("WM_CTLCOLORSTATIC");    //?
                    break;

                case 0xC115:
                    return IntPtr.Zero;

                case WM_CLOSE:
                    System.Diagnostics.Trace.WriteLine("WM_CLOSE");
                    break;
                case WM_QUIT:
                    System.Diagnostics.Trace.WriteLine("WM_QUIT");
                    break;

                // WM_INITDIALOG - at this point the ChooseColorDialog exists, so we pull the user-supplied control
                // into the FileOpenDialog now, using the SetParent API.
                case WindowMessage.InitDialog:
                {
                    //set the parent window of slider control to recieve change in value messages
                    _sliderAlpha.SetParent(hWnd);

                    //increase the width of the default dialog box
                    //place the slider and text box controls on the right most
                    POINT topLeft = new POINT();
                    POINT bottomRight = new POINT();
                    IntPtr ipNotify = new IntPtr( lParam );
                    ChooseColor cc = (ChooseColor)Marshal.PtrToStructure( ipNotify, typeof(ChooseColor) );
                    IntPtr hWndParent = NativeMethods.GetParent( hWnd );
                    NativeMethods.SetParent( _panelAlpha.Handle, hWnd);
                    RECT rc = new RECT();
                    NativeMethods.GetWindowRect( hWnd, ref rc );
                    topLeft.X = rc.right;
                    topLeft.Y = rc.top;
                    NativeMethods.ScreenToClient( hWnd, ref topLeft );
                    bottomRight.X = rc.right;
                    bottomRight.Y = rc.bottom;
                    NativeMethods.ScreenToClient( hWnd, ref bottomRight );

                    Rectangle rcClient = _panelAlpha.ClientRectangle;
                    NativeMethods.MoveWindow( hWnd, rc.left, rc.top, bottomRight.X+rcClient.Width+10, bottomRight.Y+28, true );
                    
                    return IntPtr.Zero;
                }
                    // WM_SIZE - the OpenFileDialog has been resized, so we'll resize the content and user-supplied
                    // panel to fit nicely
                case WindowMessage.Size:
                {
                    PlaceCustomControls( hWnd );
                    return IntPtr.Zero;
                }

                case 0xC072:
                case WindowMessage.ColorOK:
                {
                    //get the current ChooseColor structure
                    IntPtr ipNotify = new IntPtr(lParam);
                    ChooseColor cc = (ChooseColor)Marshal.PtrToStructure(ipNotify, typeof(ChooseColor));
                    IntPtr hWndParent = NativeMethods.GetParent(hWnd);

                    //get the alpha value from slider control
                    //
                    int outAlpha = _sliderAlpha.Value;
                    System.Drawing.Color color;
                    if (int.TryParse(_textAlpha.Text, out outAlpha) == true)
                    {
                        color = Color.FromArgb(outAlpha, COLORREFToColor(cc.rgbResult));
                    }
                    else
                    {
                        color = Color.FromArgb(_sliderAlpha.Value, COLORREFToColor(cc.rgbResult));
                    }
                     
                    Trace.WriteLine("One " + color.ToString());
                    return IntPtr.Zero;
                }
            }

            return IntPtr.Zero;
        }

        private void On_WM_DESTROY_Event(IntPtr hWnd, int wParam, int lParam)
        {
            //ChooseColor cc = (ChooseColor)Marshal.PtrToStructure(ipNotify, typeof(ChooseColor));
            System.Diagnostics.Trace.WriteLine("On_WM_DESTROY_Event " + hWnd.ToString("X") + " wP: " + wParam.ToString("x") + " lP: " + lParam.ToString("x"));
        }

        private void On_WM_CTLCOLORBTN_Event(IntPtr hWnd, int wParam, int lParam)
        {
            System.Diagnostics.Trace.WriteLine("On_WM_CTLCOLORBTN_Event " + hWnd.ToString("X") + " wP: " + wParam.ToString("x") + " lP: " + lParam.ToString("x"));
        }

        /// <summary>
        /// Layout the content of the ChooseColorDialog, according to the overall size of the dialog
        /// </summary>
        /// <param name="hWnd">handle of window that received the WM_SIZE message</param>
        private void PlaceCustomControls( IntPtr hWnd )
        {
            IntPtr hWndParent = NativeMethods.GetParent( hWnd );
            NativeMethods.SetParent( _panelAlpha.Handle, hWnd);
            RECT rc = new RECT();
            NativeMethods.GetWindowRect( hWnd, ref rc );
            POINT topLeft;
            topLeft.X = rc.right;
            topLeft.Y = rc.top;
            NativeMethods.ScreenToClient( hWnd, ref topLeft );
            POINT bottomRight;
            bottomRight.X = rc.right;
            bottomRight.Y = rc.bottom;
            NativeMethods.ScreenToClient( hWnd, ref bottomRight );

            Rectangle rcClient = _panelAlpha.ClientRectangle;// .ClientRectangle();

            NativeMethods.MoveWindow( _panelAlpha.Handle, rc.right-rc.left-rcClient.Width-10, 0, rcClient.Width, rcClient.Height, true );
        }

        /// <summary>
        /// returns the path currently selected by the user inside the OpenFileDialog
        /// </summary>
        public System.Drawing.Color SelectedColor
        {
            get
            {
                //return _color;//Marshal.PtrToStringUni( _fileNameBuffer );
                int outAlpha;
                if (int.TryParse(_textAlpha.Text, out outAlpha) == true)
                {
                    if (outAlpha > 256)
                        outAlpha = 255;
                    else if (outAlpha < 0)
                        outAlpha = 0;
                    return Color.FromArgb(outAlpha, COLORREFToColor(_cc.rgbResult));
            }
                else
                {
                    return Color.FromArgb(_sliderAlpha.Value, COLORREFToColor(_cc.rgbResult));
                }
            }
        }

        public int[] CustomColors
        {
            get { return _customColors; }
        }

        public System.Drawing.Color IntialColor
        {
            get
            {
                return _initialColor;
            }
            set
            {
                _initialColor = value;
            }
        }

        /// <summary>
        /// Initialize  Alpha (transparent) Values: 0-255
        /// </summary>
        public int InitAlphaValue
        {
            get
            {
                return _sliderAlpha.InitAlphaValue;
            }
            set
            {
                _sliderAlpha.InitAlphaValue = value;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose( true );
        }

        /// <summary>
        /// Free any unamanged memory used by this instance of OpenFileDialog
        /// </summary>
        /// <param name="disposing">true if called by Dispose, false otherwise</param>
        public void Dispose( bool disposing )
        {
            if( disposing )
            {
                GC.SuppressFinalize( this );
            }

        }

        #endregion
    }
}
