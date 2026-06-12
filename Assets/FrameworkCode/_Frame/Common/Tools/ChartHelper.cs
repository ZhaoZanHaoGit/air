using System.Collections.Generic;
using System.Linq;
using System;
using XCharts.Runtime;
using UnityEngine;

public enum LearnType
{
    None,
    LearnCount,
    LearnTime,
    Score
}
public class ChartHelper
{
    public static void UpdateChart<TEnum>(Serie serie, BarChart chart, LearnType learnType, TEnum noneValue) where TEnum : Enum {
        serie = chart.GetSerie(0);
        if (AppController.Instance.currentUserLearnDatas == null || AppController.Instance.currentUserLearnDatas.Count <= 0) {
            return;
        }

        List<int> courseIds = System.Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Where(type => !type.Equals(noneValue))
            .Select(type => (int)Convert.ChangeType(type, typeof(int)))
            .ToList();

        Dictionary<int, SoftwareLearningData> learnDataDict = AppController.Instance.currentUserLearnDatas
            .ToDictionary(data => data.CourseID, data => data);

        for (int i = 0; i < courseIds.Count; i++) {
            int courseId = courseIds[i];
            if (learnDataDict.TryGetValue(courseId, out SoftwareLearningData data)) {
                switch (learnType) {
                    case LearnType.LearnCount:
                        serie.UpdateData(i, 1, data.LearnCount);
                        break;
                    case LearnType.LearnTime:
                        float seconds = (float)data.LearnTime;
                        float minutes = seconds / 60f;
                        float roundedMinutes = Mathf.Round(minutes * 100f) / 100f;
                        double doubleMinutes = Math.Round((double)roundedMinutes, 2);
                        serie.UpdateData(i, 1, doubleMinutes);
                        break;
                    case LearnType.Score:
                        double _score = 0;
                        if (string.IsNullOrEmpty(data.Score)) {
                            _score = 0;
                        }
                        else {
                            _score = double.Parse(data.Score);
                        }
                        serie.UpdateData(i, 1, _score);
                        break;
                }
            }
            else {
                serie.UpdateData(i, 1, 0);
            }
        }

        chart.RefreshChart();
    }
    public static void UpdateChart<TEnum>(Serie serie, PieChart chart, LearnType learnType, TEnum noneValue) where TEnum : Enum {
        serie = chart.GetSerie(0);
        if (AppController.Instance.currentUserLearnDatas == null || AppController.Instance.currentUserLearnDatas.Count <= 0) {
            return;
        }

        List<int> courseIds = System.Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Where(type => !type.Equals(noneValue))
            .Select(type => (int)Convert.ChangeType(type, typeof(int)))
            .ToList();

        Dictionary<int, SoftwareLearningData> learnDataDict = AppController.Instance.currentUserLearnDatas
            .ToDictionary(data => data.CourseID, data => data);

        for (int i = 0; i < courseIds.Count; i++) {
            int courseId = courseIds[i];
            if (learnDataDict.TryGetValue(courseId, out SoftwareLearningData data)) {
                switch (learnType) {
                    case LearnType.LearnCount:
                        serie.UpdateData(i, 1, data.LearnCount);
                        break;
                    case LearnType.LearnTime:
                        float seconds = (float)data.LearnTime;
                        float minutes = seconds / 60f;
                        float roundedMinutes = Mathf.Round(minutes * 100f) / 100f;
                        double doubleMinutes = Math.Round((double)roundedMinutes, 2);
                        serie.UpdateData(i, 1, doubleMinutes);
                        break;
                    case LearnType.Score:
                        double _score = 0;
                        if (string.IsNullOrEmpty(data.Score)) {
                            _score = 0;
                        }
                        else {
                            _score = double.Parse(data.Score);
                        }
                        serie.UpdateData(i, 1, _score);
                        break;
                }
            }
            else {
                serie.UpdateData(i, 1, 0);
            }
        }

        chart.RefreshChart();
    }

