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

        /// <summary>
        /// Sets up the data structures necessary to display the CustomColorDialog
        /// </summary>
        public CustomColorDialog( IntPtr	handle )
        {
            // create the ChooseColor structure to pass to WinAPI
            _cc.lStructSize = Marshal.SizeOf( _cc );
            _cc.lpfnHook = new CCHookProc(MyHookProc);								//hook function 
            _cc.Flags = ChooseColorFlags.FullOpen | ChooseColorFlags.EnableHook | ChooseColorFlags.RGBInit;	//enable hook
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


            // create the container window to keep the alpha slider and text box controls
            _panelAlpha = new Panel();
            _sliderAlpha = new AlphaSlider();
            _textAlpha	= new TextBox();
            _labelAlpha = new Label();

            _panelAlpha.Controls.Add( _sliderAlpha );
            _panelAlpha.Controls.Add(_labelAlpha);
            _panelAlpha.Controls.Add( _textAlpha );

            _panelAlpha.Width = 34;
            _panelAlpha.Height = 300;

            _sliderAlpha.Location = new Point(5, 3);
            _sliderAlpha.Width = 26;
            _sliderAlpha.Height = 247;

            _labelAlpha.Height = 16;
            _labelAlpha.Width = 36;
            _labelAlpha.Text = "Alpha";
            _labelAlpha.Location = new Point(0, 250);

            _textAlpha.Height = 16;
            _textAlpha.Width  = 30;
            _textAlpha.MaxLength = 3;
            _textAlpha.Location = new Point(2, 272);    
        }

        // Initialize the color. Alpha value in color will initialize alpha component.
        public CustomColorDialog( IntPtr handle, Color initialColor)
            : this(handle)
        {
            IntialColor = initialColor;
            _sliderAlpha.InitAlphaValue = _initialColor.A;
        }

        // Initialize the color. Additional alpha value will overwrite the alpha value specified in initialColor.
        public CustomColorDialog(IntPtr handle, Color initialColor, int initialAlphaValue)
            : this(handle, initialColor)
        {
            _sliderAlpha.InitAlphaValue = initialAlphaValue;
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
            
            return NativeMethods.ChooseColor( ref _cc );
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
                
            // Behaviour is dependant on the message received
            switch( msg )
            {
                    // We're not interested in every possible message; just return a NULL for those we don't care about
                default:
                {
                    return IntPtr.Zero;
                }
            

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
