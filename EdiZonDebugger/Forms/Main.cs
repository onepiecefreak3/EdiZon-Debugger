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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using vJine.Lua;

namespace EdiZonDebugger
{
    public partial class Main : Form
    {
        string _scriptFolder = "script";
        string _configFolder = "config";
        string _saveFolder = "save";

        string _saveFilePath = null;
        string _luaScriptPath = null;

        EdiZonConfig _config = null;
        LuaContext _luaInstance = null;

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
                InitDebugger(of.FileName);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseConfig();
            UpdateUI();
        }

        private void extractEditedSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sf = new SaveFileDialog();

            if (sf.ShowDialog() == DialogResult.OK && File.Exists(sf.FileName))
            {
                var save = Lua.GetModifiedSaveBuffer(_luaInstance);
                File.WriteAllBytes(sf.FileName, save);
            }
        }

        private void categoriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorTextBox.Clear();

            groupBox1.Controls.Clear();
            groupBox1.Text = (string)categoriesListBox.SelectedItem;

            var p = new Point(5, 20);

            var panel = new Panel { AutoScroll = true, Location = p, Size = new Size(groupBox1.Width - p.X - 10, groupBox1.Height - p.Y - 10) };
            groupBox1.Controls.Add(panel);

            foreach (var item in _config.items.Where(i => i.category == (string)categoriesListBox.SelectedItem))
            {
                AddItem(panel, errorTextBox, item, p);
                p = new Point(p.X, p.Y + 30);
            }
        }
        #endregion

        #region Functions
        private void UpdateUI()
        {
            var opened = _config != null && _luaInstance != null;

            closeToolStripMenuItem.Enabled = opened;
            extractEditedSaveToolStripMenuItem.Enabled = opened;
            categoriesListBox.Enabled = opened;
            groupBox1.Enabled = opened;
            errorTextBox.Enabled = opened;
        }

        private void UpdateCategories()
        {
            categoriesListBox.SelectedIndexChanged -= categoriesListBox_SelectedIndexChanged;
            categoriesListBox.Items.Clear();

            if (_config.items.Any(i => i.category == null))
                categoriesListBox.Items.Add("No Category");

            foreach (var cat in _config.items.Where(i => i.category != null).Select(i => i.category).Distinct())
                categoriesListBox.Items.Add(cat);

            categoriesListBox.SelectedIndexChanged += categoriesListBox_SelectedIndexChanged;
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

            errorTextBox.Clear();
        }

        private void InitDebugger(string configName)
        {
            if (!OpenConfig(configName, out var error))
            {
                MessageBox.Show(error, "Config error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseConfig();
                return;
            }
            if (!OpenSaveFile(out error))
            {
                MessageBox.Show(error, "Savefile error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseConfig();
                return;
            }
            if (!OpenScript(out error))
            {
                MessageBox.Show(error, "Script error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseConfig();
                return;
            }

            UpdateUI();
            UpdateCategories();
        }

        private bool OpenConfig(string file, out string message)
        {
            if (Support.TryParseJObject<EdiZonConfig>(File.ReadAllText(file), out var obj, out message))
            {
                _config = obj;
                return true;
            }

            return false;
        }

        private bool OpenSaveFile(out string message)
        {
            message = "";

            if (_config == null || _config.filetype == null)
            {
                message = "Config not set";
                return false;
            }

            //Get directories
            var paths = new List<string> { _saveFolder };
            if (_config.saveFilePaths.Count > 0)
            {
                paths.AddRange(GetSavePaths(_saveFolder).ToList());
                if (!paths.Any())
                {
                    message = "No directories found.";
                    return false;
                }
            }

            //Get files
            var files = GetSaveFiles(paths).ToList();
            if (!files.Any())
            {
                message = "No files found.";
                return false;
            }

            if (files.Count == 1)
                _saveFilePath = files[0];
            else
            {
                var selector = new SaveSelector(files.ToArray());
                selector.ShowDialog();
                if (selector.ProperExit)
                    _saveFilePath = selector.SelectedFile;
                else
                {
                    message = "No file selected.";
                    return false;
                }
            }

            return true;
        }
        private IEnumerable<string> GetSavePaths(string currentPath, int depth = 0)
        {
            var tmp = new List<string>();
            foreach (var d in Directory.GetDirectories(currentPath))
            {
                var dirName = d.Split('\\').Last();
                if (Regex.IsMatch(dirName, _config.saveFilePaths[depth]))
                    tmp.Add(Path.Combine(currentPath, dirName));
            }

            if (depth + 1 >= _config.saveFilePaths.Count)
                return tmp;

            var result = new List<string>();
            foreach (var rp in tmp)
                result.AddRange(GetSavePaths(rp, depth + 1) ?? new List<string>());

            return result;
        }
        private IEnumerable<string> GetSaveFiles(List<string> paths)
        {
            foreach (var path in paths)
                foreach (var file in Directory.GetFiles(path))
                    if (Regex.IsMatch(file, _config.files))
                        yield return file;
        }

        private bool OpenScript(out string message)
        {
            message = "";

            if (_config == null || _config.filetype == null)
            {
                message = "Config not set";
                return false;
            }

            if (!SetScriptPath())
            {
                message = "Script path not found";
                return false;
            }

            if (!Lua.InitializeScript(ref _luaInstance, _luaScriptPath, _saveFilePath, out var error))
            {
                message = error;
                return false;
            }

            return true;
        }
        private bool SetScriptPath()
        {
            _luaScriptPath = Path.Combine(_scriptFolder, $"{_config.filetype}.lua");
            if (!File.Exists(_luaScriptPath))
            {
                MessageBox.Show($"{_luaScriptPath} cannot be found. Choose a script yourself.", "Script not found", MessageBoxButtons.OK);

                var of = new OpenFileDialog();
                of.Filter = "(*.lua)|*.lua";
                if (of.ShowDialog() == DialogResult.OK && File.Exists(of.FileName))
                    _luaScriptPath = of.FileName;
                else return false;
            }

            return true;
        }

        private void AddItem(Panel panel, RichTextBox error, EdiZonConfig.Item item, Point initPoint)
        {
            var label = new Label { Text = item.name + ":", Location = initPoint };
            panel.Controls.Add(label);

            initPoint = new Point(initPoint.X + label.Width + 10, initPoint.Y);

            Control itemControl = null;
            var luaValue = Lua.GetValueFromSaveFile(_luaInstance, item.strArgs.ToArray(), item.intArgs.ToArray());
            bool validItem = true;
            switch (item.widget.type)
            {
                case "int":
                    validItem = item.widget.minValue <= Convert.ToUInt32(luaValue) && Convert.ToUInt32(luaValue) <= item.widget.maxValue;

                    itemControl = new TextBox
                    {
                        Text = validItem ? Convert.ToString(luaValue) : "???",
                        Enabled = validItem
                    };

                    if (validItem)
                        (itemControl as TextBox).TextChanged += SetValue_OnChange;
                    break;
                case "bool":
                    validItem = item.widget.onValue == Convert.ToUInt32(luaValue) || item.widget.offValue == Convert.ToUInt32(luaValue);

                    itemControl = new CheckBox
                    {
                        Text = (validItem) ? "" : "???",
                        Checked = Convert.ToInt32(luaValue) == item.widget.onValue,
                        Enabled = validItem
                    };

                    if (validItem)
                        (itemControl as CheckBox).CheckedChanged += SetValue_OnChange;
                    break;
                case "list":
                    validItem = item.widget.listItemValues.Contains(Convert.ToUInt32(luaValue));

                    itemControl = new ComboBox
                    {
                        DataSource = (validItem) ? item.widget.listItemNames : new List<string> { "???" },
                        SelectedIndex = item.widget.listItemValues.IndexOf(Convert.ToUInt32(luaValue)),
                        Enabled = validItem
                    };

                    if (validItem)
                        (itemControl as ComboBox).SelectedIndexChanged += SetValue_OnChange;
                    break;
            }
            if (!validItem)
                error.Text += $"Item \"{item.name}\"{((String.IsNullOrEmpty(item.category)) ? "" : $" in Category \"{item.category}\"")} of type \"{item.widget.type}\" has an invalid value of {luaValue.ToString()}.\"\r\n";

            itemControl.Tag = item;
            itemControl.Location = initPoint;

            panel.Controls.Add(itemControl);
        }
        private void SetValue_OnChange(object sender, EventArgs e)
        {
            var item = (EdiZonConfig.Item)((Control)sender).Tag;
            switch (sender)
            {
                case TextBox textBox:
                    textBox.TextChanged -= SetValue_OnChange;

                    if (!String.IsNullOrEmpty(textBox.Text) && textBox.Text.IsNumeric())
                    {
                        textBox.Text = Math.Min(Math.Max(Convert.ToInt32(textBox.Text), item.widget.minValue), item.widget.maxValue).ToString();
                        Lua.SetValueInSaveFile(_luaInstance, item.strArgs.ToArray(), item.intArgs.ToArray(), Convert.ToInt32(textBox.Text));
                    }
                    else if (!textBox.Text.IsNumeric())
                    {
                        MessageBox.Show($"\"{textBox.Text}\" is invalid. Only numeric inputs are allowed.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox.Text = item.widget.minValue.ToString();
                        Lua.SetValueInSaveFile(_luaInstance, item.strArgs.ToArray(), item.intArgs.ToArray(), Convert.ToInt32(textBox.Text));
                    }

                    textBox.TextChanged += SetValue_OnChange;
                    break;
                case ComboBox comboBox:
                    Lua.SetValueInSaveFile(_luaInstance, item.strArgs.ToArray(), item.intArgs.ToArray(), comboBox.Enabled ? item.widget.onValue : item.widget.offValue);
                    break;
                case ListBox listBox:
                    Lua.SetValueInSaveFile(_luaInstance, item.strArgs.ToArray(), item.intArgs.ToArray(), item.widget.listItemValues[listBox.SelectedIndex]);
                    break;
            }
        }
        #endregion
    }
}
