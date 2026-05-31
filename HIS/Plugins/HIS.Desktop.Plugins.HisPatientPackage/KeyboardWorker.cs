/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;

namespace HIS.Desktop.Plugins.HisPatientPackage
{
    /// <summary>
    /// Phím tắt cho màn 6.2 Danh sách gói (UserControl):
    /// Ctrl+F = Tìm, Ctrl+R = Làm lại.
    /// </summary>
    [KeyboardAction("Refesh", "HIS.Desktop.Plugins.HisPatientPackage.UcHisPatientPackage", KeyStroke = XKeys.Control | XKeys.R)]
    [KeyboardAction("Search", "HIS.Desktop.Plugins.HisPatientPackage.UcHisPatientPackage", KeyStroke = XKeys.Control | XKeys.F)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public sealed class KeyboardWorker : Tool<IDesktopToolContext>
    {
        public KeyboardWorker() : base() { }

        public override IActionSet Actions
        {
            get { return base.Actions; }
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
