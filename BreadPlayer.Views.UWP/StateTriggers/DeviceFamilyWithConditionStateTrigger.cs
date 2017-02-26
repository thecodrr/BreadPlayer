using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BreadPlayer.StateTriggers
{
    public class DeviceFamilyWithConditionStateTrigger : StateTriggerBase, ITriggerValue
    {
        private static string deviceFamily;

        static DeviceFamilyWithConditionStateTrigger()
        {
            deviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceFamilyStateTrigger"/> class.
        /// </summary>
        public DeviceFamilyWithConditionStateTrigger()
        {
        }

        /// <summary>
        /// Gets or sets the device family to trigger on.
        /// </summary>
        /// <value>The device family.</value>
        public DeviceFamily DeviceFamily
        {
            get { return (DeviceFamily)GetValue(DeviceFamilyProperty); }
            set { SetValue(DeviceFamilyProperty, value); }
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
            if ((DeviceFamily)Enum.Parse(typeof(DeviceFamily), deviceFamily.Remove(0,8)) == val)
                obj.IsActive = obj.Condition;
        }

        /// <summary>
        /// Gets or sets the device family to trigger on.
        /// </summary>
        /// <value>The device family.</value>
        public bool Condition
        {
            get { return (bool)GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
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
            if ((DeviceFamily)Enum.Parse(typeof(DeviceFamily), deviceFamily.Remove(0, 8)) == obj.DeviceFamily)
                obj.IsActive = val;
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

        private bool m_IsActive;

        /// <summary>
        /// Gets a value indicating whether this trigger is active.
        /// </summary>
        /// <value><c>true</c> if this trigger is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return m_IsActive; }
            private set
            {
                if (m_IsActive != value)
                {
                    m_IsActive = value;
                    base.SetActive(value);
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
