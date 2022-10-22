using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GeoBase.Utils
{
    public class RecentFileList : Separator
    {
        public interface IPersist
        {
            List<RecentFileItems> RecentFiles();
            void InsertFile(string filepath, int filterIndex);
        }

        public struct RecentFileItems
        {
            public RecentFileItems(string filePath, int filterIndex)
            {
                FilePath = filePath;
                FilterIndex = filterIndex;
            }
            public string FilePath;
            public int FilterIndex;
        } ;

        public IPersist Persister { get; set; }

        public void UseRegistryPersister(Registry registry, string key) { Persister = new RegistryPersister(registry, key); }

        public int MaxPathLength { get; set; }
        public bool DisplayPathWithExt { get; set; }
        public MenuItem FileMenu { get; private set; }

        /// <summary>
        /// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
        /// Default = "_{0}:  {2}"
        /// </summary>
        public string MenuItemFormatOneToNine { get; set; }

        /// <summary>
        /// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
        /// Default = "{0}:  {2}"
        /// </summary>
        public string MenuItemFormatTenPlus { get; set; }

        public delegate string GetMenuItemTextDelegate(int index, string filepath);
        public GetMenuItemTextDelegate GetMenuItemTextHandler { get; set; }

        public event EventHandler<MenuClickEventArgs> MenuClick;

        Separator _separator;
        List<RecentFile> _recentFiles;
        public bool DisalbeInsertFile
        {
            get; set;
        }

        public RecentFileList()
        {
            MaxPathLength = 20;
            DisplayPathWithExt = false;
            MenuItemFormatOneToNine = "_{0}:  {2}";
            MenuItemFormatTenPlus = "{0}:  {2}";
            DisalbeInsertFile = false;

            Loaded += (s, e) => HookFileMenu();
        }

        void HookFileMenu()
        {
            MenuItem parent = Parent as MenuItem;
            if (parent == null) throw new ApplicationException("Parent must be a MenuItem");

            if (FileMenu == parent) return;

            if (FileMenu != null) FileMenu.SubmenuOpened -= _FileMenu_SubmenuOpened;

            FileMenu = parent;
            FileMenu.SubmenuOpened += _FileMenu_SubmenuOpened;
        }

        public List<RecentFileItems> RecentFiles { get { return Persister.RecentFiles(); } }
        public void InsertFile(string filepath, int filterIndex)
        {
            if (DisalbeInsertFile)return;
            Persister.InsertFile(filepath, filterIndex);
        }

        void _FileMenu_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            SetMenuItems();
        }

        void SetMenuItems()
        {
            RemoveMenuItems();
            LoadRecentFiles();
            InsertMenuItems();
        }

        void RemoveMenuItems()
        {
            if (_separator != null) FileMenu.Items.Remove(_separator);

            if (_recentFiles != null)
                foreach (RecentFile r in _recentFiles)
                    if (r.MenuItem != null)
                        FileMenu.Items.Remove(r.MenuItem);

            _separator = null;
            _recentFiles = null;
        }

        void InsertMenuItems()
        {
            if (_recentFiles == null) return;
            if (_recentFiles.Count == 0) return;

            int iMenuItem = FileMenu.Items.IndexOf(this);
            foreach (RecentFile r in _recentFiles)
            {
                string header = string.Empty;
                if(DisplayPathWithExt)
                    header = GetMenuItemText(r.Number + 1, r.Filepath, r.Filepath);
                else
                    header = GetMenuItemText(r.Number + 1, r.Filepath, r.DisplayPath);
                r.MenuItem = new MenuItem { Header = header };
                r.MenuItem.Click += MenuItem_Click;

                FileMenu.Items.Insert(++iMenuItem, r.MenuItem);
            }

            _separator = new Separator();
            FileMenu.Items.Insert(++iMenuItem, _separator);
        }

        string GetMenuItemText(int index, string filepath, string displaypath)
        {
            GetMenuItemTextDelegate delegateGetMenuItemText = GetMenuItemTextHandler;
            if (delegateGetMenuItemText != null) return delegateGetMenuItemText(index, filepath);

            string format = (index < 10 ? MenuItemFormatOneToNine : MenuItemFormatTenPlus);

            string shortPath = ShortenPathname(displaypath, MaxPathLength);

            return String.Format(format, index, filepath, shortPath);
        }

        // This method is taken from Joe Woodbury's article at: http://www.codeproject.com/KB/cs/mrutoolstripmenu.aspx

        /// <summary>
        /// Shortens a pathname for display purposes.
        /// </summary>
        /// <param name="pathName">The pathname to shorten.</param>
        /// <param name="maxLength">The maximum number of characters to be displayed.</param>
        /// <remarks>Shortens a pathname by either removing consecutive components of a path
        /// and/or by removing characters from the end of the filename and replacing
        /// then with three elipses (...)
        /// <para>In all cases, the root of the passed path will be preserved in it's entirety.</para>
        /// <para>If a UNC path is used or the pathname and maxLength are particularly short,
        /// the resulting path may be longer than maxLength.</para>
        /// <para>This method expects fully resolved pathnames to be passed to it.
        /// (Use Path.GetFullPath() to obtain this.)</para>
        /// </remarks>
        /// <returns></returns>
        static public string ShortenPathname(string pathName, int maxLength)
        {
            if (pathName.Length <= maxLength)
                return pathName;

            string root = Path.GetPathRoot(pathName);
            if (root.Length > 3)
                root += Path.DirectorySeparatorChar;

            string[] elements = pathName.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            int filenameIndex = elements.GetLength(0) - 1;

            if (elements.GetLength(0) == 1) // pathname is just a root and filename
            {
                if (elements[0].Length > 5) // long enough to shorten
                {
                    // if path is a UNC path, root may be rather long
                    if (root.Length + 6 >= maxLength)
                    {
                        return root + elements[0].Substring(0, 3) + "...";
                    }
                    return pathName.Substring(0, maxLength - 3) + "...";
                }
            }
            else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
            {
                root += "...\\";

                int len = elements[filenameIndex].Length;
                if (len < 6)
                    return root + elements[filenameIndex];

                if ((root.Length + 6) >= maxLength)
                {
                    len = 3;
                }
                else
                {
                    len = maxLength - root.Length - 3;
                }
                return root + elements[filenameIndex].Substring(0, len) + "...";
            }
            else if (elements.GetLength(0) == 2)
            {
                return root + "...\\" + elements[1];
            }
            else
            {
                int len = 0;
                int begin = 0;

                for (int i = 0; i < filenameIndex; i++)
                {
                    if (elements[i].Length > len)
                    {
                        begin = i;
                        len = elements[i].Length;
                    }
                }

                int totalLength = pathName.Length - len + 3;
                int end = begin + 1;

                while (totalLength > maxLength)
                {
                    if (begin > 0)
                        totalLength -= elements[--begin].Length - 1;

                    if (totalLength <= maxLength)
                        break;

                    if (end < filenameIndex)
                        totalLength -= elements[++end].Length - 1;

                    if (begin == 0 && end == filenameIndex)
                        break;
                }

                // assemble final string

                for (int i = 0; i < begin; i++)
                {
                    root += elements[i] + '\\';
                }

                root += "...\\";

                for (int i = end; i < filenameIndex; i++)
                {
                    root += elements[i] + '\\';
                }

                return root + elements[filenameIndex];
            }
            return pathName;
        }

        void LoadRecentFiles()
        {
            _recentFiles = LoadRecentFilesCore();
        }

        List<RecentFile> LoadRecentFilesCore()
        {
            List<RecentFileItems> list = RecentFiles;

            List<RecentFile> files = new List<RecentFile>(list.Count);

            int i = 0;
            foreach (var rf in list)
                files.Add(new RecentFile(i++, rf.FilePath, rf.FilterIndex));

            return files;
        }

        private class RecentFile
        {
            readonly public int Number;
            public readonly int FilterIndex;
            readonly public string Filepath;
            public MenuItem MenuItem;

            public string DisplayPath
            {
                get
                {
                    return Path.Combine(
                        Path.GetDirectoryName(Filepath),
                        Path.GetFileNameWithoutExtension(Filepath));
                }
            }

            public RecentFile(int number, string filepath, int filterIndex)
            {
                Number = number;
                Filepath = filepath;
                FilterIndex = filterIndex;
            }
        }

        public class MenuClickEventArgs : EventArgs
        {
            public string Filepath { get; private set; }
            public int FilterIndex { get; private set; }

            public MenuClickEventArgs(string filepath, int filterIndex)
            {
                Filepath = filepath;
                FilterIndex = filterIndex;
            }
        }

        void MenuItem_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            OnMenuClick(menuItem);
        }

        protected virtual void OnMenuClick(MenuItem menuItem)
        {
            RecentFile recentFilter = GetFilepath(menuItem);
            if (recentFilter == null) return;
            if (String.IsNullOrEmpty(recentFilter.Filepath)) return;

            EventHandler<MenuClickEventArgs> dMenuClick = MenuClick;
            if (dMenuClick != null)
                dMenuClick(menuItem, new MenuClickEventArgs(recentFilter.Filepath, recentFilter.FilterIndex));
        }

        RecentFile GetFilepath(MenuItem menuItem)
        {
            foreach (RecentFile r in _recentFiles)
                if (r.MenuItem == menuItem)
                    return r;

            return null;
        }

        private class RegistryPersister : IPersist
        {
            private string RegistryKey { get; set; }
            private Registry Registry { get; set; }
            private int maxLength = 6;

            public RegistryPersister(Registry registry, string key)
            {
                Registry = registry;
                RegistryKey = key;
            }
            public List<RecentFileItems> RecentFiles()
            {
                LoadFromRecent();
                var temp = _recentFileQueue.ToArray();
                var result = new List<RecentFileItems>();
                for (int i = temp.Length-1; i >= 0;i-- )
                {
                    result.Add(temp[i]);
                }
                return result;
            }
            private Queue<RecentFileItems> _recentFileQueue;
            private void LoadFromRecent()
            {
                if (_recentFileQueue != null) return;
                ProgramOption po = Registry.getEntry(RegistryKey);
                _recentFileQueue = new Queue<RecentFileItems>();
                if (po.isString())
                {
                    string[] files = po.getString().Split('\n');
                    foreach (var file in files)
                    {
                        string[] items = file.Split('?');
                        if (items.Length != 2)
                            continue;
                        var fileName = items[0];
                        int filterIndex;
                        if (!int.TryParse(items[1], out filterIndex))
                            continue;
                        _recentFileQueue.Enqueue(new RecentFileItems(fileName, filterIndex));
                    }
                }
            }
            private void SaveToRegistry()
            {
                string registryString = string.Empty;
                foreach (var item in _recentFileQueue)
                {
                    if(registryString.Length!=0)
                    {
                        registryString += "\n";
                    }
                    registryString += item.FilePath + "?" + item.FilterIndex;
                }
                Registry.setEntry(RegistryKey, new ProgramOption(registryString));
            }
            public void InsertFile(string filepath, int filterIndex)
            {
                LoadFromRecent();
                while (_recentFileQueue.Count >= maxLength)
                {
                    _recentFileQueue.Dequeue();
                }
                _recentFileQueue.Enqueue(new RecentFileItems(filepath, filterIndex));
                SaveToRegistry();
            }
        }
    }
}
