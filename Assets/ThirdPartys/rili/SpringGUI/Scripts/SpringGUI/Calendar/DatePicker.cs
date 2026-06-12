
/*=========================================
* Author: springDong
* Description: SpringGUI.DatePicker
* DatePicker has lisened onDayClick/onMonthClick/onYearClick three interfaces .
* You can set date by DateTime property.
==========================================*/

using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpringGUI
{
    public class DatePicker : UIBehaviour
    {
        private TMP_Text _dateText = null;
        private Calendar _calendar = null;
        private DateTime _dateTime = DateTime.Today;
        public bool isStart=false;
        public SelectTimeForm _selectTimeForm;
        // get data from this property
        public DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                refreshDateText();
            }
        }
        public TMP_Text timeText;

        protected override void Awake()
        {
            _dateText = this.transform.Find("DatePicker/DateText").GetComponent<TMP_Text>();
            _calendar = this.transform.Find("DatePicker/Calendar").GetComponent<Calendar>();
            _calendar.onDayClick.AddListener(dateTime => { DateTime = dateTime; });
            transform.Find("DatePicker/PickButton").GetComponent<Button>().onClick.AddListener(( ) =>
             {             
                 DateTime dateParsed = DateTime.Parse(_dateText.text);
                 string time = dateParsed.ToString("yyyy-MM-dd");
                 if (isStart)
                 {
                     dateParsed=dateParsed.AddHours(00).AddMinutes(00).AddSeconds(00);
                     _selectTimeForm._StartTime=dateParsed;
                     timeText.text = time + "(00:00:00)";
                 }
                 else {
                     dateParsed = dateParsed.AddHours(23).AddMinutes(59).AddSeconds(59);
                     _selectTimeForm._EndTime = dateParsed;
                     timeText.text = time + "(23:59:59)";
                 }       
                 this.gameObject.SetActive(false);
             });
            refreshDateText();
        }


        private void refreshDateText()
        {
            if (_calendar.DisplayType == E_DisplayType.Standard)
            {
                switch (_calendar.CalendarType)
                {
                    case E_CalendarType.Day:
                        _dateText.text = DateTime.ToShortDateString();
                        break;
                    case E_CalendarType.Month:
                        _dateText.text = DateTime.Year + "/" + DateTime.Month;
                        break;
                    case E_CalendarType.Year:
                        _dateText.text = DateTime.Year.ToString();
                        break;
                }
            }
            else
            {
                switch ( _calendar.CalendarType )
                {
                    case E_CalendarType.Day:
                        _dateText.text = DateTime.Year + "年" + DateTime.Month + "月" + DateTime.Day + "日";
                        break;
                    case E_CalendarType.Month:
                        _dateText.text = DateTime.Year + "年" + DateTime.Month + "月";
                        break;
                    case E_CalendarType.Year:
                        _dateText.text = DateTime.Year + "年";
                        break;
                }
            }
           // _calendar.gameObject.SetActive(false);
        }
    }
}