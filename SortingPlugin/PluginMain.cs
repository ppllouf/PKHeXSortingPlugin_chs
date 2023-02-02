using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SortingPlugin {
  public class SortingPlugin : IPlugin {
    public string Name => nameof(SortingPlugin);
    public int Priority => 1; // Loading order, lowest is first.
    public ISaveFileProvider SaveFileEditor { get; private set; } = null!;
    public IPKMView PKMEditor { get; private set; } = null!;

    // Static Copies
    private static object[] globalArgs;
    private static ISaveFileProvider saveFileEditor;

    public void Initialize(params object[] args) {
      Console.WriteLine($"Loading {Name}...");
      if (args == null) return;
      globalArgs = args;
      SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider);
      PKMEditor = (IPKMView)Array.Find(args, z => z is IPKMView);
      saveFileEditor = SaveFileEditor;
      LoadMenuStrip();
    }

    public void NotifySaveLoaded() {
      Console.WriteLine($"{Name} was notified that a Save File was just loaded.");
      LoadMenuStrip();
    }

    public bool TryLoadFile(string filePath) {
      Console.WriteLine($"{Name} was provided with the file path, but chose to do nothing with it.");
      return false; // no action taken
    }

    public static void LoadMenuStrip() {
      ToolStrip menu = (ToolStrip)Array.Find(globalArgs, z => z is ToolStrip);
      ToolStripDropDownItem menuTools = menu.Items.Find("Menu_Tools", false)[0] as ToolStripDropDownItem;
      menuTools.DropDownItems.RemoveByKey("SortBoxesBy");
      ToolStripMenuItem sortBoxesItem = new ToolStripMenuItem("盒子排序插件") {
        Name = "SortBoxesBy",
        Image = Properties.Resources.SortIcon
      };
      menuTools?.DropDownItems.Add(sortBoxesItem);
      ToolStripItemCollection sortItems = sortBoxesItem.DropDownItems;

      int gen = saveFileEditor.SAV.Generation;
      GameVersion version = saveFileEditor.SAV.Version;
      bool isLetsGo = version == GameVersion.GP || version == GameVersion.GE;
      if (isLetsGo) {
        sortItems.Add(GetRegionalSortButton("第7代 关都地区", Gen7_Kanto.GetSortFunctions()));
      } else {
        bool isBDSP = version == GameVersion.BD || version == GameVersion.SP;
        bool isPLA  = version == GameVersion.PLA;

        if (gen >= 1) {
          sortItems.Add(GetRegionalSortButton("第1代 关都地区", Gen1_Kanto.GetSortFunctions()));
        }

        if (gen >= 2) {
          sortItems.Add(GetRegionalSortButton("第2代 城都地区", Gen2_Johto.GetSortFunctions()));
        }

        if (gen >= 3) {
          sortItems.Add(GetRegionalSortButton("第3代 丰缘地区", Gen3_Hoenn.GetSortFunctions()));
          sortItems.Add(GetRegionalSortButton("第3代 关都地区", Gen3_Kanto.GetSortFunctions()));
        }

        if (gen >= 4) {
          sortItems.Add(GetRegionalSortButton("第4代 神奥地区 钻石/珍珠", Gen4_Sinnoh.GetDPSortFunctions()));
          sortItems.Add(GetRegionalSortButton("第4代 神奥地区 白金", Gen4_Sinnoh.GetPtSortFunctions()));
          sortItems.Add(GetRegionalSortButton("第4代 成都地区", Gen4_Johto.GetSortFunctions()));
        }

        if (gen >= 5 && !isBDSP) {
          sortItems.Add(GetRegionalSortButton("第5代 合众地区 黑/白", Gen5_Unova.GetBWSortFunctions()));
          sortItems.Add(GetRegionalSortButton("第5代 合众地区 黑2/白2", Gen5_Unova.GetB2W2SortFunctions()));
        }

        if (gen >= 6 && !isBDSP) {
          if (PluginSettings.Default.显示个人图鉴) {
            sortItems.Add(GetAreaButtons("第6代 卡洛斯地区", new ToolStripItem[] {
              GetRegionalSortButton("中央地区", Gen6_Kalos.GetCentralDexSortFunctions()),
              GetRegionalSortButton("卡洛斯海岸地区", Gen6_Kalos.GetCostalDexSortFunctions()),
              GetRegionalSortButton("卡洛斯山岳地区 ", Gen6_Kalos.GetMountainDexSortFunctions())
            }));
          }
          sortItems.Add(GetRegionalSortButton("第6代 卡洛斯地区", Gen6_Kalos.GetSortFunctions()));
          sortItems.Add(GetRegionalSortButton("第6代 丰缘地区", Gen6_Hoenn.GetSortFunctions()));
        }

        if (gen >= 7 && !isBDSP && !isPLA) {
          if (PluginSettings.Default.显示个人图鉴) {
            sortItems.Add(GetAreaButtons("第7代 阿罗拉地区 太阳/月亮 岛屿", new ToolStripItem[] {
              GetRegionalSortButton("美乐美乐岛", Gen7_Alola.GetSMMelemeleSortFunctions()),
              GetRegionalSortButton("阿卡拉岛", Gen7_Alola.GetSMAkalaSortFunctions()),
              GetRegionalSortButton("乌拉乌拉岛", Gen7_Alola.GetSMUlaulaSortFunctions()),
              GetRegionalSortButton("波尼岛", Gen7_Alola.GetSMPoniSortFunctions())
            }));
          }
          sortItems.Add(GetRegionalSortButton("第7代 阿罗拉地区 太阳/月亮", Gen7_Alola.GetSMSortFunctions()));
          if (PluginSettings.Default.显示个人图鉴) {
            sortItems.Add(GetAreaButtons("第7代 阿罗拉地区 究极之日/究极之月 岛屿", new ToolStripItem[] {
              GetRegionalSortButton("美乐美乐岛", Gen7_Alola.GetUSUMMelemeleSortFunctions()),
              GetRegionalSortButton("阿卡拉岛", Gen7_Alola.GetUSUMAkalaSortFunctions()),
              GetRegionalSortButton("乌拉乌拉岛", Gen7_Alola.GetUSUMUlaulaSortFunctions()),
              GetRegionalSortButton("波尼岛", Gen7_Alola.GetUSUMPoniSortFunctions())
            }));
          }
          sortItems.Add(GetRegionalSortButton("第7代 阿罗拉地区 究极之日/究极之月", Gen7_Alola.GetUSUMSortFunctions()));
        }

        if (gen >= 8) {
          bool isSwSh = version == GameVersion.SW || version == GameVersion.SH;
          if (!isBDSP && !isPLA) {
            bool isScVi = version == GameVersion.SL || version == GameVersion.VL;
            if (!isScVi) {
              sortItems.Add(GetRegionalSortButton("第7代 关都地区", Gen7_Kanto.GetSortFunctions()));
            }
            if (PluginSettings.Default.显示个人图鉴) {
              sortItems.Add(GetAreaButtons("第8代 伽勒尔地区", new ToolStripItem[] {
                GetRegionalSortButton("伽勒尔地区", Gen8_Galar.GetGalarDexSortFunctions()),
                GetRegionalSortButton("铠岛", Gen8_Galar.GetIoADexSortFunctions()),
                GetRegionalSortButton("王冠雪原", Gen8_Galar.GetCTDexSortFunction())
              }));
            }
            sortItems.Add(GetRegionalSortButton("第8代 伽勒尔地区", Gen8_Galar.GetFullGalarDexSortFunctions()));
          }
          if (!isSwSh) {
            sortItems.Add(GetRegionalSortButton("第8代 神奥地区", Gen8_Sinnoh.GetSortFunctions()));
            if (!isBDSP) {
              if (PluginSettings.Default.显示个人图鉴) {
                sortItems.Add(GetAreaButtons("第8代 洗翠地区", new ToolStripItem[] {
                  GetRegionalSortButton("黑曜原野", Gen8_Hisui.GetObsidianFieldlandsSortFunctions()),
                  GetRegionalSortButton("红莲湿地", Gen8_Hisui.GetCrimsonMirelandsSortFunctions()),
                  GetRegionalSortButton("群青海岸", Gen8_Hisui.GetCobaltCoastlandsSortFunctions()),
                  GetRegionalSortButton("天冠山麓", Gen8_Hisui.GetCoronetHighlandsSortFunctions()),
                  GetRegionalSortButton("纯白冻土", Gen8_Hisui.GetAlabasterIcelandsSortFunctions())
                }));
              }
              sortItems.Add(GetRegionalSortButton("第8代 洗翠地区", Gen8_Hisui.GetSortFunctions()));
            }
          }
        }

        if (gen >= 9) {
          sortItems.Add(GetRegionalSortButton("第9代 帕底亚地区", Gen9_Paldea.GetSortFunctions()));
        }

        if(gen != 1) {
          ToolStripMenuItem nationalDexSortButton = new ToolStripMenuItem("全国图鉴");
          nationalDexSortButton.Click += (s, e) => SortByFunctions();
          sortItems.Add(nationalDexSortButton);

          if(gen >= 7 && !isBDSP) {
            ToolStripMenuItem nationalDexWithFormSortButton = new ToolStripMenuItem("全国图鉴 (地区形态与世代)");
            nationalDexWithFormSortButton.Click += (s, e) => SortByFunctions(Gen_National.GetNationalDexWithRegionalFormsSortFunctions());
            sortItems.Add(nationalDexWithFormSortButton);
          }
        }
      }

      ToolStripMenuItem settingsButton = new ToolStripMenuItem("设置");
      settingsButton.Click += (s, e) => new SettingsForm().ShowDialog();
      sortItems.Add(settingsButton);
    }

    private static void SortByFunctions(Func<PKM, IComparable>[] sortFunctions = null) {
      int beginIndex = PluginSettings.Default.盒子排序结束 - 1;
      int endIndex = PluginSettings.Default.盒子排序开始 < 0 ? -1 : PluginSettings.Default.盒子排序开始 - 1;
      if (sortFunctions != null) {
        IEnumerable<PKM> sortMethod(IEnumerable<PKM> pkms, int start) => pkms.OrderByCustom(sortFunctions);
        saveFileEditor?.SAV.SortBoxes(beginIndex, endIndex, sortMethod);
      } else {
        saveFileEditor?.SAV.SortBoxes(beginIndex, endIndex);
      }
      saveFileEditor?.ReloadSlots();
    }

    private static ToolStripItem GetRegionalSortButton(string dex, Func<PKM, IComparable>[] sortFunctions) {
      ToolStripMenuItem dexSortButton = new ToolStripMenuItem($"{dex} 地区图鉴");
      dexSortButton.Click += (s, e) => SortByFunctions(sortFunctions);
      return dexSortButton;
    }

    private static ToolStripMenuItem GetAreaButtons(string name, ToolStripItem[] sortButtons) {
      ToolStripMenuItem areas = new ToolStripMenuItem(name);
      areas.DropDownItems.AddRange(sortButtons);
      return areas;
    }

  }
}
