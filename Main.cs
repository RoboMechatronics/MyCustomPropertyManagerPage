using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace MyCustomPropertyManagerPage
{
    public class Main : ISwAddin
    {

        private SldWorks swApp;
        private int swAddinID = 0;
        private CommandManager swCommandManager;
        private CommandGroup swCommandGroup;
        private const string swCommandGroupTitle = "My SOLIDWORKS Add-ins";
        private const string swCommandGroupToolTip = "Custom Command";

        public string[] IconLists = {   "../../Resources/MergedTwoImages20x20.png",
                                        "../../Resources/MergedTwoImages32x32.png",
                                        "../../Resources/MergedTwoImages40x40.png",
                                        "../../Resources/MergedTwoImages64x64.png",
                                        "../../Resources/MergedTwoImages96x96.png",
                                        "../../Resources/MergedTwoImages128x128.png"};

        public string[] MainIconLists = {   "../../Resources/column3D_20x20px.png",
                                            "../../Resources/column3D_32x32px.png",
                                            "../../Resources/column3D_40x40px.png",
                                            "../../Resources/column3D_64x64px.png",
                                            "../../Resources/column3D_96x96px.png",
                                            "../../Resources/column3D_128x128px.png"};

        public int[] swCommandID = { 1, 2 };

        public bool ConnectToSW(object ThisSW, int Cookie)
        {
            swApp = (SldWorks)ThisSW;
            swAddinID = Cookie;

            _ = swApp.SetAddinCallbackInfo2(0, this, swAddinID);

            swCommandManager = swApp.GetCommandManager(swAddinID);

            AddCommandMgr();

            return true;
        }

        public bool DisconnectFromSW()
        {
            RemoveCommandMgr();

            swApp = null;

            return true;
        }

        public void AddCommandMgr()
        {
            int Error = 0;
            swCommandGroup = swCommandManager.CreateCommandGroup2(swAddinID,
                                                                    swCommandGroupTitle,
                                                                    swCommandGroupToolTip,
                                                                    "",
                                                                    -1,
                                                                    true,
                                                                    Error);

            swCommandGroup.IconList = IconLists;
            swCommandGroup.MainIconList = MainIconLists;

            string Name;
            int Position;
            string HintString;
            string ToolTip;
            int ImageListIndex;
            string CallbackFunction;
            string EnableMethod;
            int MenuTBOption = (int)(swCommandItemType_e.swMenuItem | swCommandItemType_e.swToolbarItem);
            int[] swCommandItemIndex = { 0, 0 };

            Name = "Boss-Extrude (by HienPhan)";
            Position = 0;
            HintString = "";
            ToolTip = "";
            ImageListIndex = 0;
            CallbackFunction = "CusBossExtrudeCmd";
            EnableMethod = "EnableCusBossExtrudeCmd";

            swCommandItemIndex[0] = swCommandGroup.AddCommandItem2(Name, Position, HintString, ToolTip, ImageListIndex, CallbackFunction, EnableMethod, swCommandID[0], MenuTBOption);

            Name = "Cut-Extrude (by HienPhan)";
            Position = 1;
            HintString = "";
            ToolTip = "";
            ImageListIndex = 1;
            CallbackFunction = "CusCutExtrudeCmd";
            EnableMethod = "EnableCusCutExtrudeCmd";

            swCommandItemIndex[1] = swCommandGroup.AddCommandItem2(Name, Position, HintString, ToolTip, ImageListIndex, CallbackFunction, EnableMethod, swCommandID[1], MenuTBOption);

            swCommandGroup.HasToolbar = true;
            swCommandGroup.HasMenu = true;
            swCommandGroup.Activate();
        }

        public void CusBossExtrudeCmd()
        {
            CusBossExtrudePMPage ppage;
            ppage = new CusBossExtrudePMPage(this);
            ppage.Show();
        }

        public int EnableCusBossExtrudeCmd()
        {
            if (swApp.ActiveDoc != null)
                return 1;
            else
                return 0;
        }

        public void CusCutExtrudeCmd()
        {
            CusCutExtrudePMPage ppage;
            ppage = new CusCutExtrudePMPage(this);
            ppage.Show();
        }

        public int EnableCusCutExtrudeCmd()
        {
            if (swApp.ActiveDoc != null)
                return 1;
            else
                return 0;
        }

        public void RemoveCommandMgr()
        {
            swCommandManager.RemoveCommandGroup2(swAddinID, false);
            swCommandManager = null;
        }

        #region COM Registration
        [ComRegisterFunction]
        public static void RegisterFunction(Type t)
        {
            RegistryKey mLocalMachine = Registry.LocalMachine;

            string subKey = "SOFTWARE\\SOLIDWORKS\\ADDINS\\{" + t.GUID.ToString() + "}";
            RegistryKey mRegistryKey = mLocalMachine.CreateSubKey(subKey);

            mRegistryKey.SetValue(null, 0);
            mRegistryKey.SetValue("Description", "My SOLIDWORKS Add-In");
            mRegistryKey.SetValue("Title", "My Add-ins");
        }

        [ComUnregisterFunction]
        public static void UnregisterFunction(Type t)
        {
            RegistryKey mLocalMachine = Registry.LocalMachine;

            string subkey = "SOFTWARE\\SOLIDWORKS\\ADDINS\\{" + t.GUID.ToString() + "}";
            mLocalMachine.DeleteSubKey(subkey);
        }
        #endregion COM Registration

        public SldWorks SwApp
        {
            get { return swApp; }
        }
        public void ConvertFACTOR(double DisplayUnit, ref double FACTOR)
        {
            switch (DisplayUnit)
            {
                case (int)swLengthUnit_e.swANGSTROM:
                    // 1 A = 10^-10 m
                    FACTOR = Math.Pow(10, -10);
                    break;

                case (int)swLengthUnit_e.swCM:
                    // 1 cm = 10^-2 m
                    FACTOR = Math.Pow(10, -2);
                    break;

                case (int)swLengthUnit_e.swFEET:
                    // 1 ft = 304.8 m
                    FACTOR = 304.8;
                    break;

                case (int)swLengthUnit_e.swFEETINCHES:
                    break;

                case (int)swLengthUnit_e.swINCHES:
                    // 1 inches = 0.0254 m
                    FACTOR = 0.0254;
                    break;

                case (int)swLengthUnit_e.swMETER:
                    FACTOR = 1;
                    break;

                case (int)swLengthUnit_e.swMICRON:
                    // 1 um = 10^-6 m;
                    FACTOR = Math.Pow(10, -6);
                    break;

                case (int)swLengthUnit_e.swMIL:
                    // 1 mils = 2.54 x 10^-5 m
                    FACTOR = 2.54 * Math.Pow(10, -5);
                    break;

                case (int)swLengthUnit_e.swMM:
                    // 1 mm = 10^-3 m;
                    FACTOR = Math.Pow(10, -3);
                    break;

                case (int)swLengthUnit_e.swNANOMETER:
                    // 1 nm = 10^-9 mm;
                    FACTOR = Math.Pow(10, -9);
                    break;

                case (int)swLengthUnit_e.swUIN:
                    break;

                default:
                    break;
            }
        }
        public void ConvertAngle(double DisplayUnit, ref double FACTOR)
        {
            switch (DisplayUnit)
            {
                case (int)swAngleUnit_e.swDEG_MIN:
                    break;

                case (int)swAngleUnit_e.swDEG_MIN_SEC:
                    break;

                case (int)swAngleUnit_e.swDEGREES:
                    // 180deg = 1PI rad
                    FACTOR = Math.PI / 180;
                    break;

                case (int)swAngleUnit_e.swRADIANS:
                    FACTOR = 1;
                    break;

                default:
                    break;
            }
        }
    }
    public class CusBossExtrudePMPage
    {
        #region Members
        public Main swAddin;
        private SldWorks swApp;
        private PropertyManagerPage2 swPropertyPage;
        private myPropertyManagerPageHandler handler;
        private ModelDoc2 swModel;

        //Groups
        IPropertyManagerPageGroup From_group;
        IPropertyManagerPageGroup Direction_1_group;
        IPropertyManagerPageGroup Direction_2_group;
        IPropertyManagerPageGroup Thin_Feature_group;
        IPropertyManagerPageGroup Selected_Contours_group;

        //Controls
        PropertyManagerPageCombobox From_Combobox;

        PropertyManagerPageCombobox Direction_1_Combobox;
        PropertyManagerPageBitmapButton Direction_1_Reverse_button_bitmap;
        PropertyManagerPageSelectionbox Direction_1_SelectedBox;
        PropertyManagerPageLabel Dimensions_1_Label;
        PropertyManagerPageNumberbox Dimension_1_value;
        PropertyManagerPageNumberbox Draft_1_Angle;
        PropertyManagerPageBitmapButton Draft_1_button_bitmap;
        PropertyManagerPageCheckbox DraftOutward_1_checkbox;

        PropertyManagerPageCombobox Direction_2_Combobox;
        PropertyManagerPageBitmapButton Direction_2_Reverse_button_bitmap;
        PropertyManagerPageSelectionbox Direction_2_SelectedBox;
        PropertyManagerPageLabel Dimensions_2_Label;
        PropertyManagerPageNumberbox Dimension_2_value;
        PropertyManagerPageNumberbox Draft_2_Angle;
        PropertyManagerPageBitmapButton Draft_2_button_bitmap;
        PropertyManagerPageCheckbox DraftOutward_2_checkbox;

        PropertyManagerPageCombobox ThinFeature_Reverse_combobox;
        PropertyManagerPageBitmapButton ThinFeature_Reverse_button_bitmap;
        PropertyManagerPageLabel ThinFeature_Value_Label;
        PropertyManagerPageNumberbox ThinFeature_Value_numberbox;
        PropertyManagerPageCheckbox ThinFeature_Capends_checkbox;

        PropertyManagerPageLabel SelectedContour_Label;
        PropertyManagerPageSelectionbox SelectedContour_SelectedBox;

        //Group ID
        public const int From_group_ID = 3;
        public const int Direction_1_group_ID = 5;
        public const int Direction_2_group_ID = 6;
        public const int ThinFeature_group_ID = 7;
        public const int SelectedContours_group_ID = 8;

        //Controls ID
        public const int From_Combobox_ID = 4;

        public const int Direction_1_comboBox_ID = 9;
        public const int Direction_1_Reverse_button_bitmap_ID = 10;
        public const int Direction_1_SelectedBox_ID = 11;
        public const int Dimension_1_Label_ID = 12;
        public const int Dimension_1_value_ID = 13;
        public const int Draft_1_button_bitmap_ID = 14;
        public const int Draft_1_Angle_ID = 15;
        public const int DraftOutWard_1_checkbox_ID = 16;

        public const int Direction_2_comboBox_ID = 17;
        public const int Direction_2_Reverse_button_bitmap_ID = 18;
        public const int Direction_2_SelectedBox_ID = 19;
        public const int Dimension_2_Label_ID = 20;
        public const int Dimension_2_value_ID = 21;
        public const int Draft_2_button__bitmap_ID = 22;
        public const int Draft_2_Angle_ID = 23;
        public const int DraftOutWard_2_checkbox_ID = 24;

        public const int ThinFeature_combobox_ID = 25;
        public const int ThinFeature_Reverse_button_bitmap_ID = 26;
        public const int ThinFeature_Value_Label_ID = 27;
        public const int ThinFeature_Value_ID = 28;
        public const int ThinFeature_Capends_ID = 29;

        public const int SelectedContours_SelectedBox_ID = 30;
        public const int SelectedContours_Label_ID = 31;
   
        #endregion Members

        #region Methods
        public CusBossExtrudePMPage(Main addin)
        {
            #region Variables
            swAddin = addin;
            swApp = (SldWorks)swAddin.SwApp;
            swModel = (ModelDoc2)swApp.ActiveDoc;

            // Get current Unit System
            int DisplayUnit = swModel.LengthUnit;
            double FACTOR = 1;
            double AngleFACTOR = 1;

            // Calculate FACTOR conversion
            swAddin.ConvertFACTOR(DisplayUnit, ref FACTOR);

            // Variable to set dimension range
            int Units = (int)swNumberboxUnitType_e.swNumberBox_Length;
            double Minimum = 0.000001;
            double Maximum = 100000;
            bool Inclusive = true;
            double Increment = 5;
            double FastIncr = 5;
            double SlowIncr = 5;
            double DimensionDefaultValue = 10;
            
            Minimum *= FACTOR;
            Maximum *= FACTOR;
            DimensionDefaultValue *= FACTOR;

            Increment *= FACTOR;
            FastIncr *= FACTOR;
            SlowIncr *= FACTOR;

            //Variable to set angle range
            double MinimumAngle = 0;
            double MaximumAngle = 90;
            double IncrementAngle = 5;
            double FastIncrAngle = 5;
            double SlowIncrAngle = 5;
            double AngleDefaultValue = 0;

            swAddin.ConvertAngle(DisplayUnit, ref AngleFACTOR);

            MinimumAngle *= AngleFACTOR;
            MaximumAngle *= AngleFACTOR;
            AngleDefaultValue *= AngleFACTOR;

            IncrementAngle *= FACTOR;
            FastIncrAngle *= FACTOR;
            SlowIncrAngle *= FACTOR;
            #endregion

            #region Create Property Manager Page
            short controlType = -1;
            short align = -1;
            int errors = -1;
            int options =   (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                            (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton;
            // Create Property Manager Page
            swPropertyPage = (PropertyManagerPage2)swApp.CreatePropertyManagerPage("Boss-Extrude (by HienPhan)", options, handler, ref errors);
            // Set title icon for this page
            swPropertyPage.SetTitleBitmap2("../../Resources/bossextrude24x24px.png");
            #endregion

            #region Add Control
            if (swPropertyPage != null && errors == (int)swPropertyManagerPageStatus_e.swPropertyManagerPage_Okay)
            {
                #region Add Groups
                // Add "From" group
                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                From_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(From_group_ID, "From", options);
                // Add "Direction 1" group
                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Direction_1_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(Direction_1_group_ID, "Direction 1", options);
                // Add "Direction 2" group
                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Direction_2_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(Direction_2_group_ID, "Direction 2", options);
                // Add "Thin Feature" group
                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Thin_Feature_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(ThinFeature_group_ID, "Thin Feature", options);
                // Add "Selected Contours" group
                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Selected_Contours_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(SelectedContours_group_ID, "Selected Contours", options);
                #endregion Add Groups

                #region Add controls
                // Add controls to "From" group
                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                From_Combobox = (PropertyManagerPageCombobox)From_group.AddControl2(From_Combobox_ID, controlType, "", align, options, "From" );
                
                string[] items = {  "Sketch Plane",
                                    "Surface/Face/Plane",
                                    "Vertex",
                                    "Offset"};

                if (From_Combobox != null)
                {
                    

                    From_Combobox.AddItems(items);
                    From_Combobox.CurrentSelection = 0;
                    ((PropertyManagerPageControl)From_Combobox).Top = 20;
                    
                }

                // Add controls to "Direction 1" group
                controlType = (int)swPropertyManagerPageControlType_e.swControlType_BitmapButton;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Direction_1_Reverse_button_bitmap = (PropertyManagerPageBitmapButton)Direction_1_group.AddControl2(Direction_1_Reverse_button_bitmap_ID, controlType, "", align, options, "Reverse Direction");
                Direction_1_Reverse_button_bitmap.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
                ((PropertyManagerPageControl)Direction_1_Reverse_button_bitmap).Top = 20;
                ((PropertyManagerPageControl)Direction_1_Reverse_button_bitmap).Left = 0;

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Direction_1_Combobox = (PropertyManagerPageCombobox)Direction_1_group.AddControl2(Direction_1_comboBox_ID, controlType, "", align, options, "Direction 1");

                if (Direction_1_Combobox != null)
                {
                    items = new string[]{"Blind",
                                        "Offset from Surface",
                                        "Through All",
                                        "Up to Next",
                                        "Up to Surface"};

                    //// Set up
                    Direction_1_Combobox.AddItems(items);
                    Direction_1_Combobox.CurrentSelection = 0;
                    //// Position
                    ((PropertyManagerPageControl)Direction_1_Combobox).Top = 23;
                    ((PropertyManagerPageControl)Direction_1_Combobox).Left = 20;
                }

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;

                Direction_1_SelectedBox = (PropertyManagerPageSelectionbox)Direction_1_group.AddControl2(Direction_1_SelectedBox_ID, controlType, "", align, options, "Direction");
                Direction_1_SelectedBox.Height = 25;
                Direction_1_SelectedBox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem1);
                
                ///// Position
                ((PropertyManagerPageControl)Direction_1_SelectedBox).Top = 35;
                ((PropertyManagerPageControl)Direction_1_SelectedBox).Left = 20;

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Dimensions_1_Label = (PropertyManagerPageLabel)Direction_1_group.AddControl2(Dimension_1_Label_ID, controlType, "", align, options, "Dimension 1");
                ///// Set up
                ((PropertyManagerPageControl)Dimensions_1_Label).SetStandardPictureLabel((int)swControlBitmapLabelType_e.swBitmapLabel_LinearDistance1);
                //// Position
                ((PropertyManagerPageControl)Dimensions_1_Label).Top = 65;
                ((PropertyManagerPageControl)Dimensions_1_Label).Left = 0;

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Numberbox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Dimension_1_value = (PropertyManagerPageNumberbox)Direction_1_group.AddControl2(Dimension_1_value_ID, controlType, "", align, options, "");

                //// Set up
                Dimension_1_value.SetRange2(Units, Minimum, Maximum, Inclusive, Increment, FastIncr, SlowIncr);
                Dimension_1_value.Value = DimensionDefaultValue;
                Dimension_1_value.DisplayedUnit = DisplayUnit;

                //// Position
                ((PropertyManagerPageControl)Dimension_1_value).Top = 65;
                ((PropertyManagerPageControl)Dimension_1_value).Left = 20;

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_BitmapButton;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Draft_1_button_bitmap = (PropertyManagerPageBitmapButton)Direction_1_group.AddControl2(Draft_1_button_bitmap_ID, controlType, "", align, options, "Dimension 1");
                Draft_1_button_bitmap.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_draft);

                ((PropertyManagerPageControl)Draft_1_button_bitmap).Top = 85;
                ((PropertyManagerPageControl)Draft_1_button_bitmap).Left = 0;

                //// Add Draft 1 Angle numberbox
                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Numberbox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Draft_1_Angle = (PropertyManagerPageNumberbox)Direction_1_group.AddControl2(Draft_1_Angle_ID, controlType, "", align, options, "");
                
                ((PropertyManagerPageControl)Draft_1_Angle).Top = 85;
                ((PropertyManagerPageControl)Draft_1_Angle).Left = 20;

                Units = (int)swNumberboxUnitType_e.swNumberBox_Angle;
                short[] AngularUnit = (short[])swModel.GetAngularUnits();
                DisplayUnit = (int)AngularUnit[0];

                Draft_1_Angle.SetRange2(Units, MinimumAngle, MaximumAngle, Inclusive, IncrementAngle, FastIncrAngle, SlowIncrAngle);
                Draft_1_Angle.Value = AngleDefaultValue;
                Draft_1_Angle.DisplayedUnit = DisplayUnit;

                //// Add Draft outward checkbox
                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Checkbox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                DraftOutward_1_checkbox = (PropertyManagerPageCheckbox)Direction_1_group.AddControl2(DraftOutWard_1_checkbox_ID, controlType, "Draft outward", align, options, "Draft outward");


                // Add Control for Direction 2
                controlType = (int)swPropertyManagerPageControlType_e.swControlType_BitmapButton;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Direction_2_Reverse_button_bitmap = (PropertyManagerPageBitmapButton)Direction_2_group.AddControl2(Direction_2_Reverse_button_bitmap_ID, controlType, "", align, options, "Reverse Direction");
                
                //// Set up
                Direction_2_Reverse_button_bitmap.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
                
                //// Position
                ((PropertyManagerPageControl)Direction_2_Reverse_button_bitmap).Top = 20;
                ((PropertyManagerPageControl)Direction_2_Reverse_button_bitmap).Left = 0;

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Direction_2_Combobox = (PropertyManagerPageCombobox)Direction_2_group.AddControl2(Direction_2_comboBox_ID, controlType, "", align, options, "Direction 2");

                if (Direction_2_Combobox != null)
                {
                    items = new string[]{"Blind",
                                        "Offset from Surface",
                                        "Through All",
                                        "Up to Next",
                                        "Up to Surface"};

                    //// Set up
                    Direction_2_Combobox.AddItems(items);
                    Direction_2_Combobox.CurrentSelection = 0;
                    //// Position
                    ((PropertyManagerPageControl)Direction_2_Combobox).Top = 23;
                    ((PropertyManagerPageControl)Direction_2_Combobox).Left = 20;
                }

                #endregion Add controls
            }
            #endregion
        }
        public void Show()
        {
            swPropertyPage.Show();
        }
        #endregion Methods

    }
    public class CusCutExtrudePMPage
    {
        #region Members
        public Main swAddin;
        private SldWorks swApp;
        private PropertyManagerPage2 swPropertyPage;
        private myPropertyManagerPageHandler handler;

        //Groups
        IPropertyManagerPageGroup From_group;
        IPropertyManagerPageGroup Direction_1_group;
        IPropertyManagerPageGroup Direction_2_group;
        IPropertyManagerPageGroup Thin_Feature_group;
        IPropertyManagerPageGroup Selected_Contours_group;

        //Controls
        PropertyManagerPageCombobox From_Combobox;
        PropertyManagerPageBitmapButton Direction_1_Reverse_button_bitmap;

        // Controls ID
        public const int From_group_ID = 3;
        public const int Direction_1_group_ID = 4;
        public const int Direction_2_group_ID = 5;
        public const int ThinFeature_group_ID = 6;
        public const int SelectedContours_group_ID = 7;

        public const int From_Combobox_ID = 8;
        public const int Direction_1_Reverse_button_bitmap_ID = 13;
        #endregion Members

        #region Methods
        public CusCutExtrudePMPage(Main addin)
        {
            swAddin = addin;
            swApp = (SldWorks)swAddin.SwApp;

            #region Create Property Manager Page
            short controlType = -1;
            short align = -1;
            int errors = -1;
            int options = (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                            (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton;

            swPropertyPage = (PropertyManagerPage2)swApp.CreatePropertyManagerPage("Cut-Extrude (by HienPhan)", options, handler, ref errors);
            swPropertyPage.SetTitleBitmap2("../../Resources/cutextrude24x24px.png");
            #endregion

            if (swPropertyPage != null && errors == (int)swPropertyManagerPageStatus_e.swPropertyManagerPage_Okay)
            {
                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                From_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(From_group_ID, "From", options);

                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Direction_1_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(Direction_1_group_ID, "Direction 1", options);

                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Direction_2_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(Direction_2_group_ID, "Direction 2", options);

                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Thin_Feature_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(ThinFeature_group_ID, "Thin Feature", options);

                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Selected_Contours_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(SelectedContours_group_ID, "Selected Contours", options);

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_BitmapButton;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Direction_1_Reverse_button_bitmap = (PropertyManagerPageBitmapButton)From_group.AddControl2(Direction_1_Reverse_button_bitmap_ID, controlType, "From UI", align, options, "My Custom Cut-Extrude");
                Direction_1_Reverse_button_bitmap.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
                ((PropertyManagerPageControl)Direction_1_Reverse_button_bitmap).Top = 20;
                ((PropertyManagerPageControl)Direction_1_Reverse_button_bitmap).Left = 0;

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                From_Combobox = (PropertyManagerPageCombobox)From_group.AddControl2(From_Combobox_ID, controlType, "", align, options, "My Custom Cut-Extrude");

                if (From_Combobox != null)
                {
                    string[] items = {"Hello World"};

                    From_Combobox.AddItems(items);
                    From_Combobox.CurrentSelection = 0;
                    ((PropertyManagerPageControl)From_Combobox).Top = 20;
                    ((PropertyManagerPageControl)From_Combobox).Left = 20;

                }
            }
        }

        public void Show()
        {
            swPropertyPage.Show();
        }
        #endregion
    }

    public class myPropertyManagerPageHandler : PropertyManagerPage2Handler9
    {
        public myPropertyManagerPageHandler() { }

        public void AfterActivation()
        {
            throw new NotImplementedException();
        }

        public void OnClose(int Reason)
        {
            throw new NotImplementedException();
        }

        public void AfterClose()
        {
            throw new NotImplementedException();
        }

        public bool OnHelp()
        {
            throw new NotImplementedException();
        }

        public bool OnPreviousPage()
        {
            throw new NotImplementedException();
        }

        public bool OnNextPage()
        {
            throw new NotImplementedException();
        }

        public bool OnPreview()
        {
            throw new NotImplementedException();
        }

        public void OnWhatsNew()
        {
            throw new NotImplementedException();
        }

        public void OnUndo()
        {
            throw new NotImplementedException();
        }

        public void OnRedo()
        {
            throw new NotImplementedException();
        }

        public bool OnTabClicked(int Id)
        {
            throw new NotImplementedException();
        }

        public void OnGroupExpand(int Id, bool Expanded)
        {
            throw new NotImplementedException();
        }

        public void OnGroupCheck(int Id, bool Checked)
        {
            throw new NotImplementedException();
        }

        public void OnCheckboxCheck(int Id, bool Checked)
        {
            throw new NotImplementedException();
        }

        public void OnOptionCheck(int Id)
        {
            throw new NotImplementedException();
        }

        public void OnButtonPress(int Id)
        {
            throw new NotImplementedException();
        }

        public void OnTextboxChanged(int Id, string Text)
        {
            throw new NotImplementedException();
        }

        public void OnNumberboxChanged(int Id, double Value)
        {
            throw new NotImplementedException();
        }

        public void OnComboboxEditChanged(int Id, string Text)
        {
            throw new NotImplementedException();
        }

        public void OnComboboxSelectionChanged(int Id, int Item)
        {
            throw new NotImplementedException();
        }

        public void OnListboxSelectionChanged(int Id, int Item)
        {
            throw new NotImplementedException();
        }

        public void OnSelectionboxFocusChanged(int Id)
        {
            throw new NotImplementedException();
        }

        public void OnSelectionboxListChanged(int Id, int Count)
        {
            throw new NotImplementedException();
        }

        public void OnSelectionboxCalloutCreated(int Id)
        {
            throw new NotImplementedException();
        }

        public void OnSelectionboxCalloutDestroyed(int Id)
        {
            throw new NotImplementedException();
        }

        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            throw new NotImplementedException();
        }

        public int OnActiveXControlCreated(int Id, bool Status)
        {
            throw new NotImplementedException();
        }

        public void OnSliderPositionChanged(int Id, double Value)
        {
            throw new NotImplementedException();
        }

        public void OnSliderTrackingCompleted(int Id, double Value)
        {
            throw new NotImplementedException();
        }

        public bool OnKeystroke(int Wparam, int Message, int Lparam, int Id)
        {
            throw new NotImplementedException();
        }

        public void OnPopupMenuItem(int Id)
        {
            throw new NotImplementedException();
        }

        public void OnPopupMenuItemUpdate(int Id, ref int retval)
        {
            throw new NotImplementedException();
        }

        public void OnGainedFocus(int Id)
        {
            throw new NotImplementedException();
        }

        public void OnLostFocus(int Id)
        {
            throw new NotImplementedException();
        }

        public int OnWindowFromHandleControlCreated(int Id, bool Status)
        {
            throw new NotImplementedException();
        }

        public void OnListboxRMBUp(int Id, int PosX, int PosY)
        {
            throw new NotImplementedException();
        }

        public void OnNumberBoxTrackingCompleted(int Id, double Value)
        {
            throw new NotImplementedException();
        }
    }
}
