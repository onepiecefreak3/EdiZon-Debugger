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
using System.Text.RegularExpressions;

using EdiZonDebugger.Models;
using EdiZonDebugger.Helper;

using LuaWrapper;

namespace EdiZonDebugger
{
    public partial class Main : Form
    {
        string _scriptFolder = "script";
        string _configFolder = "config";
        string _saveFolder = "save";

        Dictionary<string, string> _saveFilePath = null;
        Dictionary<string, string> _luaScriptPath = null;

        EdiZonConfig _config = null;
        Dictionary<string, LuaContext> _luaInstance = null;

        string _currentVersion = null;

        public Main(string file)
        {
            InitializeComponent();

            if (!Directory.Exists(_scriptFolder))
                Directory.CreateDirectory(_scriptFolder);
            if (!Directory.Exists(_configFolder))
                Directory.CreateDirectory(_configFolder);
            if (!Directory.Exists(_saveFolder))
                Directory.CreateDirectory(_saveFolder);

            if (file != null && Support.TryParseJObject(File.ReadAllText(file)))
                InitDebugger(file);

            UpdateUI();
        }

        #region Events
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseConfig();

            var of = new OpenFileDialog();
            of.Filter = "(*.json)|*.json";
            of.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configFolder);

            if (of.ShowDialog() == DialogResult.OK && File.Exists(of.FileName))
            {
                errorTextBox.Clear();
                InitDebugger(of.FileName);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseConfig();
        }

        private void extractEditedSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sf = new SaveFileDialog();

