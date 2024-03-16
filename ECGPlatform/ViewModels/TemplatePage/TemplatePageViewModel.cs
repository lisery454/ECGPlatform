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

    public TemplatePageViewModel()
    {
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
        var index = await new ReadIndexFileService(new Deserializer()).Read(@"E:\Data\毕业设计\data\data_1\index.yaml");
        _ecgFileManager = new ECGFileManager(index);

        var templateDataFilePath = @"E:\Data\毕业设计\template_data.json";
        using var streamReader = new StreamReader(templateDataFilePath);
        var allText = await streamReader.ReadToEndAsync();
        var jArray = JArray.Parse(allText);
        var jArray1 = (JArray)((JObject)((JObject)jArray[1])["室性早搏"]!)["0"]!;
        var jArray2 = (JArray)((JObject)((JObject)jArray[2])["房性早搏"]!)["0"]!;
        var jArray3 = (JArray)((JObject)((JObject)jArray[3])["窦性心律"]!)["0"]!;
        var jArray4 = (JArray)((JObject)((JObject)jArray[4])["噪音"]!)["0"]!;

        var frame1 = (long)jArray1[0][0]!;
        var time1 = _ecgFileManager.FrameToTimeWave(frame1);
        var data1 = await _ecgFileManager.GetRangedWaveDataAsync(0, time1 - 1000, 2000);
        Series1.Add(BuildLineSeries(data1.Select(pointData => new ObservablePoint(pointData.time, pointData.value))));
        
        var frame2 = (long)jArray2[0][0]!;
        var time2 = _ecgFileManager.FrameToTimeWave(frame2);
        var data2 = await _ecgFileManager.GetRangedWaveDataAsync(0, time2 - 1000, 2000);
        Series2.Add(BuildLineSeries(data2.Select(pointData => new ObservablePoint(pointData.time, pointData.value))));
        
        var frame3 = (long)jArray3[0][0]!;
        var time3 = _ecgFileManager.FrameToTimeWave(frame3);
        var data3 = await _ecgFileManager.GetRangedWaveDataAsync(0, time3 - 1000, 2000);
        Series3.Add(BuildLineSeries(data3.Select(pointData => new ObservablePoint(pointData.time, pointData.value))));
        
        var frame4 = (long)jArray4[0][0]!;
        var time4 = _ecgFileManager.FrameToTimeWave(frame4);
        var data4 = await _ecgFileManager.GetRangedWaveDataAsync(0, time4 - 1000, 2000);
        Series4.Add(BuildLineSeries(data4.Select(pointData => new ObservablePoint(pointData.time, pointData.value))));
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