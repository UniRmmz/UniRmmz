using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The command window for the horizontal selection format.
    /// </summary>
    public partial class Window_HorzCommand : Window_Command
    {
        protected override int MaxCols() => 4;
        protected override Bitmap.TextAlign ItemTextAlign() => Bitmap.TextAlign.Center;
    }
}