using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PatternMatchingTool.Device.Camera.DeviceCameraAbstract;

namespace PatternMatchingTool.Device.Camera
{
    public abstract class DeviceCameraAbstract
    {

        public abstract event Action<Bitmap> OnFrameReceived;
        public abstract bool OpenDevice(int nSelectedCameraIndex);
        public abstract void CloseDevice();
        public abstract bool StartGrab();
        public abstract void StopGrab();

        public delegate void CallBackFunctionGrab(Bitmap bmp);

        public abstract bool IsConnected();
        public abstract bool SetExposureTime(double dExposureTime);
        public abstract bool SetGain(double dGain);
        
        public virtual bool SetTriggerMode() { return true; }
        public virtual bool SetFrameRate(double dFrameRate) { return true; }


        public class ImageData : ICloneable
        {
            public bool bGrabComplete = false;
            public Bitmap bitmapImage = null;
            public int iImageWidth = 0;
            public int iImageHeight = 0;
            public int iMultiGrabImageIndex = 0;
            public IntPtr iImageAddress;
            public object Clone()
            {
                ImageData objImageData = new ImageData();
                objImageData.bGrabComplete = bGrabComplete;
                objImageData.bitmapImage = (Bitmap)bitmapImage.Clone();
                objImageData.iImageWidth = iImageWidth;
                objImageData.iImageHeight = iImageHeight;
                objImageData.iMultiGrabImageIndex = iMultiGrabImageIndex;
                objImageData.iImageAddress = iImageAddress;
                return objImageData;
            }
        }
    }
}
