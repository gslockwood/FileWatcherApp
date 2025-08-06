using DCSDebriefFile;
using DCSDebriefForm.Model;
using DCSDebriefForm.View;
using FileWatcher;
using System.Text.Json;
using Utililites;

namespace DCSDebriefForm
{
    public partial class MainForm : Form
    {
        private readonly ReadReader readReader;

        private FileSystemEventWatcher? watcher;
        private readonly string lsoGradeDataJson;

        readonly TimeChecker timeChecker = new();

        readonly string lsoGradeTableFile;
        private readonly string? greenFlashSoftware;
        readonly Settings? settings;
        private bool dirty;

        public MainForm()
        {
            InitializeComponent();

            //DCSDebriefFormTests.Tests.TestReadGrades(); return;

            greenFlashSoftware = Environment.GetEnvironmentVariable("GreenFlashSoftware");
            string dataFilePath = greenFlashSoftware + "\\DCSDebriefing\\";

            if( !Directory.Exists(dataFilePath) )
                throw new DirectoryNotFoundException($"The directory '{dataFilePath}' does not exist.");


            lsoGradeDataJson = $@"{dataFilePath}settings.json";
            if( !File.Exists(lsoGradeDataJson) )
                throw new DirectoryNotFoundException($"The data file '{lsoGradeDataJson}' does not exist.");


            string json = File.ReadAllText(lsoGradeDataJson);
            settings = JsonSerializer.Deserialize<Settings>(json);
            if( settings == null ) throw new NullReferenceException(nameof(settings));

            IDictionary<DateTime, LSOGrade>? data = settings?.Data;
            if( data == null ) throw new NullReferenceException(nameof(data));

            data = new SortedDictionary<DateTime, LSOGrade>(data, Comparer<DateTime>.Create((d1, d2) => d2.CompareTo(d1)));

            IDictionary<DateTime, LSOGrade>? entries;
            if( data.Count < 10 )
                entries = data;
            else
                entries = data.Take(10).ToDictionary(entry => entry.Key, entry => entry.Value);

            if( settings == null ) throw new NullReferenceException(nameof(settings));
            settings.Data = entries;


            //string dcsBriefingLog = @"C:\Users\george s. lockwood\Saved Games\DCS\Missions\FA-18C\F18 Case Recoveries\debriefing.log";
            string? dcsBriefingLog = settings.DcsBriefingLogFileName;

            //dcsBriefingLog = @"E:\Program Files\Eagle Dynamics\DCS World\lsogrades.csv";


            if( !File.Exists(dcsBriefingLog) )
                throw new FileNotFoundException($"The file '{dcsBriefingLog}' does not exist.");


            FileInfo fileInfo = new(dcsBriefingLog);



            if( string.IsNullOrEmpty(fileInfo.DirectoryName) ) throw new DirectoryNotFoundException(fileInfo.DirectoryName);
            if( string.IsNullOrEmpty(fileInfo.Name) ) throw new FileNotFoundException(fileInfo.Name);

            watcher = new FileSystemEventWatcher(fileInfo.DirectoryName, fileInfo.Name, this);

            // Subscribe to the events.
            watcher.FileChanged += Watcher_FileChanged;
            //watcher.FileChanged += (sender, FileSystemEventArgs) => { };
            //watcher.FileCreated += OnFileCreated;
            //watcher.FileDeleted += OnFileDeleted;
            //watcher.FileRenamed += OnFileRenamed;
            //watcher.FileError += OnFileError;

            watcher.StartWatching(); // Start the watcher.

            lsoGradeTableFile = $@"{dataFilePath}LSOGRADETABLE.json";

            readReader = new DCSDebriefFile.ReadReader(dcsBriefingLog, lsoGradeTableFile);//@".\LSOGRADETABLE.json"
            readReader.ReadCompleted += (list) =>
            {
                if( list == null || list.Count == 0 ) return;
                Logger.Log("readReader.ReadCompleted with a list of grades");
                ProcessLSOList(list);
            };
            readReader.Refresh += () => { ProcessLSOList(); };


            // get the data now
            if( !Directory.Exists(dataFilePath) )
                throw new DirectoryNotFoundException($"The directory '{dataFilePath}' does not exist.");


            //// testing only
            //dcsBriefingLog = @"E:\Program Files\Eagle Dynamics\DCS World\lsogrades.csv";
            //readReader.ReadFile(dcsBriefingLog);
            //
        }

        private void UpdateDB()
        {
            if( !dirty ) return;

            if( settings == null ) throw new NullReferenceException(nameof(settings));
            if( settings.Data == null ) throw new NullReferenceException(nameof(settings.Data));

            File.WriteAllText(lsoGradeDataJson, JsonSerializer.Serialize(settings));

            //if( data == null ) throw new NullReferenceException(nameof(data));
            //File.WriteAllText(@".\LSOGrades.json", JsonSerializer.Serialize(data));
            //
        }

