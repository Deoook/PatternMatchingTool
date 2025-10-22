using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchingTool.Device.Camera
{
    public class DeviceCamera
    {
        private DeviceCameraAbstract m_objCamera;

        public event Action<Bitmap> OnFrameReceived
        {
            add { m_objCamera.OnFrameReceived += value; }
            remove { m_objCamera.OnFrameReceived -= value; }
        }

        public event Action<Bitmap> OnProcessFrameReceived
        {
            add { m_objCamera.OnProcessFrameReceived += value; }
            remove { m_objCamera.OnProcessFrameReceived -= value; }
        }

        public DeviceCamera(DeviceCameraAbstract objCamera)
        {
            m_objCamera = objCamera;
        }
        public bool OpenDevice(int nSelectedCameraIndex)
        {
            return m_objCamera.OpenDevice(nSelectedCameraIndex);
        }

        public bool OpenDevice(Config.CameraParameter objCameraParameter)
        {
            return m_objCamera.OpenDevice(objCameraParameter);
        }

        public void CloseDevice()
        {
            m_objCamera.CloseDevice();
        }
        public bool StartGrab()
        {
            return m_objCamera.StartGrab();
        }
        public void StopGrab()
        {
            m_objCamera.StopGrab();
        }

        public bool SetTriggerMode(bool bIsTriggerMode)
        {
            return m_objCamera.SetTriggerMode(bIsTriggerMode);
        }

        public bool IsConnected()
        {
            return m_objCamera.IsConnected();
        }

        public bool SetExposureTime(float fExposureTime)
        {
            return m_objCamera.SetExposureTime(fExposureTime);
        }

        public float GetExposureTime()
        {
            return m_objCamera.GetExposureTime();
        }

        public bool SetGain(float fGain)
        {
            return m_objCamera.SetGain(fGain);
        }

        public float GetGain()
        {
            return m_objCamera.GetGain();
        }

        public bool SetFrameRate(float fFrameRate)
        {
            return m_objCamera.SetFrameRate(fFrameRate);
        }

        public float GetFrameRate()
        {
            return m_objCamera.GetFrameRate();
        }

        public bool Trigger()
        {
            return m_objCamera.Trigger();
        }
    }
}
