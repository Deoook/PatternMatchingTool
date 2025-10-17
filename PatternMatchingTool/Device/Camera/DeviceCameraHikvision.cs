
using MvCamCtrl.NET;
using PatternMatchingTool.Data;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PatternMatchingTool.Device.Camera
{
    public class DeviceCameraHikvision : DeviceCameraAbstract
    {
        private MyCamera m_objCamera;
        private MyCamera.cbOutputExdelegate m_cbOutputExDelegate;
        public override event Action<Bitmap> OnFrameReceived;

        public DeviceCameraHikvision()
        {
            m_objCamera = new MyCamera();
        }

        public override bool OpenDevice(int nSelectedCameraIndex)
        {
            var pDocument = Document.GetDocument;
            bool bReturn = false;
            do
            {
                if (pDocument.m_objProcessMain.m_objProcessCameraManager.m_stDeviceList.nDeviceNum == 0 || nSelectedCameraIndex == -1)
                    break;

                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(pDocument.m_objProcessMain.m_objProcessCameraManager.m_stDeviceList.pDeviceInfo[nSelectedCameraIndex], typeof(MyCamera.MV_CC_DEVICE_INFO));

                if (m_objCamera == null)
                {
                    m_objCamera = new MyCamera();
                    if (m_objCamera == null)
                    {
                        break;
                    }
                }

                int nRet = m_objCamera.MV_CC_CreateDevice_NET(ref device);
                if (MyCamera.MV_OK != nRet)
                {
                    break;
                }

                nRet = m_objCamera.MV_CC_OpenDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    m_objCamera.MV_CC_DestroyDevice_NET();
                    Console.WriteLine("Device Open Fail");
                    break;
                }

                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    int nPacketSize = m_objCamera.MV_CC_GetOptimalPacketSize_NET();
                    if (nPacketSize > 0)
                    {
                        nRet = m_objCamera.MV_CC_SetIntValueEx_NET("GevSCPSPAcketSize", nPacketSize);
                        if (nRet != MyCamera.MV_OK)
                        {
                            Console.WriteLine("Set Packet Size Fail");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Set Packet Size Fail");
                    }
                }

                m_cbOutputExDelegate = new MyCamera.cbOutputExdelegate(ImageCallBack);
                m_objCamera.MV_CC_RegisterImageCallBackEx_NET(m_cbOutputExDelegate, IntPtr.Zero);

                bReturn = true;
            } while (false);
            return bReturn;
        }

        public void ImageCallBack(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            try
            {
                int dataSize = (int)pFrameInfo.nFrameLen;
                byte[] buffer = new byte[dataSize];
                Marshal.Copy(pData, buffer, 0, dataSize);

                PixelFormat pixelFormat = PixelFormat.Format8bppIndexed;
                var bmp = new Bitmap(pFrameInfo.nWidth, pFrameInfo.nHeight, pixelFormat);

                BitmapData bmpData = bmp.LockBits(
                    new Rectangle(0, 0, pFrameInfo.nWidth, pFrameInfo.nHeight),
                    ImageLockMode.WriteOnly,
                    pixelFormat);

                Marshal.Copy(buffer, 0, bmpData.Scan0, buffer.Length);
                bmp.UnlockBits(bmpData);

                ColorPalette palette = bmp.Palette;
                for (int i = 0; i < 256; i++) palette.Entries[i] = Color.FromArgb(i, i, i);
                bmp.Palette = palette;

                OnFrameReceived?.Invoke((Bitmap)bmp.Clone());

                bmp.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image callback error: {ex.Message}");
            }
        }

        public override void CloseDevice()
        {
            m_objCamera.MV_CC_CloseDevice_NET();
            m_objCamera.MV_CC_DestroyDevice_NET();
            GC.Collect();
        }

        public override bool StartGrab()
        {
            bool bReturn = false;
            do
            {
                if (null == m_objCamera)
                    break;

                int nRet = m_objCamera.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    break;
                }

                //m_bGrabbing = true;

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public override void StopGrab()
        {
            int nRet = m_objCamera.MV_CC_StopGrabbing_NET();
            if (nRet != MyCamera.MV_OK)
            {
                Console.WriteLine("Stop Grab Fail");
            }
        }

        public override bool IsConnected()
        {
            throw new NotImplementedException();
        }

        public override bool SetExposureTime(double dExposureTime)
        {
            throw new NotImplementedException();
        }

        public override bool SetGain(double dGain)
        {
            throw new NotImplementedException();
        }
    }
}
