using SlimDX.Windows;

namespace GameOfLife
{
  static class Program
  {
    static void Main()
    {
      //Configuration.DetectDoubleDispose = true;
      //Configuration.EnableObjectTracking = true;
      //Configuration.ThrowOnError = true;
      //Configuration.ThrowOnShaderCompileError = true;

      float elapsed;
      RenderFrame game = new RenderFrame();
      MessagePump.Run(game.RenderForm, () =>
      {
        elapsed = game.ElapsedTime();
        game.Update(elapsed);
        game.Render(elapsed);
      });

    }
  }
}