        protected override void OnLoad(EventArgs e)
        {
            //return;


            base.OnLoad(e);

            if( settings == null ) throw new NullReferenceException(nameof(settings));
            if( settings.Data == null ) throw new NullReferenceException(nameof(settings.Data));

            lsoGradeListBox.GradeItemFirstColumnClicked += GradeItemFirstColumnClicked;

            ProcessLSOList();
            //
        }

        private void GradeItemFirstColumnClicked(object? sender, GradeItemClickEventArgs e)
        {
            Clipboard.SetText(e.ClickedListViewItem.Text);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            lsoGradeListBox.Width += 1;
        }

        private void ProcessLSOList(List<LSOGrade>? list = null)
        {
            if( lsoGradeListBox.InvokeRequired )
            {
                this.Invoke(new MethodInvoker(() => { ProcessLSOList(list); }));
                return;
            }
            else
            {
                if( settings == null ) throw new NullReferenceException(nameof(settings));
                if( settings.Data == null ) throw new NullReferenceException(nameof(settings.Data));

                var data = settings.Data;

                //this.Activate();


                if( list != null )
                {
                    // update the data member with new items
                    foreach( LSOGrade lsoGrade in list )
                    {
                        if( !data.ContainsKey(lsoGrade.DateTime) )
                        {
                            this.dirty = true;
                            // add so you can pesist later
                            data.Add(lsoGrade.DateTime, lsoGrade);

                            LsoGradeItem item = new()
                            {
                                DateTime = lsoGrade.DateTime,
                                Pilot = lsoGrade.Pilot,
                                UnitType = lsoGrade.UnitType,
                                Carrier = lsoGrade.Carrier,
                                //Grade = lsoGrade.Grade,
                                Grade = $"{lsoGrade.Grade}: {readReader.GetGradeTranslation(lsoGrade.Grade)}",
                                Wire = lsoGrade.Wire,
                                LsoGrade = lsoGrade.ErrorStr,
                                Errors = readReader.GetErrors(lsoGrade.ErrorStr)
                            };

                            lsoGradeListBox.Insert(item);
                        }
                    }

                    //return;
                    //
                }

                else//if( list == null )//list == null listView.Items.Count == 0
                {
                    if( data == null ) throw new NullReferenceException(nameof(data));

                    lsoGradeListBox.Controls.Clear();
                    lsoGradeListBox.SuspendLayout();

                    foreach( KeyValuePair<DateTime, LSOGrade> kvPair in data )
                    {
                        LSOGrade lsoGrade = kvPair.Value;
                        if( lsoGrade != null )
                        {
                            try
                            {
                                if( lsoGrade != null )
                                {
                                    var startTime = kvPair.Key;

                                    LsoGradeItem item = new()
                                    {
                                        DateTime = startTime,
                                        //DateTime = lsoGrade.DateTime,// this is not in the settings file json
                                        Pilot = lsoGrade.Pilot,
                                        UnitType = lsoGrade.UnitType,
                                        Carrier = lsoGrade.Carrier,
                                        Grade = $"{lsoGrade.Grade}: {readReader.GetGradeTranslation(lsoGrade.Grade)}",
                                        Wire = lsoGrade.Wire,
                                        LsoGrade = lsoGrade.ErrorStr,
                                        Errors = readReader.GetErrors(lsoGrade.ErrorStr)

                                    };

                                    lsoGradeListBox.Controls.Add(item);
                                    //
                                }

                            }
                            catch( Exception )
                            {
                                //throw ex;
                            }
                        }

                    }

                    lsoGradeListBox.ResumeLayout();
                    lsoGradeListBox.AutoScrollPosition = new Point(0, 0);
                    //
                }

            }

        }

        private void Watcher_FileChanged(object? sender, FileSystemEventArgs e)
        {
            if( timeChecker.IsLessThanSecondsSinceLastExecution(5) ) return;

            if( e == null ) return;
            string inputFilePath = e.FullPath;

            Logger.Log($"File {inputFilePath} has been changed at {DateTime.Now}.");

            if( FileAccessChecker.IsFileAccessible(inputFilePath) )
            {
                Logger.Log($"'{inputFilePath}' is currently accessible.");
                readReader.ReadFile(inputFilePath);
            }
            else
            {
                Logger.Log($"'{inputFilePath}' is NOT currently accessible (likely in use).");

                try
                {
                    Logger.Log($"Waiting for '{inputFilePath}' to become accessible...");
                    FileAccessChecker.WaitForFileAccess(inputFilePath, 10000); // Wait up to 10 seconds
                                                                               // Now you can work with the file
                    try
                    {
                        string content = File.ReadAllText(inputFilePath);
                        Logger.Log($"Successfully read file content: {content}");
                        //File.Delete(inputFilePath); // Clean up the test file

                        readReader.ReadFile(inputFilePath);

                    }
                    catch( IOException ex )
                    {
                        Logger.Log($"Error reading file after it became accessible: {ex.Message}");
                    }
                }
                catch( TimeoutException ex )
                {
                    Logger.Log(ex.Message);
                }
            }
            //
        }

    }

}

