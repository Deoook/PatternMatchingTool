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

        public DeviceCamera(DeviceCameraAbstract objCamera)
        {
            m_objCamera = objCamera;
        }
        public bool OpenDevice(int nSelectedCameraIndex)
        {
            return m_objCamera.OpenDevice(nSelectedCameraIndex);
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

        public bool IsConnected()
        {
            return m_objCamera.IsConnected();
        }

        public bool SetExposureTime(double dExposureTime)
        {
            return m_objCamera.SetExposureTime(dExposureTime);
        }

        public bool SetGain(double dGain)
        {
            return m_objCamera.SetGain(dGain);
        }

        public bool SetFrameRate(double dFrameRate)
        {
            return m_objCamera.SetFrameRate(dFrameRate);
        }

        public bool SetTriggerMode() { return true; }
    }
}
