namespace ECGFileService;

public enum RPeakLabel
{
    NONE, // 无标签
    SINUS_RHYTHM, // 窦性心律
    VENTRICULAR_PREEXCITATION, // 心室预激
    PREMATURE_ATRIAL_CONTRACTIONS, // 房性早搏
    PREMATURE_VENTRICULAR_CONTRACTIONS, // 室性早搏
    ATRIAL_FIBRILLATION, // 心房颤动
    ATRIAL_FLUTTER, // 心房扑动 
    VENTRICULAR_FLUTTER_VENTRICULAR_FIBRILLATION, //室扑室颤
    ATRIOVENTRICULAR_BLOCK, // 房室传导阻滞
    NOISE, // 噪声
    PAROXYSMAL_SUPRAVENTRICULAR_TACHYCARDIA, //阵发性室上性心动过速
}


public static class RPeakLabelConvert{
    public static RPeakLabel StrToLabel(string labelStr)
    {
        return labelStr switch
        {
            "窦性心律" => RPeakLabel.SINUS_RHYTHM,
            "心室预激" => RPeakLabel.VENTRICULAR_PREEXCITATION,
            "房性早搏" => RPeakLabel.PREMATURE_ATRIAL_CONTRACTIONS,
            "室性早搏" => RPeakLabel.PREMATURE_VENTRICULAR_CONTRACTIONS,
            "心房颤动" => RPeakLabel.ATRIAL_FIBRILLATION,
            "心房扑动" => RPeakLabel.ATRIAL_FLUTTER,
            "室扑室颤" => RPeakLabel.VENTRICULAR_FLUTTER_VENTRICULAR_FIBRILLATION,
            "房室传导阻滞" => RPeakLabel.ATRIOVENTRICULAR_BLOCK,
            "噪声" => RPeakLabel.NOISE,
            "噪音" => RPeakLabel.NOISE,
            "阵发性室上性心动过速" => RPeakLabel.PAROXYSMAL_SUPRAVENTRICULAR_TACHYCARDIA,
            _ => RPeakLabel.NONE
        };
    }
}