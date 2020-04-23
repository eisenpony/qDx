using qdx.Samples.FileCreator.WPF.Diagnostics;
using qdx.Samples.FileCreator.WPF.ViewModel;
using Quotidian.Diagnostics;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace qdx.Samples.FileCreator.WPF.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileMakerView : Window
    {
        public FileMakerViewModel Context { get; set; }

        // override the tab behaviour on the dropdown to simulate tab-complete
        private void ComboBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab && sender is ComboBox combo)
            {
                var textBox = (TextBox)combo.Template.FindName("PART_EditableTextBox", combo);

                if (textBox.SelectionLength == 0)
                    return;

                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
                e.Handled = true;
            }
        }
    }
}
