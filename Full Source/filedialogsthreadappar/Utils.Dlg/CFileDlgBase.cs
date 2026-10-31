using System.Drawing;
using System.Windows.Forms;

namespace Utils.Dlg;

public class CFileDlgBase
{
	public string FilePath { get; set; }

	public bool DialogSuccess { get; set; }

	public string Filter { get; set; }

	public string DefaultExt { get; set; }

	public string Title { get; set; }

	public string InitialDirectory { get; set; }

	public Point StartupLocation { get; set; }

	public virtual DialogResult ShowDialog()
	{
		return DialogResult.Cancel;
	}
}
