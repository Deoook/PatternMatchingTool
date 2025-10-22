
using MvCamCtrl.NET;
using PatternMatchingTool.Data;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static MvCamCtrl.NET.MyCamera;

namespace PatternMatchingTool.Device.Camera
{
    public class DeviceCameraHikvision : DeviceCameraAbstract
    {
        private MyCamera m_objCamera;
        private MyCamera.cbOutputExdelegate m_cbOutputExDelegate;
        public override event Action<Bitmap> OnFrameReceived;
        public override event Action<Bitmap> OnProcessFrameReceived;

        public DeviceCameraHikvision()
        {
            m_objCamera = new MyCamera();
        }

        /// <summary>
        /// 카메라 셋업 페이지용
        /// </summary>
        /// <param name="nSelectedCameraIndex"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Auto Page용
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public override bool OpenDevice(Config.CameraParameter objCameraParameter)
        {
            var pDocument = Document.GetDocument;
            bool bReturn = false;
            do
            {
                if (pDocument.m_objProcessMain.m_objProcessCameraManager.m_stDeviceList.nDeviceNum == 0)
                    break;

                if (m_objCamera == null)
                {
                    m_objCamera = new MyCamera();
                    if (m_objCamera == null)
                    {
                        break;
                    }
                }

                int foundIndex = -1;

                for (int iCameraCount = 0; iCameraCount < pDocument.m_objProcessMain.m_objProcessCameraManager.m_stDeviceList.nDeviceNum; iCameraCount++)
                {
                    MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(pDocument.m_objProcessMain.m_objProcessCameraManager.m_stDeviceList.pDeviceInfo[iCameraCount],typeof(MyCamera.MV_CC_DEVICE_INFO));

                    string currentSerial = "";

                    if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                        currentSerial = gigeInfo.chSerialNumber;
                    }
                    else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                    {
                        MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                        currentSerial = usbInfo.chSerialNumber;
                    }

                    if (currentSerial == objCameraParameter.strCameraSerialNumber)
                    {
                        foundIndex = iCameraCount;
                        break;
                    }
                }

                // 동일한 SerialNumber를 가진 카메라가 없을 경우
                if (-1 == foundIndex)
                    break;

                MyCamera.MV_CC_DEVICE_INFO foundDevice = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(pDocument.m_objProcessMain.m_objProcessCameraManager.m_stDeviceList.pDeviceInfo[foundIndex], typeof(MyCamera.MV_CC_DEVICE_INFO));


                int nRet = m_objCamera.MV_CC_CreateDevice_NET(ref foundDevice);
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

                if (foundDevice.nTLayerType == MyCamera.MV_GIGE_DEVICE)
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

                // 카메라 오픈 때 Trigger Mode 설정 ???
                SetTriggerMode(true);

                m_cbOutputExDelegate = new MyCamera.cbOutputExdelegate(ImageCallBack);
                m_objCamera.MV_CC_RegisterImageCallBackEx_NET(m_cbOutputExDelegate, IntPtr.Zero);

                bReturn = true;
            } while (false);
            return bReturn;
        }

        private void ImageCallBack(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            var pDocument = Document.GetDocument;

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

                if(Define.RunMode.RUN_MODE_RUNNING == pDocument.GetRunMode())
                {
                    //검사모드일 경우 검사용 프레임 전송
                    OnProcessFrameReceived.Invoke((Bitmap)bmp.Clone());
                }
                else
                {
                    //검사 대기모드가 아닐 경우 단순 프레임 전송 ( 카메라 셋팅창 용 )
                    OnFrameReceived?.Invoke((Bitmap)bmp.Clone());
                }

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
            else
            {
                Console.WriteLine("Stop Grab Succes");
            }
        }

        public override bool IsConnected()
        {
            bool IsConnected = m_objCamera.MV_CC_IsDeviceConnected_NET();
            return IsConnected;
        }

        public override bool SetExposureTime(float fExposureTime)
        {
            bool bReturn = false;
            do
            {
                m_objCamera.MV_CC_SetEnumValue_NET("ExposureAuto", 0);
                int nRet = m_objCamera.MV_CC_SetFloatValue_NET("ExposureTime", fExposureTime);
                if (nRet != MyCamera.MV_OK)
                {
                    break;
                }

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public override float GetExposureTime()
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            m_objCamera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);

            return stParam.fCurValue;
        }

        public override bool SetGain(float fGain)
        {
            bool bReturn = false;
            do
            {
                
                m_objCamera.MV_CC_SetEnumValue_NET("GainAuto", 0);
                int nRet = m_objCamera.MV_CC_SetFloatValue_NET("Gain", fGain);
                if (nRet != MyCamera.MV_OK)
                {
                    break;
                }

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public override float GetGain()
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            m_objCamera.MV_CC_GetFloatValue_NET("Gain", ref stParam);

            return stParam.fCurValue;
        }

        public override bool SetFrameRate(float fFrameRate)
        {
            bool bReturn = false;
            do
            {
                int nRet = m_objCamera.MV_CC_SetFloatValue_NET("AcquisitionFrameRate", fFrameRate);
                if (nRet != MyCamera.MV_OK)
                {
                    break;
                }

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public override float GetFrameRate()
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            m_objCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam);

            return stParam.fCurValue;
        }

        public override bool SetTriggerMode(bool bIsTriggerMode)
        {
            bool bReturn = false;
            do
            {
                if (true == bIsTriggerMode)
                {
                    //SW Trigger로 고정
                    m_objCamera.MV_CC_SetTriggerSource_NET((uint)MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
                    m_objCamera.MV_CC_SetEnumValueByString_NET("TriggerMode", "On");
                }
                else
                {
                    m_objCamera.MV_CC_SetEnumValueByString_NET("TriggerMode", "Off");
                    m_objCamera.MV_CC_SetEnumValueByString_NET("AcquisitionMode", "Continuous");
                }

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public override bool Trigger()
        {
            bool bReturn = false;
            do
            {
                int result = m_objCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");

                if(result != MyCamera.MV_OK)
                {
                    break;
                }

                bReturn = true;
            } while (false);

            return bReturn;
        }
    }
}
