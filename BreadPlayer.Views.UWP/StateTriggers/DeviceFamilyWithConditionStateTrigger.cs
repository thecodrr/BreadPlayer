using System;
using Windows.System.Profile;
using Windows.UI.Xaml;

namespace BreadPlayer.StateTriggers
{
    public class DeviceFamilyWithConditionStateTrigger : StateTriggerBase, ITriggerValue
    {
        private static string _deviceFamily;

        static DeviceFamilyWithConditionStateTrigger()
        {
            _deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
        }

        /// <summary>
        /// Gets or sets the device family to trigger on.
        /// </summary>
        /// <value>The device family.</value>
        public DeviceFamily DeviceFamily
        {
            get => (DeviceFamily)GetValue(DeviceFamilyProperty);
            set => SetValue(DeviceFamilyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DeviceFamily"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DeviceFamilyProperty =
            DependencyProperty.Register("DeviceFamily", typeof(bool), typeof(DeviceFamilyWithConditionStateTrigger),
            new PropertyMetadata(false, OnDeviceFamilyPropertyChanged));

        private static void OnDeviceFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (DeviceFamilyWithConditionStateTrigger)d;
            var val = (DeviceFamily)e.NewValue;
            if ((DeviceFamily)Enum.Parse(typeof(DeviceFamily), _deviceFamily.Remove(0, 8)) == val)
            {
                obj.IsActive = obj.Condition;
            }
        }

        /// <summary>
        /// Gets or sets the device family to trigger on.
        /// </summary>
        /// <value>The device family.</value>
        public bool Condition
        {
            get => (bool)GetValue(ConditionProperty);
            set => SetValue(ConditionProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DeviceFamily"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ConditionProperty =
            DependencyProperty.Register("Condition", typeof(bool), typeof(DeviceFamilyWithConditionStateTrigger),
            new PropertyMetadata(false, OnConditionPropertyChanged));

        private static void OnConditionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (DeviceFamilyWithConditionStateTrigger)d;
            var val = (bool)e.NewValue;
            if ((DeviceFamily)Enum.Parse(typeof(DeviceFamily), _deviceFamily.Remove(0, 8)) == obj.DeviceFamily)
            {
                obj.IsActive = val;
            }
            //else if (deviceFamily == "Windows.Desktop")
            //    obj.IsActive = (val == DeviceFamily.Desktop);
            //else if (deviceFamily == "Windows.Team")
            //    obj.IsActive = (val == DeviceFamily.Team);
            //else if (deviceFamily == "Windows.IoT")
            //    obj.IsActive = (val == DeviceFamily.IoT);
            //else if (deviceFamily == "Windows.Holographic")
            //    obj.IsActive = (val == DeviceFamily.Holographic);
            //else if (deviceFamily == "Windows.Xbox")
            //    obj.IsActive = (val == DeviceFamily.Xbox);
            //else
            //    obj.IsActive = (val == DeviceFamily.Unknown);
        }

        #region ITriggerValue

        private bool _mIsActive;

        /// <summary>
        /// Gets a value indicating whether this trigger is active.
        /// </summary>
        /// <value><c>true</c> if this trigger is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get => _mIsActive;
            private set
            {
                if (_mIsActive != value)
                {
                    _mIsActive = value;
                    SetActive(value);
                    IsActiveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="IsActive" /> property has changed.
        /// </summary>
        public event EventHandler IsActiveChanged;

        #endregion ITriggerValue
    }
}