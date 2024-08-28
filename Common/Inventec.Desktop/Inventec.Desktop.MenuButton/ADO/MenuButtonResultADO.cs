
namespace Inventec.Desktop.MenuButton.ADO
{
    public class MenuButtonResultADO
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Success { get; set; }
        public MenuButtonResultADO()
        {
           
        }
        public MenuButtonResultADO(int width, int height, bool success)
        {
            this.Width = width;
            this.Height = height;
            this.Success = success;
        }
    }
}