namespace DCSDebriefFormTests
{
    public static class Tests
    {
        static DCSDebriefFile.ILsoGradeTranslator? lsoGradeTranslator;

        public static void TestReadGrades()
        {
            string dcsBriefingLog = @"C:\Users\george s. lockwood\Saved Games\DCS\Logs\LSOGrades.csv";
            string lsoGradeTableFile = @"C:\Users\george s. lockwood\OneDrive\GreenFlashSoftware\DCSDebriefing\LSOGRADETABLE.json";
            ReadReader readReader = new(dcsBriefingLog, lsoGradeTableFile);
            readReader.ReadCompleted += (list) =>
            {
                if( list == null || list.Count == 0 ) return;
                Logger.Log("readReader.ReadCompleted with a list of grades");
                if( list != null )
                {
                    // update the data member with new items
                    foreach( LSOGrade lsoGrade in list )
                    {
                        var DateTime = lsoGrade.DateTime;
                        var Pilot = lsoGrade.Pilot;
                        var UnitType = lsoGrade.UnitType;
                        var Carrier = lsoGrade.Carrier;
                        //Grade = lsoGrade.Grade,
                        var Grade = $"{lsoGrade.Grade}: {readReader.GetGradeTranslation(lsoGrade.Grade)}";
                        var Wire = lsoGrade.Wire;
                        var LsoGrade = lsoGrade.ErrorStr;
                        var Errors = readReader.GetErrors(lsoGrade.ErrorStr);
                    }
                    //
                }
            };
            readReader.ReadFile(dcsBriefingLog);

        }
        public static void TestGrades()
        {
            string grade = "LSO: GRADE:C : (DLX)  _LULX_  _LULIM_  (DLIM)  _LULIC_  (DLIC)  (LLIW)  LRWDIW  3PTSIW  WIRE# 1 _EGIW_ ";

            //DCSDebriefFile.ILsoGradeTranslator lsoGradeTranslator;

            //lsoGradeTranslator = new DCSDebriefFile.LsoGradeTranslator(@".\LSOGRADETABLE.json");
            lsoGradeTranslator = new DCSDebriefFile.LsoGradeTranslatorLSOGradeFile(@".\LSOGRADETABLE.json");

            grade = "2025-05-16 17:54:24,GSL Hornet,CVN-72 Abraham Lincoln,GRADE:B  _DRX_  _LURX_  FX  _DRIM_  (LURIM)  _LOIC_  _LOAR_  BIW WIRE# 1 ";

            LSOGrade? statement = lsoGradeTranslator.GetLSOGrade(grade);
            if( statement != null )
            {
                if( statement.Grade != null )
                    Logger.Log($"{statement.Grade}\n{lsoGradeTranslator.GetGradeTranslation(statement.Grade)}");

                Logger.Log($"{grade}\n{statement}");

            }
        }

        //    again:
        //DCSDebriefFile.LSOGradeInfo? lSOGradeInfo = null;

        //    lSOGradeInfo = new ("LSO: GRADE:C : (DLX)  _LULX_  _LULIM_  (DLIM)  _LULIC_  (DLIC)  (LLIW)  LRWDIW  3PTSIW  WIRE# 1 _EGIW_ ");
        //Logger.Log($"{lSOGradeInfo.LsoGradesComment}\n{lSOGradeInfo.Translation}");

        //lSOGradeInfo = new ("LSO: GRADE:B _SLOX_ _LOIM_ _TMRDIC_ _LOIC_ WIRE#3[bC]");
        //Logger.Log($"{lSOGradeInfo.LsoGradesComment}\n{lSOGradeInfo.Translation}");

        //lSOGradeInfo = new ("LSO: GRADE:C : LNFIW _EGIW_ WIRE# 2[BC]");
        //Logger.Log($"\n{lSOGradeInfo.LsoGradesComment}\n{lSOGradeInfo.Translation}");

        //lSOGradeInfo = new ("LSO: GRADE:B _DRX_ _LURX_ _FX_ (LURIM) (DLIC)");
        //Logger.Log($"\n{lSOGradeInfo.LsoGradesComment}\n{lSOGradeInfo.Translation}");

        //lSOGradeInfo = new ("LSO: GRADE:WO _LULIM_ (LURIC) WO(AFU)IC _LRTL_");
        //Logger.Log($"\n{lSOGradeInfo.LsoGradesComment}\n{lSOGradeInfo.Translation}");
        //goto again;

    }

}