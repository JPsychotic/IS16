using SlimDX;
using System.IO;
using System;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace GameOfLife.Storage
{
  static class Config
  {
    private static int screens;

    #region Public properties / Config settings

    static int thickness = 21;
    public static int LineThickness { get { return thickness; } set { thickness = value; if (thickness < 1) thickness = 1; } }
    public static bool ShowFPS { get; set; }
    public static bool DisplayHelp { get; set; }
    public static int Delay = 8;
    public static int Width { get; set; }
    public static int Height { get; set; }
    public static int Vsync { get; set; }

    public static int DisplayScreen { get { return screens; } set { screens = value >= Screen.AllScreens.Length ? 0 : value; } }

    public static bool Paused { get; internal set; } = true;

    // summe an lebenden pixeln im umkreis:
    // 8 7 6 5 4 3 2 1 0
    // bei 3 (normale BirthRule):
    // bit an position 000001000
    // = 1 << 3   ==   0x8
    //
    // DeathRule: stirb außer bei 2 oder 3
    // bits an: 111110011
    // == 0x1F3

    public static event RuleChangedEventHandler RulesChanged;

    static uint death, birth;
    public static uint BirthRule { get { return birth; } internal set { birth = value; RulesChanged?.Invoke(); } }
    public static uint DeathRule { get { return death; } internal set { death = value; RulesChanged?.Invoke(); } }

    public static int MSAA_SampleCount = 1;
    public static int MSAA_Quality = 1;

    #endregion Public properties / Config settings

    static Config()
    {
      // 12345/3 a MAZE ing
      // 12345/7 zeigt die einzelnen zeichenlinien an
      // 23/3 normal
      // 237/3 normal aber eher ausbreitend
      // 238/3 normal aber mit schweif (leicht anderes verhalten)

      SetRuleFromString("23/3");

      ShowFPS = true;
      CultureInfo customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
      customCulture.NumberFormat.NumberDecimalSeparator = ".";
      Thread.CurrentThread.CurrentCulture = customCulture;

      ReadConfig();
    }

    public static void SetRuleFromString(string rule)
    {
      DeathRule = 0x1FF;
      foreach (var c in rule.Split('/')[0])
      {
        DeathRule -= (uint)(1 << int.Parse(c.ToString()));
      }
      BirthRule = 0;
      foreach (var c in rule.Split('/')[1])
      {
        BirthRule |= (uint)(1 << int.Parse(c.ToString()));
      }
    }

    public static string GetRuleAsString()
    {
      string rule = "";
      for (int i = 0; i < 9; i++)
      {
        if (((DeathRule >> i) & 1) == 0) rule += i;
      }
      rule += "/";
      for (int i = 0; i < 9; i++)
      {
        if (((BirthRule >> i) & 1) > 0) rule += i;
      }

      return rule;
    }

    public static void ReadConfig()
    {
      var lines = File.ReadAllLines("config.ini");

      foreach (var line in lines)
      {
        try
        {
          if (string.IsNullOrWhiteSpace(line) || line.IndexOf('#') == 0) continue;
          var l = line.Replace(" ", string.Empty);
          var parts = l.Split('=');
          var name = parts[0];
          var value = parts[1];

          var propertyInfo = typeof(Config).GetProperty(name);
          var type = propertyInfo.PropertyType;
          object parsedValue = null;

          if (type == typeof(float))
          {
            parsedValue = float.Parse(value);
          }
          else if (type == typeof(int))
          {
            parsedValue = int.Parse(value);
          }
          else if (type == typeof(Color3))
          {
            var valparts = value.Split(',');
            parsedValue = new Color3(float.Parse(valparts[0]) / 255, float.Parse(valparts[1]) / 255, float.Parse(valparts[2]) / 255);
          }
          else if (type == typeof(Vector3))
          {
            var valparts = value.Split(',');
            parsedValue = new Vector3(float.Parse(valparts[0]), float.Parse(valparts[1]), float.Parse(valparts[2]));
          }
          else if (type == typeof(bool))
          {
            parsedValue = int.Parse(value) != 0;
          }

          propertyInfo.SetValue(null, parsedValue, null);
        }
        catch
        {
          MessageBox.Show("Could not parse line:" + Environment.NewLine + '"' + line + '"');
        }
      }
    }

    internal static void Overwrite()
    {
      var lines = File.ReadAllLines("config.ini");

      for (int i = 0; i < lines.Length; i++)
      {
        var line = lines[i];
        try
        {
          if (string.IsNullOrWhiteSpace(line) || line.IndexOf('#') == 0) continue;
          var l = line.Replace(" ", string.Empty);
          var parts = l.Split('=');
          var name = parts[0];
          var value = parts[1];

          var propertyInfo = typeof(Config).GetProperty(name);
          var type = propertyInfo.PropertyType;
          object parsedValue = null;

          if (type == typeof(float))
          {
            parsedValue = float.Parse(value);
          }
          else if (type == typeof(int))
          {
            parsedValue = int.Parse(value);
          }
          else if (type == typeof(Color3))
          {
            var valparts = value.Split(',');
            parsedValue = new Color3(float.Parse(valparts[0]) / 255, float.Parse(valparts[1]) / 255, float.Parse(valparts[2]) / 255);
          }
          else if (type == typeof(Vector3))
          {
            var valparts = value.Split(',');
            parsedValue = new Vector3(float.Parse(valparts[0]), float.Parse(valparts[1]), float.Parse(valparts[2]));
          }
          else if (type == typeof(bool))
          {
            parsedValue = int.Parse(value) != 0;
          }
          propertyInfo.SetValue(null, parsedValue, null);

          lines[i] = line;
        }
        catch { }
      }

      File.WriteAllLines("config.ini", lines);
    }
  }

  public delegate void RuleChangedEventHandler();
}
