using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _927.ViewModels
{
    /// <summary>
    /// 警報記錄 ViewModel - 用於 DataGridView 綁定
    /// 從 927-Demo 的 dgvAlarm 移植，增強為 MVVM 模式
    /// </summary>
    public class AlarmEntryViewModel : INotifyPropertyChanged
    {
        private DateTime _timestamp;
        private string _alarmCode = string.Empty;
        private string _message = string.Empty;
        private AlarmLevel _level;
        private int _occurrenceCount;

        /// <summary>
        /// 警報發生時間
        /// </summary>
        public DateTime Timestamp
        {
            get => _timestamp;
            set
            {
                if (_timestamp != value)
                {
                    _timestamp = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 警報代碼 (如：ALM-001)
        /// </summary>
        public string AlarmCode
        {
            get => _alarmCode;
            set
            {
                if (_alarmCode != value)
                {
                    _alarmCode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 警報訊息內容
        /// </summary>
        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 警報等級 (決定顏色編碼)
        /// </summary>
        public AlarmLevel Level
        {
            get => _level;
            set
            {
                if (_level != value)
                {
                    _level = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 發生次數 (用於警報風暴抑制後的累計顯示)
        /// </summary>
        public int OccurrenceCount
        {
            get => _occurrenceCount;
            set
            {
                if (_occurrenceCount != value)
                {
                    _occurrenceCount = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 顯示用的次數字串 (如："x5" 或 "")
        /// </summary>
        public string OccurrenceDisplay => OccurrenceCount > 1 ? $"x{OccurrenceCount}" : string.Empty;

        /// <summary>
        /// 格式化時間顯示 (HH:mm:ss)
        /// </summary>
        public string TimeDisplay => Timestamp.ToString("HH:mm:ss");

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// 掃描記錄 ViewModel - 用於 dgvScanLog 綁定
    /// 從 927-Demo 的 ScanLog 移植
    /// </summary>
    public class ScanEntryViewModel : INotifyPropertyChanged
    {
        private DateTime _scanTime;
        private string _barcode = string.Empty;
        private bool _isSuccess;
        private string _recipeNo = string.Empty;
        private string _statusMessage = string.Empty;

        /// <summary>
        /// 掃描時間
        /// </summary>
        public DateTime ScanTime
        {
            get => _scanTime;
            set
            {
                if (_scanTime != value)
                {
                    _scanTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 條碼內容
        /// </summary>
        public string Barcode
        {
            get => _barcode;
            set
            {
                if (_barcode != value)
                {
                    _barcode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否成功解碼
        /// </summary>
        public bool IsSuccess
        {
            get => _isSuccess;
            set
            {
                if (_isSuccess != value)
                {
                    _isSuccess = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 配方編號
        /// </summary>
        public string RecipeNo
        {
            get => _recipeNo;
            set
            {
                if (_recipeNo != value)
                {
                    _recipeNo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 狀態訊息
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// 配方資料 ViewModel - 用於 Recipe 分頁綁定
    /// </summary>
    public class RecipeViewModel : INotifyPropertyChanged
    {
        private uint _recipeNo;
        private string _recipeName = string.Empty;
        private bool _isValid;
        private string _validationMessage = string.Empty;

        /// <summary>
        /// 配方編號
        /// </summary>
        public uint RecipeNo
        {
            get => _recipeNo;
            set
            {
                if (_recipeNo != value)
                {
                    _recipeNo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 配方名稱
        /// </summary>
        public string RecipeName
        {
            get => _recipeName;
            set
            {
                if (_recipeName != value)
                {
                    _recipeName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 驗證是否通過
        /// </summary>
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 驗證失敗訊息
        /// </summary>
        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                if (_validationMessage != value)
                {
                    _validationMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
