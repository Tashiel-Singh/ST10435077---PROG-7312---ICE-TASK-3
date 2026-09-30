using System.Windows;

namespace ProductInventoryManager.App.Views;

public partial class ConfirmDialog : Window
{
    public ConfirmDialog(string productName, int productId, string price)
    {
        InitializeComponent();
        MessageTextBlock.Text = $"Are you sure you want to permanently delete Product #{productId}: \"{productName}\" ({price}) from the Dictionary?";
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
