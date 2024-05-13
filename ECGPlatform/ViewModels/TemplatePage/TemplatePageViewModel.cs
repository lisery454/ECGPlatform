namespace ECGPlatform;

public partial class TemplatePageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Axis> _xAxes1;
    [ObservableProperty] private ObservableCollection<Axis> _yAxes1;
    [ObservableProperty] private DrawMarginFrame _drawMarginFrame1;
    [ObservableProperty] private ObservableCollection<ISeries> _series1;

    [ObservableProperty] private ObservableCollection<Axis> _xAxes2;
    [ObservableProperty] private ObservableCollection<Axis> _yAxes2;
    [ObservableProperty] private DrawMarginFrame _drawMarginFrame2;
    [ObservableProperty] private ObservableCollection<ISeries> _series2;

    [ObservableProperty] private ObservableCollection<Axis> _xAxes3;
    [ObservableProperty] private ObservableCollection<Axis> _yAxes3;
    [ObservableProperty] private DrawMarginFrame _drawMarginFrame3;
    [ObservableProperty] private ObservableCollection<ISeries> _series3;

    [ObservableProperty] private ObservableCollection<Axis> _xAxes4;
    [ObservableProperty] private ObservableCollection<Axis> _yAxes4;
    [ObservableProperty] private DrawMarginFrame _drawMarginFrame4;
    [ObservableProperty] private ObservableCollection<ISeries> _series4;

    [ObservableProperty] private float _yGridValue = 0.5f;
    [ObservableProperty] private float _xGridValue = 200;

    private ECGFileManager? _ecgFileManager;

    private IndexFileService _indexFileService;

    public TemplatePageViewModel(IndexFileService indexFileService)
    {
        _indexFileService = indexFileService;
        _xAxes1 = new ObservableCollection<Axis> { BuildXAxis() };
        _yAxes1 = new ObservableCollection<Axis> { BuildYAxis() };
        _drawMarginFrame1 = BuildDrawMarginFrame();
        _series1 = new ObservableCollection<ISeries>();

        _xAxes2 = new ObservableCollection<Axis> { BuildXAxis() };
        _yAxes2 = new ObservableCollection<Axis> { BuildYAxis() };
        _drawMarginFrame2 = BuildDrawMarginFrame();
        _series2 = new ObservableCollection<ISeries>();

        _xAxes3 = new ObservableCollection<Axis> { BuildXAxis() };
        _yAxes3 = new ObservableCollection<Axis> { BuildYAxis() };
        _drawMarginFrame3 = BuildDrawMarginFrame();
        _series3 = new ObservableCollection<ISeries>();

        _xAxes4 = new ObservableCollection<Axis> { BuildXAxis() };
        _yAxes4 = new ObservableCollection<Axis> { BuildYAxis() };
        _drawMarginFrame4 = BuildDrawMarginFrame();
        _series4 = new ObservableCollection<ISeries>();

        _ecgFileManager = null;
    }

    [RelayCommand]
    private async Task Init()
    {
        var index = await _indexFileService.Read(@"E:\Data\毕业设计\data\data_1\index.yaml");
        _ecgFileManager = new ECGFileManager(index);

        var templateDataFilePath = @"E:\Data\毕业设计\template_data.json";
        using var streamReader = new StreamReader(templateDataFilePath);
        var allText = await streamReader.ReadToEndAsync();
        var jArray = JArray.Parse(allText);

        Series1 = await GetSeries("室性早搏", 1, 2000, jArray, _ecgFileManager);
        Series2 = await GetSeries("房性早搏", 2, 2000, jArray, _ecgFileManager);
        Series3 = await GetSeries("窦性心律", 3, 2000, jArray, _ecgFileManager);
        Series4 = await GetSeries("噪音", 4, 2000, jArray, _ecgFileManager);

        async Task<ObservableCollection<ISeries>> GetSeries(string label, int id, long duration, JArray array,
            ECGFileManager ecgFileManager)
        {
            var subArray0 = (JArray)((JObject)((JObject)array[id])[label]!)["0"]!;
            var subArray1 = (JArray)((JObject)((JObject)array[id])[label]!)["1"]!;
            var subArray2 = (JArray)((JObject)((JObject)array[id])[label]!)["2"]!;

            var frameList = new List<long>();

            try
            {
                var value0 = subArray0[0][0];
                if (value0 != null) frameList.Add((long)value0);
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                var value1 = subArray1[0][0];
                if (value1 != null) frameList.Add((long)value1);
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                var value2 = subArray2[0][0];
                if (value2 != null) frameList.Add((long)value2);
            }
            catch (Exception)
            {
                // ignored
            }


            var timeList = frameList.Select(frame => ecgFileManager.FrameToTimeWave(frame) - duration / 2).ToList();
            var result = new ObservableCollection<ISeries>();

            foreach (var time in timeList)
            {
                var data = await ecgFileManager.GetRangedWaveDataAsync(0, time, duration);
                result.Add(BuildLineSeries(
                    data.Select(pointData => new ObservablePoint(pointData.time - time, pointData.value))));
            }

            return result;
        }
    }

    [RelayCommand]
    private void UnLoaded()
    {
        _ecgFileManager?.Dispose();
    }

    private static SKColor LabelColor => GetSKColor("ColorPrimaryAlpha90");
    private static SKColor SeparatorColor => GetSKColor("ColorOppositeAlpha40");
    private static SKColor SubseparatorsColor => GetSKColor("ColorOppositeAlpha20");
    private static SKColor TickColor => GetSKColor("ColorOppositeAlpha40");
    private static SKColor LineColor => GetSKColor("ColorPrimaryAlpha90");
    private static string FontFamily => "Chill Round Gothic Regular";


    private Axis BuildXAxis()
    {
        var labelsPaint = new SolidColorPaint
        {
            Color = LabelColor,
            FontFamily = FontFamily,
            SKFontStyle = new SKFontStyle(SKFontStyleWeight.Bold, SKFontStyleWidth.Normal,
                SKFontStyleSlant.Italic)
        };

        var separatorsPaint = new SolidColorPaint
        {
            StrokeThickness = 1f,
            Color = SeparatorColor,
        };
        var labeler = (double d) =>
        {
            var ms = (int)d;
            var s = ms / 1000;
            ms %= 1000;
            var m = s / 60;
            s %= 60;
            var h = m / 60;
            m %= 60;
            // return ms % 1000 == 0 ? $"{h}:{m:D2}:{s:D2}" : string.Empty;
            return string.Empty;
        };
        var subseparatorsPaint = true
            ? new SolidColorPaint
            {
                Color = SubseparatorsColor,
                StrokeThickness = 0.3f,
                // PathEffect = new DashEffect(new float[] { 3, 3 })
            }
            : new SolidColorPaint
            {
                Color = SKColors.Transparent,
            };
        var ticksPaint = new SolidColorPaint
        {
            StrokeThickness = 1f,
            Color = TickColor,
        };


        return new Axis
        {
            SeparatorsPaint = separatorsPaint,
            MinStep = XGridValue,
            ForceStepToMin = true,
            Labeler = labeler,
            Position = AxisPosition.End,
            LabelsPaint = labelsPaint,
            SubseparatorsPaint = subseparatorsPaint,
            SubseparatorsCount = 4,
            TicksPaint = ticksPaint,
        };
    }

    private Axis BuildYAxis()
    {
        var labelsPaint = new SolidColorPaint
        {
            Color = LabelColor,
            FontFamily = FontFamily,
            SKFontStyle = new SKFontStyle(SKFontStyleWeight.Bold, SKFontStyleWidth.Normal,
                SKFontStyleSlant.Oblique)
        };
        string Labeler(double d) => string.Empty;
        var separatorsPaint = new SolidColorPaint
        {
            Color = SeparatorColor,
            StrokeThickness = 1f,
        };
        var subseparatorsPaint = true
            ? new SolidColorPaint
            {
                Color = SubseparatorsColor,
                StrokeThickness = 0.3f,
            }
            : new SolidColorPaint
            {
                Color = SKColors.Transparent,
            };

        return new Axis
        {
            LabelsPaint = labelsPaint,
            LabelsAlignment = Align.End,
            Labeler = Labeler,
            TextSize = 10,
            MinStep = YGridValue,
            ForceStepToMin = true,
            UnitWidth = 1,
            SeparatorsPaint = separatorsPaint,
            SubseparatorsPaint = subseparatorsPaint,
            SubseparatorsCount = 4
        };
    }

    private DrawMarginFrame BuildDrawMarginFrame()
    {
        return new DrawMarginFrame
        {
            Fill = null,
            Stroke = null,
        };
    }

    private static SKColor GetSKColor(string name)
    {
        var color = (Color)Application.Current.FindResource(name)!;
        return new SKColor(color.R, color.G, color.B, color.A);
    }

    private ISeries BuildLineSeries(IEnumerable<ObservablePoint> points)
    {
        return new LineSeries<ObservablePoint>
        {
            DataPadding = new LvcPoint(0f, 0f),
            GeometryStroke = null,
            GeometryFill = null,
            Values = points,
            Fill = null,
            LineSmoothness = 1,
            Stroke = new SolidColorPaint(LineColor, 1.5f),
        };
    }
}