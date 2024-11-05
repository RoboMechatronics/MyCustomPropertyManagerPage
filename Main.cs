using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms.Integration;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System.Diagnostics;
using System.Windows.Interop;
using System.Windows;

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
    }
    public class CusBossExtrudePMPage
    {
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
        PropertyManagerPageBitmapButton Reverse_direction_1_button_bitmap;
        PropertyManagerPageSelectionbox Direction_1_SelectedBox;
        PropertyManagerPageLabel Dimensions_1_bitmap;
        PropertyManagerPageNumberbox Dimension_1_value;
        PropertyManagerPageCombobox Direction_2_Combobox;
        PropertyManagerPageBitmapButton Reverse_direction_2_button_bitmap;

        //Control IDs
        public const int From_group_ID = 3;
        public const int Direction_1_group_ID = 4;
        public const int Direction_2_group_ID = 5;
        public const int Thin_Feature_group_ID = 6;
        public const int Selected_Contours_group_ID = 7;

        public const int From_Combobox_ID = 8;
        public const int Direction_1_ComboBox_ID = 9;
        public const int Reverse_direction_1_button_bitmap_ID = 10;
        public const int Direction_1_SelectedBox_ID = 16;

        public const int Dimension_1_bitmap_ID = 17;
        public const int Dimension_1_value_ID = 18;

        public const int Direction_2_comboBox_ID = 11;
        public const int Reverse_direction_2_button_bitmap_ID = 12;

        public const int Thin_Feature_dotnet_ID = 13;
        public const int Reverse_Thin_Feature_button_bitmap_ID = 14;

        public const int Selected_Contours_SelectedBox_ID = 15;

        public CusBossExtrudePMPage(Main addin)
        {
            swAddin = addin;
            swApp = (SldWorks)swAddin.SwApp;

            swModel = (ModelDoc2)swApp.ActiveDoc;

            #region Get Unit System
            int CurrentUnitSystem = swModel.LengthUnit;
            int DisplayUnit = 0;

            switch (CurrentUnitSystem){
               case (int)swLengthUnit_e.swMM:
                    break;

               case (int)swLengthUnit_e.swCM:
                    break;

               case (int)swLengthUnit_e.swMETER:
                    break;

               case (int)swLengthUnit_e.swINCHES:
                    break;

               default:
                    break;
            }
            DisplayUnit = (int)swLengthUnit_e.swMM;
            #endregion

            #region Create Property Manager Page
            short controlType = -1;
            short align = -1;
            int errors = -1;
            int options =   (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                            (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton;

            swPropertyPage = (PropertyManagerPage2)swApp.CreatePropertyManagerPage("Boss-Extrude (by HienPhan)", options, handler, ref errors);
            swPropertyPage.SetTitleBitmap2("../../Resources/bossextrude24x24px.png");
            #endregion

            #region Add Control
            if (swPropertyPage != null && errors == (int)swPropertyManagerPageStatus_e.swPropertyManagerPage_Okay)
            {
                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                From_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(From_group_ID, "From", options);

                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Direction_1_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(Direction_1_group_ID, "Direction 1", options);

                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Direction_2_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(Direction_2_group_ID, "Direction 2", options);

                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Thin_Feature_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(Thin_Feature_group_ID, "Thin Feature", options);

                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Selected_Contours_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(Selected_Contours_group_ID, "Selected Contours", options);


                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                From_Combobox = (PropertyManagerPageCombobox)From_group.AddControl2(From_Combobox_ID, controlType, "", align, options, "From" );

                if (From_Combobox != null)
                { 
                    string[] items = {  "Sketch Plane", 
                                        "Surface/Face/Plane", 
                                        "Vertex", 
                                        "Offset" };
                    
                    From_Combobox.AddItems(items);
                    From_Combobox.CurrentSelection = 0;
                    ((PropertyManagerPageControl)From_Combobox).Top = 20;
                    //((PropertyManagerPageControl)From_Combobox).Left = 20;

                }

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_BitmapButton;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Reverse_direction_1_button_bitmap = (PropertyManagerPageBitmapButton)Direction_1_group.AddControl2(Reverse_direction_1_button_bitmap_ID, controlType, "", align, options, "Reverse Direction");
                Reverse_direction_1_button_bitmap.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
                ((PropertyManagerPageControl)Reverse_direction_1_button_bitmap).Top = 20;
                ((PropertyManagerPageControl)Reverse_direction_1_button_bitmap).Left = 0;

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Direction_1_Combobox = (PropertyManagerPageCombobox)Direction_1_group.AddControl2(Direction_1_ComboBox_ID, controlType, "", align, options, "Direction 1");

                if (Direction_1_Combobox != null)
                {
                    string[] items = {"Blind",
                                      "Offset from Surface",
                                      "Through All",
                                      "Up to Next",
                                      "Up to Surface"};

                    Direction_1_Combobox.AddItems(items);
                    Direction_1_Combobox.CurrentSelection = 0;
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
                ((PropertyManagerPageControl)Direction_1_SelectedBox).Top = 35;
                ((PropertyManagerPageControl)Direction_1_SelectedBox).Left = 20;

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Dimensions_1_bitmap = (PropertyManagerPageLabel)Direction_1_group.AddControl2(Dimension_1_bitmap_ID, controlType, "", align, options, "Dimension 1");
                ((PropertyManagerPageControl)Dimensions_1_bitmap).SetStandardPictureLabel((int)swControlBitmapLabelType_e.swBitmapLabel_LinearDistance1);
                ((PropertyManagerPageControl)Dimensions_1_bitmap).Top = 65;
                ((PropertyManagerPageControl)Dimensions_1_bitmap).Left = 0;

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Numberbox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Dimension_1_value = (PropertyManagerPageNumberbox)Direction_1_group.AddControl2(Dimension_1_value_ID, controlType, "", align, options, "");
                Dimension_1_value.SetRange2((int)swNumberboxUnitType_e.swNumberBox_Length,0.00000001, 200.0, true, 0.01, 0.01, 0.01);
                Dimension_1_value.Value = (double)10/1000;
                Dimension_1_value.DisplayedUnit = DisplayUnit;
                ((PropertyManagerPageControl)Dimension_1_value).Top = 65;
                ((PropertyManagerPageControl)Dimension_1_value).Left = 20;
            }
            #endregion
        }
        public void Show()
        {
            swPropertyPage.Show();
        }
    }
    public class CusCutExtrudePMPage
    {
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
        PropertyManagerPageBitmapButton Reverse_direction_1_button_bitmap;

        public const int From_group_ID = 3;
        public const int Direction_1_group_ID = 4;
        public const int Direction_2_group_ID = 5;
        public const int Thin_Feature_group_ID = 6;
        public const int Selected_Contours_group_ID = 7;

        public const int From_Combobox_ID = 8;
        public const int Reverse_direction_1_button_bitmap_ID = 13;

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
                Thin_Feature_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(Thin_Feature_group_ID, "Thin Feature", options);

                options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
                Selected_Contours_group = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(Selected_Contours_group_ID, "Selected Contours", options);

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_BitmapButton;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                Reverse_direction_1_button_bitmap = (PropertyManagerPageBitmapButton)From_group.AddControl2(Reverse_direction_1_button_bitmap_ID, controlType, "From UI", align, options, "My Custom Cut-Extrude");
                Reverse_direction_1_button_bitmap.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
                ((PropertyManagerPageControl)Reverse_direction_1_button_bitmap).Top = 20;
                ((PropertyManagerPageControl)Reverse_direction_1_button_bitmap).Left = 0;

                controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
                align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
                options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;
                From_Combobox = (PropertyManagerPageCombobox)From_group.AddControl2(From_Combobox_ID, controlType, "", align, options, "My Custom Cut-Extrude");

                if (From_Combobox != null)
                {
                    string[] items = {"Hello World"};

                    From_Combobox.AddItems(items);
                    //From_Combobox.Height = 20;
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
    }

    public class myPropertyManagerPageHandler : PropertyManagerPage2Handler9
    {
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
