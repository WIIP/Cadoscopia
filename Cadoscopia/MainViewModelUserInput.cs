using Microsoft.Win32;

namespace Cadoscopia
{
    class MainViewModelUserInput : IMainViewModelUserInput
    {
        public string GetSaveFileName()
        {
            var sfd = new SaveFileDialog
            {
                Filter = "XML (*.xml)|*.xml"
            };
            return sfd.ShowDialog() == true ? sfd.FileName : null;
        }
    }
}