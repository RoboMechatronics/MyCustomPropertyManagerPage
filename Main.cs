using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms.Integration;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace ClassLibrary1
{
    public class Main : ISwAddin
    {

        private SldWorks swApp;
        private int swAddinID = 0;
        private CommandManager swCommandManager;
        private CommandGroup swCommandGroup;
        private const string swCommandGroupTitle = "My SOLIDWORKS Add-ins";
        private const string swCommandGroupToolTip = "My custom command for SOLIDWORKS";

        public string[] IconLists = {   "../../Resources/online-library_20x20px.png",
                                        "../../Resources/online-library_32x32px.png",
                                        "../../Resources/online-library_40x40px.png",
                                        "../../Resources/online-library_64x64px.png",
                                        "../../Resources/online-library_96x96px.png",
                                        "../../Resources/online-library_128x128px.png"};

        public string[] MainIconLists = {   "../../Resources/solidworks20x20px.png",
                                            "../../Resources/solidworks32x32px.png",
                                            "../../Resources/solidworks40x40px.png",
                                            "../../Resources/solidworks64x64px.png",
                                            "../../Resources/solidworks96x96px.png",
                                            "../../Resources/solidworks128x128px.png"};

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
            int[] swCommandItemIndex = { 0, 0};

            Name = "Boss-Extrude (by HienPhan)";
            Position = 0;
            HintString = "";
            ToolTip = "";
            ImageListIndex = 0;
            CallbackFunction = "AddPropertyManagerTab";
            EnableMethod = "EnableAddPropertyManagerTab";

            swCommandItemIndex[0] = swCommandGroup.AddCommandItem2(Name, Position, HintString, ToolTip, ImageListIndex, CallbackFunction, EnableMethod, swCommandID[0], MenuTBOption);

            swCommandGroup.HasToolbar = true;
            swCommandGroup.HasMenu = true;
            swCommandGroup.Activate();

        }

        public void AddPropertyManagerTab()
        {
            UserPMPage ppage;
            ppage = new UserPMPage(this);
            ppage.Show();
        }

        public int EnableAddPropertyManagerTab()
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
    public class UserPMPage
    {
        public Main swAddin;
        private SldWorks swApp;
        private PropertyManagerPage2 swPropertyPage;
        private PMPHandler handler;

        //Groups
        IPropertyManagerPageGroup From_group;
        IPropertyManagerPageGroup Direction_1_group;
        IPropertyManagerPageGroup Direction_2_group;
        IPropertyManagerPageGroup Thin_Feature_group;
        IPropertyManagerPageGroup Selected_Contours_group;

        //Controls
        PropertyManagerPageCombobox From_dotnet;
        //IPropertyManagerPageWindowFromHandle Direction_1_dotnet;
        //IPropertyManagerPageWindowFromHandle Direction_2_dotnet;
        //IPropertyManagerPageWindowFromHandle Thin_Feature_dotnet;
        //IPropertyManagerPageWindowFromHandle Selected_Contours_dotnet;

        //Control IDs
        public const int From_group_ID = 3;
        public const int Direction_1_group_ID = 4;
        public const int Direction_2_group_ID = 5;
        public const int Thin_Feature_group_ID = 6;
        public const int Selected_Contours_group_ID = 7;

        public const int From_dotnet_ID = 8;
        //public const int Direction_1_dotnet_ID = 9;
        //public const int Direction_2_dotnet_ID = 10;
        //public const int Thin_Feature_dotnet_ID = 11;
        //public const int Selected_Contours_dotnet_ID = 12;

        public UserPMPage(Main addin)
        {
            swAddin = addin;
            swApp = (SldWorks)swAddin.SwApp;

            #region Create Property Manager Page
            short controlType = -1;
            short align = -1;
            int errors = -1;
            int options =   (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                            (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton;

            swPropertyPage = (PropertyManagerPage2)swApp.CreatePropertyManagerPage("Boss-Extrude (by HienPhan)", options, handler, ref errors);
            swPropertyPage.SetTitleBitmap2("../../Resources/TV3D24x24px.png");
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
                From_dotnet = (PropertyManagerPageCombobox)From_group.AddControl2(From_dotnet_ID, controlType, "From UI", align, options, "My Custom Boss-Extrude" );
                
                if (From_dotnet != null)
                {
                    string[] items = {
                                        "Sketch Plane (Custom)",
                                        "Surface/Face/Plane (Custom)",
                                        "Vertex (Custom)",
                                        "Offset (Custom)"};
                    
                    From_dotnet.AddItems(items);
                    From_dotnet.Height = 20;
                    From_dotnet.CurrentSelection = 0;
                }
            }
            #endregion
        }
        public void Show()
        {
            swPropertyPage.Show();
        }
    }
    
    public class PMPHandler : PropertyManagerPage2Handler9
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
