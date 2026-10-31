using System;
using System.Threading;
using System.Windows.Forms;

namespace Utils.Dlg;

public class CFileSaveDlgThreadApartmentSafe : CFileDlgBase
{
	public override DialogResult ShowDialog()
	{
		DialogResult dlgRes = DialogResult.Cancel;
		Thread thread = new Thread((ThreadStart)delegate
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				RestoreDirectory = true
			};
			if (!string.IsNullOrEmpty(base.FilePath))
			{
				saveFileDialog.FileName = base.FilePath;
			}
			if (!string.IsNullOrEmpty(base.Filter))
			{
				saveFileDialog.Filter = base.Filter;
			}
			if (!string.IsNullOrEmpty(base.DefaultExt))
			{
				saveFileDialog.DefaultExt = base.DefaultExt;
			}
			if (!string.IsNullOrEmpty(base.Title))
			{
				saveFileDialog.Title = base.Title;
			}
			if (!string.IsNullOrEmpty(base.InitialDirectory))
			{
				saveFileDialog.InitialDirectory = base.InitialDirectory;
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
			dlgRes = saveFileDialog.ShowDialog(form);
			if (dlgRes == DialogResult.OK)
			{
				base.FilePath = saveFileDialog.FileName;
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