    public static void ResetChart<TEnum>(Serie serie, BarChart chart, TEnum noneValue) {
        serie = chart.GetSerie(0);
        List<int> courseIds = System.Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Where(type => !type.Equals(noneValue))
            .Select(type => (int)Convert.ChangeType(type, typeof(int)))
            .ToList();
        Dictionary<int, SoftwareLearningData> learnDataDict = AppController.Instance.currentUserLearnDatas
            .ToDictionary(data => data.CourseID, data => data);

        for (int i = 0; i < courseIds.Count; i++) {
            serie.UpdateData(i, 1, 0);
        }
        chart.RefreshChart();
    }
    public static void ResetChart<TEnum>(Serie serie, PieChart chart, TEnum noneValue) {
        serie = chart.GetSerie(0);
        List<int> courseIds = System.Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Where(type => !type.Equals(noneValue))
            .Select(type => (int)Convert.ChangeType(type, typeof(int)))
            .ToList();
        Dictionary<int, SoftwareLearningData> learnDataDict = AppController.Instance.currentUserLearnDatas
            .ToDictionary(data => data.CourseID, data => data);

        for (int i = 0; i < courseIds.Count; i++) {
            serie.UpdateData(i, 1, 0);
        }
        chart.RefreshChart();
    }
    // 辅助方法：获取枚举的 None 值（需要手动实现）
    public static TEnum GetNoneValue<TEnum>() where TEnum : Enum {
        if (typeof(TEnum) == typeof(CLSType)) {
            return (TEnum)(object)CLSType.None;
        }
        else if (typeof(TEnum) == typeof(POTSType)) {
            return (TEnum)(object)POTSType.None;
        }
        else if (typeof(TEnum) == typeof(EVSType)) {
            return (TEnum)(object)EVSType.None;
        }
        throw new NotSupportedException($"None value not defined for {typeof(TEnum)}");
    }

    public static void UpdateChart_AllUserCount<TEnum>(List<SoftwareLearningData> datas, Serie serie, BarChart chart, LearnType learnType, TEnum noneValue) where TEnum : Enum {
        serie = chart.GetSerie(0);
        if (datas == null || datas.Count <= 0) {
            return;
        }

        List<int> courseIds = System.Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Where(type => !type.Equals(noneValue))
            .Select(type => (int)Convert.ChangeType(type, typeof(int)))
            .ToList();
        for (int i = 0; i < courseIds.Count; i++) {
            switch (learnType) {
                case LearnType.LearnCount:
                    int usercount = datas.FindAll(a => a.CourseID == courseIds[i]).Count;
                    serie.UpdateData(i, 1, usercount);
                    break;
                case LearnType.LearnTime:
                    List<SoftwareLearningData> datasArray = datas.FindAll(a => a.CourseID == courseIds[i]);
                    if (datasArray != null || datasArray.Count > 0) {
                        int total = datasArray.Sum(item => item.LearnTime);
                        float minutes = total / 60f;
                        float roundedMinutes = Mathf.Round(minutes * 100f) / 100f;
                        double doubleMinutes = Math.Round((double)roundedMinutes, 2);
                        serie.UpdateData(i, 1, doubleMinutes);
                    }
                    else {
                        serie.UpdateData(i, 1, 0);
                    }
                    break;
                case LearnType.Score:
                    break;
            }
        }

        chart.RefreshChart();
    }
    public static void UpdateChart_AllUserCount<TEnum>(List<SoftwareLearningData> datas, Serie serie, PieChart chart, LearnType learnType, TEnum noneValue) where TEnum : Enum {
        serie = chart.GetSerie(0);
        if (datas == null || datas.Count <= 0) {
            return;
        }

        List<int> courseIds = Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Where(type => !type.Equals(noneValue))
            .Select(type => (int)Convert.ChangeType(type, typeof(int)))
            .ToList();
        for (int i = 0; i < courseIds.Count; i++) {
            switch (learnType) {
                case LearnType.LearnCount:
                    int usercount = datas.FindAll(a => a.CourseID == courseIds[i]).Count;
                    serie.UpdateData(i, 1, usercount);
                    break;
                case LearnType.LearnTime:
                    List<SoftwareLearningData> datasArray = datas.FindAll(a => a.CourseID == courseIds[i]);
                    if (datasArray != null && datasArray.Count > 0) {
                        int total = datasArray.Sum(item => item.LearnTime);
                        float minutes = total / 60f;
                        float roundedMinutes = Mathf.Round(minutes * 100f) / 100f;
                        double doubleMinutes = Math.Round((double)roundedMinutes, 2);
                        serie.UpdateData(i, 1, doubleMinutes);
                    }
                    else {
                        serie.UpdateData(i, 1, 0);
                    }
                    break;
                case LearnType.Score:
                    List<SoftwareLearningData> datas1 = datas.FindAll(a => a.CourseID == courseIds[i]);
                    if (datas1 != null && datas1.Count > 0) {
                        float total = datas1.Sum(item => float.Parse(item.Score));
                        float minutes = total / (float)datas1.Count;
                        float roundedMinutes = Mathf.Round(minutes * 100f) / 100f;
                        double doubleMinutes = Math.Round((double)roundedMinutes, 2);
                        serie.UpdateData(i, 1, doubleMinutes);
                    }
                    else {
                        serie.UpdateData(i, 1, 0);
                    }
                    break;
            }
        }

        chart.RefreshChart();
    }
}
