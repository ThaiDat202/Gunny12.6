using System;
using System.Threading;
using System.Windows.Forms;

namespace Utils.Dlg;

public class CFileOpenDlgThreadApartmentSafe : CFileDlgBase
{
	public override DialogResult ShowDialog()
	{
		DialogResult dlgRes = DialogResult.Cancel;
		Thread thread = new Thread((ThreadStart)delegate
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Multiselect = false,
				RestoreDirectory = true
			};
			if (!string.IsNullOrEmpty(base.FilePath))
			{
				openFileDialog.FileName = base.FilePath;
			}
			if (!string.IsNullOrEmpty(base.Filter))
			{
				openFileDialog.Filter = base.Filter;
			}
			if (!string.IsNullOrEmpty(base.DefaultExt))
			{
				openFileDialog.DefaultExt = base.DefaultExt;
			}
			if (!string.IsNullOrEmpty(base.Title))
			{
				openFileDialog.Title = base.Title;
			}
			if (!string.IsNullOrEmpty(base.InitialDirectory))
			{
				openFileDialog.InitialDirectory = base.InitialDirectory;
			}
			Form form = new Form();
			_ = base.StartupLocation;
			if (true)
			{
				form.StartPosition = FormStartPosition.Manual;
				form.Location = base.StartupLocation;
				form.DesktopLocation = base.StartupLocation;
			}
			form.Width = 0;
			form.Height = 0;
			dlgRes = openFileDialog.ShowDialog(form);
			if (dlgRes == DialogResult.OK)
			{
				base.FilePath = openFileDialog.FileName;
			}
		});
		try
		{
			thread.TrySetApartmentState(ApartmentState.STA);
			thread.Start();
			while (!thread.IsAlive)
			{
				Thread.Sleep(1);
			}
			Thread.Sleep(1);
			thread.Join();
			base.DialogSuccess = true;
		}
		catch (Exception)
		{
			base.DialogSuccess = false;
		}
		return dlgRes;
	}
}