            if (sf.ShowDialog() == DialogResult.OK)
            {
                var save = Lua.GetModifiedSaveBuffer(_luaInstance[_currentVersion]);
                File.WriteAllBytes(sf.FileName, save);
            }
        }

        private void versionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentVersion = (string)versionComboBox.SelectedItem;
            UpdateCategories();
        }

        private void categoriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateItems();
        }
        #endregion

        #region Functions
        private void UpdateUI()
        {
            var opened = _config?.configs != null && _luaInstance != null;

            closeToolStripMenuItem.Enabled = opened;
            extractEditedSaveToolStripMenuItem.Enabled = opened;
            categoriesListBox.Enabled = opened;
            groupBox1.Enabled = opened;
            versionComboBox.Enabled = opened;
        }

        private void UpdateVersions()
        {
            versionComboBox.SelectedIndexChanged -= versionComboBox_SelectedIndexChanged;

            versionComboBox.Items.Clear();

            foreach (var item in _config?.configs)
                versionComboBox.Items.Add(item.Key);

            versionComboBox.SelectedIndexChanged += versionComboBox_SelectedIndexChanged;
            versionComboBox.SelectedIndex = 0;
        }

        private void UpdateCategories()
        {
            categoriesListBox.SelectedIndexChanged -= categoriesListBox_SelectedIndexChanged;

            categoriesListBox.Items.Clear();

            if (_config?.configs[_currentVersion].items.Any(i => i.category == null) ?? false)
                categoriesListBox.Items.Add("No Category");

            foreach (var cat in _config?.configs[_currentVersion].items.Where(i => i.category != null).Select(i => i.category).Distinct())
                categoriesListBox.Items.Add(cat);

            categoriesListBox.SelectedIndexChanged += categoriesListBox_SelectedIndexChanged;
            categoriesListBox.SelectedIndex = 0;
        }

        private void UpdateItems()
        {
            groupBox1.Controls.Clear();
            groupBox1.Text = (string)categoriesListBox.SelectedItem;

            var p = new Point(5, 20);

            var panel = new Panel
            {
                AutoScroll = true,
                Location = p,
                Size = new Size(groupBox1.Width - p.X - 10, groupBox1.Height - p.Y - 10),
                Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right)
            };
            groupBox1.Controls.Add(panel);

            foreach (var item in _config?.configs[_currentVersion].items.Where(i => i.category == (string)categoriesListBox.SelectedItem))
            {
                AddItem(panel, item, p);
                p = new Point(p.X, p.Y + 30);
            }
        }

        private void CloseConfig()
        {
            _saveFilePath = null;
            _luaScriptPath = null;
            _config = null;
            _luaInstance = null;

            groupBox1.Controls.Clear();
            groupBox1.Text = "";

            categoriesListBox.Items.Clear();
            versionComboBox.Items.Clear();
            versionComboBox.Text = "";

            BetaLabel.Text = "Beta:";

            UpdateUI();
        }

        private void InitDebugger(string configName)
        {
            LogConsole.LogBox = errorTextBox;

            if (!OpenConfig(configName, out var error))
            {
                LogConsole.Instance.Log("Failed to load config file: " + error, LogLevel.FATAL);
                CloseConfig();
                return;
            }
            foreach (var item in _config?.configs)
            {
                _currentVersion = item.Key;

                if (!OpenSaveFile(out error))
                {
                    LogConsole.Instance.Log($"Failed to load save file for version \"{item.Key}\": " + error, LogLevel.FATAL);
                    CloseConfig();
                    return;
                }
                if (!OpenScript(out error))
                {
                    LogConsole.Instance.Log($"Failed to load script file for version \"{item.Key}\": " + error, LogLevel.FATAL);
                    CloseConfig();
                    return;
                }
            }

            UpdateUI();
            UpdateVersions();
        }

        private bool OpenConfig(string file, out string message, List<string> searchedJsons = null)
        {
            var content = File.ReadAllText(file);

            if (Support.IsBeta(content))
                BetaLabel.Text = "Beta: True";
            else
                BetaLabel.Text = "Beta: False";

            if (Support.IsUsingInstead(content, out var config))
            {
                var combPath = Path.Combine(_configFolder, config.useInstead);

                if (searchedJsons == null)
                    searchedJsons = new List<string>();

                if (!searchedJsons.Contains(combPath))
                {
                    searchedJsons.Add(combPath);
                    return OpenConfig(combPath, out message, searchedJsons);
                }
                else
                {
                    message = "UseInstead loop detected.";
                    return false;
                }
            }

            return Support.TryParseConfig(content, out _config, out message);
        }

        private bool OpenSaveFile(out string message)
        {
            message = "";

            if (!CheckConfig(out message))
                return false;

            //Get directories
            var paths = new List<string> { _saveFolder };
            if (_config?.configs[_currentVersion].saveFilePaths.Count > 0)
            {
                paths.AddRange(GetSavePaths(_saveFolder, _config?.configs[_currentVersion].saveFilePaths.ToArray()).ToList());
                if (!paths.Any())
                {
                    message = "No directories found.";
                    return false;
                }
            }

            //Get files
            var files = GetSaveFiles(paths.ToArray(), _config?.configs[_currentVersion].files).ToList();
            if (!files.Any())
            {
                message = "No files found.";
                return false;
            }

            if (_saveFilePath == null)
                _saveFilePath = new Dictionary<string, string>();

            if (files.Count == 1)
                _saveFilePath.Add(_currentVersion, files[0]);
            else
            {
                var selector = new SaveSelector(files.ToArray());
                selector.ShowDialog();
                if (selector.ProperExit)
                    _saveFilePath.Add(_currentVersion, selector.SelectedFile);
                else
                {
                    message = "No file selected.";
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<string> GetSavePaths(string currentPath, string[] saveFilePaths, int depth = 0)
        {
            var tmp = new List<string>();
            foreach (var d in Directory.GetDirectories(currentPath))
            {
                var dirName = d.Split('\\').Last();
                if (Regex.IsMatch(dirName, saveFilePaths[depth]))
                    tmp.Add(Path.Combine(currentPath, dirName));
            }

            if (depth + 1 >= saveFilePaths.Length)
                return tmp;

            var result = new List<string>();
            foreach (var rp in tmp)
                result.AddRange(GetSavePaths(rp, saveFilePaths, depth + 1) ?? new List<string>());

            return result;
        }
        private IEnumerable<string> GetSaveFiles(string[] paths, string files)
        {
            foreach (var path in paths)
                foreach (var file in Directory.GetFiles(path))
                    if (Regex.IsMatch(file, files))
                        yield return file;
        }

        private bool OpenScript(out string message)
        {
            message = "";

            if (!CheckConfig(out message))
                return false;

            if (!SetScriptPath())
            {
                message = "Script path not found";
                return false;
            }

            if (_luaInstance == null)
                _luaInstance = new Dictionary<string, LuaContext>();

            var context = new LuaContext();
            if (!Lua.InitializeScript(ref context, _luaScriptPath[_currentVersion], _saveFilePath[_currentVersion], _config?.configs[_currentVersion].encoding, out var error))
            {
                message = error;
                return false;
            }

            _luaInstance.Add(_currentVersion, context);
            return true;
        }
        private bool SetScriptPath()
        {
            if (_luaScriptPath == null)
                _luaScriptPath = new Dictionary<string, string>();

            var path = Path.Combine(_scriptFolder, $"{_config?.configs[_currentVersion].filetype}.lua");

            if (!File.Exists(path))
            {
                LogConsole.Instance.Log($"{_luaScriptPath} cannot be found. Choose a script yourself.", LogLevel.WARNING);

                var of = new OpenFileDialog();
                of.Filter = "(*.lua)|*.lua";
                if (of.ShowDialog() == DialogResult.OK && File.Exists(of.FileName))
                    _luaScriptPath.Add(_currentVersion, of.FileName);
                else return false;
            }
            else
            {
                _luaScriptPath.Add(_currentVersion, path);
            }

            return true;
        }

        private bool CheckConfig(out string message)
        {
            if (_config?.configs == null)
            {
                message = "Config not set";
                return false;
            }
            if (!_config?.configs.ContainsKey(_currentVersion) ?? false)
            {
                message = "Version doesn't exist";
                return false;
            }
            if (_config?.configs[_currentVersion].filetype == null)
            {
                message = "FileType not set";
                return false;
            }

            message = "";
            return true;
        }

        private void AddItem(Panel panel, EdiZonConfig.VersionConfig.Item item, Point initPoint)
        {
            var label = new Label { Text = item.name + ":", Location = initPoint };
            panel.Controls.Add(label);

            initPoint = new Point(initPoint.X + label.Width + 10, initPoint.Y);

            Control itemControl = null;

            var luaValue = Convert.ToUInt32(Lua.GetValueFromSaveFile(_luaInstance[_currentVersion], item.strArgs.ToArray(), item.intArgs.ToArray()));
            if (item.widget.postEquationInverse != null)
                luaValue = Convert.ToUInt32(Lua.ExecuteCalculation(item.widget.postEquationInverse, luaValue));

            bool validItem = true;
            switch (item.widget.type)
            {
                case "int":
                    validItem = item.widget.minValue <= luaValue && luaValue <= item.widget.maxValue;

                    itemControl = new TextBox
                    {
                        Text = validItem ? item.widget.preEquation != null ? Lua.ExecuteCalculation(item.widget.preEquation, luaValue).ToString() : luaValue.ToString() : "???",
                        Enabled = validItem,
                        ReadOnly = true
                    };
                    itemControl.Tag = new List<object>();
                    if (validItem)
                    {
                        itemControl.KeyDown += TextBox_OnDown;            //To edit int value by +/- stepSize by arrow keys
                        (itemControl.Tag as List<object>).Add(item);
                        (itemControl.Tag as List<object>).Add(luaValue);
                    }
                    break;
                case "bool":
                    validItem = item.widget.onValue == luaValue || item.widget.offValue == luaValue;

                    itemControl = new CheckBox
                    {
                        Text = (validItem) ? "" : "???",
                        Checked = luaValue == item.widget.onValue,
                        Enabled = validItem
                    };

                    if (validItem)
                    {
                        (itemControl as CheckBox).CheckedChanged += SetValue_OnChange;
                        itemControl.Tag = item;
                    }
                    break;
                case "list":
                    validItem = item.widget.listItemValues.Contains(luaValue);

                    itemControl = new ComboBox
                    {
                        DataSource = (validItem) ? item.widget.listItemNames : new List<string> { "???" },
                        SelectedIndex = item.widget.listItemValues.IndexOf(luaValue),
                        Enabled = validItem
                    };

                    if (validItem)
                    {
                        (itemControl as ComboBox).SelectedIndexChanged += SetValue_OnChange;
                        itemControl.Tag = item;
                    }
                    break;
            }
            if (!validItem)
                LogConsole.Instance.Log($"Item \"{item.name}\"{((String.IsNullOrEmpty(item.category)) ? "" : $" in Category \"{item.category}\"")} of type \"{item.widget.type}\" has an invalid value of {luaValue.ToString()}.\"\r\n", LogLevel.ERROR);

            itemControl.Location = initPoint;

            panel.Controls.Add(itemControl);
        }
        private void SetValue_OnChange(object sender, EventArgs e)
        {
            var item = (EdiZonConfig.VersionConfig.Item)((Control)sender).Tag;
            switch (sender)
            {
                case ComboBox comboBox:
                    Lua.SetValueInSaveFile(_luaInstance[_currentVersion], item.strArgs.ToArray(), item.intArgs.ToArray(), comboBox.Enabled ? item.widget.onValue : item.widget.offValue);
                    break;
                case ListBox listBox:
                    Lua.SetValueInSaveFile(_luaInstance[_currentVersion], item.strArgs.ToArray(), item.intArgs.ToArray(), item.widget.listItemValues[listBox.SelectedIndex]);
                    break;
            }
        }
        private void TextBox_OnDown(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox)sender;

            var list = (List<object>)textBox.Tag;
            var value = Convert.ToInt64(list[1]);
            var item = (EdiZonConfig.VersionConfig.Item)list[0];

            if (e.KeyCode == Keys.Left)
                value -= item.widget.stepSize ?? 1;
            else if (e.KeyCode == Keys.Right)
                value += item.widget.stepSize ?? 1;

            value = (uint)Math.Min(Math.Max(value, item.widget.minValue), item.widget.maxValue);
            list[1] = value;

            textBox.Text = item.widget.preEquation == null ? value.ToString() : Lua.ExecuteCalculation(item.widget.preEquation, value).ToString();
            Lua.SetValueInSaveFile(_luaInstance[_currentVersion], item.strArgs.ToArray(), item.intArgs.ToArray(), item.widget.postEquation == null ? value : Lua.ExecuteCalculation(item.widget.postEquation, value));
        }
        #endregion
    }
}
