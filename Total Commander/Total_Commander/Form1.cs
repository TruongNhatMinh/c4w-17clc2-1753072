using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using System.Reflection;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace Total_Commander
{
    public delegate void methodWithNoParam();

    public partial class TotalCommander : Form
    {
        #region Win32 declarations
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x1;

        public event methodWithNoParam loadView1;
        public event methodWithNoParam loadView2;
        public event methodWithNoParam loadViewAll;

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        #endregion

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        #region declareOther
        private string destinationFolder;

        public const int EM_SETSEL = 0xB1;
        public const int LVM_FIRST = 0x1000;
        public const int LVM_GETEDITCONTROL = (LVM_FIRST + 24);
        #endregion

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int len, IntPtr order);


        #region variablesMouseDown
        private Rectangle doubleClickRectangle = new Rectangle();
        private Timer doubleClickTimer = new Timer();
        private bool isFirstClick = true;
        private bool isDoubleClick = false;
        private bool doubleClick = false;
        private int milliseconds = 0;
        #endregion

        private void EventMouseDown(object sender, MouseEventArgs e)
        {
            if (isFirstClick)
            {
                isFirstClick = false;

                doubleClickRectangle = new Rectangle(
                    e.X - (SystemInformation.DoubleClickSize.Width / 2),
                    e.Y - (SystemInformation.DoubleClickSize.Height / 2),
                    SystemInformation.DoubleClickSize.Width,
                    SystemInformation.DoubleClickSize.Height);

                doubleClickTimer.Start();
            }
            else
            {
                if (doubleClickRectangle.Contains(e.Location) &&
                    milliseconds < SystemInformation.DoubleClickTime)
                {
                    isDoubleClick = true;
                }
            }
        }
        private void DoubleClickTimer_Tick(object sender, EventArgs e)
        {
            milliseconds += 100;

            if (milliseconds >= SystemInformation.DoubleClickTime)
            {
                doubleClickTimer.Stop();

                if (isDoubleClick)
                {
                    doubleClick = true;
                }
                else
                {
                    doubleClick = false;
                }

                isFirstClick = true;
                isDoubleClick = false;
                milliseconds = 0;
            }
        }

        public TotalCommander()
        {
            InitializeComponent();
            doubleClickTimer.Interval = 100;
            doubleClickTimer.Tick += new EventHandler(DoubleClickTimer_Tick);
            loadView1 += new methodWithNoParam(LoadView1);
            loadView2 += new methodWithNoParam(LoadView2);
            loadViewAll += loadView1 + loadView2;
        }

        #region variablesControl
        thisPC myPC = new thisPC();
        bool lstR = false;
        string pathL, pathR;

        ListView.SelectedListViewItemCollection lst;
        string path1, path2;
        ListView lv;

        bool contextControl = false;
        bool isCopy = true;
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            Option_Notepad.Checked = true;
            Option_Hidden.Checked = false;
        }
        private void BindDriveListToComboBox(ListView cbDrives)
        {
            IconLargeForFile.ImageSize = SystemInformation.IconSize;

            SHFILEINFO shinfo = new SHFILEINFO();

            myPC.listDrive = DriveInfo.GetDrives();
            cbDrives.Size = new Size(cbDrives.Size.Width, System.Linq.Enumerable.Count(myPC.listDrive) * 55 / 2);

            foreach (var drive in myPC.listDrive)
            {

                SHGetFileInfo(drive.Name, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
                try
                {
                    IconSmallForFile.Images.Add(drive.Name, Icon.FromHandle(shinfo.hIcon));
                }
                catch { }
                SHGetFileInfo(drive.Name, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                try
                {
                    IconLargeForFile.Images.Add(drive.Name, Icon.FromHandle(shinfo.hIcon));
                }
                catch { }
                cbDrives.Items.Add(drive.Name, IconLargeForFile.Images.IndexOfKey(drive.Name));
            }
        }
        private void ComboBox_CurrentDrive1_Click(object sender, MouseEventArgs e)
        {
            ListView_Drive1.Clear();
            ActiveControl = null;
            ListView_Drive1.Visible = !ListView_Drive1.Visible;
            BindDriveListToComboBox(ListView_Drive1);
        }
        private void ComboBox_CurrentDrive2_Click(object sender, MouseEventArgs e)
        {
            ListView_Drive2.Clear();
            ActiveControl = null;
            ListView_Drive2.Visible = !ListView_Drive2.Visible;
            BindDriveListToComboBox(ListView_Drive2);
        }
        private void ListView_Drive1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listDrive = sender as ListView;
            if (listDrive.SelectedItems.Count == 1)
            {
                if (ComboBox_CurrentDrive1.Text.Equals(listDrive.SelectedItems[0].Text)) return;
                TextBox_path1.Text = listDrive.SelectedItems[0].Text;
                pathL = TextBox_path1.Text;
                listDrive.Visible = false;
                loadView1();
            }

        }
        private void ListView_Drive2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listDrive = sender as ListView;
            if (listDrive.SelectedItems.Count == 1)
            {
                if (ComboBox_CurrentDrive2.Text.Equals(listDrive.SelectedItems[0].Text)) return;
                TextBox_path2.Text = listDrive.SelectedItems[0].Text;
                pathR = TextBox_path2.Text;
                listDrive.Visible = false;
                loadView2();
            }
        }
        private int getIndexImage(FileSystemInfo item)
        {
            string ext = item.Extension;
            if (String.IsNullOrEmpty(ext))
            {
                if ((item.Attributes & FileAttributes.Directory) != 0)
                    ext = "5EEB255733234c4dBECF9A128E896A1E"; // for directories
                else
                    ext = "F9EB930C78D2477c80A51945D505E9C4"; // for files without extension
            }
            if (!IconSmallForFile.Images.Keys.Contains(ext))
            {
                try
                {
                    IconSmallForFile.Images.Add(ext, Icon.ExtractAssociatedIcon(item.FullName));
                    IconLargeForFile.Images.Add(ext, Icon.ExtractAssociatedIcon(item.FullName));
                }
                catch
                {
                    SHFILEINFO shinfo = new SHFILEINFO();
                    try
                    {
                        SHGetFileInfo(item.FullName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
                        IconSmallForFile.Images.Add(ext, Icon.FromHandle(shinfo.hIcon));
                        SHGetFileInfo(item.FullName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                        IconLargeForFile.Images.Add(ext, Icon.FromHandle(shinfo.hIcon));
                    }
                    catch { }
                }

            }
            return IconSmallForFile.Images.IndexOfKey(ext);
        }
        private void reloadView(ListView ListView_File, TextBox TextBox_path, ref string path, ComboBox ComboBox_CurrentDrive)
        {
            ListView_File.Items.Clear();
            ListView_File.BeginUpdate();
            if (TextBox_path.Text.Length > 3)
            {
                var row = new string[] { "..", "", "", "", };
                var liv = new ListViewItem(row);
                liv.ImageIndex = 0;
                ListView_File.Items.Add(liv);
            }
            if (TextBox_path.Text.Last() != 92)
            {
                TextBox_path.Text += "\\";
            }
            foreach (DirectoryInfo item in myPC.allFolderInDrive(TextBox_path.Text))
            {
                if (item.Attributes.HasFlag(FileAttributes.Hidden) && !Option_Hidden.Checked) continue;
                var row = new string[] { item.Name, "File folder", "", string.Format("{0:D}", item.LastWriteTime) };
                var liv = new ListViewItem(row);
                if (item.Attributes.HasFlag(FileAttributes.Hidden))
                    liv.ForeColor = Color.DimGray;
                liv.ImageIndex = getIndexImage(item);
                liv.Name = item.Name;
                ListView_File.Items.Add(liv);
            }
            foreach (FileInfo item in myPC.allFileInDrive(TextBox_path.Text))
            {
                if (item.Attributes.HasFlag(FileAttributes.Hidden) && !Option_Hidden.Checked) continue;

                string iE = getExtension(item.Extension);
                var row = new string[] { item.Name.Substring(0, (item.Name == item.Extension ? item.Name.Length : item.Name.Length - item.Extension.Length)), item.Extension, (item.Length / 1000 > 0 ? item.Length / 1000 : 1).ToString() + " KB", string.Format("{0:D}", item.LastWriteTime) };
                var liv = new ListViewItem(row);
                if (item.Attributes.HasFlag(FileAttributes.Hidden))
                    liv.ForeColor = Color.Black;
                liv.ImageIndex = getIndexImage(item);
                liv.Name = item.Name;
                ListView_File.Items.Add(liv);
            }
            path = TextBox_path.Text;
            ComboBox_CurrentDrive.Text = TextBox_path.Text.Substring(0, 3);
            ListView_File.EndUpdate();

        }
        private string getExtension(string ex)
        {
            if (ex == ".txt")
                return "Text Document";
            return ex.Substring(1, ex.Length - 1).ToUpper() + " File";
        }
        private void LoadView1()
        {
            reloadView(ListView_File1, TextBox_path1, ref pathL, ComboBox_CurrentDrive1);
            if (lstR)
            {
                foreach (ListViewItem item in ListView_File1.SelectedItems)
                    item.Selected = false;
            }
        }
        private void LoadView2()
        {
            reloadView(ListView_File2, TextBox_path2, ref pathR, ComboBox_CurrentDrive2);
            if (!lstR)
            {
                foreach (ListViewItem item in ListView_File2.SelectedItems)
                    item.Selected = false;
            }
        }
        private void ListView_File1_MouseDown(object sender, MouseEventArgs e)
        {
            checkShow(e);
            lstR = false;
            if (e.Button == MouseButtons.Left)
            {
                EventMouseDown(sender, e);
                foreach (var item in ListView_File1.SelectedItems)
                {
                    var input = item.ToString();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (TextBox_path1.Text != "")
                {
                    ListView_File1.ContextMenuStrip = contextMenu;
                    ContextMenu_Opening(sender, null);
                }                  
            }
        }
        private void ListView_File2_MouseDown(object sender, MouseEventArgs e)
        {
            checkShow(e);
            lstR = true;
            if (e.Button == MouseButtons.Left)
            {
                EventMouseDown(sender, e);
                foreach (var item in ListView_File2.SelectedItems)
                {
                    var input = item.ToString();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (TextBox_path2.Text != "")
                {
                    ListView_File2.ContextMenuStrip = contextMenu;
                    ContextMenu_Opening(sender, null);
                }                 
            }
        }
        private void ListView_File1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (loadSubItems(sender as ListView, TextBox_path1))
                loadView1();
        }
        private void ListView_File2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (loadSubItems(sender as ListView, TextBox_path2))
                loadView2();
        }
        private bool loadSubItems(ListView listView_File, TextBox TextBox_path)
        {
            if (listView_File.SelectedItems.Count > 0)
            {
                if (listView_File.SelectedItems[0].Text.Equals(".."))
                {
                    string tmp = TextBox_path.Text;
                    while (tmp.LastIndexOf('\\') == tmp.Length - 1)
                        tmp = tmp.Remove(tmp.LastIndexOf('\\'));
                    tmp = tmp.Remove(tmp.LastIndexOf('\\') + 1);
                    TextBox_path.Text = tmp;
                    return true;
                }
                else
                {
                    foreach (ListViewItem item in listView_File.SelectedItems)
                    {
                        try
                        {
                            if (myPC.getTypeFile(TextBox_path.Text + item.Name) == TYPE.FILE)
                            {
                                System.Diagnostics.Process.Start(TextBox_path.Text + item.Name);
                                listView_File.Items[item.Index].Selected = false;
                                return false;
                            }
                        }
                        catch { continue; }
                    }

                    TextBox_path.Text += listView_File.SelectedItems[0].Text + "\\";
                    return true;
                }
            }
            return false;
        }
        private void ListView_File1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control || e.Modifiers == Keys.Shift || e.Modifiers == Keys.Insert)
            {
                lst = ListView_File1.SelectedItems;
                lv = ListView_File1;
            }
            handleKeyDown(ListView_File1, e);
        }
        private void ListView_File2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control || e.Modifiers == Keys.Shift || e.Modifiers == Keys.Insert)
            {
                lst = ListView_File2.SelectedItems;
                lv = ListView_File2;
            }
            handleKeyDown(ListView_File2, e);
        }
        private void handleKeyDown(ListView lst, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4 && e.Modifiers == Keys.Alt)
            {
                this.Close();
            }
            switch (e.KeyCode)
            {
                case Keys.F3:
                    MenuFile_View_Click(null, null);
                    break;
                case Keys.F4:
                    MenuFile_Edit_Click(null, null);
                    break;
                case Keys.F5:
                    MenuFile_Copy_Click(null, null);
                    break;
                case Keys.F6:
                    MenuFile_Move_Click(null, null);
                    break;
                case Keys.F7:
                    MenuFile_NewFolder_Click(null, null);
                    break;
                case Keys.F8:
                    MenuFile_Delete_Click(null, null);
                    break;
            }
        }
        private void ListView_File1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            this.destinationFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            try
            {
                string sourceDirName = Path.Combine(new string[] { pathL, ListView_File1.Items[e.Item].Text });
                string destDirName = Path.Combine(new string[] { pathL, e.Label });

                Directory.Move(sourceDirName, destDirName);
                if (sourceDirName + "\\" == pathR)
                {
                    TextBox_path2.Text = destDirName;
                    reloadView(ListView_File2, TextBox_path2, ref pathR, ComboBox_CurrentDrive2);
                    if (!lstR)
                    {
                        foreach (ListViewItem item in ListView_File2.SelectedItems)
                            item.Selected = false;
                    }
                }
                loadViewAll();

                e.CancelEdit = true;
            }
            catch (Exception ex)
            {
                e.CancelEdit = true;
            }
        }
        private void ListView_File2_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            this.destinationFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            try
            {
                string sourceDirName = Path.Combine(new string[] { pathR, ListView_File2.Items[e.Item].Text });
                string destDirName = Path.Combine(new string[] { pathR, e.Label });

                Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(sourceDirName, destDirName);
                if (sourceDirName + "\\" == pathL)
                {
                    TextBox_path1.Text = destDirName;
                    reloadView(ListView_File1, TextBox_path1, ref pathL, ComboBox_CurrentDrive1);
                    if (lstR)
                    {
                        foreach (ListViewItem item in ListView_File1.SelectedItems)
                            item.Selected = false;
                    }
                }
                loadViewAll();

                e.CancelEdit = true;
            }
            catch (Exception ex)
            {
                e.CancelEdit = true;
            }
        }
        private void TextBox_path1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TextBox_path1.SelectAll();
            TextBox_path1.Focus();
        }
        private void TextBox_path2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TextBox_path2.SelectAll();
            TextBox_path2.Focus();
        }
        private void TextBox_path1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                if (checkPath(TextBox_path1, pathL))
                {
                    loadView1();
                }
                else
                {
                    MessageBox.Show("Windows can't find '" + TextBox_path1.Text + "'. Check the spelling and try again", "File Explorer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TextBox_path1.Text = pathL;
                }
                HideCaret(TextBox_path1.Handle);
            }
        }
        private void TextBox_path2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (checkPath(TextBox_path2, pathR))
                {
                    loadView2();
                }
                else
                {
                    MessageBox.Show("Windows can't find '" + TextBox_path2.Text + "'. Check the spelling and try again", "File Explorer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TextBox_path2.Text = pathR;
                }
                HideCaret(TextBox_path2.Handle);
            }
        }
        private bool checkPath(TextBox link, string preLink)
        {
            if (!myPC.validPath(link.Text))
            {
                return false;
            }
            if (link.Text.Last() != '\\')
            {
                link.Text += '\\';
            }
            return true;
        }
        private void MenuFile_View_Click(object sender, EventArgs e)
        {
            if (!lstR)
            {
                viewFile(ListView_File1, pathL);
            }
            else
            {
                viewFile(ListView_File2, pathR);
            }
        }
        private void MenuFile_Edit_Click(object sender, EventArgs e)
        {
            if (!lstR)
            {
                editFile(ListView_File1, pathL);
            }
            else
            {
                editFile(ListView_File2, pathR);
            }
        }
        private void MenuFile_Copy_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Are you sure you want to copy it?", "Copy", MessageBoxButtons.OKCancel)) return;
            if (pathL == pathR) return;
            Handle handle = new Handle();
            if (!contextControl)
            {
                if (!lstR)
                {
                    lst = this.ListView_File1.SelectedItems;
                    path1 = pathL;
                    path2 = pathR;
                    lv = ListView_File2;
                }
                else
                {
                    lst = this.ListView_File2.SelectedItems;
                    path1 = pathR;
                    path2 = pathL;
                    lv = ListView_File1;
                }
            }
            
            foreach (ListViewItem item in lst)
            {
                if (checkExist(path2, item.Name))
                {
                    if (handle.result != 0 && handle.result != 2 && handle.result != 4)
                        handle.ShowDialog();
                    if (handle.result == 0) break;
                    else if (handle.result == 1 || handle.result == 2) continue;
                    else if (handle.result == 3 || handle.result == 4) myPC.deleteFile(Path.Combine(path2, item.Name));
                }
                Task t = new Task(new Action(() =>
                {
                    copyFile(item, lv, path1, path2);

                }));
                t.Start();
            }
        }
        private void MenuFile_Move_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Are you sure you want to move it?", "Move", MessageBoxButtons.OKCancel)) return;
            if (pathL == pathR)
            {
                if (!lstR)
                    ListView_File1.SelectedItems[0].BeginEdit();
                else
                    ListView_File2.SelectedItems[0].BeginEdit();
                return;
            }
            Handle handle = new Handle();
            if (!contextControl)
            {
                if (!lstR)
                {
                    lst = this.ListView_File1.SelectedItems;
                    path1 = pathL;
                    path2 = pathR;
                    lv = ListView_File2;
                }
                else
                {
                    lst = this.ListView_File2.SelectedItems;
                    path1 = pathR;
                    path2 = pathL;
                    lv = ListView_File1;
                }
            }
            
            foreach (ListViewItem item in lst)
            {
                if (path1 + item.Name + "\\" == path2) continue;
                if (checkExist(path2, item.Name))
                {
                    if (handle.result != 0 && handle.result != 2 && handle.result != 4)
                        handle.ShowDialog();
                    if (handle.result == 0) break;
                    else if (handle.result == 1 || handle.result == 2) continue;
                    else if (handle.result == 3 || handle.result == 4) myPC.deleteFile(Path.Combine(path2, item.Name));
                }
                Task t = new Task(new Action(() =>
                {
                    moveFile(item, lv, path1, path2);

                }));
                t.Start();
            }

        }
        private void MenuFile_NewFolder_Click(object sender, EventArgs e)
        {
            if (!lstR)
            {
                newFolder(ListView_File1, pathL);
            }
            else
            {
                newFolder(ListView_File2, pathR);
            }
        }
        private void MenuFile_Delete_Click(object sender, EventArgs e)
        {
            if (!lstR)
            {
                deleteFile(ListView_File1, pathL);
            }
            else
            {
                deleteFile(ListView_File2, pathR);
            }
        }
        private void MenuFile_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void MenuHelp_About_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            MessageBox.Show("◣ ◢" + "\n" +
                "Total Commander®" + "\n" +
                "Developer: Truong Nhat Minh" + "\n" +
                "ID: 1753072" + "\n" +
                "Contact via: tnhatminh99@gmail.com" + "\n" +
                "🤞🏻🤞🏻🤞🏻" + "\n", "About", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification
                );
        }
        private void MenuHelp_Help_Click(object sender, EventArgs e)
        {
            string stringString = Application.StartupPath;
            string[] stringSeparators = new string[] { "bin" };
            Application.StartupPath.Split(stringSeparators, StringSplitOptions.None).First();
            System.Diagnostics.Process.Start(Application.StartupPath.Split(stringSeparators, StringSplitOptions.None).First() + "Help.pdf");
        }
        private void Button_refresh_Click(object sender, EventArgs e)
        {
            loadViewAll();
        }
        private void Button_viewIcon_Click(object sender, EventArgs e)
        {
            ListView_File1.View = System.Windows.Forms.View.SmallIcon;
            ListView_File2.View = System.Windows.Forms.View.SmallIcon;
        }
        private void Button_viewDetail_Click(object sender, EventArgs e)
        {
            ListView_File1.View = System.Windows.Forms.View.Details;
            ListView_File2.View = System.Windows.Forms.View.Details;
        }
        private void Button_viewList_Click(object sender, EventArgs e)
        {
            ListView_File1.View = System.Windows.Forms.View.List;
            ListView_File2.View = System.Windows.Forms.View.List;
        }
        private void F3_Click(object sender, EventArgs e)
        {
            MenuFile_View_Click(sender, e);
        }
        private void F4_Click(object sender, EventArgs e)
        {
            MenuFile_Edit_Click(sender, e);
        }
        private void F5_Click(object sender, EventArgs e)
        {
            MenuFile_Copy_Click(null, null);
        }
        private void F6_Click(object sender, EventArgs e)
        {
            MenuFile_Move_Click(null, null);
        }
        private void F7_Click(object sender, EventArgs e)
        {
            MenuFile_NewFolder_Click(null, null);
        }
        private void F8_Click(object sender, EventArgs e)
        {
            MenuFile_Delete_Click(sender, e);
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void viewFile(ListView lst, string path)
        {
            View view = new View();
            foreach (ListViewItem item in lst.SelectedItems)
            {
                view.SHOW(myPC.getTypeFile(path + item.Name), myPC.getFileSystemInfo(path + item.Name));
            }
        }
        private void editFile(ListView lst, string path)
        {
            foreach (ListViewItem item in lst.SelectedItems)
            {
                if (Option_Notepad.Checked)
                {
                    System.Diagnostics.Process.Start("notepad.exe", path + item.Name);
                }

                else if (Option_SublimeText.Checked)
                {
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.FileName = @"C:\Program Files\Sublime Text 3\subl.exe";
                    startInfo.Arguments = "\"" + path + item.Name + "\"";
                    System.Diagnostics.Process.Start(startInfo);
                }

                else if (Option_VisualCode.Checked)
                {
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    string userNams = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1].ToString();
                    startInfo.FileName = @"C:\Users\" + userNams + @"\AppData\Local\Programs\Microsoft VS Code\Code.exe";
                    startInfo.Arguments = "\"" + path + item.Name + "\"";
                    System.Diagnostics.Process.Start(startInfo);
                }
            }
        }
        public void copyFile(ListViewItem item, ListView lst, string path1, string path2)
        {
            if ((Directory.Exists(path1 + item.Name) | File.Exists(path1 + item.Name)) & path2 != null)
            {
                if (lst.InvokeRequired)
                {
                    lst.Invoke(new MethodInvoker(delegate
                    {
                        if (myPC.getTypeFile(Path.Combine(path1, item.Name)) == TYPE.FILE)
                        {
                            System.IO.File.Copy(path1 + item.Name, Path.Combine(path2, item.Name), true);
                        }
                        else
                        {
                            if (System.IO.Directory.Exists(path1 + item.Name))
                            {
                                Directory.CreateDirectory(path2 + "\\" + item.Name);
                                string[] files = System.IO.Directory.GetFiles(path1 + item.Name);
                                foreach (string s in files)
                                {
                                    System.IO.File.Copy(s, Path.Combine(path2 + "\\" + item.Name, Path.GetFileName(s)), true);
                                }
                            }
                        }
                        loadViewAll();
                        //lst1.EnsureVisible(lst1.Items.Count - 1);

                        Application.DoEvents();
                    }));
                    return;
                }
            }
        }
        private void moveFile(ListViewItem item, ListView lst, string path1, string path2)
        {
            if (Directory.Exists(path1 + item.Name) | File.Exists(path1 + item.Name) & path2 != null)
            {
                if (lst.InvokeRequired)
                {
                    lst.Invoke(new MethodInvoker(delegate
                    {
                        if (myPC.getTypeFile(Path.Combine(path1, item.Name)) == TYPE.FILE)
                        {
                            System.IO.File.Move(path1 + item.Name, Path.Combine(path2, item.Name));
                        }
                        else
                        {
                            if (System.IO.Directory.Exists(path1 + item.Name))
                            {
                                Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(path1 + item.Name, Path.Combine(path2, item.Name));
                            }
                        }
                        loadViewAll();

                        //lst1.EnsureVisible(lst1.Items.Count - 1);

                        Application.DoEvents();
                    }));
                    return;
                }
            }
        }
        private void newFolder(ListView lst, string path)
        {
            if (path != null)
            {
                int index = 1;
                string name;
                do
                {
                    if (index == 1)
                    {
                        name = "New Folder";
                    }
                    else
                    {
                        name = string.Format("New Folder ({0})", index);
                    }
                    index++;
                } while (index >= 0 && (System.IO.Directory.Exists(path + name)));
                Directory.CreateDirectory(path + name);
                sameDir();
                lst.Items[lst.FindItemWithText(name).Index].BeginEdit();
            }
        }
        private bool deleteFile(ListView lst, string path)
        {
            if (path != null)
            {
                if (lst.SelectedItems.Count == 0) return false;
                bool succession = false;
                bool folder = false;
                DialogResult Result = DialogResult.None;
                foreach (ListViewItem item in lst.SelectedItems)
                {
                    string nameFile = item.Name;
                    Result = MessageBox.Show(nameFile + " will be deleted!", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Result == DialogResult.Yes)
                    {
                        if (myPC.getTypeFile(Path.Combine(path, nameFile)) == TYPE.FOLDER)
                        {
                            if (myPC.allFileInDrive(Path.Combine(path, nameFile)).Count > 0 ||
                                myPC.allFolderInDrive(Path.Combine(path, nameFile)).Count > 0)
                            {
                                Result = MessageBox.Show(nameFile + " has content. " + nameFile + " will be deleted!", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                folder = true;
                            }
                        }
                        if (Result == DialogResult.Yes)
                        {
                            succession = myPC.deleteFile(Path.Combine(path, nameFile));
                            if (succession == true)
                            {
                                if ((myPC.getTypeFile(Path.Combine(path, nameFile)) == TYPE.NONE) & !lstR & pathL + item.Name + "\\" == pathR & folder == true)
                                {
                                    TextBox_path2.Text = TextBox_path1.Text;
                                    pathR = pathL;
                                }
                                else if ((myPC.getTypeFile(Path.Combine(path, nameFile)) == TYPE.NONE) & lstR & pathR + item.Name + "\\" == pathL & folder == true)
                                {
                                    TextBox_path1.Text = TextBox_path2.Text;
                                    pathL = pathR;
                                }
                                sameDir();
                            }
                        }
                    }
                }
                return succession;
            }
            return false;
        }
        private void sameDir()
        {
            if (pathL == pathR)
                loadViewAll();
            else if (!lstR)
                loadView1();
            else if (lstR)
                loadView2();
        }
        private bool checkExist(string path, string name)
        {
            if (Directory.Exists(path + name) | File.Exists(path + name))
                return true;
            return false;
        }
        private void Option_Notepad_Click(object sender, EventArgs e)
        {
            Option_Notepad.Checked = true;
            Option_SublimeText.Checked = false;
            Option_VisualCode.Checked = false;
        }
        private void Option_SublimeText_Click(object sender, EventArgs e)
        {
            Option_Notepad.Checked = false;
            Option_SublimeText.Checked = true;
            Option_VisualCode.Checked = false;
        }
        private void Option_VisualCode_Click(object sender, EventArgs e)
        {
            Option_Notepad.Checked = false;
            Option_SublimeText.Checked = false;
            Option_VisualCode.Checked = true;
        }

        
        #region contextMenuHandle
        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            string path;
            ListView listView_File;
            TextBox textBox;
            if (!lstR)
            {
                listView_File = ListView_File1;
                textBox = TextBox_path1;
            }
            else
            {
                listView_File = ListView_File2;
                textBox = TextBox_path2;
            }
            if (listView_File.SelectedItems.Count > 1)
            {
                viewToolStripMenuItem1.Visible = false;
                editToolStripMenuItem1.Visible = false;
                copyToolStripMenuItem1.Visible = true;
                cutToolStripMenuItem1.Visible = true;
                pasteToolStripMenuItem1.Visible = false;
                newFolderToolStripMenuItem1.Visible = false;
                deleteToolStripMenuItem1.Visible = true;
                renameToolStripMenuItem1.Visible = false;
            }
            else if (listView_File.SelectedItems.Count == 1)
            {
                path = textBox.Text + listView_File.SelectedItems[0].Text;
                if (Directory.Exists(path))
                {
                    viewToolStripMenuItem1.Visible = false;
                    editToolStripMenuItem1.Visible = false;
                    copyToolStripMenuItem1.Visible = true;
                    cutToolStripMenuItem1.Visible = true;
                    pasteToolStripMenuItem1.Visible = false;
                    newFolderToolStripMenuItem1.Visible = false;
                    deleteToolStripMenuItem1.Visible = true;
                    renameToolStripMenuItem1.Visible = true;
                }
                else
                {
                    viewToolStripMenuItem1.Visible = true;
                    editToolStripMenuItem1.Visible = true;
                    copyToolStripMenuItem1.Visible = true;
                    cutToolStripMenuItem1.Visible = true;
                    pasteToolStripMenuItem1.Visible = false;
                    newFolderToolStripMenuItem1.Visible = false;
                    deleteToolStripMenuItem1.Visible = true;
                    renameToolStripMenuItem1.Visible = true;
                }
            }
            else
            {
                viewToolStripMenuItem1.Visible = false;
                editToolStripMenuItem1.Visible = false;
                toolStripSeparator8.Visible = false;
                copyToolStripMenuItem1.Visible = false;
                cutToolStripMenuItem1.Visible = false;
                if (lst == null || lst.Count == 0)
                    pasteToolStripMenuItem1.Visible = false;
                else
                    pasteToolStripMenuItem1.Visible = true;
                toolStripSeparator9.Visible = false;
                newFolderToolStripMenuItem1.Visible = true;
                deleteToolStripMenuItem1.Visible = false;
                renameToolStripMenuItem1.Visible = false;
            }
        }
        private void ViewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MenuFile_View_Click(sender, e);
        }
        private void EditToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MenuFile_Edit_Click(sender, e);
        }
        private void CopyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isCopy = true;
            contextControl = true;
            if (!lstR)
            {
                lst = ListView_File1.SelectedItems;
                path1 = pathL;
            }
            else
            {
                lst = ListView_File2.SelectedItems;
                path1 = pathR;
            }
        }
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isCopy = false;
            contextControl = true;
            if (!lstR)
            {
                lst = ListView_File1.SelectedItems;
                path1 = pathL;
            }
            else
            {
                lst = ListView_File2.SelectedItems;
                path1 = pathR;
            }
        }
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!lstR)
            {
                path2 = pathL;
                lv = ListView_File2;
            }
            else
            {
                path2 = pathR;
                lv = ListView_File1;
            }
            if (isCopy)
            {
                MenuFile_Copy_Click(sender, e);
            }
            else
            {
                MenuFile_Move_Click(sender, e);
            }
        }
        private void NewFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MenuFile_NewFolder_Click(sender, e);
        }
        private void DeleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MenuFile_Delete_Click(sender, e);
        }
        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!lstR)
                ListView_File1.SelectedItems[0].BeginEdit();
            else
                ListView_File2.SelectedItems[0].BeginEdit();
        }
        #endregion

        #region hideSelectDrives
        private void Menu_MouseDown(object sender, MouseEventArgs e)
        {
            checkShow(e);
        }
        private void Toolbar1_MouseDown(object sender, MouseEventArgs e)
        {
            checkShow(e);
        }
        private void TextBox_path1_MouseDown(object sender, MouseEventArgs e)
        {
            checkShow(e);
        }
        private void TextBox_path2_MouseDown(object sender, MouseEventArgs e)
        {
            checkShow(e);
        }
        private void SplitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            checkShow(e);
        }
        private void SplitContainer1_Panel2_MouseDown(object sender, MouseEventArgs e)
        {
            checkShow(e);
        }
        private void ToolBar2_MouseDown(object sender, MouseEventArgs e)
        {
            checkShow(e);
        }
        private void checkShow(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (ListView_Drive1.Visible == true)
                    ListView_Drive1.Visible = false;
                if (ListView_Drive2.Visible == true)
                    ListView_Drive2.Visible = false;
            }
        }
        #endregion
    }

    public enum TYPE { FILE, FOLDER, NONE }
    public class thisPC
    {
        DriveInfo[] _listDrive;
        public thisPC()
        {
            _listDrive = DriveInfo.GetDrives();
        }
        public DriveInfo[] listDrive
        {
            get; set;
        }
        public List<DirectoryInfo> allFolderInDrive(string path)
        {
            string[] Filse;
            try
            {
                Filse = Directory.GetDirectories(path);
            }
            catch
            {
                return null;
            }
            List<DirectoryInfo> listFolder = new List<DirectoryInfo>();
            foreach (string entry in Directory.GetDirectories(path))
            {
                listFolder.Add(new DirectoryInfo(entry));
            }
            return listFolder;
        }
        public List<FileInfo> allFileInDrive(string path)
        {
            string[] F;
            try
            {
                F = Directory.GetFiles(path);
            }
            catch
            {
                return null;
            }
            List<FileInfo> listFile = new List<FileInfo>();
            foreach (string entry in F)
            {
                listFile.Add(new FileInfo(entry));
            }
            return listFile;
        }
        public bool validPath(string path)
        {
            if (Directory.Exists(path) || File.Exists(path))
                return true;
            return false;
        }
        public TYPE getTypeFile(string path)
        {
            if (File.Exists(path))
            {
                return TYPE.FILE;
            }
            else if (Directory.Exists(path))
            {
                return TYPE.FOLDER;
            }
            else return TYPE.NONE;
        }
        public bool deleteFile(string path)
        {
            TYPE t = getTypeFile(path);
            if (t == TYPE.NONE) return false;
            try
            {
                if (t == TYPE.FILE)
                    File.Delete(path);
                else if (t == TYPE.FOLDER)
                    Directory.Delete(path, true);
                return true;
            }
            catch { return false; }
        }
        public FileSystemInfo getFileSystemInfo(string path)
        {
            if (new DirectoryInfo(path).Exists)
            {
                return (new DirectoryInfo(path));
            }
            else if (new FileInfo(path).Exists)
            {
                return new FileInfo(path);
            }
            return null;
        }
    }

}
