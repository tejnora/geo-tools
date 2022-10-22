using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.Utils;

namespace CAD.GUI
{
    public partial class TextBoxPropPage 
        :PpWindow
    {
        private TextBoxEdit _TextBox;
        private IModel _dataModel;
        private bool _init = false;
        public TextBoxPropPage(IModel aDataMode, TextBoxEdit aTextBox)
            :base("PPTextBox")
        {
            _init = true;
            _TextBox = aTextBox;
            InitializeComponent();
            _dataModel = aDataMode;
            InitValues();
            SetOwner(aTextBox);
            _init = false;
            refresPPItems();
        }
        public override void SetOwner(IDrawObject aOwner)
        {
            _TextBox = (TextBoxEdit)aOwner;
            loadValues();
            saveValues();
        }
        private void InitValues()
        {
            foreach (var c in _dataModel.getColors())
            {
                Color cc = Color.FromRgb(c.R, c.G, c.B);
                _colorComboBox.Items.Add(new SolidColorBrush(cc));
            }
            string[] strArray = new string[72];
            for (Int32 i = 1; i <= 72; i++)
                strArray[i - 1] = i.ToString();
            _hight.ItemsSource = strArray; ;
            _textblok.Document.PageWidth = 10000;
        }
        private void loadValues()
        {
            foreach (var c in _colorComboBox.Items)
            {
                SolidColorBrush br=c as SolidColorBrush;
                if (WPFToFormConverter.comapareColor(_TextBox.FontColor, br.Color))
                {
                    _colorComboBox.SelectedItem = c;
                    break;
                }
            }
            _hight.SelectedIndex = System.Convert.ToInt32(_TextBox.FontSize);

            _fontfamily.SelectedItem = _TextBox.FontFamily;
            if ((_TextBox.FontStyle & System.Drawing.FontStyle.Bold) != 0)
            {
                _bold.IsChecked = true;
            }
            else
                _bold.IsChecked = false;
            if ((_TextBox.FontStyle & System.Drawing.FontStyle.Italic) != 0)
            {
                _italics.IsChecked = true;
            }
            else
                _italics.IsChecked = false;
            if ((_TextBox.FontStyle & System.Drawing.FontStyle.Strikeout) != 0)
            {
                _strikethrough.IsChecked = true;
            }
            else
                _strikethrough.IsChecked = false;
            _angleOfRotation.Text = _TextBox.AngleOfRotation.ToString();
            _pointPostion.SelectedItem = (int)_TextBox.PointPosition;
            _mFontScale.Text = _TextBox.FontScale.ToString();
            setText(_textblok.Document, _TextBox.Text);
        }
        private void refresPPItems()
        {
            if (_textblok == null || _init) return;
            _textblok.SelectAll();
            _textblok.FontFamily = (FontFamily)_fontfamily.SelectedItem;
            if (_bold.IsChecked == true)
                _textblok.FontWeight = FontWeights.Bold;
            else
                _textblok.FontWeight = FontWeights.Normal;
            if (_italics.IsChecked == true)
                _textblok.FontStyle = FontStyles.Italic;
            else
                _textblok.FontStyle = FontStyles.Normal;
            if (_strikethrough.IsChecked == true)
                _textblok.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
            else
                _textblok.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            _textblok.FontSize = System.Convert.ToDouble(_hight.SelectedIndex);
            _textblok.SetValue(Paragraph.LineHeightProperty, 1.0);
            _textblok.Foreground = (SolidColorBrush)_colorComboBox.SelectedValue;
        }
        public void saveValues()
        {
            if (_textblok == null || _init) return;
            _TextBox.FontFamily = new System.Drawing.FontFamily(_textblok.FontFamily.ToString());
            if (_bold.IsChecked.HasValue && _bold.IsChecked.Value)
            {
                _TextBox.FontStyle |= System.Drawing.FontStyle.Bold;
            }
            else
            {
                _TextBox.FontStyle &= ~System.Drawing.FontStyle.Bold;
            }
            if (_italics.IsChecked.HasValue && _italics.IsChecked.Value)
            {
                _TextBox.FontStyle |= System.Drawing.FontStyle.Italic;
            }
            else
            {
                _TextBox.FontStyle &= ~System.Drawing.FontStyle.Italic;
            }
            if (_strikethrough.IsChecked.HasValue && _strikethrough.IsChecked.Value)
            {
                _TextBox.FontStyle |= System.Drawing.FontStyle.Strikeout;
            }
            else
            {
                _TextBox.FontStyle &= ~System.Drawing.FontStyle.Strikeout;
            }
            SolidColorBrush fontColor = (SolidColorBrush)_colorComboBox.SelectedValue;
            if (fontColor != null)
                _TextBox.FontColor = WPFToFormConverter.getFormColor(fontColor.Color);
            _TextBox.FontSize = _textblok.FontSize;
            if (_angleOfRotation.Text.Length == 0)
                _TextBox.AngleOfRotation = 0;
            else
                _TextBox.AngleOfRotation = System.Convert.ToDouble(_angleOfRotation.Text);
            _TextBox.PointPosition = (PointPositions)_pointPostion.SelectedIndex;
            _TextBox.Text = GetText(_textblok.Document);
            if (_mFontScale.Text.Length == 0)
                _TextBox.FontScale= 0;
            else
                _TextBox.FontScale = (float)System.Convert.ToDouble(_mFontScale.Text);
            _TextBox.Reset();
        }
        public void onChanged(object sender, EventArgs e)
        {
            saveValues();
            refresPPItems();
        }
        private void onKeyUp(object sender, EventArgs e)
        {
            saveValues();
        }
        private void setText(FlowDocument doc, string aText)
        {
            if (aText.Length == 0) return;
            string[] paragraphs = aText.Split('\n');
            doc.Blocks.Clear();
            for(int i=0;i<paragraphs.Length;i++)
            {
                Paragraph myParagraph = new Paragraph();
                myParagraph.Inlines.Add(new Run(paragraphs[i]));
                doc.Blocks.Add(myParagraph);
            }
        }
        private string GetText(FlowDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            foreach (TextElement el in GetRunsAndParagraphs(doc))
            {
                Run run = el as Run;
                if(run!=null)
                    sb.Append(run == null ? Environment.NewLine : run.Text);
            }
            return sb.ToString();
        }
        private IEnumerable<TextElement> GetRunsAndParagraphs(FlowDocument doc)
        {
            // use the GetNextContextPosition method to iterate through the  
            // FlowDocument  

            for (TextPointer position = doc.ContentStart;
                position != null && position.CompareTo(doc.ContentEnd) <= 0;
                position = position.GetNextContextPosition(LogicalDirection.Forward))
            {
                if (position.GetPointerContext(LogicalDirection.Forward) ==
                    TextPointerContext.ElementEnd)
                {
                    // return solely the Runs and Paragraphs. all other elements are   
                    // ignored since they aren't supported by FormattedText.  

                    Run run = position.Parent as Run;

                    if (run != null)
                    {
                        yield return run;
                    }
                    else
                    {
                        Paragraph para = position.Parent as Paragraph;

                        if (para != null)
                        {
                            yield return para;
                        }
                    }
                }
            }
        }  
    }
}
