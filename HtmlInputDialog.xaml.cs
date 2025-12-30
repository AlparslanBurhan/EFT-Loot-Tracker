using System.Windows;

namespace EFTLootTracker;

public partial class HtmlInputDialog : Window
{
    public string HtmlContent { get; private set; } = string.Empty;

    public HtmlInputDialog()
    {
        InitializeComponent();
        HtmlTextBox.Focus();
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(HtmlTextBox.Text))
        {
            MessageBox.Show("Lütfen HTML içeriği yapıştırın!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!HtmlTextBox.Text.Contains("table-progress-tracking") && !HtmlTextBox.Text.Contains("tpt-"))
        {
            var result = MessageBox.Show(
                "Yapıştırılan içerik Collector tablosu içermiyor gibi görünüyor.\n\nDevam etmek istiyor musunuz?", 
                "Uyarı", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);
            
            if (result != MessageBoxResult.Yes)
                return;
        }

        HtmlContent = HtmlTextBox.Text;
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
